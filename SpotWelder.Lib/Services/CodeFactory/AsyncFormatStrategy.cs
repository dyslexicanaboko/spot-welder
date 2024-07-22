namespace SpotWelder.Lib.Services.CodeFactory;

public class AsyncFormatStrategy
  : AsynchronicityFormatStrategyBase
{
  public override void Configure()
  {
    TaskVoid = "async Task";
    TaskOpen = "async Task<";
    TaskClose = ">";
    Await = "await ";
    TaskMethodOpen = "await ";
    TaskMethodClose = string.Empty;
    AwaitOpen = "(await ";
    AwaitClose = ")";
    AsyncSuffix = "Async";
  }
}