using Avalonia_Monogame_Dock_Template.Events;
using Avalonia_Monogame_Dock_Template.Events.Project;
using Avalonia_Monogame_Dock_Template.Models.Yaml;
using Avalonia_Monogame_Dock_Template.Monogame;
using Avalonia_Monogame_Dock_Template.Services;
using DynamicData;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Avalonia_Monogame_Dock_Template.ViewModels
{
    public class FileExplorerViewModel : ViewModelBase
    {
        private ObservableCollection<YamlNode> _rootNodes = new();
        public ObservableCollection<YamlNode> RootNodes => _rootNodes;

        public IProjectService ProjectService { get; set; }

        public FileExplorerViewModel()
        {
            ProjectService = Locator.Current.GetService<IProjectService>() ?? throw new InvalidOperationException("IProjectService not registered");
            _rootNodes.CollectionChanged += (s, e) => this.RaisePropertyChanged(nameof(RootNodes));
            GlobalMessageBus.Instance.Listen<EventProjectLoaded>().Subscribe(evt =>
            {
                string yamlText = ProjectService.GetYamlString();
                RootNodes.Clear();
                RootNodes.AddRange(YamlParser.LoadYaml(yamlText));

                //GetYaml();
            });
        }

        public void GetYaml()
        {
            //GetYaml();
            //RootNodes.Clear();
            //string yamlText = ProjectService.GetYamlString();
            //Debug.WriteLine($"aaaaaaaaaaaa {yamlText}");
            // string yamlText = File.ReadAllText("C:\\Users\\vilte\\source\\Avalonia-Monogame-Dock-Template\\Projects\\test\\project5.vanim");
            //RootNodes.AddRange(YamlParser.LoadYaml(yamlText));
        }
    }
}
