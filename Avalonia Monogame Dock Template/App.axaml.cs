using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia_Monogame_Dock_Template.Views;

namespace Avalonia_Monogame_Dock_Template;

public class App : Application
{
    // Inicializácia aplikácie - naèítanie XAML definície
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // Nastavenie správania aplikácie pri jej inicializácii
    public override void OnFrameworkInitializationCompleted()
    {
        // Kontrola, èi aplikácia beží ako desktopová aplikácia
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            // Nastavenie hlavného okna aplikácie s DataContextom
            desktopLifetime.MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(),
                Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://MyApp/Assets/Icons/icon-anim.png")))
            };
        }

        // Kontrola, èi aplikácia beží ako single-view aplikácia (napr. pre mobilné zariadenia)
        if (ApplicationLifetime is ISingleViewApplicationLifetime singleLifetime)
        {
            // Nastavenie hlavného zobrazenia aplikácie s DataContextom
            singleLifetime.MainView = new MainView()
            { DataContext = new MainViewModel() };
        }

        // Zavolanie základnej implementácie po inicializácii
        base.OnFrameworkInitializationCompleted();

        // Povolenie nástrojov pre ladenie v debug režime
#if DEBUG
        this.AttachDevTools();
#endif
    }
}
