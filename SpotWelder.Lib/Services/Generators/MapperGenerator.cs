using SpotWelder.Lib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
	public class MapperGenerator
		: GeneratorBase
	{
		/// <summary>
		/// In this particular case the mapper accounts for these elections in one shot:
		///   GenerationElections.CloneModelToEntity
		///   GenerationElections.CloneEntityToModel
		///   GenerationElections.CloneInterfaceToEntity
		///   GenerationElections.CloneInterfaceToModel
		/// </summary>
		public override GenerationElections Election => GenerationElections.GenerateMapper;
		
		protected override string TemplateName => "Mapper.cs.template";

		private static readonly Dictionary<GenerationElections, string> ChildTemplates = new()
		{
			{ GenerationElections.MapEntityToModel, "MapEntityToModel.cs.template" },
			{ GenerationElections.MapModelToEntity, "MapModelToEntity.cs.template" },
			{ GenerationElections.MapInterfaceToEntity, "MapInterfaceToEntity.cs.template" },
			{ GenerationElections.MapInterfaceToModel, "MapInterfaceToModel.cs.template" },
			{ GenerationElections.MapCreateModelToEntity, "MapCreateModelToEntity.cs.template" },
			{ GenerationElections.MapPatchModelToEntity, "MapPatchModelToEntity.cs.template" }
		};

	public override GeneratedResult FillTemplate(ClassInstructions instructions)
		{
			instructions.ClassName = instructions.SubjectName;

			var strTemplate = GetTemplate(TemplateName);

			var template = new StringBuilder(strTemplate);
			
			template.Replace("{{Body}}", BuildBodyTemplate(instructions.Elections));
			template.Replace("{{Namespace}}", instructions.Namespace);
			template.Replace("{{ClassName}}", instructions.ClassName);
			template.Replace("{{EntityName}}", instructions.EntityName);
			template.Replace("{{ModelName}}", instructions.ModelName);
			template.Replace("{{InterfaceName}}", instructions.InterfaceName);
			template.Replace("{{Namespaces}}", FormatNamespaces(instructions.Namespaces));
			
			return GetFormattedCSharpResult($"{instructions.ClassName}Mapper.cs", template);
		}

		private string BuildBodyTemplate(GenerationElections elections)
		{
			var childElections = GetChildElections(elections, Election);

			var sb = new StringBuilder();

			foreach (var child in childElections)
			{
				sb
					.AppendLine(GetTemplate(ChildTemplates[child]))
					.AppendLine();
			}

			return sb.ToString();
		}

		//FYI: 07/20/2024 This is a literal clone method, which I don't want to get rid of yet, but I won't be using right now.
		//There is a place for this, I am just not sure where yet. This is more of a DTO Maker thing,
		//but I could see needing this for scaffolding potentially too.
		private string FormatCloneBody(
			GenerationElections flag,
			ClassInstructions instructions,
			string from,
			string to)
		{
			if (!instructions.Elections.HasFlag(flag))
			{
				var exception = GetNotImplementedException(
					$"Cloning option \"{flag}\" was excluded from generation. Delete this method.");

				return exception;
			}

			var content = GetTextBlock(
				instructions.Properties,
				p => $"			{to}.{p.Property} = {from}.{p.Property};",
				Environment.NewLine);

			return content;
		}
	}
}
