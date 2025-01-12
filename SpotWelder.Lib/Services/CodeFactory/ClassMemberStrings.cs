﻿using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Reflection;
using Microsoft.CSharp;
//using Microsoft.JScript;
using Microsoft.VisualBasic;
using SpotWelder.Lib.Models;

namespace SpotWelder.Lib.Services.CodeFactory
{
  public class ClassMemberStrings
  {
    private readonly CodeDomProvider _provider;

    public ClassMemberStrings(PropertyInfo property)
      : this(new SchemaColumn(property))
    {
    }

    public ClassMemberStrings(SchemaColumn sc, CodeType type = CodeType.CSharp)
    {
      switch (type)
      {
        case CodeType.CSharp:
          _provider = new CSharpCodeProvider();

          break;

        case CodeType.VbNet:
          _provider = new VBCodeProvider();

          break;

        case CodeType.JavaScript:
          //_provider = new JScriptCodeProvider(); //Not sure if I really need this anymore

          break;

        case CodeType.TypeScript:
          //_provider = new TypeScriptCodeProvider();
          break;
      }

      DatabaseTypeName = sc.SqlType.ToLower(); //Case is inconsistent, so making it lower on purpose

      DatabaseType = TypesService.SqlTypes[DatabaseTypeName];

      Size = sc.Size;

      Precision = sc.Precision;

      Scale = sc.Scale;

      IsPrimaryKey = sc.IsPrimaryKey;

      IsIdentity = sc.IsIdentity;

      IsDbNullable = sc.IsDbNullable;

      IsImplicitlyNullable =
        sc.SystemType == typeof(string) ||
        sc.SystemType.BaseType == typeof(Array);

      //Remove unnecessary extra padding if it shows up
      var trimmedColumnName = sc.ColumnName.Trim();

      SetPropertyAndField(trimmedColumnName);

      SetColumnName(trimmedColumnName);

      SystemType = sc.SystemType;

      SystemTypeName = sc.SystemType.Name;

      //Getting the system type as the alias.
      //Removes the "System." prefix if it exists and sets the "InSystemNamespace" to true if found.
      SystemTypeAlias = GetSystemTypeAsAlias(sc.SystemType, IsImplicitlyNullable, IsDbNullable);

      ConversionMethodSignature = GetConversionMethodSignature(sc.SystemType, SystemTypeName);

      TypeScriptType = GetTypeScriptType(sc.SystemType);
    }

    //For cloning only, bypasses all of the logic and is a straight copy
    private ClassMemberStrings(ClassMemberStrings source, CodeDomProvider provider)
    {
      _provider = provider;

      //Sorted in the the order of property appearance
      ColumnName = source.ColumnName;
      DatabaseTypeName = source.DatabaseTypeName;
      DatabaseType = source.DatabaseType;
      Size = source.Size;
      Precision = source.Precision;
      Scale = source.Scale;
      IsPrimaryKey = source.IsPrimaryKey;
      IsIdentity = source.IsIdentity;
      IsDbNullable = source.IsDbNullable;
      IsImplicitlyNullable = source.IsImplicitlyNullable;
      InSystemNamespace = source.InSystemNamespace;
      Field = source.Field;
      Parameter = source.Parameter;
      Property = source.Property;
      SystemType = source.SystemType;
      SystemTypeAlias = source.SystemTypeAlias;
      SystemTypeName = source.SystemTypeName;
      ConversionMethodSignature = source.ConversionMethodSignature;
      TypeScriptType = source.TypeScriptType;
    }

    /// <summary>Qualified SQL Column name</summary>
    public string ColumnName { get; private set; }

    /// <summary>SQL Server database type name in lower case</summary>
    public string DatabaseTypeName { get; }

    /// <summary>SQL Server database type enumeration</summary>
    public SqlDbType DatabaseType { get; }

    /// <summary>Column size for varchar, nvarchar, char, nchar etc...</summary>
    public int Size { get; }

    /// <summary>Column precision for numeric types such as decimal(p,s)</summary>
    public int Precision { get; }

    /// <summary>Column scale for numeric types such as decimal(p,s) and for datetime2(s)</summary>
    public int Scale { get; }

    /// <summary>Column is the Primary Key for the table.</summary>
    /// <example>ColumnA INT NOT NULL CONSTRAINT [PK_dbo.TableA_ColumnA] PRIMARY KEY([ColumnA])</example>
    public bool IsPrimaryKey { get; }

