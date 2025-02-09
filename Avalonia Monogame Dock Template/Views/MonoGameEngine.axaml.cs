using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia_Monogame_Dock_Template.ViewModels;

namespace Avalonia_Monogame_Dock_Template.Views
{
    public partial class MonoGameEngine : UserControl
    {
        public MonoGameViewModel ViewModel { get; }

        public MonoGameEngine()
        {
            InitializeComponent();

            ViewModel = new MonoGameViewModel();
            DataContext = ViewModel;
            this.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
            this.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
            this.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel);

        }

        private void OnPointerMoved(object sender, PointerEventArgs e)
        {
            ViewModel.Game.OnPointerMoved(e.GetPosition(this));
        }

        private void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            ViewModel.Game.OnMousePressed();
        }

        private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            ViewModel.Game.OnMouseReleased();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
