namespace SpotWelder.Ui
{
  public partial class App
  {
    private const string LoremIpsum = """
                                      Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
                                      Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. 
                                      Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris 
                                      nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in 
                                      reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
                                        Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
                                      Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. 
                                      Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris 
                                      nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in 
                                      reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.
                                      """;

    private static void Debug_ResultWindow()
    {
      var win = new ResultWindow("Test", LoremIpsum);

      win.Show();
    }

    private static void Debug_ParentResultWindow()
    {
      var win = new ParentResultsWindow();

      win.AddTab("Test", LoremIpsum, string.Empty);

      win.Show();
    }

    private static void Debug_YamlBulkGen(ref string[] args)
    {
      args = ["C:\\Dev\\spot-welder\\SpotWelder.IntegrationTests\\single-query.yaml"];
    }
  }
}
