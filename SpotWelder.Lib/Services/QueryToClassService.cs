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

    public IList<GeneratedResult>? Generate(QueryToClassParameters parameters)
    {
      if (!parameters.HasElections) return null;

      _queryToClassRepository.ChangeConnectionString(parameters.ConnectionString);

      var baseInstructions = GetBaseInstructions(parameters);

      return GenerateClasses(baseInstructions);

      //Writing to files will be handled again later
      //if (p.SaveAsFile)
      //    WriteClassToFile(p, content);
    }

    public IList<GeneratedResult> Generate(DtoInstructions instructions)
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
      var schema = GetSchema(p.SourceSqlType, p.SourceSqlText, p.TableQuery);

      var ins = new ClassInstructions
      {
        Namespace = p.Namespace, 
        SubjectName = p.SubjectName, 
        EntityName = p.EntityName,
        ModelName = p.ModelName,
        ApiRoute = p.SubjectName.ToLower(), //TODO: Use a humanizer that makes this plural and camel case https://github.com/Humanizr/Humanizer
        IsAsynchronous = p.Elections.HasFlag(GenerationElections.MakeAsynchronous),
        InterfaceName = $"I{p.SubjectName}",
        TableQuery = p.TableQuery,
        Elections = p.Elections,
      };

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
    private IList<GeneratedResult> GenerateClasses(ClassInstructions baseInstructions)
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

        lst.Add(result);
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
