using System.Text;

namespace SpotWelder.Lib.Services.CodeFactory
{
  public abstract class AsynchronicityFormatStrategyBase
  {
    /// <summary>The `void` return type is replaced with `async Task`</summary>
    public string TaskVoid { get; set; } = string.Empty;

    /// <summary>Return types are wrapped with `async Task<` to start.</summary>
    public string TaskOpen { get; set; } = string.Empty;

    /// <summary>Return types are wrapped with `>` to finish.</summary>
    public string TaskClose { get; set; } = string.Empty;

    /// <summary>
    /// Method calls are either:
    ///   `await Task.FromResult(` for synchronous methods being used in an async context
    ///   `await ` for asynchronous methods in an async context
    /// to start.
    /// </summary>
    public string TaskMethodOpen { get; set; } = string.Empty;

    /// <summary>
    /// Method calls are either:
    ///   `)` for synchronous methods being used in an async context
    ///   `` for asynchronous methods in an async context
    /// to finish.
    /// </summary>
    public string TaskMethodClose { get; set; } = string.Empty;

    /// <summary>Appending the `await ` keyword and space in an asynchronous context.</summary>
    public string Await { get; set; } = string.Empty;

    /// <summary>Method calls that return enumerations are wrapped with `(await ` to start.</summary>
    public string AwaitOpen { get; set; } = string.Empty;

    /// <summary>Method calls that return enumerations are wrapped with `)` to finish before the `.ToList()` extension can be called.</summary>
    public string AwaitClose { get; set; } = string.Empty;

    /// <summary>Some methods requires the `Async` suffix when used in an asynchronous context.</summary>
    public string AsyncSuffix { get; set; } = string.Empty;

    public abstract void Configure();

    /// <summary>Replaces the tags in the string builder with the appropriate values.</summary>
    public virtual StringBuilder ReplaceTags(StringBuilder sb)
      => sb.Replace("[void]", TaskVoid)
        .Replace("[AO]", TaskOpen)
        .Replace("[AC]", TaskClose)
        .Replace("[AS]", AsyncSuffix)
        .Replace("[A]", Await)
        .Replace("[AMO]", TaskMethodOpen)
        .Replace("[AMC]", TaskMethodClose)
        .Replace("[AWO]", AwaitOpen)
        .Replace("[AWC]", AwaitClose);
  }
}
