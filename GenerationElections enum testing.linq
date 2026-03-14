<Query Kind="Program" />

void Main()
{
	GenerationElections g = GenerationElections.ApiController;
	
	EnsureSingleElection(g);
}

private static void Test1()
{
	GenerationElections g = GenerationElections.None;

	g |= GenerationElections.GenerateEntity | GenerationElections.GenerateMapper | GenerationElections.GenerateModel;
	g |= GenerationElections.ApiController | GenerationElections.SerializeCsv;

	g.Dump();
}

public static void EnsureSingleElection(GenerationElections election)
{
	if (election == GenerationElections.None)
		throw new ArgumentException($"One election must be selected. Value was: {election}");

	if ((election & (election - 1)) != 0)
		throw new ArgumentException($"Only one election may be selected. Value was: {election}");
}

// You can define other methods, fields, classes and namespaces here
[Flags]
public enum GenerationElections : long
{
	[Ignore]
	None = 0,

	/// <summary> Should class methods be asynchronous? </summary>
	[Ignore]
	MakeAsynchronous = 1L << 0,

	/// <summary> Generate an entity for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
	GenerateEntity = 1L << 1,

	/// <summary> Generate the <see cref="IEquatable{T}"/> interface implementation for the target entity. </summary>
	[Child(GenerateEntity)]
	GenerateEntityIEquatable = 1L << 2,

	/// <summary> Generate the <see cref="IComparable"/> interface implementation for the target entity. </summary>
	[Child(GenerateEntity)]
	GenerateEntityIComparable = 1L << 3,

	/// <summary> Generate an <see cref="EqualityComparer{T}"/> class for the target entity. </summary>
	GenerateEntityEqualityComparer = 1L << 4,

	/// <summary> Generate a model for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
	GenerateModel = 1L << 5,

	/// <summary> Generate a REST API Create model for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
	GenerateCreateModel = 1L << 6,

	/// <summary> Generate a REST API Patch model for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
	GeneratePatchModel = 1L << 7,

	/// <summary> Name of the subject with the `I` prefix.</summary>
	/// <example> Subject named: `Task`, the interface would be `ITask`.</example>
	GenerateInterface = 1L << 8,

	/// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
	[Child(GenerateMapper)]
	MapEntityToModel = 1L << 9,

	/// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
	[Child(GenerateMapper)]
	MapModelToEntity = 1L << 10,

	//TODO: Am I keeping this? Currently disconnected and unused.
	/// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
	[Child(GenerateMapper)]
	MapInterfaceToEntity = 1L << 11,

	//TODO: Am I keeping this? Currently disconnected and unused.
	/// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
	[Child(GenerateMapper)]
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

	GenerateEntityAsTypeScript = 1L << 22,

	GenerateEntityAsJavaScript = 1L << 23,

	/// <summary> Services all mapper elections. </summary>
	GenerateMapper = 1L << 24,

	/// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
	[Child(GenerateMapper)]
	MapCreateModelToEntity = 1L << 25,

	/// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
	[Child(GenerateMapper)]
	MapPatchModelToEntity = 1L << 26,

	/// <summary> Generate a REST API Created model for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
	GenerateCreatedModel = 1L << 27,

	/// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
	[Child(GenerateMapper)]
	MapEntityToCreatedModel = 1L << 28,

	/// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
	[Child(GenerateMapper)]
	MapRecordToEntity = 1L << 29,

	/// <summary> Dependent on <see cref="GenerateMapper"/> </summary>
	[Child(GenerateMapper)]
	MapEntityToRecord = 1L << 30,

	/// <summary> Generate a record for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
	GenerateRecord = 1L << 31,

	/// <summary> Generate a record for the target <see cref="ClassInstructions.SubjectName"/>. </summary>
	GenerateImmutables = 1L << 32,

	GenerateValidation = 1L << 33
}

public class ChildAttribute : Attribute
{
	public GenerationElections Parent { get; }

	public ChildAttribute(GenerationElections parent)
	{
		Parent = parent;
	}
}

public class IgnoreAttribute : Attribute
{

}