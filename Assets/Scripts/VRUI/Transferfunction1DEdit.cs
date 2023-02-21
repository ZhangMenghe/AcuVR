using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityVolumeRendering;

//menghe:REMOVE
[ExecuteInEditMode]
public class Transferfunction1DEdit : MonoBehaviour
{
    public RayInteractorCursorVisual RightControllerCursor;
    public Transform ColorSchemeDropDownObj;
    public Transform MeshTransform;
    public Image tfGUIImage;
    public Image PaletteImage;

    public CylinderUI RootUIManager;

    private Material tfGUIMat = null;
    private Material tfPaletteGUIMat = null;

    private Texture2D histTex = null;

    // Controlling Points
    private int movingColPointIndex = -1;
    private int movingAlphaPointIndex = -1;
    private int selectedColPointIndex = -1;
    private void Start()
    {
        tfGUIMat = Resources.Load<Material>("Materials/TransferFunctionGUIMat");
        tfPaletteGUIMat = Resources.Load<Material>("Materials/TransferFunctionPaletteGUIMat");

        tfGUIImage.material = tfGUIMat;
        PaletteImage.material = tfPaletteGUIMat;
    }
    void Update()
    {
        if (!RootUIManager.mTargetVolume) return;

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
                histTex = HistogramTextureGenerator.GenerateHistogramTextureOnGPU(RootUIManager.mTargetVolume.dataset);
            else
                histTex = HistogramTextureGenerator.GenerateHistogramTexture(RootUIManager.mTargetVolume.dataset);
        }

        TransferFunction tf = RootUIManager.mTargetVolume.transferFunction;
        tfGUIMat.SetTexture("_TFTex", tf.GetTexture());
        tfGUIMat.SetTexture("_HistTex", histTex);

        tfPaletteGUIMat.SetTexture("_TFTex", tf.GetTexture());

        var ColorSchemeDropDown = ColorSchemeDropDownObj.GetComponent<TMPro.TMP_Dropdown>();
        ColorSchemeDropDown.onValueChanged.AddListener(delegate {
            OnColorSchemeChanged(ColorSchemeDropDown.value);
        });
    }
    private void ClearSelection()
    {
        movingColPointIndex = -1;
        movingAlphaPointIndex = -1;
        selectedColPointIndex = -1;
    }
    public void OnResetButtonClicked()
    {

        var tf = new TransferFunction();
        tf.alphaControlPoints.Add(new TFAlphaControlPoint(0.2f, 0.0f));
        tf.alphaControlPoints.Add(new TFAlphaControlPoint(0.8f, 1.0f));
        tf.colourControlPoints.Add(new TFColourControlPoint(0.5f, new Color(0.469f, 0.354f, 0.223f, 1.0f)));
        
        if (RootUIManager.mTargetVolume)
            RootUIManager.mTargetVolume.SetTransferFunction(tf);

        ClearSelection();
    }

    public void OnColorSchemeChanged(int value) {
        if (RootUIManager.mTargetVolume)
            RootUIManager.mTargetVolume.SetColorScheme((UnityVolumeRendering.ColorTransferScheme)value);
    }
    public void OnPaletteCanvasClicked()
    {
        //Debug.LogWarning("=====================" + RightControllerCursor.transform.position);
        //Debug.LogWarning("=========start============" + MeshTransform.position);
    }
}
