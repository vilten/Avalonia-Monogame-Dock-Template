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

    // Nastavenie spr�vania aplik�cie pri jej inicializ�cii
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Vytvor�me a nastav�me hlavn� okno cez n� Prism kontajner.
            desktop.MainWindow = new MainWindow()
            {
                DataContext = new MonoGameViewModel(),
                Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://ToonFlick/Assets/Icons/icon-anim.png")))
            };
        }

        // Zavolanie z�kladnej implement�cie po inicializ�cii
        base.OnFrameworkInitializationCompleted();

        // Povolenie n�strojov pre ladenie v debug re�ime
#if DEBUG
        this.AttachDevTools();
#endif
    }
}
