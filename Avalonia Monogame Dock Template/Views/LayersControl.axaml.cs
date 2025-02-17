using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia_Monogame_Dock_Template.ViewModels;

namespace Avalonia_Monogame_Dock_Template.Views;

public partial class LayersControl : UserControl
{
    private LayersViewModel _viewModel;
    public LayersControl()
    {
        InitializeComponent();
        _viewModel = new LayersViewModel();
        DataContext = _viewModel;

    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void AddLayer_OnClick(object? sender, RoutedEventArgs e)
    {
        _viewModel.AddLayer();
    }

    private void RemoveLayer_OnClick(object? sender, RoutedEventArgs e)
    {
        _viewModel.RemoveLayer();
    }

    private void MoveUpLayer_OnClick(object? sender, RoutedEventArgs e)
    {
        _viewModel.MoveLayerUp();
    }

    private void MoveDownLayer_OnClick(object? sender, RoutedEventArgs e)
    {
        _viewModel.MoveLayerDown();
    }

}