//using System.Collections.Generic;

namespace SpotWelder.Lib.Models.Meta
{
    public interface IMetaAssembly
    {
        string Name { get; set; }
        
        //I want to enforce a list of interface, but any inheritors are confined to this exact interface
        //List<IMetaClass> Classes { get; }
    }
}