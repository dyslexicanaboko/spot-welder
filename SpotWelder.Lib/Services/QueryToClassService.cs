using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Lib.Services.Generators;
using System.Collections.Generic;

namespace SpotWelder.Lib.Services
{
  public class QueryToClassService
    : ClassMetaDataBase, IQueryToClassService
  {
    public QueryToClassService(
      IQueryToClassRepository repository, 
      IGeneralDatabaseQueries genericDatabaseQueries)
      : base(repository, genericDatabaseQueries)
    {
    }

    public IList<GeneratedResult>? Generate(QueryToClassParameters parameters)
    {
      if (!parameters.HasElections) return null;

      _queryToClassRepository.ChangeConnectionString(parameters.ConnectionString);

      var baseInstructions = GetBaseInstructions(parameters);

      var rClasses = GenerateClasses(parameters.Elections, baseInstructions);

      var rServices = GenerateServices(parameters.Elections, baseInstructions);

      var rRepositories = GenerateRepositories(parameters.Elections, baseInstructions);

      var lst = new List<GeneratedResult>(
        rClasses.Count +
        rServices.Count +
        rRepositories.Count);

      lst.AddRange(rClasses);
      lst.AddRange(rServices);
      lst.AddRange(rRepositories);

      //Writing to files will be handled again later
      //if (p.SaveAsFile)
      //    WriteClassToFile(p, content);

      return lst;
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
      
      return GenerateClasses(instructions.Elections, ci);
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
        ApiRoute = p.ApiRoute,
        IsAsynchronous = p.Elections.HasFlag(GenerationElections.MakeAsynchronous),
        InterfaceName = $"I{p.SubjectName}",
        TableQuery = p.TableQuery
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
    private static IList<GeneratedResult> GenerateClasses(
      GenerationElections elections,
      ClassInstructions baseInstructions)
    {
      var lst = new List<GeneratedResult>();

      var interfaceName = string.Empty;

      if (elections.HasFlag(GenerationElections.GenerateInterface))
      {
        interfaceName = baseInstructions.InterfaceName;

        var ins = baseInstructions.Clone();
        ins.ClassName = ins.InterfaceName;

        var svc = new ClassInterfaceGenerator(ins);

        lst.Add(svc.FillTemplate());
      }

      if (elections.HasFlag(GenerationElections.GenerateEntity))
      {
        var ins = baseInstructions.Clone();
        //Interface is only included if it was elected to be generated
        ins.InterfaceName = interfaceName;
        ins.IsPartial = elections.HasFlag(GenerationElections.GenerateEntityIEquatable) || elections.HasFlag(GenerationElections.GenerateEntityIComparable);

        var svc = new ClassEntityGenerator(ins);

        lst.Add(svc.FillTemplate());

        if (elections.HasFlag(GenerationElections.GenerateEntityIEquatable))
        {
          var insSub = baseInstructions.Clone();
          //This is the same name as the Entity because it's a partial class implementation
          insSub.ClassName = baseInstructions.EntityName;

          var svcSub = new ClassEntityIEquatableGenerator(insSub);

          lst.Add(svcSub.FillTemplate());
        }

        if (elections.HasFlag(GenerationElections.GenerateEntityIComparable))
        {
          var insSub = baseInstructions.Clone();
          //This is the same name as the Entity because it's a partial class implementation
          insSub.ClassName = baseInstructions.EntityName;

          var svcSub = new ClassEntityIComparableGenerator(insSub);

          lst.Add(svcSub.FillTemplate());
        }

        if (elections.HasFlag(GenerationElections.GenerateEntityEqualityComparer))
        {
          var insSub = baseInstructions.Clone();
          //This is a prefix, the template appends EqualityComparer to it
          insSub.ClassName = baseInstructions.EntityName;

          var svcSub = new ClassEntityEqualityComparerGenerator(insSub);

          lst.Add(svcSub.FillTemplate());
        }
      }

      if (elections.HasFlag(GenerationElections.GenerateModel))
      {
        var ins = baseInstructions.Clone();
        ins.ClassName = baseInstructions.ModelName;
        ins.InterfaceName = interfaceName;

        var svc = new ClassModelGenerator(ins);

        lst.Add(svc.FillTemplate());
      }

      if (elections.HasFlag(GenerationElections.GenerateCreateModel))
      {
        var svc = new ClassModelCreateGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (elections.HasFlag(GenerationElections.GeneratePatchModel))
      {
        var svc = new ClassModelPatchGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (baseInstructions.Languages.HasFlag(CodeType.JavaScript))
      {
        var svc = new LanguageJavaScriptGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (baseInstructions.Languages.HasFlag(CodeType.TypeScript))
      {
        var svc = new LanguageTypeScriptGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      return lst;
    }

    private static IList<GeneratedResult> GenerateServices(
      GenerationElections elections,
      ClassInstructions baseInstructions)
    {
      if (elections == GenerationElections.None) return new List<GeneratedResult>(0);
      
      var lst = new List<GeneratedResult>();

      if (elections.HasFlag(GenerationElections.SerializeCsv))
      {
        var svc = new ServiceSerializationCsvGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (elections.HasFlag(GenerationElections.SerializeJson))
      {
        var svc = new ServiceSerializationJsonGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (elections.HasFlag(GenerationElections.Service))
      {
        var svc = new ServiceGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (elections.HasFlag(GenerationElections.ApiController))
      {
        var svc = new ApiControllerGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (elections.HasFlag(GenerationElections.CloneModelToEntity) ||
          elections.HasFlag(GenerationElections.CloneEntityToModel) ||
          elections.HasFlag(GenerationElections.CloneInterfaceToEntity) ||
          elections.HasFlag(GenerationElections.CloneInterfaceToModel))
      {
        var svc = new MapperGenerator(baseInstructions.Clone(), elections);

        lst.Add(svc.FillTemplate());
      }

      return lst;
    }

    private static IList<GeneratedResult> GenerateRepositories(
      GenerationElections elections,
      ClassInstructions baseInstructions)
    {
      var lst = new List<GeneratedResult>();

      if (elections.HasFlag(GenerationElections.RepoStatic))
      {
        var svc = new RepositoryStaticGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (elections.HasFlag(GenerationElections.RepoDapper))
      {
        var svc = new RepositoryDapperGenerator(baseInstructions.Clone());
        
        lst.Add(svc.FillTemplate());
      }

      return lst;
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
