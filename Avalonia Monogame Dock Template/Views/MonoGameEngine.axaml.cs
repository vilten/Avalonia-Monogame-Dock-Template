using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia_Monogame_Dock_Template.Events.Project;
using Avalonia_Monogame_Dock_Template.Events;
using Avalonia_Monogame_Dock_Template.ViewModels;
using Avalonia.Media;
using IconPacks.Avalonia.Material;
using Avalonia.Media.Imaging;
using Avalonia;
using IconPacks.Avalonia.Material.Converter;
using IconPacks.Avalonia.PhosphorIcons;
using IconPacks.Avalonia.PhosphorIcons.Converter;
using IconPacks.Avalonia.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Avalonia.Controls.Shapes;
using Avalonia_Monogame_Dock_Template.Controls;

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
            ViewModel.Game.OnPointerMoved(new Avalonia.Point(-1, -1));
            GlobalMessageBus.Instance.Listen<PackIconPhosphorIconsKind>().Subscribe(evt =>
            {
                Cursor = CursorManager.GetCursor(evt);
            });

        }


        private void UserControl_PointerLeave(object? sender, PointerEventArgs e)
        {
            ViewModel.Game.OnPointerMoved(new Avalonia.Point(-1, -1));
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
