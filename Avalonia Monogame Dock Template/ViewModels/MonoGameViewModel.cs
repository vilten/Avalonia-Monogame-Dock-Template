using System;
using Avalonia_Monogame_Dock_Template.Monogame;
using Avalonia_Monogame_Dock_Template.Services;
using Splat;

namespace Avalonia_Monogame_Dock_Template.ViewModels
{
    public class MonoGameViewModel : ViewModelBase
    {
        private Game1 _game;
        private readonly IProjectService _projectService;

        public MonoGameViewModel()
        {
            _projectService = Locator.Current.GetService<IProjectService>() ?? throw new InvalidOperationException("IProjectService not registered");
            _game = new Game1(_projectService);
        }

        public Game1 Game
        {
            get => _game;
            set => _game = value;
        }
    }
}
