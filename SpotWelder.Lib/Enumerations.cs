using System;

namespace SpotWelder.Lib
{
  public enum SourceSqlType
  {
    Query,

    TableName
  }

  [Flags]
  public enum CodeType
  {
    None = 0,

    CSharp = 1,

    [Obsolete("Looking to get rid of this option and therefore this Enum all together. VB sucks.")]
    VbNet = 2,

    JavaScript = 4,

    TypeScript = 8
  }

  [Flags]
  public enum TableQueryQualifiers
  {
    None = 0,

    Table = 1,

    Schema = 2,

    Database = 4,

    LinkedServer = 8
  }

  [Flags]
  public enum ClassServices
  {
    None = 0,

    CloneEntityToModel = 1, //2^0

    CloneModelToEntity = 2, //2^1

    CloneInterfaceToEntity = 4, //2^2

    CloneInterfaceToModel = 8, //2^3

    SerializeCsv = 16, //2^4

    SerializeJson = 32, //2^5

    RepoStatic = 64, //2^6

    RepoDynamic = 128, //2^7

    RepoBulkCopy = 256, //2^8

    RepoDapper = 512, //2^9

    RepoEfFluentApi = 1024, //2^10

    Service = 2048, //2^11

    ApiController = 4096, //2^12
  }

  [Flags]
  public enum ClassRepositories
  {
    None = 0,

    StaticStatements = 1,

    Dapper = 2
  }
}
