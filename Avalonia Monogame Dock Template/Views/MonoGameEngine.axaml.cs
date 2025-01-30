using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia_Monogame_Dock_Template.Views
{
    public partial class MonoGameEngine : UserControl
    {
        public Views.MainViewModel ViewModel { get; }

        public MonoGameEngine()
        {
            InitializeComponent();

            ViewModel = new Views.MainViewModel();
            DataContext = ViewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
