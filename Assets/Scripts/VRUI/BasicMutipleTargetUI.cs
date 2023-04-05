using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicMutipleTargetUI : MonoBehaviour
{
    public TMPro.TMP_Dropdown TargetDropDown;
    public Button VisibilityBtn;
    
    [HideInInspector]
    public int mTargetId = -1;
    protected int mTotalId = 0;

    [HideInInspector]
    public List<bool> mIsVisibles = new List<bool>();
    [HideInInspector]
    public List<Transform> mTargetObjs = new List<Transform>();
    
    protected List<GameObject> mHandGrabInteractableObjs = new List<GameObject>();
    protected List<MeshRenderer> mBBoxRenderer = new List<MeshRenderer>();

    protected bool mVisibleButtonStatus = true;
    protected Color mPlaneColor;
    protected Color mPlaneColorInactive;

    protected void Initialize()
    {
        mTargetId = -1;
        //mManipulateMode = false;

        TargetDropDown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(TargetDropDown.value);
        });

        VisibilityBtn.onClick.AddListener(delegate {
            OnChangeVisibilityStatus();
        });
    }

    protected void AddOptionToTargetDropDown(string entry, bool targetOnNew = true)
    {
        TMPro.TMP_Dropdown.OptionData newOption = new TMPro.TMP_Dropdown.OptionData();
        newOption.text = entry;
        TargetDropDown.options.Add(newOption);

        if (targetOnNew)
        {
            TargetDropDown.value = mIsVisibles.Count;
            mTargetId = mIsVisibles.Count - 1;
        }

        TargetDropDown.RefreshShownValue();
    }

    protected void RemoveTargetOptionFromDropDown()
    {
        mIsVisibles.RemoveAt(mTargetId);
        TargetDropDown.options.RemoveAt(mTargetId + 1);
        mTargetId = -1;
        mVisibleButtonStatus = true;
        TargetDropDown.SetValueWithoutNotify(0);
    }
    protected virtual void OnRemoveTarget()
    {
        if (!mIsVisibles[mTargetId]) VRUICommonUtils.SwapSprite(ref VisibilityBtn);
        mIsVisibles.RemoveAt(mTargetId);
        mHandGrabInteractableObjs.RemoveAt(mTargetId);
        mTargetObjs.RemoveAt(mTargetId);
        mBBoxRenderer.RemoveAt(mTargetId);


        TargetDropDown.options.RemoveAt(mTargetId + 1);
        mTargetId = -1;
        mVisibleButtonStatus = true;
        TargetDropDown.SetValueWithoutNotify(0);
    }
    protected virtual void OnChangeVisibilityStatus()
    {
        if (mTargetId < 0) return;
        mIsVisibles[mTargetId] = !mIsVisibles[mTargetId];
        mTargetObjs[mTargetId].gameObject.SetActive(mIsVisibles[mTargetId]);
        mVisibleButtonStatus = !mVisibleButtonStatus;
        VRUICommonUtils.SwapSprite(ref VisibilityBtn);
    }

    protected virtual void UpdateSnapableObjectStatus(int value) {
        if (mTargetId >= 0)
        {
            mBBoxRenderer[mTargetId].material.SetColor("_Color", mPlaneColorInactive);
            mHandGrabInteractableObjs[mTargetId].SetActive(false);
        }
        //enable the new one
        if (value >= 0)
        {
            mBBoxRenderer[value].material.SetColor("_Color", mPlaneColor);
            mHandGrabInteractableObjs[value].SetActive(true);
        }
    }

    protected virtual void DropdownChangeFinalize() { }
    //protected virtual void ResetManipulator() { }

    protected void DropdownValueChanged(int value)
    {
        if ((value-1) == mTargetId) return;
        UpdateSnapableObjectStatus(value - 1);
        
        if(value > 0 && mIsVisibles[value - 1] != mVisibleButtonStatus)
        {
            VRUICommonUtils.SwapSprite(ref VisibilityBtn);
            mVisibleButtonStatus = !mVisibleButtonStatus;
        }
        if (value == 0 && mVisibleButtonStatus == false)
        {
            VRUICommonUtils.SwapSprite(ref VisibilityBtn);
            mVisibleButtonStatus = !mVisibleButtonStatus;
        }

        mTargetId = value - 1;
        DropdownChangeFinalize();
        //ResetManipulator();
    }
    /*
    protected void InitializeManipulator()
    {
        ManipulateSprite = Resources.Load<Sprite>("icons/manipulation");
        ManulSprite = Resources.Load<Sprite>("icons/grab");

        mManipulator = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/RotationManipulator"));
        mManipulator.name = "RotationManipulator";
        HideManipulator();
    }
    protected void HideManipulator()
    {
        mManipulator.transform.parent = null;
        mManipulator.SetActive(false);
    }
    protected void OnChangeTransformManipulator()
    {
        mManipulateMode = !mManipulateMode;
        ResetManipulator();
    }
    */
}
