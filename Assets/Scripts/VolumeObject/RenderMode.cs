using UnityEngine;

namespace UnityVolumeRendering
{
    public enum RenderMode
    {
        DirectVolumeRendering,
        MaximumIntensityProjectipon,
        IsosurfaceRendering
    }

    public enum TFRenderMode
    {
        [InspectorName("1D Transfer Function")]
        TF1D,
        [InspectorName("2D Transfer Function")]
        TF2D
    }

    public enum ColorTransferScheme
    {
        None,
        GrayscaleColor,
        HSVColor,
        BrightColor,
        FireColor,
        CETLabColor
    }
}
