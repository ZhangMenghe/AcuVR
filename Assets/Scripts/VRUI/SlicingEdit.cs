using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicingEdit : CrossSectionEdit
{
    private void Start()
    {
        Initialize();
    }

    public void OnAddSlicingPlane()
    {
        if (RootUIManager.mTargetVolume)
        {
            RootUIManager.mTargetVolume.CreateSlicingPlane();
            mSectionVisibilities.Add(true);
            AddOptionToTargetDropDown("SlicingPlane " + mSectionVisibilities.Count.ToString());
        }
    }
    public void OnRemoveSlicingPlane()
    {
        if (RootUIManager.mTargetVolume && mTargetId >= 0)
        {
            RootUIManager.mTargetVolume.DeleteSlicingPlaneAt(mTargetId);
            RemoveTargetOptionFromDropDown();
        }
    }

    protected override void OnChangePlaneStatus()
    {
        if (mTargetId < 0) return;

        mSectionVisibilities[mTargetId] = !mSectionVisibilities[mTargetId];
        RootUIManager.mTargetVolume.SlicingPlaneList[mTargetId].gameObject.SetActive(mSectionVisibilities[mTargetId]);

        UpdateSprite(mSectionVisibilities[mTargetId]);
    }
}
