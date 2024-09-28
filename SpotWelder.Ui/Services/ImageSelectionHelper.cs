using SpotWelder.Lib;
using System;
using System.Windows.Media.Imaging;

namespace SpotWelder.Ui.Services
{
  internal class ImageSelectionHelper
  {
    public static BitmapImage GetConnectionStringLogo(SqlEngine sqlEngine)
    {
      var path = GetConnectionStringLogoPath(sqlEngine);

      var bmp = new BitmapImage();
      bmp.BeginInit();
      bmp.UriSource = new Uri(path, UriKind.Relative);
      bmp.EndInit();

      return bmp;
    }

    public static string GetConnectionStringLogoPath(SqlEngine sqlEngine)
    {
      return sqlEngine switch
      {
        SqlEngine.SqlServer => "images/SqlServer.png",
        SqlEngine.Postgres => "images/Postgres.png",
        _ => "images/Sad shrugging coffee cup.jpg"
      };
    }
  }
}
