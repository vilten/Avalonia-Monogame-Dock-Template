using System;
using System.Collections.Generic;

namespace Avalonia_Monogame_Dock_Template.Models
{
    public class LayerItem
    {
        // Unikátny identifikátor polygonu
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public LayerItemType LayerItemType { get; set; } = LayerItemType.Point;

        // Zoznam vrcholov polygonu; použijeme Avalonia.Point pre reprezentáciu bodov
        public List<Verticle> Verticles { get; set; } = new List<Verticle>();

        // zoznam polygon
        public List<BezierPolygon> Polygons { get; set; } = new List<BezierPolygon>();

        // opacity
    }
}
