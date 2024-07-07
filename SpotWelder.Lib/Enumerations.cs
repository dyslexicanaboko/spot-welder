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

    CloneEntityToModel = 1,

    CloneModelToEntity = 2,

    CloneInterfaceToEntity = 4,

    CloneInterfaceToModel = 8,

    SerializeCsv = 16,

    SerializeJson = 32,

    RepoStatic = 64,

    RepoDynamic = 128,

    RepoBulkCopy = 512,

    RepoDapper = 1024,

    RepoEfFluentApi = 2048,
    
    Service = 4096,
  }

  [Flags]
  public enum ClassRepositories
  {
    None = 0,

    StaticStatements = 1,

    Dapper = 2
  }
}
