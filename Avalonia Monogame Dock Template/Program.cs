using System;
using Avalonia;
using Dock.Model.Avalonia;

namespace Avalonia_Monogame_Dock_Template
{
    internal class Program
    {
        [STAThread] // Zabezpečuje, že hlavné vlákno aplikácie bude pracovať v STA (Single Threaded Apartment) režime, čo je potrebné pre UI aplikácie
        private static void Main(string[] args)
        {
            // Inicializuje a spustí Avalonia aplikáciu v desktopovom režime
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
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