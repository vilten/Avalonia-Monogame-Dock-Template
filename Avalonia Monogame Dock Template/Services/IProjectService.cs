using System.Threading.Tasks;
using Avalonia_Monogame_Dock_Template.Models;

namespace Avalonia_Monogame_Dock_Template.Services
{
    public interface IProjectService
    {
        public ProjectData? CurrentProject { get; set; }
        public string? CurrentFilePath { get; set; }
        Task LoadProjectAsync(string filePath);
        Task SaveProjectAsync();
        void NewProject(string name);
    }
}
