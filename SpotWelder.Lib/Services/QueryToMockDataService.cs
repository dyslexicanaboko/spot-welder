using System.Linq;
using SpotWelder.Lib.DataAccess;
using SpotWelder.Lib.Events;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Lib.Services.Generators;

namespace SpotWelder.Lib.Services
{
    public interface IQueryToMockDataService
    {
        string GetEntity(QueryToMockDataParameters parameters);

        GeneratedResult GetMockData(QueryToMockDataParameters parameters, int? top = null);

        event QueryToMockDataService.RowProcessedHandler RowProcessed;
    }

    public class QueryToMockDataService
        : ClassMetaDataBase, IQueryToMockDataService
    {
        private readonly IQueryToClassRepository _repository;
        private ClassInstructions _instructions;

        public delegate void RowProcessedHandler(object sender, RowProcessedEventArgs e);

        public event RowProcessedHandler RowProcessed;

        public QueryToMockDataService(IQueryToClassRepository repository, IGeneralDatabaseQueries genericDatabaseQueries)
            : base(repository, genericDatabaseQueries)
        {
            _repository = repository;
        }

        public string GetEntity(QueryToMockDataParameters parameters)
        {
          _repository.ChangeConnectionString(parameters.ConnectionString);

            //Get the meta data needed about the entity
            var instructions = GetInstructions(parameters);

            var generator = new ClassEntitySimpleGenerator();

            //Generate the string representation of the class for preview
            var res = generator.FillTemplate(instructions);

            return res.Contents;
        }

        public GeneratedResult GetMockData(QueryToMockDataParameters parameters, int? top = null)
        {
            //Generate the mock data constructs using the entity for as much data as is requested.
            //Most of this is going to be contained inside of this service because there is no other way

            _repository.ChangeConnectionString(parameters.ConnectionString);
            
            _genericDatabaseQueries.ChangeConnectionString(parameters.ConnectionString);

            //Get the meta data needed about the entity
            var instructions = GetInstructions(parameters);

            var dt = GetRowData(parameters.SourceSqlType, parameters.SourceSqlText, top);

            var generator = new ClassEntitySimpleGenerator();
            generator.RowProcessed += MockData_RowProcessed;

            //Generate the string representation of the class for preview
            var res = generator.FillMockDataTemplate(instructions, dt);

            return res;
        }

        private void MockData_RowProcessed(object sender, RowProcessedEventArgs e) => RowProcessed?.Invoke(this, e);

        private ClassInstructions GetInstructions(QueryToMockDataParameters parameters)
        {
          //Only get the instructions if they are not already cached and have not changed since the last time
          if (_instructions != null &&
              _instructions.EntityName == parameters.ClassEntityName &&
              _instructions.TableQuery == parameters.TableQuery)
          {
            return _instructions;
          }

          var schema = GetSchema(parameters.SourceSqlType, parameters.SourceSqlText, parameters.TableQuery);

          var ins = new ClassInstructions();

          ins.EntityName = parameters.ClassEntityName;
          ins.TableQuery = parameters.TableQuery;
          ins.Properties = schema.ColumnsAll.Select(x => new ClassMemberStrings(x)).ToList();

          _instructions = ins;

          return ins;
        }
    }
}
