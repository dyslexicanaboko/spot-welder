using Microsoft.Extensions.Logging;
using SpotWelder.Lib;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services;
using SpotWelder.Lib.Services.TableQueryFormats;
using SpotWelder.Ui.Models.Cli;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SpotWelder.Ui.Containers
{
  public class CliMode(
    ILogger<CliMode> logger,
    IQueryToClassService queryToClassService,
    ITableQueryFormatFactory tableQueryFormatFactory)
  {
    public const int ExitSuccess = 0;
    public const int ExitError = 1;

    public int ProcessFile(string fullFilePath)
    {
      if (!File.Exists(fullFilePath))
      {
        LogWarning($"File not found: {fullFilePath}.\r\nPlease verify it exists and try again.");

        return ExitError;
      }

      if(Path.GetExtension(fullFilePath) != ".yaml")
      {
        LogWarning($"File must be a YAML file: {fullFilePath}.\r\nPlease check the file extension and try again.");

        return ExitError;
      }

      var sw = new Stopwatch();
      sw.Start();

      try
      {
        logger.LogInformation("Processing file: {FullFilePath}", fullFilePath);

        var bulk = Deserialize(fullFilePath);

        var parameters = new QueryToClassParameters();

        //Common level
        parameters.Namespace = bulk.Namespace;
        parameters.ServerConnection = new ServerConnection
        {
          ConnectionString = bulk.ConnectionString,
          SqlEngine = bulk.SqlEngine,
        };

        var tableQueryStrategy = tableQueryFormatFactory.GetStrategy(bulk.SqlEngine);

        var server = parameters.ServerConnection;

        //Aggregating elections
        parameters.Elections = ConvertListToBitMask(bulk.ElectionsList);

        logger.LogInformation("Generating {QueriesCount} subjects.", bulk.Queries.Count);

        for (var i = 0; i < bulk.Queries.Count; i++)
        {
          var query = bulk.Queries[i];

          logger.LogInformation("Subject {I:00}/{QueriesCount:00}: {QuerySubjectName}",
            i+1,
            bulk.Queries.Count,
            query.SubjectName);

          //Server connection
          server.SourceSqlType = query.SourceSqlType;
          server.SourceSqlText = query.SourceSqlText;
          server.TableQuery = tableQueryStrategy.ParseTableName(query.SourceSqlText);

          //Query level will change on each iteration
          parameters.SubjectName = query.SubjectName;

          var results = queryToClassService.Generate(parameters);

          if (results == null)
          {
            logger.LogWarning("No results generated for subject: {QuerySubjectName}", query.SubjectName);

            continue;
          }

          logger.LogInformation("Saving {ResultsCount} files.", results.Count);

          SaveAll(bulk.DestinationFolder, results);
        }

        logger.LogInformation("Processing completed successfully.");
        
        return ExitSuccess;
      }
      catch (Exception ex)
      {
        LogError(ex, "Unexpected error during processing encountered.");
        
        return ExitError;
      }
      finally
      {
        sw.Stop();

        var timing = sw.ElapsedMilliseconds > 1000
          ? $"{sw.ElapsedMilliseconds / 1000D:N2} seconds"
          : $"{sw.ElapsedMilliseconds:N0} milliseconds";

        logger.LogInformation("Processing took {Timing}.", timing);
      }
    }

    private static GenerationElections ConvertListToBitMask(List<string> electionsList)
      => electionsList
        .Select(Enum.Parse<GenerationElections>)
        .Aggregate(GenerationElections.None, (current, election) => current | election);

    private static void SaveAll(string destinationFolder, List<GeneratedResult> results)
    {
      foreach (var file in results)
      {
        var paths = file.ContainingNamespace.Split('.').ToList();

        paths.Insert(0, destinationFolder);
        paths.Add(file.Filename);

        var fullFilePath = Path.Combine(paths.ToArray());

        Directory.CreateDirectory(Path.GetDirectoryName(fullFilePath)!);

        Utils.WriteFile(fullFilePath, file.Contents);
      }
    }

    private void LogWarning(string message)
    {
      logger.LogWarning(message);

      Console.Error.WriteLine(message);
    }

    private void LogError(Exception ex, string message)
    {
      logger.LogError(ex, message);

      Console.Error.WriteLine(message);
    }

    public static BulkGenerationModel Deserialize(string fullFilePath)
    {
      var yaml = File.ReadAllText(fullFilePath);

      var deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

      var bulk = deserializer.Deserialize<BulkGenerationModel>(yaml);

      /* Because YAML is stupid, it will not recognize CRLF, it normalizes everything to LF.
         This doesn't work for me, so I have to change it back right away. */
      bulk.Queries.ForEach(x => x.SourceSqlText = x.SourceSqlText.ReplaceLineEndings());

      return bulk;
    }
  }
}
