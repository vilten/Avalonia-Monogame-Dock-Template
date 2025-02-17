using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Avalonia_Monogame_Dock_Template.Views;

public partial class NodifyMView : UserControl
{
    public NodifyMView()
    {
        InitializeComponent();
        DataContext = new NodifyMViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}