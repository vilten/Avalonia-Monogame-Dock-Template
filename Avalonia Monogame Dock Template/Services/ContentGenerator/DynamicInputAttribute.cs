using System;

namespace Avalonia_Monogame_Dock_Template.Services.ContentGenerator
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DynamicInputAttribute : Attribute
    {
        /// <summary>
        /// Minimálna hodnota (pre čísla) alebo minimálna dĺžka (pre reťazce).
        /// </summary>
        public double Min { get; }
        /// <summary>
        /// Maximálna hodnota (pre čísla) alebo maximálna dĺžka (pre reťazce).
        /// </summary>
        public double Max { get; }
        /// <summary>
        /// Ak je vyplneny tak to tam prida ako text
        /// </summary>
        public string Binding { get; }
        /// <summary>
        /// Ak je vyplneny tak to tam prida ako text
        /// </summary>
        public bool IsReadonly { get; }

        public DynamicInputAttribute(double min = 0, double max = 100, string binding = null, bool isReadonly = false)
        {
            Min = min;
            Max = max;
            Binding = binding;
            IsReadonly = isReadonly;
        }
    }
}
