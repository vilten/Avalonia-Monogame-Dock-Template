using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia_Monogame_Dock_Template.Events;
using Avalonia_Monogame_Dock_Template.Models;
using Avalonia_Monogame_Dock_Template.Models.Wrapper;
using Avalonia_Monogame_Dock_Template.Services.Plugins;
using Avalonia_Monogame_Dock_Template.ViewModels;
using IconPacks.Avalonia.PhosphorIcons;
using Splat;

namespace Avalonia_Monogame_Dock_Template.Views
{
    public partial class RightToolsPanel : UserControl
    {
        private readonly IPluginsService _pluginsService;
        private RightToolsViewModel _rightToolsViewModel;
        private readonly RightToolsPanelWrapper _rightToolsPanelWrapper;
        public RightToolsPanel()
        {
            _pluginsService = Locator.Current.GetService<IPluginsService>() ?? throw new InvalidOperationException("IPluginsService not registered");

            InitializeComponent();
            _rightToolsViewModel = new RightToolsViewModel();
            DataContext = _rightToolsViewModel;
            this.AttachedToVisualTree += RightToolsPanel_Loaded;
            _rightToolsPanelWrapper = new RightToolsPanelWrapper(this);
        }

        private void RightToolsPanel_Loaded(object? sender, VisualTreeAttachmentEventArgs e)
        {
            foreach (var plugin in _pluginsService.EnginePlugins)
            {
                plugin.RegisterToolGroups(_rightToolsPanelWrapper);
            }
            foreach (var plugin in _pluginsService.EnginePlugins)
            {
                plugin.RegisterButtonInGroup(_rightToolsPanelWrapper);
            }

            WrapPanel wrapPanel = new WrapPanel()
            {
                Orientation = Orientation.Horizontal,
                Width = 108f
            };
            foreach (var group in _rightToolsViewModel.groups)
            {
                TextBlock textBlock = new TextBlock()
                {
                    Text = group.Key,
                    Width = 108f,
                    FontSize = 12,
                    Background = Brush.Parse("#202020")
                };
                wrapPanel.Children.Add(textBlock);

                foreach (var toolButton in group.Value)
                {
                    Button newButton = new Button()
                    {
                        FontSize = 12,
                        Padding = Thickness.Parse("10 2 10 2"),
                        Margin = Thickness.Parse("0 5"),
                        Content = new PackIconPhosphorIcons
                        {

                            Kind = toolButton.Icon, // Nastavenie ikony
                            Width = 12,
                            Height = 16
                        },
                        Background = Brushes.Transparent,
                        CommandParameter = toolButton.MessageType,
                        Command = new DelegateCommand(() =>
                        {
                            GlobalMessageBus.Instance.SendMessage<object>(toolButton.MessageType);
                        })
                    };
                    ToolTip.SetTip(newButton, toolButton.Name);
                    wrapPanel.Children.Add(newButton);
                }
            }
            this.Content = wrapPanel;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        public void RegisterGroup(String name)
        {
            if (!_rightToolsViewModel.groups.ContainsKey(name))
                _rightToolsViewModel.groups.Add(name, new List<ToolButton>());
        }

        public void RegisterButtonInGroup(String groupName, ToolButton toolButton)
        {
            if (_rightToolsViewModel.groups.ContainsKey(groupName))
            {
                _rightToolsViewModel.groups[groupName].Add(toolButton);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ClickToolButton(object? sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"RightToolsPanel attached, running on UI thread: {Dispatcher.UIThread.CheckAccess()}");
        }
    }
    public class DelegateCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged;
    }
}
