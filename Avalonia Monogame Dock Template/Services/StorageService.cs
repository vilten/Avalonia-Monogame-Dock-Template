using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;

namespace Avalonia_Monogame_Dock_Template.Services;

/// <summary>
/// Poskytuje služby pre výber a manipuláciu so súbormi v aplikácii Avalonia.
/// </summary>
internal static class StorageService
{
    /// <summary>
    /// Definuje typ súborov pre výber všetkých súborov (*.*).
    /// </summary>
    public static FilePickerFileType All { get; } = new("All")
    {
        Patterns = new[] { "*.*" }, // Všetky súbory
        MimeTypes = new[] { "*/*" } // Všetky MIME typy
    };

    /// <summary>
    /// Definuje typ súborov pre výber JSON súborov (*.json).
    /// </summary>
    public static FilePickerFileType Json { get; } = new("Json")
    {
        Patterns = new[] { "*.json" }, // JSON súbory
        AppleUniformTypeIdentifiers = new[] { "public.json" }, // Apple UTI pre JSON
        MimeTypes = new[] { "application/json" } // MIME typ pre JSON
    };

    /// <summary>
    /// Získa poskytovateľa úložiska (StorageProvider) na manipuláciu so súbormi.
    /// </summary>
    /// <returns>Vracia <see cref="IStorageProvider"/> alebo <c>null</c>, ak nie je dostupný.</returns>
    public static IStorageProvider? GetStorageProvider()
    {
        // Ak je aplikácia v režime desktopovej aplikácie (napr. Windows, Linux, MacOS)
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window })
        {
            return window.StorageProvider;
        }

        // Ak je aplikácia v režime single-view (napr. v mobilných aplikáciách)
        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime { MainView: { } mainView })
        {
            var visualRoot = mainView.GetVisualRoot(); // Získa koreňový vizuálny prvok
            if (visualRoot is TopLevel topLevel)
            {
                return topLevel.StorageProvider;
            }
        }

        // Ak nie je dostupný žiadny spôsob získania StorageProvider, vráti null
        return null;
    }
}
