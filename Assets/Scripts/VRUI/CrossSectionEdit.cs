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
    protected Texture2D TargetTex;
    protected Texture2D unTargetTex;

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
            OnChangeVisibilityStatus();
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
        TargetTex = Resources.Load<Texture2D>("Textures/CrossSectionPlaneTexture");
        unTargetTex = Resources.Load<Texture2D>("Textures/CrossSectionPlaneTextureDim");
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

    protected virtual void OnChangeVisibilityStatus()
    {
        if (mTargetId < 0) return;

        mSectionVisibilities[mTargetId] = !mSectionVisibilities[mTargetId];
        RootUIManager.mTargetVolume.m_cs_planes[mTargetId].parent.parent.gameObject.SetActive(mSectionVisibilities[mTargetId]);
        RootUIManager.mTargetVolume.m_cs_planes[mTargetId].gameObject.SetActive(mSectionVisibilities[mTargetId]);
        UpdateSprite(mSectionVisibilities[mTargetId]);
    }

    protected virtual void UpdateSnapableObjectStatus(int value)
    {
        //disable current one
        if (mTargetId >= 0)
        {
            RootUIManager.mTargetVolume.m_cs_planes[mTargetId].GetComponent<MeshRenderer>().material.SetTexture("_MainTex", unTargetTex);
            RootUIManager.mTargetVolume.m_cs_planes[mTargetId].parent.parent.Find("HandGrabInteractable").gameObject.SetActive(false);
        }
        //enable the new one
        if (value >= 0)
        {
            RootUIManager.mTargetVolume.m_cs_planes[value].GetComponent<MeshRenderer>().material.SetTexture("_MainTex", TargetTex);
            RootUIManager.mTargetVolume.m_cs_planes[value].parent.parent.Find("HandGrabInteractable").gameObject.SetActive(true);
        }
    }

    protected void DropdownValueChanged(int value)
    {
        if ((value-1) == mTargetId) return;
        //update the interactable status
        UpdateSnapableObjectStatus(value - 1);

        //Update visibility status of the current plane
        UpdateSprite(value < 1 ? true : mSectionVisibilities[value-1]);

        mTargetId = value - 1;
    }
}
