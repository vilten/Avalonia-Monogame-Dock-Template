using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia_Monogame_Dock_Template.Events;
using Avalonia_Monogame_Dock_Template.Events.Project;
using Avalonia_Monogame_Dock_Template.Models;
using Avalonia_Monogame_Dock_Template.Threads;
using Microsoft.Xna.Framework;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Avalonia_Monogame_Dock_Template.Services.Plugins
{
    public class ProjectService : IProjectService
    {
        private readonly IDeserializer _deserializer;
        private readonly ISerializer _serializer;
        private readonly Stack<ProjectData> _undoStack = new();
        private readonly Stack<ProjectData> _redoStack = new();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private Layer? _selectedLayer;

        private ProjectData? _currentProject;


        public string? CurrentFilePath { get; set; }

        public ProjectService()
        {
            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new ColorYamlTypeConverter())
                .WithTypeConverter(new Vector2YamlTypeConverter())
                .Build();

            _serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new ColorYamlTypeConverter())
                .WithTypeConverter(new Vector2YamlTypeConverter())
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
                    CurrentFilePath = null;
                    _redoStack.Clear();
                }
                finally
                {
                    _semaphore.Release();
                }
                _selectedLayer = _currentProject.Layers?[0];
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
                    CurrentFilePath = filePath;
                }
                finally
                {
                    _semaphore.Release();
                }
                _selectedLayer = _currentProject.Layers?[0];
                GlobalMessageBus.Instance.SendMessage(new EventProjectLoaded());
            });
        }

        public async Task SaveProjectAsync()
        {
            if (_currentProject == null)
                throw new InvalidOperationException("No project loaded.");

            await ProjectThread.Instance.RunVoidAsync(async () =>
            {
                await _semaphore.WaitAsync();
                try
                {
                    SaveStateForUndo();
                    _currentProject.LastModified = DateTime.UtcNow;
                    string yamlContent = _serializer.Serialize(_currentProject);

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
            if (_currentProject != null)
            {
                _undoStack.Push(new ProjectData
                {
                    Name = _currentProject.Name,
                    CreatedAt = _currentProject.CreatedAt,
                    LastModified = _currentProject.LastModified
                });
            }
        }
        public ProjectData? getCurrentProjectOrCreateNew()
        {
            if (_currentProject == null)
                this.NewProject("Unknown project auto");
            return _currentProject;
        }

        public Layer? getSelectedLayer()
        {
            return _selectedLayer;
        }

        public void selectLayer(String id)
        {
            foreach (var layer in _currentProject.Layers)
            {
                if (layer.Id.Equals(id))
                    _selectedLayer = layer;
            }
        }
    }

    internal class Vector2YamlTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(Vector2);
        }

        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            var scalar = parser.Consume<Scalar>();
            string value = scalar.Value.Trim();
            return new Vector2(float.Parse(value.Split(" ")[0], CultureInfo.InvariantCulture), float.Parse(value.Split(" ")[1], CultureInfo.InvariantCulture));
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            Vector2 vValue = (Vector2)value;
            emitter.Emit(new Scalar($"{vValue.X} {vValue.Y}"));
        }
    }

    internal class ColorYamlTypeConverter : IYamlTypeConverter
    {
        internal Microsoft.Xna.Framework.Color ToXnaColor(System.Drawing.Color drawingColor)
        {
            return new Microsoft.Xna.Framework.Color(
                drawingColor.R,
                drawingColor.G,
                drawingColor.B,
                drawingColor.A);
        }

        internal System.Drawing.Color ToSystemColor(Microsoft.Xna.Framework.Color color)
        {
            return System.Drawing.Color.FromArgb(
                color.A, color.R, color.G, color.B
                );
        }

        public bool Accepts(Type type)
        {
            return type == typeof(Color);
        }

        public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        {
            var scalar = parser.Consume<Scalar>();
            string value = scalar.Value;
            if (String.IsNullOrEmpty(value))
                return Color.White;
            if (!value.StartsWith("#"))
                return ToXnaColor(System.Drawing.Color.FromName(value));
            else return fromHexString(value);
        }

        internal Color fromHexString(string input)
        {
            if (input.Length == 7 || input.Length == 9)
            {
                int red = int.Parse(input.Substring(1, 2), NumberStyles.HexNumber);
                int green = int.Parse(input.Substring(3, 2), NumberStyles.HexNumber);
                int blue = int.Parse(input.Substring(5, 2), NumberStyles.HexNumber);
                if (input.Length == 7)
                    return new Color(red, green, blue);
                int alpha = int.Parse(input.Substring(7, 2), NumberStyles.HexNumber);
                return new Color(red, green, blue, alpha);
            }

            return Color.White;
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
        {
            System.Drawing.Color sColor = ToSystemColor((Color)value);
            if (sColor.IsKnownColor)
                emitter.Emit(new Scalar(sColor.ToKnownColor().ToString()));
            emitter.Emit(new Scalar($"#{sColor.R:X2}{sColor.G:X2}{sColor.B:X2}{sColor.A:X2}"));
        }
    }
}