    /// <summary>Column is an Identity column where a value is auto-determined on insert.</summary>
    /// <example>ColumnA INT IDENTITY(1,1) NOT NULL</example>
    public bool IsIdentity { get; }

    /// <summary>Column is nullable.</summary>
    /// <example>ColumnA INT NULL</example>
    public bool IsDbNullable { get; }

    /// <summary>Property is nullable.</summary>
    /// <remarks>This is a synonym for <see cref="IsDbNullable"/></remarks>
    public bool IsNullable => IsDbNullable;

    //TODO: This is for older version of C#, now things such as `string?` are permitted
    /// <summary>
    /// Some code types are nullable without having to qualify it with a question mark
    /// in front of the type. In other words don't add a question mark to types that
    /// are already nullable such as strings.
    /// </summary>
    public bool IsImplicitlyNullable { get; }

    public bool InSystemNamespace { get; private set; }

    public string Field { get; private set; }

    /// <summary>Parameter name in code. Such that it is formatted in camelCase.</summary>
    public string Parameter { get; private set; }

    /// <summary>Property name in code. Such that it is formatted in PascalCase.</summary>
    public string Property { get; private set; }

    public Type SystemType { get; }

    /// <summary>Property's System Type in code from typeof(T). This is the alias of the type.</summary>
    public string SystemTypeAlias { get; }

    /// <summary>
    /// Property's System Type in code from typeof(T). This is the full class name of the type, not the alias.
    /// Does not include the "System." namespace as a prefix.
    /// </summary>
    public string SystemTypeName { get; }

    /// <summary>The C# method signature for converting one type into another.</summary>
    public string ConversionMethodSignature { get; }

    /// <summary>The Type used in TypeScript.</summary>
    public string TypeScriptType { get; set; }

    private string GetSystemTypeAsAlias(Type target, bool isImplicitlyNullable, bool isDbNullable)
    {
      //This returns the alias if there is one
      var str = _provider.GetTypeOutput(new CodeTypeReference(target)); 

      //This removes the possibility of knowing what the base type is, but in code this is what is needed
      //If this column is nullable then mark it with a question mark
      if (!isImplicitlyNullable && isDbNullable)
        str += "?";

      if (!str.StartsWith("System.")) return str;

      //This is a side-effect, but for now I will let it pass
      InSystemNamespace = true;

      str = str.Replace("System.", string.Empty);

      return str;
    }

    
    private static string GetTypeScriptType(Type target)
    {
      //Example of nullable property in TypeScript
      //property?: string;
      //Example of required property in TypeScript
      //property: string;
      //The question mark is added after the property, not the type, if it is nullable
      
      //TODO: It would be better if there were a stock class that handled this
      //var str = _provider.GetTypeOutput(new CodeTypeReference(target)); //This returns the alias if there is one
      
      return !TypesService.MapSystemToTypeScript.TryGetValue(target, out var str) ? "any" : str;
    }

    //TODO: How to handle arrays? Like blobs from the database?
    private static string GetConversionMethodSignature(Type type, string systemTypeName)
    {
      //Guid does not have a method in the Convert class
      if (type == typeof(Guid))

        //DataReader column has to be converted to string first
        return "Guid.Parse(Convert.ToString({0}))";

      var c = "Convert.To" + systemTypeName + "({0})";

      return c;
    }

    private void SetPropertyAndField(string unqualifiedColumnName)
    {
      //Removing any whitespace
      Property = unqualifiedColumnName.Replace(" ", string.Empty);

      var firstChar = Property.Substring(0, 1);
      var remainder = Property.Substring(1, Property.Length - 1);

      //Pascal Case the property name
      Property = firstChar.ToUpper() + remainder;

      //Camel case the parameter name
      Parameter = firstChar.ToLower() + remainder;

      //Camel case the field name
      Field = "_" + Parameter;
    }

    private void SetColumnName(string trimmedColumnName)
    {
      //Qualifying the column name for SQL
      ColumnName = trimmedColumnName.Contains(" ") ? "[" + trimmedColumnName + "]" : trimmedColumnName;
    }

    public ClassMemberStrings Clone() => new ClassMemberStrings(this, _provider);
  }
}
