using SpotWelder.Lib.Models;
using System;
using System.Collections.Generic;
using Child = SpotWelder.Lib.Services.Generators.Elections.GenerationElectionChildAttribute;
using Ignore = SpotWelder.Lib.Services.Generators.Elections.GenerationElectionIgnoreAttribute;

namespace SpotWelder.Lib;

//Not every election is tied directly to a generator.
/// <summary>
/// Currently supporting 32 elections. May be expanded in the future.
/// The value zero indicates that nothing was elected.
/// </summary>
[Flags]
public enum GenerationElections : long
{
  [Ignore]
  None = 0,

  /// <summary> Should class methods be asynchronous? </summary>
  [Ignore]
  MakeAsynchronous = 1L << 0,

  /// <summary> Generate an entity for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
  Entity = 1L << 1,

  /// <summary> Generate the <see cref="IEquatable{T}"/> interface implementation for the target entity. </summary>
  [Child(Entity)]
  EntityIEquatable = 1L << 2,

  /// <summary> Generate the <see cref="IComparable"/> interface implementation for the target entity. </summary>
  [Child(Entity)]
  EntityIComparable = 1L << 3,

  /// <summary> Generate an <see cref="EqualityComparer{T}"/> class for the target entity. </summary>
  EntityEqualityComparer = 1L << 4,

  /// <summary> Generate a model for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
  Model = 1L << 5,

  /// <summary> Generate a REST API Create model for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
  CreateModel = 1L << 6,

  /// <summary> Generate a REST API Patch model for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
  PatchModel = 1L << 7,

  /// <summary> Name of the subject with the `I` prefix.</summary>
  /// <example> Subject named: `Task`, the interface would be `ITask`.</example>
  Interface = 1L << 8,

  /// <summary> Dependent on <see cref="Mapper"/> </summary>
  [Child(Mapper)]
  MapEntityToModel = 1L << 9,

  /// <summary> Dependent on <see cref="Mapper"/> </summary>
  [Child(Mapper)]
  MapModelToEntity = 1L << 10,

  //TODO: Am I keeping this? Currently disconnected and unused.
  /// <summary> Dependent on <see cref="Mapper"/> </summary>
  [Child(Mapper)]
  MapInterfaceToEntity = 1L << 11,

  //TODO: Am I keeping this? Currently disconnected and unused.
  /// <summary> Dependent on <see cref="Mapper"/> </summary>
  [Child(Mapper)]
  MapInterfaceToModel = 1L << 12,

  SerializeCsv = 1L << 13,

  SerializeJson = 1L << 14,

  RepoStatic = 1L << 15,

  //TODO: Support for this ended
  [Ignore]
  RepoDynamic = 1L << 16,

  //TODO: Support for this ended
  [Ignore]
  RepoBulkCopy = 1L << 17,

  RepoDapper = 1L << 18,

  //TODO: Support for this ended
  [Ignore]
  RepoEfFluentApi = 1L << 19,

  Manager = 1L << 20,

  ApiController = 1L << 21,

  EntityAsTypeScript = 1L << 22,
    
  EntityAsJavaScript = 1L << 23,
    
  /// <summary> Services all mapper elections. </summary>
  Mapper = 1L << 24,

  /// <summary> Dependent on <see cref="Mapper"/> </summary>
  [Child(Mapper)]
  MapCreateModelToEntity = 1L << 25,

  /// <summary> Dependent on <see cref="Mapper"/> </summary>
  [Child(Mapper)]
  MapPatchModelToEntity = 1L << 26,

  /// <summary> Generate a REST API Created model for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
  CreatedModel = 1L << 27,

  /// <summary> Dependent on <see cref="Mapper"/> </summary>
  [Child(Mapper)]
  MapEntityToCreatedModel = 1L << 28,

  /// <summary> Dependent on <see cref="Mapper"/> </summary>
  [Child(Mapper)]
  MapRecordToEntity = 1L << 29,

  /// <summary> Dependent on <see cref="Mapper"/> </summary>
  [Child(Mapper)]
  MapEntityToRecord = 1L << 30,

  /// <summary> Generate a record for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
  Record = 1L << 31,

  /// <summary> Generate a record for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
  Immutables = 1L << 32,

  Validation = 1L << 33
}
