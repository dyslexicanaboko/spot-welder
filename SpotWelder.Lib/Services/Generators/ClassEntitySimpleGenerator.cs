using SpotWelder.Lib.Events;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SpotWelder.Lib.Services.Generators
{
  /// <summary>
  /// This is a one off generator that's not part of the normal generation process.
  /// </summary>
  public class ClassEntitySimpleGenerator
    : GeneratorBase
  {
    public override GenerationElections Election => GenerationElections.None; //Cannot be elected

    public delegate void RowProcessedHandler(object sender, RowProcessedEventArgs e);

    protected override string TemplateName => "EntitySimple.cs.template";

    public event RowProcessedHandler RowProcessed;

    public override GeneratedResult FillTemplate(ClassInstructions instructions)
    {
      var strTemplate = GetTemplate(TemplateName);

      var template = new StringBuilder(strTemplate);

      template.Replace("{{ClassName}}", instructions.EntityName);

      var t = template.ToString();

      t = RemoveExcessBlankSpace(t);

      t = t.Replace("{{Properties}}", FormatProperties(instructions.Properties));

      var r = GetResult(instructions.ClassName);
      r.Contents = t;

      return r;
    }

    public GeneratedResult FillMockDataTemplate(ClassInstructions instructions, DataTable dataTable)
    {
      var lst = new List<string>(dataTable.Rows.Count);

      var cn = instructions.EntityName;

      var count = 0;

      foreach (DataRow r in dataTable.Rows)
      {
        var sb = new StringBuilder();

        sb.AppendLine($"new {cn}")
          .AppendLine("{");

        foreach (DataColumn c in dataTable.Columns)
        {
          var p = instructions.Properties.Single(
            x =>
              x.Property.Equals(c.ColumnName, StringComparison.OrdinalIgnoreCase));

          var value = GetValueString(p, r[c]);

          sb.Append(p.Property).Append(" = ").Append(Convert.ToString(value)).AppendLine(",");
        }

        sb.AppendLine("}");

        lst.Add(sb.ToString());

        RaiseRowProcessedEvent(new RowProcessedEventArgs { Count = ++count, Total = dataTable.Rows.Count });
      }

      var sbFinal = new StringBuilder();

      sbFinal.AppendLine($"var lst = new List<{cn}>")
        .AppendLine("{")
        .Append(string.Join("," + Environment.NewLine, lst))
        .AppendLine("};");

      var result = GetResult(instructions.ClassName);
      result.Contents = sbFinal.ToString();

      return result;
    }

    private static string GetValueString(ClassMemberStrings property, object value)
    {
      if (value == DBNull.Value) return "null";

      var strValue = Convert.ToString(value);

      if (property.SystemType == typeof(string))
        return string.IsNullOrEmpty(strValue) ? "string.Empty" : $"\"{strValue}\"";

      if (property.SystemType == typeof(bool)) return strValue.ToLower();

      if (property.SystemType == typeof(decimal)) return $"{strValue}M";

      if (property.SystemType == typeof(double)) return $"{strValue}D";

      if (property.SystemType == typeof(DateTime)) return $"DateTime.Parse(\"{strValue}\")";

      if (property.SystemType == typeof(Guid)) return $"Guid.Parse(\"{strValue}\")";

      return strValue;
    }

    private void RaiseRowProcessedEvent(RowProcessedEventArgs e) => RowProcessed?.Invoke(this, e);
  }
}
