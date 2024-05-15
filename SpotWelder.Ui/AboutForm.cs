﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SpotWelder.Ui
{
  internal partial class AboutForm : Form
  {
    public const string DyslexicAppsUrl = "http://obscureproblemsandgotchas.com/dyslexicapps/";

    public AboutForm()
    {
      InitializeComponent();
      Text = $"About {AssemblyTitle}";
      labelProductName.Text = AssemblyProduct;
      labelVersion.Text = $"Version {AssemblyVersion}";
      labelCopyright.Text = AssemblyCopyright;
      labelCompanyName.Text = AssemblyCompany;
      textBoxDescription.Text = AssemblyDescription;
    }

    private void lnkDyslexicApps_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      try
      {
        Process.Start(new ProcessStartInfo
        {
          FileName = DyslexicAppsUrl,
          UseShellExecute = true
        });
      }
      catch (Exception ex)
      {
        ex.ShowAsErrorMessage();
      }
    }

    #region Assembly Attribute Accessors
    public string AssemblyTitle
    {
      get
      {
        var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

        if (attributes.Length > 0)
        {
          var titleAttribute = (AssemblyTitleAttribute)attributes[0];

          if (titleAttribute.Title != "") return titleAttribute.Title;
        }

        return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
      }
    }

    public string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

    public string AssemblyDescription
    {
      get
      {
        var attributes = Assembly.GetExecutingAssembly()
          .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);

        if (attributes.Length == 0) return "";

        return ((AssemblyDescriptionAttribute)attributes[0]).Description;
      }
    }

    public string AssemblyProduct
    {
      get
      {
        var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);

        if (attributes.Length == 0) return "";

        return ((AssemblyProductAttribute)attributes[0]).Product;
      }
    }

    public string AssemblyCopyright
    {
      get
      {
        var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

        if (attributes.Length == 0) return "";

        return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
      }
    }

    public string AssemblyCompany
    {
      get
      {
        var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

        if (attributes.Length == 0) return "";

        return ((AssemblyCompanyAttribute)attributes[0]).Company;
      }
    }
    #endregion
  }
}
