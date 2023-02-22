using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicingEdit : CrossSectionEdit
{
    private void Start()
    {
        Initialize();
        DisplayRackFactory.AttachToRack(DisplayRackFactory.DisplayRacks.ROOM_LEFT_BOARD, "Slicing Planes");
    }

    public void OnAddSlicingPlane()
    {
        if (RootUIManager.mTargetVolume)
        {
            RootUIManager.mTargetVolume.CreateSlicingPlane();
            //DisplayRackFactory.AddFrame(DisplayRackFactory.DisplayRacks.ROOM_LEFT_BOARD, RootUIManager.mTargetVolume.CreateSlicingPlane());

            mSectionVisibilities.Add(true);
            AddOptionToTargetDropDown("SlicingPlane " + mSectionVisibilities.Count.ToString());
        }
    }
    public void OnRemoveSlicingPlane()
    {
        if (RootUIManager.mTargetVolume && mTargetId >= 0)
        {
            RootUIManager.mTargetVolume.DeleteSlicingPlaneAt(mTargetId);
            DisplayRackFactory.RemoveFrame(DisplayRackFactory.DisplayRacks.ROOM_LEFT_BOARD, mTargetId);
            RemoveTargetOptionFromDropDown();
        }
    }

    protected override void OnChangePlaneStatus()
    {
        if (mTargetId < 0) return;

        mSectionVisibilities[mTargetId] = !mSectionVisibilities[mTargetId];
        RootUIManager.mTargetVolume.SlicingPlaneList[mTargetId].gameObject.SetActive(mSectionVisibilities[mTargetId]);

        UpdateSprite(mSectionVisibilities[mTargetId]);

        DisplayRackFactory.ChangeFrameVisibilityStatus(DisplayRackFactory.DisplayRacks.ROOM_LEFT_BOARD, mTargetId, mSectionVisibilities[mTargetId]);
    }
    private void OnDestroy()
    {
        DisplayRackFactory.DeAttachFromRack(DisplayRackFactory.DisplayRacks.ROOM_LEFT_BOARD);
    }

    private void Update()
    {
        DisplayRackFactory.RenderFrames();
    }
}
