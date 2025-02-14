using System.Threading.Tasks;
using Avalonia_Monogame_Dock_Template.Models;

namespace Avalonia_Monogame_Dock_Template.Services
{
    public interface IProjectService
    {
        public ProjectData? getCurrentProjectOrCreateNew();
        public Layer? getSelectedLayer();
        public void selectLayer(string id);
        public string? CurrentFilePath { get; set; }
        Task LoadProjectAsync(string? filePath);
        Task SaveProjectAsync();
        void NewProject(string name);
    }
}
