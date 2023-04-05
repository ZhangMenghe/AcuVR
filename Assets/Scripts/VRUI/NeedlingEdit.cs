using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityVolumeRendering;

public class NeedlingEdit : MonoBehaviour
{
    public TMPro.TMP_Dropdown TargetDropDown;
    public Button NeedleTargetButton;

    private List<Transform> mNeedleObjs = new List<Transform>();
    private List<string> mNeedleNames = new List<string>();

    private Transform mAcuNeedleRoot;

    private int mTargetId = -1;
    private int mTotalId = 0;
    private bool mNeedleTargetVolume = true;
    private void Start()
    {
        mAcuNeedleRoot = new GameObject("NeedleRoot").transform;
        mTargetId = -1;
        TargetDropDown.onValueChanged.AddListener(delegate {
            OnTargetValueChanged(TargetDropDown.value-1);
        });
        NeedleTargetButton.onClick.AddListener(delegate {
            OnChangeNeedleTarget();
        });
    }
    private void ResetTargetNeedle()
    {
        mNeedleObjs[mTargetId].position = Camera.main.transform.position + new Vector3(.0f, .0f, 0.3f);
        mNeedleObjs[mTargetId].rotation = Quaternion.identity;
        mNeedleObjs[mTargetId].localScale = Vector3.one;

        mNeedleObjs[mTargetId].GetComponent<GrabbaleAcuNeedle>().OnReset();
    }
    public void OnAddNeedle()
    {
        mTotalId++;

        var acuNeedleObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/AAAAcuNeedle")).transform;
        acuNeedleObj.name = "Needle " + mTotalId;
        acuNeedleObj.parent = mAcuNeedleRoot;
        acuNeedleObj.GetComponent<GrabbaleAcuNeedle>().SetNeedlingManagerPanel(this);
        mNeedleObjs.Add(acuNeedleObj);
        mNeedleNames.Add(acuNeedleObj.name);
        AddOptionToTargetDropDown(acuNeedleObj.name);
        OnResetNeedle();
    }
    public void OnResetNeedle()
    {
        if (mTargetId < 0) return;
        ResetTargetNeedle();
    }
    public void OnRemoveNeedleFromList(string name)
    {
        int index = mNeedleNames.FindIndex(item => item == name);
        if (index < 0) return;

        if(mTargetId >= 0)
        {
            if (index != mTargetId)
                mNeedleObjs[mTargetId].GetComponent<GrabbaleAcuNeedle>().OnChangeTarget(false);
            mTargetId = -1;
        }

        mNeedleObjs.RemoveAt(index);
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
        TargetDropDown.value = mNeedleObjs.Count;
        TargetDropDown.RefreshShownValue();

        OnTargetValueChanged(mNeedleObjs.Count - 1);
    }
    private void OnTargetValueChanged(int newTarget)
    {
        if (mTargetId == newTarget) return;

        if (mTargetId >= 0) mNeedleObjs[mTargetId].GetComponent<GrabbaleAcuNeedle>().OnChangeTarget(false);
        if (newTarget >= 0) mNeedleObjs[newTarget].GetComponent<GrabbaleAcuNeedle>().OnChangeTarget(true);

        mTargetId = newTarget;
    }
}
