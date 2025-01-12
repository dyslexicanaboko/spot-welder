﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SpotWelder.Lib
{
  public static class Utils
  {
    public static void LogError(Exception ex, string message = null)
    {
      //Logging
      throw new NotImplementedException("Logging is not implemented anywhere yet.");
    }

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
  }
}
