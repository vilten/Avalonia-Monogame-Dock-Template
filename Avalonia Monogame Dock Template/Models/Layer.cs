using System;
using System.Collections.Generic;

namespace Avalonia_Monogame_Dock_Template.Models
{
    public class Layer
    {
        // Unikátny identifikátor vrstvy
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Názov vrstvy
        public string Name { get; set; } = "New Layer";

        // Nastavenia pre túto vrstvu – podobne ako v projekte môžete použiť slovník alebo vlastnú triedu
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        // Zoznam polygonov, ktoré patria do tejto vrstvy
        public List<Polygon> Polygons { get; set; } = new List<Polygon>();
    }
}
