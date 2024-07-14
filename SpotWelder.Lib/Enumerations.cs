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

  [Flags]
  public enum GenerationElections
  {
    None = 0,

    /// <summary> Should class methods be asynchronous? </summary>
    MakeAsynchronous = 1 << 0,

    /// <summary> Generate an entity for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
    GenerateEntity = 1 << 1,

    /// <summary> Generate the <see cref="IEquatable{T}"/> interface implementation for the target entity. </summary>
    GenerateEntityIEquatable = 1 << 2,

    /// <summary> Generate the <see cref="IComparable"/> interface implementation for the target entity. </summary>
    GenerateEntityIComparable = 1 << 3,

    /// <summary> Generate an <see cref="EqualityComparer{T}"/> class for the target entity. </summary>
    GenerateEntityEqualityComparer = 1 << 4,

    /// <summary> Generate a model for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
    GenerateModel = 1 << 5,

    /// <summary> Generate a REST API Create model for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
    GenerateCreateModel = 1 << 6,

    /// <summary> Generate a REST API Patch model for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
    GeneratePatchModel = 1 << 7,

    /// <summary> Name of the subject with the `I` prefix.</summary>
    /// <example> Subject named: `Task`, the interface would be `ITask`.</example>
    GenerateInterface = 1 << 8,

    /// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
    CloneEntityToModel = 1 << 9,

    /// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
    CloneModelToEntity = 1 << 10,

    /// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
    CloneInterfaceToEntity = 1 << 11,

    /// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
    CloneInterfaceToModel = 1 << 12,

    SerializeCsv = 1 << 13,

    SerializeJson = 1 << 14,

    RepoStatic = 1 << 15,

    RepoDynamic = 1 << 16,

    RepoBulkCopy = 1 << 17,

    RepoDapper = 1 << 18,

    RepoEfFluentApi = 1 << 19,

    Service = 1 << 20,

    ApiController = 1 << 21,

    GenerateEntityAsTypeScript = 1 << 22,
    
    GenerateEntityAsJavaScript = 1 << 23,
    
    /// <summary> Services all of the clone elections. </summary>
    GenerateMapper = 1 << 24,
  }
}
