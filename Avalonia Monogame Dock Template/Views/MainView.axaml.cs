using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia_Monogame_Dock_Template.Services;
using Avalonia_Monogame_Dock_Template.ViewModels;
using Dock.Avalonia.Controls;
using Dock.Model;
using Dock.Model.Core;
using Dock.Serializer;
using Splat;

namespace Avalonia_Monogame_Dock_Template;

public partial class MainView : UserControl
{
    private readonly IDockSerializer _serializer;
    private readonly IDockState _dockState;

    public IProjectService ProjectService { get; set; }

    public MainViewModel ViewModel { get; }

    public MainView()
    {
        InitializeComponent();

        ViewModel = new MainViewModel();
        DataContext = ViewModel;

        _serializer = new DockSerializer(typeof(AvaloniaList<>));
        // _serializer = new AvaloniaDockSerializer();

        _dockState = new DockState();

        if (Dock is { })
        {
            var layout = Dock.Layout;
            if (layout is { })
            {
                _dockState.Save(layout);
            }
        }

        ProjectService = Locator.Current.GetService<IProjectService>() ?? throw new InvalidOperationException("IProjectService not registered");

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private List<FilePickerFileType> GetOpenOpenLayoutFileTypes()
    {
        return new List<FilePickerFileType>
        {
            StorageService.Json,
            StorageService.All
        };
    }

    private List<FilePickerFileType> GetSaveOpenLayoutFileTypes()
    {
        return new List<FilePickerFileType>
        {
            StorageService.Json,
            StorageService.All
        };
    }

    private List<FilePickerFileType> GetOpenOpenProjectFileTypes()
    {
        return new List<FilePickerFileType>
        {
            StorageService.Vanim,
            StorageService.All
        };
    }

    private List<FilePickerFileType> GetSaveOpenProjectFileTypes()
    {
        return new List<FilePickerFileType>
        {
            StorageService.Vanim,
            StorageService.All
        };
    }

    private async Task OpenLayout()
    {
        var storageProvider = StorageService.GetStorageProvider();
        if (storageProvider is null)
        {
            return;
        }

        var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open layout",
            FileTypeFilter = GetOpenOpenLayoutFileTypes(),
            AllowMultiple = false
        });

        var file = result.FirstOrDefault();

        if (file is not null)
        {
            try
            {
                await using var stream = await file.OpenReadAsync();
                using var reader = new StreamReader(stream);
                var dock = this.FindControl<DockControl>("Dock");
                if (dock is { })
                {
                    var layout = _serializer.Load<IDock?>(stream);
                    // TODO:
                    // var layout = await JsonSerializer.DeserializeAsync(
                    //     stream, 
                    //     AvaloniaDockSerializer.s_serializerContext.RootDock);
                    if (layout is { })
                    {
                        dock.Layout = layout;
                        _dockState.Restore(layout);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private async Task SaveLayout()
    {
        var storageProvider = StorageService.GetStorageProvider();
        if (storageProvider is null)
        {
            return;
        }

        var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save layout",
            FileTypeChoices = GetSaveOpenLayoutFileTypes(),
            SuggestedFileName = "layout",
            DefaultExtension = "json",
            ShowOverwritePrompt = true
        });

        if (file is not null)
        {
            try
            {
                await using var stream = await file.OpenWriteAsync();
                var dock = this.FindControl<DockControl>("Dock");
                if (dock?.Layout is { })
                {
                    _serializer.Save(stream, dock.Layout);
                    // TODO:
                    // await JsonSerializer.SerializeAsync(
                    //     stream, 
                    //     (RootDock)dock.Layout, AvaloniaDockSerializer.s_serializerContext.RootDock);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private void CloseLayout()
    {
        var dock = this.FindControl<DockControl>("Dock");
        if (dock is { })
        {
            dock.Layout = null;
        }
    }

    private async void FileOpenLayout_OnClick(object? sender, RoutedEventArgs e)
    {
        await OpenLayout();
    }

    private async void FileSaveLayout_OnClick(object? sender, RoutedEventArgs e)
    {
        await SaveLayout();
    }

    private void FileCloseLayout_OnClick(object? sender, RoutedEventArgs e)
    {
        CloseLayout();
    }

    private async void FileOpenProject_OnClick(object? sender, RoutedEventArgs e)
    {
        var storageProvider = StorageService.GetStorageProvider();
        if (storageProvider is null)
        {
            return;
        }

        var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Project File",
            FileTypeFilter = GetOpenOpenProjectFileTypes(),
            AllowMultiple = false
        });

        var file = result.FirstOrDefault();

        if (file is not null)
        {
            try
            {
                await ProjectService.LoadProjectAsync(file.TryGetLocalPath());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    private async void FileNewProject_OnClick(object? sender, RoutedEventArgs e)
    {
        ProjectService.NewProject("untitled");
    }

    private async void FileSaveProject_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ProjectService.CurrentFilePath is null)
        {
            var storageProvider = StorageService.GetStorageProvider();
            if (storageProvider is null)
            {
                return;
            }

            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save project",
                FileTypeChoices = GetSaveOpenProjectFileTypes(),
                SuggestedFileName = "project",
                DefaultExtension = "vanim",
                ShowOverwritePrompt = true
            });

            if (file is not null)
            {
                try
                {
                    ProjectService.CurrentFilePath = file.TryGetLocalPath().ToString();
                    await ProjectService.SaveProjectAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        else
        {
            await ProjectService.SaveProjectAsync();
        }
    }

    public async void FileExit_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            desktopLifetime.Shutdown();
        }
    }
}

