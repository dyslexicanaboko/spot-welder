namespace SpotWelder.Lib.Services.CodeFactory;

public class SyncFormatStrategy
  : AsynchronicityFormatStrategyBase
{
  public override void Configure()
  {
    TaskVoid = "void";
    TaskMethodOpen = "await Task.FromResult(";
    TaskMethodClose = ")";
  }
}