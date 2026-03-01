using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpotWelder.Lib
{
  public static class Utils
  {
    public static bool HasAnyFlag(this GenerationElections elections, params GenerationElections[] flags)
      => flags.Any(f => elections.HasFlag(f));

    public static List<T> GetFlags<T>(this T enumValue) where T : Enum
    {
      var flags = new List<T>();

      foreach (T value in Enum.GetValues(typeof(T)))
      {
        if (enumValue.HasFlag(value) && Convert.ToInt64(value) != 0) // Exclude the 'None' value, assuming it's represented by 0
        {
          flags.Add(value);
        }
      }

      return flags;
    }

    public static Dictionary<string, T> GetEnumDictionary<T>(bool? keyIsLowerCase = null)
      where T : struct, IConvertible
    {
      if (!typeof(T).IsEnum)
        throw new ArgumentException("T must be an enumerated type");

      var t = typeof(T);

      var names = Enum.GetNames(t);

      if (keyIsLowerCase.HasValue)
      {
        Func<string, string> f;

        if (keyIsLowerCase.Value)
          f = s => s.ToLower();
        else
          f = s => s.ToUpper();

        names = names.Select(x => f(x)).ToArray();
      }

      var values = (T[])Enum.GetValues(t);

      var dict = new Dictionary<string, T>(names.Length);

      for (var i = 0; i < names.Length; i++) dict.Add(names[i], values[i]);

      return dict;
    }

    /// <summary>
    /// Writes the specified text to a file at the given path using UTF-8 encoding, overwriting any existing content.
    /// Specifying in one place how files are saved. C# for example defaults to UTF-8 without BOM.
    /// </summary>
    /// <param name="path">
    /// The file path where the text will be written. If the file does not exist,
    /// it will be created. Cannot be null or an empty string.
    /// </param>
    /// <param name="contents">The text to write to the file. If null, an empty file will be created.</param>
    public static void WriteFile(string path, string? contents)
      => File.WriteAllText(path, contents, new System.Text.UTF8Encoding(false));
  }
}
