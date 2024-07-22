using SpotWelder.Lib.Models;
using System;
using System.Collections.Generic;
using Child = SpotWelder.Lib.Services.Generators.Elections.GenerationElectionChildAttribute;
using Ignore = SpotWelder.Lib.Services.Generators.Elections.GenerationElectionIgnoreAttribute;

namespace SpotWelder.Lib;

//Not every election is tied directly to a generator.
[Flags]
public enum GenerationElections
{
  [Ignore]
  None = 0,

  /// <summary> Should class methods be asynchronous? </summary>
  [Ignore]
  MakeAsynchronous = 1 << 0,

  /// <summary> Generate an entity for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
  GenerateEntity = 1 << 1,

  /// <summary> Generate the <see cref="IEquatable{T}"/> interface implementation for the target entity. </summary>
  [Child(GenerateEntity)]
  GenerateEntityIEquatable = 1 << 2,

  /// <summary> Generate the <see cref="IComparable"/> interface implementation for the target entity. </summary>
  [Child(GenerateEntity)]
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
  [Child(GenerateMapper)]
  MapEntityToModel = 1 << 9,

  /// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
  [Child(GenerateMapper)]
  MapModelToEntity = 1 << 10,

  /// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
  [Child(GenerateMapper)]
  MapInterfaceToEntity = 1 << 11,

  /// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
  [Child(GenerateMapper)]
  MapInterfaceToModel = 1 << 12,

  SerializeCsv = 1 << 13,

  SerializeJson = 1 << 14,

  RepoStatic = 1 << 15,

  [Ignore]
  RepoDynamic = 1 << 16,

  [Ignore]
  RepoBulkCopy = 1 << 17,

  RepoDapper = 1 << 18,

  [Ignore]
  RepoEfFluentApi = 1 << 19,

  Service = 1 << 20,

  ApiController = 1 << 21,

  GenerateEntityAsTypeScript = 1 << 22,
    
  GenerateEntityAsJavaScript = 1 << 23,
    
  /// <summary> Services all of the mapper elections. </summary>
  GenerateMapper = 1 << 24,

  /// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
  [Child(GenerateMapper)]
  MapCreateModelToEntity = 1 << 25,

  /// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
  [Child(GenerateMapper)]
  MapPatchModelToEntity = 1 << 26,
}
