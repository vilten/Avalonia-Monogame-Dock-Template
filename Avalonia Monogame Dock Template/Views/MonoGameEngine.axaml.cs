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
                setCursor(evt);
            });

        }

        public static Cursor Create(Visual visual, PixelSize size, PixelPoint hotSpot)
        {
            using var rtb = new RenderTargetBitmap(size, new(96, 96));
            rtb.Render(visual);
            return new Cursor(rtb, hotSpot);
        }

        public void setCursor(PackIconPhosphorIconsKind packIconPhosphorIconsKind)
        {
            string data = null;
            PackIconDataFactory<PackIconPhosphorIconsKind>.DataIndex.Value?.TryGetValue(packIconPhosphorIconsKind, out data);

            var shadowPath = new Path
            {
                Data = StreamGeometry.Parse(data),
                StrokeThickness = 1,
                Fill = Brushes.Black, // Tieň v čiernej farbe
                Stroke = Brushes.Black,
                Opacity = 1, // Polopriehľadný tieň
                RenderTransform = new ScaleTransform(0.9, 0.9)
            };

            var originalPath = new Path
            {
                Data = StreamGeometry.Parse(data),
                StrokeThickness = 0,
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                RenderTransform = new ScaleTransform(0.8,0.8)
            };

            int length = 24;
            var grid = new Grid();
            grid.Children.Add(shadowPath);
            grid.Children.Add(originalPath);

            var vb = new Viewbox
            {
                Width = length,
                Height = length,
                Child = grid
            };
            vb.Measure(new(length, length));
            vb.Arrange(new Avalonia.Rect(0, 0, length, length));
            Cursor = Create(vb, new Avalonia.PixelSize(length, length), new Avalonia.PixelPoint(5, 5));
            //Cursor = new Cursor(standardCursorType);
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
