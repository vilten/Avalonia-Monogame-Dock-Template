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
    // Inicializ�cia aplik�cie - na��tanie XAML defin�cie
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // Nastavenie spr�vania aplik�cie pri jej inicializ�cii
    public override void OnFrameworkInitializationCompleted()
    {
        // Kontrola, �i aplik�cia be�� ako desktopov� aplik�cia
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            // Nastavenie hlavn�ho okna aplik�cie s DataContextom
            desktopLifetime.MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(),
                Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://MyApp/Assets/Icons/icon-anim.png")))
            };
        }

        // Kontrola, �i aplik�cia be�� ako single-view aplik�cia (napr. pre mobiln� zariadenia)
        if (ApplicationLifetime is ISingleViewApplicationLifetime singleLifetime)
        {
            // Nastavenie hlavn�ho zobrazenia aplik�cie s DataContextom
            singleLifetime.MainView = new MainView()
            { DataContext = new MainViewModel() };
        }

        // Zavolanie z�kladnej implement�cie po inicializ�cii
        base.OnFrameworkInitializationCompleted();

        // Povolenie n�strojov pre ladenie v debug re�ime
#if DEBUG
        this.AttachDevTools();
#endif
    }
}
