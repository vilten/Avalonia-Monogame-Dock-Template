using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia_Monogame_Dock_Template.ViewModels;

namespace Avalonia_Monogame_Dock_Template.Views;

public partial class FileExplorerView : UserControl
{
    private FileExplorerViewModel _viewModel;
    public FileExplorerView()
    {
        InitializeComponent();
        _viewModel = new FileExplorerViewModel();
        DataContext = _viewModel;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        _viewModel.GetYaml();
    }
}