using UnityEngine;
using UnityEngine.UI;
using UnityVolumeRendering;

public class Transferfunction1DEdit : MonoBehaviour
{
    public Transform ColorSchemeDropDownObj;
    public Image tfGUIImage;
    public Image PaletteImage;
    public PaletteHandler PalettePanel;
    public HistHandler HistPanel;

    public VolumeDataEdit VolumeManager;

    private Material tfGUIMat = null;
    private Material tfPaletteGUIMat = null;

    private Texture2D histTex = null;
    public void OnInitialize()
    {
        tfGUIMat = Resources.Load<Material>("Materials/TransferFunctionGUIMat");
        tfPaletteGUIMat = Resources.Load<Material>("Materials/TransferFunctionPaletteGUIMat");

        tfGUIImage.material = tfGUIMat;
        PaletteImage.material = tfPaletteGUIMat;
        var ColorSchemeDropDown = ColorSchemeDropDownObj.GetComponent<TMPro.TMP_Dropdown>();
        ColorSchemeDropDown.onValueChanged.AddListener(delegate {
            OnColorSchemeChanged(ColorSchemeDropDown.value);
        });

        //menghe:REMOVE
        if (!tfGUIMat)
        {
            tfGUIMat = Resources.Load<Material>("Materials/TransferFunctionGUIMat");
            tfGUIImage.material = tfGUIMat;
        }
        if (!tfPaletteGUIMat)
        {
            tfPaletteGUIMat = Resources.Load<Material>("Materials/TransferFunctionPaletteGUIMat");
            PaletteImage.material = tfPaletteGUIMat;
        }
        if (!histTex)
        {
            if (SystemInfo.supportsComputeShaders)
                histTex = HistogramTextureGenerator.GenerateHistogramTextureOnGPU(VolumeObjectFactory.gTargetVolume.dataset);
            else
                histTex = HistogramTextureGenerator.GenerateHistogramTexture(VolumeObjectFactory.gTargetVolume.dataset);
        }
        TransferFunction tf = VolumeObjectFactory.gTargetVolume.transferFunction;
        tfGUIMat.SetTexture("_TFTex", tf.GetTexture());
        tfGUIMat.SetTexture("_HistTex", histTex);

        tfPaletteGUIMat.SetTexture("_TFTex", tf.GetTexture());
    }

    public void OnResetButtonClicked()
    {
        var tf = new TransferFunction();
        tf.alphaControlPoints.Add(new TFAlphaControlPoint(0.2f, 0.0f));
        tf.alphaControlPoints.Add(new TFAlphaControlPoint(0.8f, 1.0f));
        tf.colourControlPoints.Add(new TFColourControlPoint(0.5f, new Color(0.469f, 0.354f, 0.223f, 1.0f)));
        
        if (VolumeObjectFactory.gTargetVolume)
            VolumeObjectFactory.gTargetVolume.SetTransferFunction(tf);
        
        PalettePanel.OnReset();
        HistPanel.OnReset();
    }

    public void OnColorSchemeChanged(int value) {
        if (VolumeObjectFactory.gTargetVolume)
            VolumeObjectFactory.gTargetVolume.SetColorScheme((UnityVolumeRendering.ColorTransferScheme)value);
    }
    public void OnPaletteCanvasClicked()
    {
    }
}
