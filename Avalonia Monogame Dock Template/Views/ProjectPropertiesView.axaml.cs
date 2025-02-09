using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia_Monogame_Dock_Template.Services;
using Avalonia_Monogame_Dock_Template.Services.ContentGenerator;
using Avalonia_Monogame_Dock_Template.ViewModels;

namespace Avalonia_Monogame_Dock_Template.Views;

public partial class ProjectPropertiesView : UserControl
{
    public ProjectFormViewModel ProjectFormViewModel { get; }

    public ProjectPropertiesView()
    {
        InitializeComponent();
        ProjectFormViewModel = new ProjectFormViewModel();
        DataContext = ProjectFormViewModel;

        WrapPanel wrapPanel = new WrapPanel()
        {
            Margin = Thickness.Parse("15"),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
        };

        // VytvorÌme inötanciu modelu.
        var model = new ProjectPropertiesModel();

        // VytvorÌme inötanciu naöej sluûby a vygenerujeme dynamick˝ UserControl.
        var generator = new DynamicUserControlGeneratorService();
        var dynamicControl = generator.GenerateUserControlForModel(model);

        // MÙûete nastaviù generated control ako obsah okna, alebo ho vloûiù do konkrÈtneho kontajnera.
        this.Content = dynamicControl;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}