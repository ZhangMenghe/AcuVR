using UnityEngine;

namespace UnityVolumeRendering
{
    public enum RenderMode
    {
        DirectVolumeRendering=0,
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
        None=0,
        GrayscaleColor,
        HSVColor,
        BrightColor,
        FireColor,
        CETLabColor
    }

}
