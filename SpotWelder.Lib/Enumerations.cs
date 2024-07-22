using System;

namespace SpotWelder.Lib
{
  public enum SourceSqlType
  {
    Query = 0,

    TableName = 1
  }

  [Flags]
  public enum CodeType
  {
    None = 0,

    CSharp = 1 << 0,

    [Obsolete("Looking to get rid of this option and therefore this Enum all together. VB sucks.")]
    VbNet = 1 << 1,

    JavaScript = 1 << 2,

    TypeScript = 1 << 3
  }

  [Flags]
  public enum TableQueryQualifiers
  {
    None = 0,

    Table = 1 << 0,

    Schema = 1 << 1,

    Database = 1 << 2,

    LinkedServer = 1 << 3
  }
}
