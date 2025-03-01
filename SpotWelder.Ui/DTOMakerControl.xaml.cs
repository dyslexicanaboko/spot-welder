﻿using Microsoft.Win32;
using SpotWelder.Lib;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Models.Meta;
using SpotWelder.Lib.Services;
using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Ui.Helpers;
using SpotWelder.Ui.Services;
using SpotWelder.Ui.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SpotWelder.Ui
{
  /// <summary>
  ///   Interaction logic for DtoMakerControl.xaml
  /// </summary>
  public partial class DtoMakerControl : UserControl, IUsesResultWindow
  {
    private const string TxtFullyQualifiedClassName_GhostText = "Caution: Everything is loaded by default...";

    private const string TxtClassSourceCode_GhostText = "Optional: Enter source code for one class here";

    private readonly Brush _dragAndDropTargetBackgroundOriginal;

    private readonly ResultWindowManager _resultWindowManager;

    private ICSharpCompilerService _compilerService;

    private IDtoGenerator _generator;

    private IQueryToClassService _queryToClassService;

    private ObservableCollection<MetaAssemblyViewModel> _treeViewItemSource;

    private IMetaViewModelService _viewModelService;

    private readonly Dictionary<GenerationElections, CheckBox> _electionToCheckBoxMap;

    public DtoMakerControl()
    {
      InitializeComponent();

      //Lock in what the background color is at start
      _dragAndDropTargetBackgroundOriginal = DragAndDropTarget.Background;

      _resultWindowManager = new ResultWindowManager();

      ResetInputs();
      
      _electionToCheckBoxMap = new Dictionary<GenerationElections, CheckBox>
      {
        { GenerationElections.GenerateInterface, CbExtractInterface },
        { GenerationElections.GenerateEntityIEquatable, CbImplementIEquatableOfTInterface },
        { GenerationElections.MapEntityToModel, CbMethodEntityToDto },
        { GenerationElections.MapModelToEntity, CbMethodDtoToEntity },
        { GenerationElections.GenerateEntityAsJavaScript, CbEquivalentJavaScript },
        { GenerationElections.GenerateEntityAsTypeScript, CbEquivalentTypeScript }
      };
    }

    public void CloseResultWindows() => _resultWindowManager.CloseAll();

    internal void Dependencies(
      IDtoGenerator generator,
      IMetaViewModelService viewModelService,
      IQueryToClassService queryToClassService,
      ICSharpCompilerService compilerService)
    {
      _generator = generator;
      _viewModelService = viewModelService;
      _queryToClassService = queryToClassService;
      _compilerService = compilerService;
    }

    private void BtnAssemblyOpenDialog_Click(object sender, RoutedEventArgs e)
    {
      var ofd = new OpenFileDialog();

      var ok = ofd.ShowDialog();

      if (!ok.GetValueOrDefault())
        return;

      SetSelectedAssembly(ofd.FileName);
    }

    private void SetSelectedAssembly(string fullFilePath)
    {
      TxtAssemblyFullFilePath.Text = fullFilePath;

      LblAssemblyChosen.Content = Path.GetFileName(fullFilePath);
    }

    private void LblClassName_MouseEnter(object sender, MouseEventArgs e)
    {
      Cursor = Cursors.Help;
    }

    private void LblClassName_MouseLeave(object sender, MouseEventArgs e)
    {
      Cursor = Cursors.Arrow;
    }

    private void LblClassName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      const string strHelp =
        @"A Fully Qualified Class Name is providing the class name with its namespace separated by periods. 
Example: If you have a class named Product, but it exists in My.Project.Business,
then enter: My.Project.Business.Product, in the text box below. 
Please keep in mind casing matters.";

      MessageBox.Show(strHelp, "What is a Fully Qualified Class Name?");
    }

    private void CheckFqdnForGhostText()
    {
      if (TxtFullyQualifiedClassName.Text == TxtFullyQualifiedClassName_GhostText) TxtFullyQualifiedClassName.Clear();
    }

    private void BtnLoadClass_Click(object sender, RoutedEventArgs e)
    {
      CheckFqdnForGhostText();

      LoadClass(true);
    }

    private void LoadClass(bool reloadAssembly)
    {
      try
      {
        if (reloadAssembly) _generator.LoadAssembly(TxtAssemblyFullFilePath.Text);

        var asmData = string.IsNullOrWhiteSpace(TxtFullyQualifiedClassName.Text) ?
          _generator.GetMetaClasses() :
          _generator.GetMetaClassProperties(TxtFullyQualifiedClassName.Text);

        var asmViewModel = _viewModelService.ToViewModel(asmData, CbPropertiesSelectAllToggle);

        _treeViewItemSource = new ObservableCollection<MetaAssemblyViewModel> { asmViewModel };

        //Everything is loaded via XAML bindings
        TvAssembliesAndClasses.ItemsSource = _treeViewItemSource;
        TvAssembliesAndClasses.Focus();
      }
      catch (Exception ex)
      {
        ex.ShowAsErrorMessage();
      }
    }

    private void TreeViewMetaClass_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      var tvi = e.Source as TreeViewItem;

      //TODO: This is still triggering even when double clicking properties - replace with button?
      if (!(tvi?.Header is IMetaClass c)) return;

      TxtFullyQualifiedClassName.Text = c.FullName;

      LoadClass(false);
    }

    //TODO: Many to dos
    /*   primitives, dark blue -> RGB  86, 156, 214 - #569cd6 - visual studio keyword
     *   classes   , teal      -> RGB  78, 201, 176 - #4ec9b0 - estimated
     *   Interfaces, grnyellow -> RGB 184, 215, 163 - #b8d7a3 - estimated
     *   enums     , grnyellow -> RGB 184, 215, 163 - #b8d7a3 - estimated
     *
     * Extract interface option. What other options make sense?
     *
     * Generate DTO button should read from the Tree View to take full advantage of it
     * 
     * How to handle large assemblies?
     *
     * Show progress bar when user clicks to expand a TreeViewItem. Can't find a way to do this, only found OnExpanded event.
     * I need something like "On data loading" or "User expanded TV Item". I don't think it exists.
     *
     * Multi-select is not supported by TreeView.
     * */
    private DtoInstructions GetInstructions(IDtoGenerator generator)
    {
      //Since using the FQDN, need to get class name by itself
      var t = generator.GetClass(TxtFullyQualifiedClassName.Text);

      var p = new DtoInstructions
      {
        SourceClassName = t.Name,
        Elections = 
          GenerationElections.GenerateEntity | 
          GenerationElections.GenerateModel |
          _electionToCheckBoxMap.GetChosenGenerationElections()
      };
      
      //There should only be one assembly and one class loaded
      var electedProperties =
        _treeViewItemSource.Single() //Only one Assembly
          .Classes.Single() //Only one class
          .Properties.Where(x => x.IsChecked) //Elected properties of that class
          .ToList();

      //Get all of the reflected properties as ClassMemberStrings and only return what was elected
      p.Properties = generator.GetProperties(t)
        .Where(x => electedProperties.Exists(e => e.Name == x.Property))
        .ToList();

      return p;
    }

    private void BtnGenerate_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (!_generator.IsLoaded) _generator.LoadAssembly(TxtAssemblyFullFilePath.Text);

        var p = GetInstructions(_generator);

        foreach (var g in _queryToClassService.Generate(p)) _resultWindowManager.Show(g.Filename, g.Contents);
      }
      catch (Exception ex)
      {
        ex.ShowAsErrorMessage();
      }
    }

    // NOTE: Drag and drop won't work when running VS as admin, but it works when running normally.
    // Might want to consider putting a label that shows up when in admin mode.
    private void DragAndDropTarget_OnDrop(object sender, DragEventArgs e)
    {
      try
      {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
          return;

        var arr = e.Data.GetData(DataFormats.FileDrop) as string[];

        if (arr == null) return;

        SetSelectedAssembly(arr.First());
      }
      catch (Exception ex)
      {
        ex.ShowAsErrorMessage();
      }
    }

    private void DragAndDropTarget_OnMouseEnter(object sender, MouseEventArgs e)
      => DragAndDropTarget.Background = Brushes.Yellow;

    private void DragAndDropTarget_OnMouseLeave(object sender, MouseEventArgs e)
      => DragAndDropTarget.Background = _dragAndDropTargetBackgroundOriginal;

    private void BtnHelp_OnClick(object sender, RoutedEventArgs e)
      => LblClassName_MouseDoubleClick(sender, null);

    private void BtnResetTree_OnClick(object sender, RoutedEventArgs e)
      => ResetInputs();

    private void ResetInputs()
    {
      TxtFullyQualifiedClassName.Text = TxtFullyQualifiedClassName_GhostText;

      TxtClassSourceCode.Text = TxtClassSourceCode_GhostText;

      TvAssembliesAndClasses.ItemsSource = Array.Empty<MetaAssembly>();
    }

    private void TxtFullyQualifiedClassName_OnGotFocus(object sender, RoutedEventArgs e)
      => CheckFqdnForGhostText();

    private void TxtClassSourceCode_GotFocus(object sender, RoutedEventArgs e)
    {
      if (TxtClassSourceCode.Text == TxtClassSourceCode_GhostText) TxtClassSourceCode.Clear();
    }

    //sender is the MetaClassViewModel
    //e just says which property changed, in this case it's always "IsChecked"
    private void CbPropertiesSelectAllToggle(object sender, PropertyChangedEventArgs e)
    {
      if (sender == null) return;

      var vmClass = (MetaClassViewModel)sender;

      foreach (var p in vmClass.Properties) p.IsChecked = vmClass.IsChecked;
    }

    private void TxtClassSourceCode_LostFocus(object sender, RoutedEventArgs e)
    {
      try
      {
        var result = _compilerService.Compile(TxtClassSourceCode.Text);

        if (result.CompiledSuccessfully)
        {
          //Point the generator's assembly reference to the fake assembly loaded in memory already. No need to save it to disk.
          _generator.LoadAssembly(result);

          SetSelectedAssembly(result.AssemblyPath);

          //Clear it so that the one class is shown
          TxtFullyQualifiedClassName.Clear();

          LoadClass(false);

          return;
        }

        var problems = string.Join(Environment.NewLine, result.Errors);

        MessageBox.Show(
          $"The compiler didn't like your code:{Environment.NewLine}{problems}",
          "Compile error - use an IDE to debug it");
      }
      catch (Exception ex)
      {
        var error =
          "An exception occurred while attempting to compile the source code you provided. " +
          "Please make sure it works first in LinqPad or something similar before putting it here. " +
          $"This isn't an IDE. Here is the error message that surfaced if it helps:{Environment.NewLine}{ex.Message}";

        MessageBox.Show(error, "Compile error");
      }
    }
  }
}
