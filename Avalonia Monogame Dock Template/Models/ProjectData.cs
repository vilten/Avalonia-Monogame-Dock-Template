using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Avalonia_Monogame_Dock_Template.Models
{
    public class ProjectData
    {
        public string Name { get; set; } = "Untitled";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModified { get; set; } = DateTime.UtcNow;
        public string Version { get; set; } = "1.0.0";
        public string Author { get; set; } = "Unknown";
        public string Description { get; set; } = "Project description...";

        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();

        // Zoznam vrstiev v projekte
        public ObservableCollection<Layer> Layers { get; set; } = new ObservableCollection<Layer>();
    }
}
