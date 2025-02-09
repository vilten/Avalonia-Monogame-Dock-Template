using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia_Monogame_Dock_Template.Monogame;

namespace Avalonia_Monogame_Dock_Template.ViewModels
{
    public class MonoGameViewModel : ViewModelBase
    {
        private Game1 _game = new Game1();

        public MonoGameViewModel()
        {
        }

        public Game1 Game
        {
            get => _game;
            set => _game = value;
        }
    }
}
