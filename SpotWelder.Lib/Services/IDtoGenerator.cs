using System;
using System.Collections.Generic;
using System.Reflection;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Models.Meta;
using SpotWelder.Lib.Services.CodeFactory;

namespace SpotWelder.Lib.Services
{
    public interface IDtoGenerator
    {
        bool IsLoaded { get; }

        Assembly AssemblyReference { get; }

        void LoadAssembly(string assemblyPath);

        void LoadAssembly(CompilerResult dynamicAssembly);

        Type GetClass(string fullyQualifiedClassName);

        IList<ClassMemberStrings> GetProperties(Type metaClass);

        MetaAssembly GetMetaClassProperties(string className);

        MetaAssembly GetMetaClasses();
    }
}