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

      var rClasses = GenerateClasses(parameters, baseInstructions);

      var rServices = GenerateServices(parameters, baseInstructions);

      var rRepositories = GenerateRepositories(parameters, baseInstructions);

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

    public IList<GeneratedResult> Generate(DtoInstructions parameters)
    {
      var p = parameters;

      var ci = new ClassInstructions
      {
        ClassName = p.SourceClassName,
        EntityName = p.SourceClassName,
        ModelName = $"{p.SourceClassName}Dto",
        InterfaceName = $"I{p.SourceClassName}",
        Namespace = "Namespace1",
        Properties = p.Properties,
        IsPartial = p.ImplementIEquatableOfTInterface
      };

      var co = new ClassOptions
      {
        EntityName = ci.EntityName,
        ModelName = ci.ModelName,
        GenerateEntity = true,
        GenerateModel = true,
        GenerateEntityIEquatable = p.ImplementIEquatableOfTInterface,
        GenerateInterface = p.ExtractInterface
      };

      if (p.EquivalentJavaScript) co.Languages |= CodeType.JavaScript;
      if (p.EquivalentTypeScript) co.Languages |= CodeType.TypeScript;

      var qtcParameters = new QueryToClassParameters { ClassOptions = co };

      if (p.MethodEntityToDto) qtcParameters.ClassServices |= ClassServices.CloneEntityToModel;

      if (p.MethodDtoToEntity) qtcParameters.ClassServices |= ClassServices.CloneModelToEntity;

      return GenerateClasses(qtcParameters, ci);
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
        SubjectName = p.ClassOptions.SubjectName, 
        EntityName = p.ClassOptions.EntityName,
        ModelName = p.ClassOptions.ModelName,
        ApiRoute = p.ClassOptions.ApiRoute,
        IsAsynchronous = p.ClassOptions.IsAsynchronous,
        InterfaceName = $"I{p.ClassOptions.SubjectName}",
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
      QueryToClassParameters parameters,
      ClassInstructions baseInstructions)
    {
      var co = parameters.ClassOptions;

      var lst = new List<GeneratedResult>();

      var interfaceName = string.Empty;

      if (co.GenerateInterface)
      {
        interfaceName = baseInstructions.InterfaceName;

        var ins = baseInstructions.Clone();
        ins.ClassName = ins.InterfaceName;

        var svc = new ClassInterfaceGenerator(ins);

        lst.Add(svc.FillTemplate());
      }

      if (co.GenerateEntity)
      {
        var ins = baseInstructions.Clone();
        ins.ClassName = co.EntityName;
        //Interface is only included if it was elected to be generated
        ins.InterfaceName = interfaceName;
        ins.IsPartial = co.GenerateEntityIEquatable || co.GenerateEntityIComparable;

        var svc = new ClassEntityGenerator(ins);

        lst.Add(svc.FillTemplate());

        if (co.GenerateEntityIEquatable)
        {
          var insSub = baseInstructions.Clone();
          //This is the same name as the Entity because it's a partial class implementation
          insSub.ClassName = co.EntityName;

          var svcSub = new ClassEntityIEquatableGenerator(insSub);

          lst.Add(svcSub.FillTemplate());
        }

        if (co.GenerateEntityIComparable)
        {
          var insSub = baseInstructions.Clone();
          //This is the same name as the Entity because it's a partial class implementation
          insSub.ClassName = co.EntityName;

          var svcSub = new ClassEntityIComparableGenerator(insSub);

          lst.Add(svcSub.FillTemplate());
        }

        if (co.GenerateEntityEqualityComparer)
        {
          var insSub = baseInstructions.Clone();
          //This is a prefix, the template appends EqualityComparer to it
          insSub.ClassName = co.EntityName;

          var svcSub = new ClassEntityEqualityComparerGenerator(insSub);

          lst.Add(svcSub.FillTemplate());
        }
      }

      if (co.GenerateModel)
      {
        var ins = baseInstructions.Clone();
        ins.ClassName = co.ModelName;
        ins.InterfaceName = interfaceName;

        var svc = new ClassModelGenerator(ins);

        lst.Add(svc.FillTemplate());
      }

      if (co.GenerateCreateModel)
      {
        var svc = new ClassModelCreateGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (co.GeneratePatchModel)
      {
        var svc = new ClassModelPatchGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (co.Languages.HasFlag(CodeType.JavaScript))
      {
        var svc = new LanguageJavaScriptGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (co.Languages.HasFlag(CodeType.TypeScript))
      {
        var svc = new LanguageTypeScriptGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      return lst;
    }

    private static IList<GeneratedResult> GenerateServices(
      QueryToClassParameters parameters,
      ClassInstructions baseInstructions)
    {
      if (parameters.ClassServices == ClassServices.None) return new List<GeneratedResult>(0);

      var services = parameters.ClassServices;

      var lst = new List<GeneratedResult>();

      if (services.HasFlag(ClassServices.SerializeCsv))
      {
        var svc = new ServiceSerializationCsvGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (services.HasFlag(ClassServices.SerializeJson))
      {
        var svc = new ServiceSerializationJsonGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (services.HasFlag(ClassServices.Service))
      {
        var svc = new ServiceGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (services.HasFlag(ClassServices.ApiController))
      {
        var svc = new ApiControllerGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (services.HasFlag(ClassServices.CloneModelToEntity) ||
          services.HasFlag(ClassServices.CloneEntityToModel) ||
          services.HasFlag(ClassServices.CloneInterfaceToEntity) ||
          services.HasFlag(ClassServices.CloneInterfaceToModel))
      {
        var svc = new MapperGenerator(baseInstructions.Clone(), services);

        lst.Add(svc.FillTemplate());
      }

      return lst;
    }

    private static IList<GeneratedResult> GenerateRepositories(
      QueryToClassParameters parameters,
      ClassInstructions baseInstructions)
    {
      if (parameters.ClassRepositories == ClassRepositories.None) return new List<GeneratedResult>(0);

      var repositories = parameters.ClassRepositories;

      var lst = new List<GeneratedResult>();

      if (repositories.HasFlag(ClassRepositories.StaticStatements))
      {
        var svc = new RepositoryStaticGenerator(baseInstructions.Clone());

        lst.Add(svc.FillTemplate());
      }

      if (repositories.HasFlag(ClassRepositories.Dapper))
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
