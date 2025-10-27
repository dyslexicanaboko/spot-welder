using NUnit.Framework;
using SpotWelder.UnitTests.Common;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SpotWelder.IntegrationTests
{
  public abstract class SpotWelderTestBase
    : TestBase
  {
    protected void AssertAreEqualIgnoreWhiteSpace(string expected, string actual)
    {
      var re = new Regex(@"\s+");

      expected = re.Replace(expected, string.Empty);
      actual = re.Replace(actual, string.Empty);

      Assert.That(actual, Is.EqualTo(expected));
    }

    protected void DumpToFile(string expected, string actual)
    {
      //This is for debug only
      DumpFile("Expected.cs", expected);
      DumpFile("Actual.cs", actual);
    }

    protected void DumpFile(string fileName, string contents)
    {
      var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dump");
      
      Directory.CreateDirectory(path);

      path = Path.Combine(path, fileName);

      File.WriteAllText(path, contents);
    }
  }
}
