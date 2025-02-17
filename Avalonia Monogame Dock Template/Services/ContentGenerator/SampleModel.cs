using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia_Monogame_Dock_Template.Services.ContentGenerator
{
    public class SampleModel
    {
        [DynamicInput(min:10)]
        public int Age { get; set; }

        // Pre reťazec môžu min a max predstavovať napr. minimálnu a maximálnu dĺžku.
        [DynamicInput(min:3, binding: "CurrentProject.Name", isReadonly: true)]
        public string? Name { get; set; }

        [DynamicInput(0.0, 5.0)]
        public double Rating { get; set; }
    }
}
