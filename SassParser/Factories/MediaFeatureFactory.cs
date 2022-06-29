using System;
using System.Collections.Generic;

namespace SassParser
{
    internal sealed class MediaFeatureFactory
    {
        private delegate MediaFeature Creator(Token token);

        private static readonly Lazy<MediaFeatureFactory> Lazy =
          new Lazy<MediaFeatureFactory>(() => new MediaFeatureFactory());

        internal static MediaFeatureFactory Instance => Lazy.Value;

        private MediaFeatureFactory()
        {
        }

        public MediaFeature Create(string name, Token token)
        {
            return _creators.TryGetValue(name, out Creator creator)
                ? creator(token)
                : default;
        }

        #region Creators
        private readonly Dictionary<string, Creator> _creators =
            new Dictionary<string, Creator>(StringComparer.OrdinalIgnoreCase)
            {
                {FeatureNames.MinWidth, (t) =>new WidthMediaFeature(FeatureNames.MinWidth, t)},
                {FeatureNames.MaxWidth, (t) =>new WidthMediaFeature(FeatureNames.MaxWidth, t)},
                {FeatureNames.Width, (t) =>new WidthMediaFeature(FeatureNames.Width, t)},
                {FeatureNames.MinHeight, (t) =>new HeightMediaFeature(FeatureNames.MinHeight, t)},
                {FeatureNames.MaxHeight, (t) =>new HeightMediaFeature(FeatureNames.MaxHeight, t)},
                {FeatureNames.Height, (t) =>new HeightMediaFeature(FeatureNames.Height, t)},
                {FeatureNames.MinDeviceWidth, (t) =>new DeviceWidthMediaFeature(FeatureNames.MinDeviceWidth, t)},
                {FeatureNames.MaxDeviceWidth, (t) =>new DeviceWidthMediaFeature(FeatureNames.MaxDeviceWidth, t)},
                {FeatureNames.DeviceWidth, (t) =>new DeviceWidthMediaFeature(FeatureNames.DeviceWidth, t)},
                {FeatureNames.MinDevicePixelRatio, (t) =>new DevicePixelRatioFeature(FeatureNames.MinDevicePixelRatio, t)},
                {FeatureNames.MaxDevicePixelRatio, (t) =>new DevicePixelRatioFeature(FeatureNames.MaxDevicePixelRatio, t)},
                {FeatureNames.DevicePixelRatio, (t) =>new DevicePixelRatioFeature(FeatureNames.DevicePixelRatio, t)},
                {FeatureNames.MinDeviceHeight, (t) =>new DeviceHeightMediaFeature(FeatureNames.MinDeviceHeight, t)},
                {FeatureNames.MaxDeviceHeight, (t) =>new DeviceHeightMediaFeature(FeatureNames.MaxDeviceHeight, t)},
                {FeatureNames.DeviceHeight, (t) =>new DeviceHeightMediaFeature(FeatureNames.DeviceHeight, t)},
                {FeatureNames.MinAspectRatio, (t) =>new AspectRatioMediaFeature(FeatureNames.MinAspectRatio, t)},
                {FeatureNames.MaxAspectRatio, (t) =>new AspectRatioMediaFeature(FeatureNames.MaxAspectRatio, t)},
                {FeatureNames.AspectRatio, (t) =>new AspectRatioMediaFeature(FeatureNames.AspectRatio, t)},
                {FeatureNames.MinDeviceAspectRatio,(t) =>new DeviceAspectRatioMediaFeature(FeatureNames.MinDeviceAspectRatio, t)},
                {FeatureNames.MaxDeviceAspectRatio,(t) =>new DeviceAspectRatioMediaFeature(FeatureNames.MaxDeviceAspectRatio, t)},
                {FeatureNames.DeviceAspectRatio, (t) =>new DeviceAspectRatioMediaFeature(FeatureNames.DeviceAspectRatio, t)},
                {FeatureNames.MinColor, (t) =>new ColorMediaFeature(FeatureNames.MinColor, t)},
                {FeatureNames.MaxColor, (t) =>new ColorMediaFeature(FeatureNames.MaxColor, t)},
                {FeatureNames.Color, (t) =>new ColorMediaFeature(FeatureNames.Color, t)},
                {FeatureNames.MinColorIndex, (t) =>new ColorIndexMediaFeature(FeatureNames.MinColorIndex, t)},
                {FeatureNames.MaxColorIndex, (t) =>new ColorIndexMediaFeature(FeatureNames.MaxColorIndex, t)},
                {FeatureNames.ColorIndex, (t) =>new ColorIndexMediaFeature(FeatureNames.ColorIndex, t)},
                {FeatureNames.MinMonochrome, (t) =>new MonochromeMediaFeature(FeatureNames.MinMonochrome, t)},
                {FeatureNames.MaxMonochrome, (t) =>new MonochromeMediaFeature(FeatureNames.MaxMonochrome, t)},
                {FeatureNames.Monochrome, (t) =>new MonochromeMediaFeature(FeatureNames.Monochrome, t)},
                {FeatureNames.MinResolution, (t) =>new ResolutionMediaFeature(FeatureNames.MinResolution, t)},
                {FeatureNames.MaxResolution, (t) =>new ResolutionMediaFeature(FeatureNames.MaxResolution, t)},
                {FeatureNames.Resolution, (t) =>new ResolutionMediaFeature(FeatureNames.Resolution, t)},
                {FeatureNames.Orientation, (t) =>new OrientationMediaFeature(t)},
                {FeatureNames.Grid, (t) =>new GridMediaFeature(t)},
                {FeatureNames.Scan, (t) =>new ScanMediaFeature(t)},
                {FeatureNames.UpdateFrequency, (t) =>new UpdateFrequencyMediaFeature(t)},
                {FeatureNames.Scripting, (t) =>new ScriptingMediaFeature(t)},
                {FeatureNames.Pointer, (t) =>new PointerMediaFeature(t)},
                {FeatureNames.Hover, (t) =>new HoverMediaFeature(t)}
            };
        #endregion
    }
}