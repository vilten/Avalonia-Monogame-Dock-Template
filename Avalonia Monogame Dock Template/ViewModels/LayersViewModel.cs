using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using Avalonia_Monogame_Dock_Template.Monogame;
using Avalonia_Monogame_Dock_Template.Models;
using Avalonia_Monogame_Dock_Template.Services;
using Splat;
using Avalonia_Monogame_Dock_Template.Events.Project;
using Avalonia_Monogame_Dock_Template.Events;
using System.Reflection;
using System.Diagnostics;

namespace Avalonia_Monogame_Dock_Template.ViewModels
{
    public class LayersViewModel : ViewModelBase
    {

        public ObservableCollection<Layer> Layers { get; } = new ObservableCollection<Layer>();
        public IProjectService ProjectService { get; set; }

        private Layer _selectedLayer;
        public Layer? SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedLayer, value);
                if (ProjectService != null && value != null)
                    ProjectService.selectLayer(value.Id);
            }
        }

        public LayersViewModel()
        {

            ProjectService = Locator.Current.GetService<IProjectService>() ?? throw new InvalidOperationException("IProjectService not registered");
            Layers = new ObservableCollection<Models.Layer>(ProjectService.getCurrentProjectOrCreateNew().Layers);

            GlobalMessageBus.Instance.Listen<EventLayersUpdated>().Subscribe(evt =>
            {
                RefreshLayers();
            });
            GlobalMessageBus.Instance.Listen<EventProjectLoaded>().Subscribe(evt =>
            {
                RefreshLayers();
                SelectedLayer = null;
                SelectedLayer = ProjectService.getCurrentProjectOrCreateNew().Layers[0];
                ProjectService.selectLayer(SelectedLayer.Id);
            });
        }

        private void RefreshLayers()
        {
            Layers.Clear();
            try
            {
                foreach (var item in ProjectService.getCurrentProjectOrCreateNew().Layers)
                {
                    Layers.Add(item);
                }
            }
            catch { }
        }

        public void AddLayer()
        {
            // Pridáme novú vrstvu so vzorovým názvom
            var newLayer = new Layer { Name = $"Vrstva {Layers.Count + 1}" };
            var index = 0;
            if (SelectedLayer != null)
                index = Layers.IndexOf(SelectedLayer);
            ProjectService.getCurrentProjectOrCreateNew().Layers.Insert(index, newLayer);
            RefreshLayers();
            SelectedLayer = newLayer;
            ProjectService.selectLayer(SelectedLayer.Id);
        }

        public void RemoveLayer()
        {
            if (SelectedLayer != null)
            {
                int index = Layers.IndexOf(SelectedLayer);
                ProjectService.getCurrentProjectOrCreateNew().Layers.RemoveAt(index);
                RefreshLayers();
                if (Layers.Count > 0)
                {
                    if (index >= Layers.Count)
                        index = Layers.Count - 1;
                    SelectedLayer = null;
                    SelectedLayer = Layers[index];
                    ProjectService.selectLayer(SelectedLayer.Id);
                }
            }
        }

        public void MoveLayerUp()
        {
            if (SelectedLayer != null)
            {
                int index = Layers.IndexOf(SelectedLayer);
                if (index > 0)
                {
                    ProjectService.getCurrentProjectOrCreateNew().Layers.Move(index, index - 1);
                    RefreshLayers();
                    SelectedLayer = null;
                    SelectedLayer = Layers[index - 1];
                    ProjectService.selectLayer(SelectedLayer.Id);
                }
            }
        }

        public void MoveLayerDown()
        {
            if (SelectedLayer != null)
            {
                int index = Layers.IndexOf(SelectedLayer);
                if (index < Layers.Count - 1)
                {
                    Layers.Move(index, index + 1);
                    ProjectService.getCurrentProjectOrCreateNew().Layers.Move(index, index + 1);
                    RefreshLayers();
                    SelectedLayer = null;
                    SelectedLayer = Layers[index + 1];
                    ProjectService.selectLayer(SelectedLayer.Id);
                }
            }
        }

        public void HideLayer(Layer layer)
        {

        }
    }
}
