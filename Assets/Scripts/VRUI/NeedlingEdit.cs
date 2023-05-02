using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeedlingEdit : MonoBehaviour
{
    public TMPro.TMP_Dropdown TargetDropDown;
    public Button NeedleTargetButton;
    public Button GlowBtn;
    public Button LockBtn;

    public List<GrabbaleAcuNeedle> mGrabbableNeedles{ get; private set; }
    public List<string> mNeedleNames { get; private set; }

    private Transform mAcuNeedleRoot;

    public int mTargetId { get; private set; }
    private int mTotalId = 0;
    private bool mNeedleTargetVolume = true;
    private bool mIsGlowing = false;
    private bool mCurrentLock;

    private void Start()
    {
        mCurrentLock = false; mTargetId = -1;
        mGrabbableNeedles = new List<GrabbaleAcuNeedle>();
        mNeedleNames = new List<string>();

        //mAcuNeedleRoot = new GameObject("NeedleRoot").transform;
        mAcuNeedleRoot = VolumeObjectFactory.gTargetVolume.transform.parent;
        mTargetId = -1;
        TargetDropDown.onValueChanged.AddListener(delegate {
            OnTargetValueChanged(TargetDropDown.value-1);
        });
        NeedleTargetButton.onClick.AddListener(delegate {
            OnChangeNeedleTarget();
        });
        GlowBtn.onClick.AddListener(delegate {
            OnGlowStatusChange();
        });
        LockBtn.onClick.AddListener(delegate {
            OnTargetLockChange();
        });
        StandardModelFactory.setNeedlingManager(this);
    }
    private void ResetTargetNeedle()
    {
        if (mTargetId < 0) return;
        mGrabbableNeedles[mTargetId].OnReset();
        if (mCurrentLock)
        {
            //unlock the target needle during reset
            mCurrentLock = !mCurrentLock;
            VRUICommonUtils.SwapSprite(ref LockBtn);
        }
    }
    public void OnAddNeedle()
    {
        mTotalId++;

        var acuNeedleObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/AAAAcuNeedle")).GetComponent<GrabbaleAcuNeedle>();
        
        acuNeedleObj.transform.name = "Needle " + mTotalId;
        acuNeedleObj.transform.parent = mAcuNeedleRoot;
        acuNeedleObj.OnInitialized(mTotalId);
        mGrabbableNeedles.Add(acuNeedleObj);
        
        mNeedleNames.Add(acuNeedleObj.name);
        AddOptionToTargetDropDown(acuNeedleObj.name);
        OnTargetReset();

        if (mCurrentLock)
        {
            //unlock the target needle during reset
            mCurrentLock = !mCurrentLock;
            VRUICommonUtils.SwapSprite(ref LockBtn);
        }
    }
    public void OnGlowStatusChange()
    {
        mIsGlowing = !mIsGlowing;
        VRUICommonUtils.SwapSprite(ref GlowBtn);

        foreach(GrabbaleAcuNeedle needle_obj in mGrabbableNeedles)
            needle_obj.GlowObj.SetActive(mIsGlowing);
    }
    public void OnTargetLockChange()
    {
        if (mTargetId < 0) return;

        mGrabbableNeedles[mTargetId].OnLockStatusChange();
        mCurrentLock = !mCurrentLock;
        VRUICommonUtils.SwapSprite(ref LockBtn);
    }
    public void OnTargetReset()
    {
        if (mTargetId < 0) return;
        ResetTargetNeedle();
    }
    public void OnTargetDelete()
    {
        if (mTargetId < 0) return;

        GameObject.Destroy(mGrabbableNeedles[mTargetId].gameObject);

        mGrabbableNeedles.RemoveAt(mTargetId);
        mNeedleNames.RemoveAt(mTargetId);
        RemoveTargetOptionFromDropDown(mTargetId);

        if (mNeedleTargetVolume)
            StandardModelFactory.OnDeleteNeedle(mTargetId);
        mTargetId = -1;
        VolumeObjectFactory.OnTargetNeedleChange(-1, null);
    }

    public void OnRemoveNeedleFromList(string name)
    {
        int index = mNeedleNames.FindIndex(item => item == name);
        if (index < 0) return;

        if(mTargetId >= 0)
        {
            if (index != mTargetId)
                mGrabbableNeedles[mTargetId].OnChangeTarget(false);
            mTargetId = -1;
            VolumeObjectFactory.OnTargetNeedleChange(-1, null);

        }

        mGrabbableNeedles.RemoveAt(index);
        mNeedleNames.RemoveAt(index);
        RemoveTargetOptionFromDropDown(index);

        if (mNeedleTargetVolume)
            StandardModelFactory.OnDeleteNeedle(index);
    }

    private void OnChangeNeedleTarget()
    {
        mNeedleTargetVolume = !mNeedleTargetVolume;
        VRUICommonUtils.SwapSprite(ref NeedleTargetButton);
        //MENGHE:UNFINISHED
    }

    private void RemoveTargetOptionFromDropDown(int target)
    {
        TargetDropDown.options.RemoveAt(target + 1);
        TargetDropDown.SetValueWithoutNotify(0);
    }
    private void AddOptionToTargetDropDown(string entry)
    {
        TMPro.TMP_Dropdown.OptionData newOption = new TMPro.TMP_Dropdown.OptionData();
        newOption.text = entry;
        TargetDropDown.options.Add(newOption);
        TargetDropDown.value = mGrabbableNeedles.Count;
        TargetDropDown.RefreshShownValue();

        OnTargetValueChanged(mGrabbableNeedles.Count - 1);
    }
    private void OnTargetValueChanged(int newTarget)
    {
        if (mTargetId == newTarget) return;
        bool new_target_locked = false;

        if (mTargetId >= 0) mGrabbableNeedles[mTargetId].OnChangeTarget(false);
        if (newTarget >= 0)
        {
            mGrabbableNeedles[newTarget].OnChangeTarget(true);
            new_target_locked = mGrabbableNeedles[newTarget].IsLock;
        }

        if (mCurrentLock != new_target_locked)
        {
            mCurrentLock = !mCurrentLock;
            VRUICommonUtils.SwapSprite(ref LockBtn);
        }
        mTargetId = newTarget;
        VolumeObjectFactory.OnTargetNeedleChange(mTargetId, mTargetId<0?null:mGrabbableNeedles[mTargetId]);
    }
}
