using System;
using System.Diagnostics;
using System.Xml;
using Avalonia_Monogame_Dock_Template.Events;
using Avalonia_Monogame_Dock_Template.Events.Project;
using Avalonia_Monogame_Dock_Template.Models;
using Avalonia_Monogame_Dock_Template.Monogame;
using Avalonia_Monogame_Dock_Template.Services;
using ReactiveUI;
using Splat;

namespace Avalonia_Monogame_Dock_Template.ViewModels
{
    public class ProjectFormViewModel : ViewModelBase
    {
        private readonly IProjectService _projectService;
        private ProjectData _currentProject;
        public ProjectData CurrentProject { get => _currentProject; set => this.RaiseAndSetIfChanged(ref _currentProject, value); }

        public ProjectFormViewModel()
        {
            _projectService = Locator.Current.GetService<IProjectService>() ?? throw new InvalidOperationException("IProjectService not registered");
            CurrentProject = _projectService.CurrentProject;

            GlobalMessageBus.Instance.Listen<EventProjectLoaded>().Subscribe(evt =>
            {
                CurrentProject = _projectService.CurrentProject;
            });
        }
    }
}
