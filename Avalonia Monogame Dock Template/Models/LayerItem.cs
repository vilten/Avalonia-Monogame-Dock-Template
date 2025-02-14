using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Avalonia_Monogame_Dock_Template.Models
{
    public class LayerItem
    {
        // Unikátny identifikátor polygonu
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public LayerItemType LayerItemType { get; set; } = LayerItemType.Point;

        // Zoznam vrcholov polygonu; použijeme Avalonia.Point pre reprezentáciu bodov
        public List<Verticle> Verticles { get; set; } = new List<Verticle>();

        // opacity
    }
}
