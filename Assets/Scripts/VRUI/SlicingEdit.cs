using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicingEdit : CrossSectionEdit
{
    private void Start()
    {
        TargetTex = Resources.Load<Texture2D>("Textures/SlicingPlaneBBoxTexture");
        unTargetTex = Resources.Load<Texture2D>("Textures/SlicingPlaneBBoxTextureDim");

        Initialize();
        DisplayRackFactory.AttachToRack(DisplayRackFactory.DisplayRacks.ROOM_LEFT_BOARD, "Slicing Planes");
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
            DisplayRackFactory.RemoveFrame(DisplayRackFactory.DisplayRacks.ROOM_LEFT_BOARD, mTargetId);
            RemoveTargetOptionFromDropDown();
        }
    }

    protected override void OnChangeVisibilityStatus()
    {
        if (mTargetId < 0) return;

        mSectionVisibilities[mTargetId] = !mSectionVisibilities[mTargetId];
        RootUIManager.mTargetVolume.SlicingPlaneList[mTargetId].parent.parent.gameObject.SetActive(mSectionVisibilities[mTargetId]);

        UpdateSprite(mSectionVisibilities[mTargetId]);

        DisplayRackFactory.ChangeFrameVisibilityStatus(DisplayRackFactory.DisplayRacks.ROOM_LEFT_BOARD, mTargetId, mSectionVisibilities[mTargetId]);
    }
    protected override void UpdateSnapableObjectStatus(int value)
    {
        //disable current one
        if (mTargetId >= 0)
        {
            RootUIManager.mTargetVolume.SlicingPlaneList[mTargetId].GetComponent<UnityVolumeRendering.SlicingPlane>().mPlaneBoundaryRenderer.material.SetTexture("_MainTex", unTargetTex);

            RootUIManager.mTargetVolume.SlicingPlaneList[mTargetId].parent.parent.Find("HandGrabInteractable").gameObject.SetActive(false);
        }
        //enable the new one
        if (value >= 0)
        {
            RootUIManager.mTargetVolume.SlicingPlaneList[value].GetComponent<UnityVolumeRendering.SlicingPlane>().mPlaneBoundaryRenderer.material.SetTexture("_MainTex", TargetTex);

            RootUIManager.mTargetVolume.SlicingPlaneList[value].parent.parent.Find("HandGrabInteractable").gameObject.SetActive(true);
        }

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
