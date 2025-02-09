using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia_Monogame_Dock_Template.Events;
using Avalonia_Monogame_Dock_Template.Events.Project;
using Avalonia_Monogame_Dock_Template.Models;
using Avalonia_Monogame_Dock_Template.Threads;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Avalonia_Monogame_Dock_Template.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IDeserializer _deserializer;
        private readonly ISerializer _serializer;
        private readonly Stack<ProjectData> _undoStack = new();
        private readonly Stack<ProjectData> _redoStack = new();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private ProjectData? _currentProject;
        public ProjectData? CurrentProject { get; set; }


        public string? CurrentFilePath { get; set; }
        ProjectData? IProjectService.CurrentProject { get => _currentProject; set => _currentProject = value; }

        public ProjectService()
        {
            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            _serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
        }

        public void NewProject(string name)
        {
            ProjectThread.Instance.Run(() =>
            {
                _semaphore.Wait();
                try
                {
                    SaveStateForUndo();
                    _currentProject = new ProjectData
                    {
                        Name = name,
                        CreatedAt = DateTime.UtcNow,
                        LastModified = DateTime.UtcNow,
                        Settings = [],
                        Layers = new ObservableCollection<Layer>()
                    };
                    _currentProject.Settings.Add("theme", "dark");
                    _currentProject.Layers.Add(new Layer()
                    {
                        Name = "Unknown layear",
                    });
                    CurrentProject = _currentProject;
                    CurrentFilePath = null;
                    _redoStack.Clear();
                }
                finally
                {
                    _semaphore.Release();
                }
                GlobalMessageBus.Instance.SendMessage(new EventProjectLoaded());
            });
        }

        public async Task LoadProjectAsync(string filePath)
        {
            await ProjectThread.Instance.RunVoidAsync(async () =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    if (!File.Exists(filePath))
                        throw new FileNotFoundException("Project file not found.", filePath);

                    string yamlContent = File.ReadAllText(filePath);
                    _currentProject = _deserializer.Deserialize<ProjectData>(yamlContent);
                    CurrentProject = _currentProject;

                    CurrentFilePath = filePath;
                }
                finally
                {
                    _semaphore.Release();
                }
                GlobalMessageBus.Instance.SendMessage(new EventProjectLoaded());
            });
        }

        public async Task SaveProjectAsync()
        {
            if (CurrentProject == null)
                throw new InvalidOperationException("No project loaded.");

            await ProjectThread.Instance.RunVoidAsync(async () =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    SaveStateForUndo();
                    CurrentProject.LastModified = DateTime.UtcNow;
                    string yamlContent = _serializer.Serialize(CurrentProject);

                    if (CurrentFilePath == null)
                        throw new InvalidOperationException("No file path specified for saving.");

                    File.WriteAllText(CurrentFilePath, yamlContent);
                    _redoStack.Clear();
                }
                finally
                {
                    _semaphore.Release();
                }
            });
        }

        private void SaveStateForUndo()
        {
            if (CurrentProject != null)
            {
                _undoStack.Push(new ProjectData
                {
                    Name = CurrentProject.Name,
                    CreatedAt = CurrentProject.CreatedAt,
                    LastModified = CurrentProject.LastModified
                });
            }
        }
    }
}
