using Humanizer;
using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpotWelder.Lib.Services
{
  public class QueryToClassService
    : ClassMetaDataBase, IQueryToClassService
  {
    private readonly ICodeGenerationFactory _factory;

    public QueryToClassService(
      IQueryToClassRepository repository, 
      IGeneralDatabaseQueries genericDatabaseQueries,
      ICodeGenerationFactory factory)
      : base(repository, genericDatabaseQueries)
    {
      _factory = factory;
    }

    public List<GeneratedResult>? Generate(QueryToClassParameters parameters)
    {
      if (!parameters.HasElections) return null;

      //If you are using a repository, then you are automatically using the Record to Entity mapper.
      if (parameters.Elections.HasAnyFlag(
            GenerationElections.RepoDapper,
            GenerationElections.RepoStatic))
      {
        parameters.Elections |= GenerationElections.GenerateRecord;
        parameters.Elections |= GenerationElections.MapRecordToEntity;
      }

      //If any mapping is selected, then also generate the mapper class
      if (parameters.Elections.HasAnyFlag(
            GenerationElections.MapModelToEntity,
            GenerationElections.MapEntityToModel,
            GenerationElections.MapCreateModelToEntity,
            GenerationElections.MapPatchModelToEntity,
            GenerationElections.MapRecordToEntity))
        parameters.Elections |= GenerationElections.GenerateMapper;

      _queryToClassRepository.ConfigureSqlClient(parameters.ServerConnection);

      var baseInstructions = GetBaseInstructions(parameters);

      return GenerateClasses(baseInstructions);
    }

    public List<GeneratedResult> Generate(DtoInstructions instructions)
    {
      var ci = new ClassInstructions
      {
        ClassName = instructions.SourceClassName,
        EntityName = instructions.SourceClassName,
        ModelName = $"{instructions.SourceClassName}Dto",
        InterfaceName = $"I{instructions.SourceClassName}",
        Namespace = "Namespace1",
        Languages = instructions.Languages,
        Properties = instructions.Properties,
        IsPartial = instructions.Elections.HasFlag(GenerationElections.GenerateEntityIEquatable)
      };
      
      return GenerateClasses(ci);
    }

    /// <summary>
    /// This is to be thought of as factual information. The properties provided here
    /// should not be overwritten, but can be when necessary.
    /// </summary>
    /// <param name="p">User elections</param>
    /// <returns>Instructions</returns>
    private ClassInstructions GetBaseInstructions(QueryToClassParameters p)
    {
      //TODO: Incoming parameters require validation.

      var schema = GetSchema(p.ServerConnection);

      var ins = new ClassInstructions
      {
        Namespace = p.Namespace, 
        SubjectName = p.SubjectName,
        EntityName = p.EntityName,
        RecordName = $"{p.SubjectName}Record",
        ModelName = p.ModelName,
        ApiRoute = p.SubjectName.ToLower().Pluralize(),
        IsAsynchronous = p.Elections.HasFlag(GenerationElections.MakeAsynchronous),
        InterfaceName = $"I{p.SubjectName}",
        SourceSqlType = p.ServerConnection.SourceSqlType,
        TableQuery = p.ServerConnection.TableQuery,
        SourceQuery = p.ServerConnection.SourceSqlText,
        Elections = p.Elections,
        SqlEngine = p.ServerConnection.SqlEngine
      };

      //TODO: defaulting the language to CSharp, not sure what I am going to do with this at the moment
      if(p.LanguageType == CodeType.None) p.LanguageType = CodeType.CSharp;

      foreach (var sc in schema.ColumnsAll)
      {
        var prop = new ClassMemberStrings(sc, p.LanguageType);

        //Add the system namespace if any of the properties require it
        if (prop.InSystemNamespace) ins.AddNamespace("System");

        ins.Properties.Add(prop);
      }

      return ins;
    }

    /// <summary>
    ///   The main internal method that orchestrates the code generation for the provided parameters
    /// </summary>
    /// <returns>The generated class code as a StringBuilder</returns>
    private List<GeneratedResult> GenerateClasses(ClassInstructions baseInstructions)
    {
      //Get all elections that can be generated directly, leave out the ones that cannot.
      //Look at the enumeration directly for more information.
      var allTests = Enum.GetValues(typeof(GenerationElections))
        .Cast<GenerationElections>()
        .Where(IsParent)
        .ToList();

      var lst = new List<GeneratedResult>(allTests.Count);

      foreach (var e in allTests)
      {
        var result = _factory.Generate(baseInstructions, e);

        if(result == null) continue;

        //The immutables case calls for this check
        if(!string.IsNullOrWhiteSpace(result.Filename)) lst.Add(result);

        //Corresponding interfaces exist for some elections only
        if (result.CorrespondingInterface != null) lst.Add(result.CorrespondingInterface);

        if (result.Heap == null || result.Heap.Count == 0) continue;

        //A heap of generated results exists for some elections only
        lst.AddRange(result.Heap);
      }

      lst.TrimExcess();

      return lst;
    }

    private static bool IsParent(GenerationElections election)
    {
      var fi = election.GetType().GetField(election.ToString());

      if (fi == null) return false;
      
      return fi.GetCustomAttributes(false).Length == 0;
    }

    #region Generate GridView
    //This is a relic of the past, not sure if I will continue to support this as it is just another template esssentially
    //public string GenerateGridViewColumns(QueryToClassParameters parameters)
    //{
    //    var p = parameters;

    //    _repository.ChangeConnectionString(p.ConnectionString);

    //    var sql = p.SourceSqlType == SourceSqlType.TableName ? ("SELECT TOP 0 * FROM " + p.SourceSqlText) : p.SourceSqlText;

    //    var dt = _repository.GetSchema(sql);

    //    var sb = new StringBuilder();

    //    foreach (DataColumn dc in dt.Columns)
    //    {
    //        sb.Append("<asp:BoundField HeaderText=\"")
    //          .Append(dc.ColumnName)
    //          .Append("\" DataField=\"")
    //          .Append(dc.ColumnName)
    //          .AppendLine("\">")
    //          .AppendLine("<HeaderStyle HorizontalAlign=\"" + (IsNumber(dc.DataType) ? "Right" : "Left") + "\" />")
    //          .AppendLine("</asp:BoundField>");
    //    }

    //    var content = sb.ToString();

    //    return content;
    //}
    #endregion
  }
}
