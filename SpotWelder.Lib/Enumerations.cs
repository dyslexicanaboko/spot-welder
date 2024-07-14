using SpotWelder.Lib.Models;
using System;
using System.Collections.Generic;

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

    CSharp = 2^0,

    [Obsolete("Looking to get rid of this option and therefore this Enum all together. VB sucks.")]
    VbNet = 2^1,

    JavaScript = 2^2,

    TypeScript = 2^3
  }

  [Flags]
  public enum TableQueryQualifiers
  {
    None = 0,

    Table = 2^0,

    Schema = 2^1,

    Database = 2^2,

    LinkedServer = 2^3
  }

  [Flags]
  public enum GenerationElections
  {
    None = 0,

    /// <summary> Should class methods be asynchronous? </summary>
    MakeAsynchronous = 2^0,

    /// <summary> Generate an entity for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
    GenerateEntity = 2^1,

    /// <summary> Generate the <see cref="IEquatable{T}"/> interface implementation for the target entity. </summary>
    GenerateEntityIEquatable = 2^2,

    /// <summary> Generate the <see cref="IComparable"/> interface implementation for the target entity. </summary>
    GenerateEntityIComparable = 2^3,

    /// <summary> Generate an <see cref="EqualityComparer{T}"/> class for the target entity. </summary>
    GenerateEntityEqualityComparer = 2^4,

    /// <summary> Generate a model for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
    GenerateModel = 2^5,

    /// <summary> Generate a REST API Create model for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
    GenerateCreateModel = 2^6,

    /// <summary> Generate a REST API Patch model for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
    GeneratePatchModel = 2^7,

    /// <summary> Name of the subject with the `I` prefix.</summary>
    /// <example> Subject named: `Task`, the interface would be `ITask`.</example>
    GenerateInterface = 2^8,

    CloneEntityToModel = 2^9,

    CloneModelToEntity = 2^10,

    CloneInterfaceToEntity = 2^11,

    CloneInterfaceToModel = 2^12,

    SerializeCsv = 2^13,

    SerializeJson = 2^14,

    RepoStatic = 2^15,

    RepoDynamic = 2^16,

    RepoBulkCopy = 2^17,

    RepoDapper = 2^18,

    RepoEfFluentApi = 2^19,

    Service = 2^20,

    ApiController = 2^21,
  }
}
