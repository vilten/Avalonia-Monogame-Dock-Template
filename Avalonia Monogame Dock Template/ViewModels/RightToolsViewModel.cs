using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia_Monogame_Dock_Template.Models;
using Avalonia_Monogame_Dock_Template.Monogame;

namespace Avalonia_Monogame_Dock_Template.ViewModels
{
    public class RightToolsViewModel : ViewModelBase
    {
        public Dictionary<string,List<ToolButton>> groups { get; set; } = new Dictionary<string, List<ToolButton>>();
        public RightToolsViewModel()
        {
        }
    }
}
