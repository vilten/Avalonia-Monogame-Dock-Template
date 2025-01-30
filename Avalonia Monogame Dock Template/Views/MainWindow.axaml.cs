using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia_Monogame_Dock_Template.Views;

namespace Avalonia_Monogame_Dock_Template;

public partial class MainWindow : Window
{

    public MainViewModel ViewModel { get; }

    public MainWindow()
    {
        InitializeComponent();

        ViewModel = new MainViewModel();
        DataContext = ViewModel;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
