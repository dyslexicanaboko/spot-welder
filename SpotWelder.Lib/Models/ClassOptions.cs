using System;
using System.Collections.Generic;

namespace SpotWelder.Lib.Models
{
  /// <summary>
  ///   Generation options. Each property should be read as "Generate PropertyName".
  /// </summary>
  public class ClassOptions
  {
    /// <summary>
    /// Name of the target subject for generation. The subject can be a source class, table or query.
    /// Additionally, this name does not have a prefix or suffix such as `Entity`, `Model`, `Dto`, etc.
    /// In other words, it's JUST the name of the subject.
    /// </summary>
    /// <example> Table named: `dbo.Task`, the subject would just be `Task`.</example>
    public string SubjectName { get; set; }

    /// <summary> Generate an entity for the target <see cref="SubjectName"/>. </summary>
    public bool GenerateEntity { get; set; }

    /// <summary> Generate the <see cref="IEquatable{T}"/> interface implementation for the target entity. </summary>
    public bool GenerateEntityIEquatable { get; set; }

    /// <summary> Generate the <see cref="IComparable"/> interface implementation for the target entity. </summary>
    public bool GenerateEntityIComparable { get; set; }

    /// <summary> Generate an <see cref="EqualityComparer{T}"/> class for the target entity. </summary>
    public bool GenerateEntityEqualityComparer { get; set; }

    /// <summary> Name of the subject with the `Entity` suffix.</summary>
    /// <example> Subject named: `Task`, the entity would be `TaskEntity`.</example>
    public string EntityName { get; set; }

    /// <summary> Generate a model for the target <see cref="SubjectName"/>. </summary>
    public bool GenerateModel { get; set; }

    /// <summary> Generate a REST API Create model for the target <see cref="SubjectName"/>. </summary>
    public bool GenerateCreateModel { get; set; }

    /// <summary> Generate a REST API Patch model for the target <see cref="SubjectName"/>. </summary>
    public bool GeneratePatchModel { get; set; }

    /// <summary> Name of the subject with the `Model` suffix.</summary>
    /// <example> Subject named: `Task`, the entity would be `TaskModel`.</example>
    public string ModelName { get; set; }

    /// <summary> Name of the subject with the `I` prefix.</summary>
    /// <example> Subject named: `Task`, the interface would be `ITask`.</example>
    public bool GenerateInterface { get; set; }

    /// <summary>
    /// This is a temporary concept until I can do better. The different languages to
    /// generate something in. Vague explanation for a vague implementation.
    /// </summary>
    public CodeType Languages { get; set; }
  }
}
