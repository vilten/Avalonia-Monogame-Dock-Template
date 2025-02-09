using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_Monogame_Dock_Template.Services.ContentGenerator
{
    public class ProjectPropertiesModel
    {
        [DynamicInput(min: 3, binding: "CurrentProject.Name")]
        public string Name { get; set; }
        [DynamicInput(min: 3, binding: "CurrentProject.Version")]
        public string Version { get; set; }
        [DynamicInput(min: 3, binding: "CurrentProject.Author")]
        public string Author { get; set; }
        [DynamicInput(min: 3, binding: "CurrentProject.Description")]
        public string Description { get; set; }
        [DynamicInput(min: 3, binding: "CurrentProject.CreatedAt", isReadonly: true)]
        public string Created { get; set; }
        [DynamicInput(min: 3, binding: "CurrentProject.LastModified", isReadonly: true)]
        public string LastModified { get; set; }
    }
}
