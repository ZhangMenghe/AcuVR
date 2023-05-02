using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class VolumePropoertiesEdit : MonoBehaviour
{
    [SerializeField]
    private Transform BrightnessSliderGroup;
    [SerializeField]
    private Button CutOffBtn;
    [SerializeField]
    private Slider ScaleSlider;
    [SerializeField]
    private Slider ContrastMinSlider;
    [SerializeField]
    private Slider ContrastMaxSlider;
    [SerializeField]
    private Slider BrightnessSlider;
    [SerializeField]
    private TextMeshProUGUI ScaleText;
    [SerializeField]
    private TextMeshProUGUI CMinText;
    [SerializeField]
    private TextMeshProUGUI CMaxText;
    [SerializeField]
    private TextMeshProUGUI BrightText;
    [SerializeField]
    private TMPro.TMP_Dropdown RendringMethodDropDown;

    private Sprite CheckSprite;
    private Sprite UncheckSprite;
    //public GameObject BrightnessObj;

    // Start is called before the first frame update
    private void Awake()
    {
        CheckSprite = Resources.Load<Sprite>("icons/checkbox_check");
        UncheckSprite = Resources.Load<Sprite>("icons/checkbox");
        
        RendringMethodDropDown.onValueChanged.AddListener(delegate {
            OnVolumeRenderingMethodChange(RendringMethodDropDown.value);
        });

        CutOffBtn.onClick.AddListener(delegate {
            OnVolumeCutOffChange();
        });
    }
    public void OnReset()
    {
        if (!VolumeObjectFactory.gTargetVolume) return;

        ScaleSlider.value = VolumeObjectFactory.gTargetVolume.GetVolumeUnifiedScale();
        ScaleText.SetText(ScaleSlider.value.ToString("0.00"));
    }

    public void OnVolumeScaleChange()
    {
        if (!VolumeObjectFactory.gTargetVolume) return;

        ScaleText.SetText(ScaleSlider.value.ToString("0.00"));
        VolumeObjectFactory.gTargetVolume.SetVolumeUnifiedScale(ScaleSlider.value);
        StandardModelFactory.OnTargetVolumeTransformationChanged();
    }

    public void OnVolumeRenderingMethodChange(int change)
    {
        if (VolumeObjectFactory.gTargetVolume)
            VolumeObjectFactory.gTargetVolume.SetRenderMode((UnityVolumeRendering.RenderMode)change);
    }
    public void OnVolumeCutOffChange()
    {
        if (!VolumeObjectFactory.gTargetVolume) return;

        bool new_status = !VolumeObjectFactory.gTargetVolume.GetContrastCutoffEnabled();
        
        UpdateSprite(CutOffBtn, new_status);
        
        VolumeObjectFactory.gTargetVolume.SetContrastCutoffEnabled(new_status);
        
        Vector3 contrast = VolumeObjectFactory.gTargetVolume.GetVisibilityWindow();
        ContrastMinSlider.value = Math.Max(contrast.x, .0f);
        ContrastMaxSlider.value = Math.Min(contrast.y, 1.0f);

        BrightnessSliderGroup.gameObject.SetActive(!new_status);
        if (!new_status) BrightnessSlider.value = contrast.z;
    }

    public void OnVolumeContrastMinChange()
    {
        if (!VolumeObjectFactory.gTargetVolume) return;

        Vector3 contrast = VolumeObjectFactory.gTargetVolume.GetVisibilityWindow();
        contrast.x = ContrastMinSlider.value;
        VolumeObjectFactory.gTargetVolume.SetVisibilityWindow(contrast);
        CMinText.SetText(contrast.x.ToString("0.00"));
    }

    public void OnVolumeContrastMaxChange()
    {
        if (!VolumeObjectFactory.gTargetVolume) return;

        Vector3 contrast = VolumeObjectFactory.gTargetVolume.GetVisibilityWindow();
        contrast.y = ContrastMaxSlider.value;
        VolumeObjectFactory.gTargetVolume.SetVisibilityWindow(contrast);
        CMaxText.SetText(contrast.y.ToString("0.00"));
    }

    public void OnBrightnessChange()
    {
        if (!VolumeObjectFactory.gTargetVolume) return;
        
        Vector3 contrast = VolumeObjectFactory.gTargetVolume.GetVisibilityWindow();
        contrast.z = BrightnessSlider.value;
        VolumeObjectFactory.gTargetVolume.SetVisibilityWindow(contrast);
        BrightText.SetText(contrast.z.ToString("0.00"));
    }
    private void UpdateSprite(Button btn, bool isChecked)
    {
        SpriteState spriteState = btn.spriteState;

        // Toggle between the ON and OFF sprites
        if (isChecked)
        {
            btn.image.sprite = CheckSprite;
            spriteState.pressedSprite = CheckSprite;
        }
        else
        {
            btn.image.sprite = UncheckSprite;
            spriteState.highlightedSprite = UncheckSprite;
        }

        btn.spriteState = spriteState;
    }
}
