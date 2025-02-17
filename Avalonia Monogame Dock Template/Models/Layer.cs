using System;
using System.Collections.Generic;
using Avalonia_Monogame_Dock_Template.Events.Project;
using Avalonia_Monogame_Dock_Template.Events;

namespace Avalonia_Monogame_Dock_Template.Models
{
    public class Layer
    {
        // Unikátny identifikátor vrstvy
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // Názov vrstvy
        public string Name { get; set; } = "New Layer";
        public Boolean ViewportVisible { get; set; } = true;
        public Boolean RenderVisible { get; set; } = true;

        // Nastavenia pre túto vrstvu – podobne ako v projekte môžete použiť slovník alebo vlastnú triedu
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        // Zoznam polygonov, ktoré patria do tejto vrstvy
        public List<LayerItem> LayerItems { get; set; } = new List<LayerItem>();

        public void ToggleViewportVisible()
        {
            ViewportVisible = !ViewportVisible;
            GlobalMessageBus.Instance.SendMessage(new EventLayersUpdated());
        }

        public void ToggleRenderVisible()
        {
            RenderVisible = !RenderVisible;
            GlobalMessageBus.Instance.SendMessage(new EventLayersUpdated());
        }
    }
}
