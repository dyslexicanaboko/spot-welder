using System.Collections.Generic;
using SpotWelder.Lib.Services.CodeFactory;

namespace SpotWelder.Lib.Models
{
  /// <summary> Instructions for generating DTOs. </summary>
  public class DtoInstructions
  {
    //Right now I am going to stop at top level implementation, no automatic nested implementations.
    //None of what is produced has to compile as far as I am concerned.
    //public bool ExcludeCollections { get; set; } //Might still need this... not sure yet

    /// <summary>
    /// In this context, the class name is the source class name that was provided for generation.
    /// This is different subject in that it is actually a class name, not just a subject.
    /// </summary>
    public string SourceClassName { get; set; }
    
    public CodeType Languages { get; set; } = CodeType.None;

    public GenerationElections Elections { get; set; } = GenerationElections.None;

    public IList<ClassMemberStrings> Properties { get; set; } = new List<ClassMemberStrings>();
  }
}
