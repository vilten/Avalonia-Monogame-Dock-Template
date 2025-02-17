using Microsoft.Xna.Framework.Graphics;

namespace Avalonia_Monogame_Dock_Template.Monogame
{
    public static class BlendModeFactory
    {
        public static BlendState GetBlendState(string blendMode)
        {
            return blendMode.ToLower() switch
            {
                "normal" => BlendState.AlphaBlend,
                "multiply" => new BlendState
                {
                    ColorSourceBlend = Blend.DestinationColor,
                    ColorDestinationBlend = Blend.Zero,
                    ColorBlendFunction = BlendFunction.Add
                },
                "screen" => new BlendState
                {
                    ColorSourceBlend = Blend.One,
                    ColorDestinationBlend = Blend.InverseSourceColor,
                    ColorBlendFunction = BlendFunction.Add
                },
                "overlay" => new BlendState
                {
                    ColorSourceBlend = Blend.SourceAlpha,
                    ColorDestinationBlend = Blend.InverseSourceAlpha,
                    ColorBlendFunction = BlendFunction.Add
                },
                "add" => BlendState.Additive,
                "difference" => new BlendState
                {
                    ColorSourceBlend = Blend.One,
                    ColorDestinationBlend = Blend.One,
                    ColorBlendFunction = BlendFunction.ReverseSubtract
                },
                "hue" => new BlendState
                {
                    ColorSourceBlend = Blend.SourceAlpha,
                    ColorDestinationBlend = Blend.InverseSourceAlpha,
                    ColorBlendFunction = BlendFunction.Add
                },
                "saturation" => new BlendState
                {
                    ColorSourceBlend = Blend.SourceAlpha,
                    ColorDestinationBlend = Blend.InverseSourceAlpha,
                    ColorBlendFunction = BlendFunction.Add
                },
                "color" => new BlendState
                {
                    ColorSourceBlend = Blend.SourceAlpha,
                    ColorDestinationBlend = Blend.InverseSourceAlpha,
                    ColorBlendFunction = BlendFunction.Add
                },
                "luminosity" => new BlendState
                {
                    ColorSourceBlend = Blend.SourceAlpha,
                    ColorDestinationBlend = Blend.InverseSourceAlpha,
                    ColorBlendFunction = BlendFunction.Add
                },
                "soft light" => new BlendState
                {
                    ColorSourceBlend = Blend.SourceAlpha,
                    ColorDestinationBlend = Blend.InverseSourceAlpha,
                    ColorBlendFunction = BlendFunction.Add
                },
                "dark light" => new BlendState
                {
                    ColorSourceBlend = Blend.One,
                    ColorDestinationBlend = Blend.One,
                    ColorBlendFunction = BlendFunction.Min
                },
                "color dodge" => new BlendState
                {
                    ColorSourceBlend = Blend.One,
                    ColorDestinationBlend = Blend.InverseSourceAlpha,
                    ColorBlendFunction = BlendFunction.Add
                },
                "color burn" => new BlendState
                {
                    ColorSourceBlend = Blend.Zero,
                    ColorDestinationBlend = Blend.SourceColor,
                    ColorBlendFunction = BlendFunction.ReverseSubtract
                },
                "colorize" => new BlendState
                {
                    ColorSourceBlend = Blend.Zero,
                    ColorDestinationBlend = Blend.DestinationColor,
                    ColorBlendFunction = BlendFunction.ReverseSubtract
                },
                _ => BlendState.AlphaBlend // Default blend mode
            };
        }
    }
}
