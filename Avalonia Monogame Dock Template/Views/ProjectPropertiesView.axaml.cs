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

        // Vytvor�me in�tanciu modelu.
        var model = new ProjectPropertiesModel();

        // Vytvor�me in�tanciu na�ej slu�by a vygenerujeme dynamick� UserControl.
        var generator = new DynamicUserControlGeneratorService();
        var dynamicControl = generator.GenerateUserControlForModel(model);

        // M��ete nastavi� generated control ako obsah okna, alebo ho vlo�i� do konkr�tneho kontajnera.
        this.Content = dynamicControl;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}