using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_Monogame_Dock_Template.Models.Yaml
{
    public class YamlNode
    {
        public string Name { get; set; }
        public string Icon { get; set; }  // Ikona pre daný uzol
        public ObservableCollection<YamlNode> Children { get; set; } = new();

        public YamlNode(string name, string icon)
        {
            Name = name;
            Icon = icon;
        }
    }
}
