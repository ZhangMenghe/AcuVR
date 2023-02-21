using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossSectionEdit : MonoBehaviour
{
    public CylinderUI RootUIManager;
    public Transform TargetDropDownObj;
    public Button VisibilityBtn;

    protected int mTargetId = -1;

    protected List<bool> mSectionVisibilities = new List<bool>();
    protected TMPro.TMP_Dropdown TargetDropDown;

    protected Sprite InvisibleSprite;
    protected Sprite VisibleSprite;

    protected void Initialize()
    {
        VisibleSprite = Resources.Load<Sprite>("icons/see");
        InvisibleSprite = Resources.Load<Sprite>("icons/unsee");

        mTargetId = -1;
        TargetDropDown = TargetDropDownObj.GetComponent<TMPro.TMP_Dropdown>();
        TargetDropDown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(TargetDropDown.value);
        });

        VisibilityBtn.onClick.AddListener(delegate {
            OnChangePlaneStatus();
        });
    }
    protected void AddOptionToTargetDropDown(string entry)
    {
        TMPro.TMP_Dropdown.OptionData newOption = new TMPro.TMP_Dropdown.OptionData();
        newOption.text = entry;
        TargetDropDown.options.Add(newOption);

        TargetDropDown.RefreshShownValue();
        TargetDropDown.Show();
    }
    protected void RemoveTargetOptionFromDropDown()
    {
        mSectionVisibilities.RemoveAt(mTargetId);
        TargetDropDown.options.RemoveAt(mTargetId + 1);
        mTargetId = -1;
        TargetDropDown.SetValueWithoutNotify(0);
        UpdateSprite(true);

    }
    protected void UpdateSprite(bool isChecked)
    {
        SpriteState spriteState = VisibilityBtn.spriteState;

        // Toggle between the ON and OFF sprites
        if (isChecked)
        {
            VisibilityBtn.image.sprite = VisibleSprite;
            //spriteState.highlightedSprite = VisibleSprite;
        }
        else
        {
            VisibilityBtn.image.sprite = InvisibleSprite;
            //spriteState.highlightedSprite = InvisibleSprite;
        }

        VisibilityBtn.spriteState = spriteState;
    }

    private void Start()
    {
        Initialize();
    }

    public void OnAddCrossSectionPlane()
    {
        //MENGHE: ADD A NOTIFICATION OR STH
        if (!RootUIManager.mTargetVolume || mSectionVisibilities.Count > UnityVolumeRendering.VolumeRenderedObject.MAX_CS_PLANE_NUM) return;

        RootUIManager.mTargetVolume.CreateCrossSectionPlane();
        mSectionVisibilities.Add(true);
        AddOptionToTargetDropDown("CSPlane " + mSectionVisibilities.Count.ToString());
    }
    public void OnRemoveCrossSectionPlane()
    {
        if (RootUIManager.mTargetVolume && mTargetId >= 0)
        {
            RootUIManager.mTargetVolume.DeleteCrossSectionPlaneAt(mTargetId);
            RemoveTargetOptionFromDropDown();
        }
    }

    protected virtual void OnChangePlaneStatus()
    {
        if (mTargetId < 0) return;

        mSectionVisibilities[mTargetId] = !mSectionVisibilities[mTargetId];
        RootUIManager.mTargetVolume.m_cs_planes[mTargetId].gameObject.SetActive(mSectionVisibilities[mTargetId]);

        UpdateSprite(mSectionVisibilities[mTargetId]);
    }

    protected void DropdownValueChanged(int value)
    {
        mTargetId = value - 1;
        UpdateSprite(mTargetId<0? true:mSectionVisibilities[mTargetId]);
    }
}
