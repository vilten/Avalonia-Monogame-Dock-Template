using System;
using Avalonia;
using Avalonia_Monogame_Dock_Template.Services;
using Avalonia_Monogame_Dock_Template.Services.Plugins;
using Dock.Model.Avalonia;
using Splat;

namespace Avalonia_Monogame_Dock_Template
{
    internal class Program
    {
        [STAThread] // Zabezpečuje, že hlavné vlákno aplikácie bude pracovať v STA (Single Threaded Apartment) režime, čo je potrebné pre UI aplikácie
        private static void Main(string[] args)
        {
            RegisterServices();
            // Inicializuje a spustí Avalonia aplikáciu v desktopovom režime
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        private static void RegisterServices()
        {
            // Zaregistrujeme IProjectService => ProjectService ako singleton
            Locator.CurrentMutable.RegisterLazySingleton<IProjectService>(() => new ProjectService());
            Locator.CurrentMutable.RegisterLazySingleton<IPluginsService>(() => new PluginsService());

            // Prípadne iné služby...
            // Locator.CurrentMutable.RegisterLazySingleton<IOtherService>(() => new OtherService());
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            // Zabráni garbage collectoru (GC) uvoľniť Assembly obsahujúcu Factory, čím predchádza možným runtime chybám
            GC.KeepAlive(typeof(Factory).Assembly);

            // Konfiguruje Avalonia aplikáciu:
            return AppBuilder.Configure<App>()
                .UsePlatformDetect() // Automaticky detekuje platformu a použije vhodné UI backendy
                .LogToTrace(); // Povolenie logovania udalostí pre debugovanie
        }
    }
}