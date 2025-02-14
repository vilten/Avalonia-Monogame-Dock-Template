using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia_Monogame_Dock_Template.ViewModels;

namespace Avalonia_Monogame_Dock_Template;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // Nastavenie správania aplikácie pri jej inicializácii
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Vytvoríme a nastavíme hlavné okno cez náš Prism kontajner.
            desktop.MainWindow = new MainWindow()
            {
                DataContext = new MonoGameViewModel(),
                Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://ToonFlick/Assets/Icons/icon-anim.png")))
            };
        }

        // Zavolanie základnej implementácie po inicializácii
        base.OnFrameworkInitializationCompleted();

        // Povolenie nástrojov pre ladenie v debug režime
#if DEBUG
        this.AttachDevTools();
#endif
    }
}
