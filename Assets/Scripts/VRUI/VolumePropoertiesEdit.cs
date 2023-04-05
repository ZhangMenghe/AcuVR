using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class VolumePropoertiesEdit : MonoBehaviour
{

    //public VolumeDataEdit VolumeManager;
    public Transform ScaleSliderGroup;
    public Transform ContrastMinSliderGroup;
    public Transform ContrastMaxSliderGroup;
    public Transform BrightnessSliderGroup;
    public Transform RendringMethodDropDownGroup;
    public Button CutOffBtn;


    private Slider ScaleSlider;
    private Slider ContrastMinSlider;
    private Slider ContrastMaxSlider;
    private Slider BrightnessSlider;

    private TextMeshProUGUI ScaleText;
    private TextMeshProUGUI CMinText;
    private TextMeshProUGUI CMaxText;
    private TextMeshProUGUI BrightText;

    private Sprite CheckSprite;
    private Sprite UncheckSprite;
    //public GameObject BrightnessObj;

    // Start is called before the first frame update
    private void Awake()
    {
        CheckSprite = Resources.Load<Sprite>("icons/checkbox_check");
        UncheckSprite = Resources.Load<Sprite>("icons/checkbox");
    }
    void Start()
    {
        ScaleSlider = ScaleSliderGroup.Find("Slider").GetComponent<Slider>();
        ScaleText = ScaleSliderGroup.Find("valueText").GetComponent<TextMeshProUGUI>();

        ContrastMinSlider = ContrastMinSliderGroup.Find("Slider").GetComponent<Slider>();
        CMinText = ContrastMinSliderGroup.Find("valueText").GetComponent<TextMeshProUGUI>();

        ContrastMaxSlider = ContrastMaxSliderGroup.Find("Slider").GetComponent<Slider>();
        CMaxText = ContrastMaxSliderGroup.Find("valueText").GetComponent<TextMeshProUGUI>();

        BrightnessSlider = BrightnessSliderGroup.Find("Slider").GetComponent<Slider>();
        BrightText = BrightnessSliderGroup.Find("valueText").GetComponent<TextMeshProUGUI>();

        var RendringMethodDropDown = RendringMethodDropDownGroup.GetComponent<TMPro.TMP_Dropdown>();
        RendringMethodDropDown.onValueChanged.AddListener(delegate {
            OnVolumeRenderingMethodChange(RendringMethodDropDown.value);
        });

        CutOffBtn.onClick.AddListener(delegate {
            OnVolumeCutOffChange();
        });
    }


    public void OnVolumeScaleChange()
    {

        if (ScaleText && ScaleSlider)
            ScaleText.SetText(ScaleSlider.value.ToString("0.00"));
        //VolumeObjectFactory.gTargetVolume.GetComponent<VolumeRenderedObject>().SetVolumeUnifiedScale(ScaleSlider.value);
        //ref VolumeRenderedObject target = ref VolumeManager.getTargetVolume();
        if (VolumeObjectFactory.gTargetVolume)
            VolumeObjectFactory.gTargetVolume.SetVolumeUnifiedScale(ScaleSlider.value);
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
    private void Update()
    {
        if (VolumeObjectFactory.gVolumeScaleDirty)
        {
            ScaleSlider.value = VolumeObjectFactory.gTargetVolume.GetVolumeUnifiedScale();
            ScaleText.SetText(ScaleSlider.value.ToString("0.00"));
        }
    }
}
