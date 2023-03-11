using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityVolumeRendering;
public class SlicingEdit : BasicMutipleTargetUI
{
    public Transform PreviewContentParent;
    private List<SlicingPlane> mSlicingPlanes = new List<SlicingPlane>();
    private List<Transform> mPreviewSlices = new List<Transform>();
    private List<Material> mPreviewSliceMat = new List<Material>();

    //private Transform mWorkingTable;
    //private string mLastWorkingTableTarget = "";
    //private bool mWorkingTableDirty;
    private void Awake()
    {
        mPlaneColor = new Color(.15f, 0.5f, .25f);
        mPlaneColorInactive = new Color(.25f, 0.35f, .3f);
    }
    private void Start()
    {
        Initialize();
    }

    public void OnAddSlicingPlane()
    {
        if (!VolumeObjectFactory.gTargetVolume) return;

        Transform planeRoot = VolumeObjectFactory.gTargetVolume.CreateSlicingPlane();
        var slicePlane = VolumeObjectFactory.gTargetVolume.SlicingPlaneList[mIsVisibles.Count];
        var box_renderer = slicePlane.Find("CollidingBBox").GetComponent<MeshRenderer>();
        box_renderer.material.SetColor("_Color", mPlaneColor);
        mBBoxRenderer.Add(box_renderer);
        mTargetObjs.Add(planeRoot);
        mHandGrabInteractableObjs.Add(planeRoot.Find("HandGrabInteractable").gameObject);
        mIsVisibles.Add(true);

        AddOptionToTargetDropDown("SlicingPlane " + (++mTotalId));
        DropdownValueChanged(mIsVisibles.Count);

        var preview_plane = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/SlicingPlanePreview")).GetComponent<RectTransform>();
        preview_plane.parent = PreviewContentParent;

        preview_plane.localRotation = Quaternion.identity;
        preview_plane.localScale = Vector3.one;
        preview_plane.localPosition = Vector3.zero;

        var slicingPlaneData = slicePlane.GetComponent<SlicingPlane>();
        mSlicingPlanes.Add(slicingPlaneData);
        var frame_material = new Material(preview_plane.GetComponentInChildren<Image>().material);

        frame_material.SetTexture("_DataTex", slicingPlaneData.mDataTex);
        frame_material.SetTexture("_TFTex", slicingPlaneData.mTFTex);
        mPreviewSliceMat.Add(frame_material);

        preview_plane.Find("Title").GetComponent<TMPro.TMP_Text>().SetText("Slice " + mTotalId);
        mPreviewSlices.Add(preview_plane);
    }
    public void OnRemoveSlicingPlane()
    {
        if (!VolumeObjectFactory.gTargetVolume || mTargetId < 0) return;
        VolumeObjectFactory.gTargetVolume.DeleteSlicingPlaneAt(mTargetId);
        GameObject.Destroy(mPreviewSlices[mTargetId].gameObject);
        mPreviewSlices.RemoveAt(mTargetId);
        mPreviewSliceMat.RemoveAt(mTargetId);
        mSlicingPlanes.RemoveAt(mTargetId);
        OnRemoveTarget();
    }
    //private void UpdateWorkingTableCanvas()
    //{
    //    if (mTargetId < 0 || !mWorkingTable) return;

    //    //MENGHE: Instead of deleting, setActive off
    //    if (mLastWorkingTableTarget.Length > 0)
    //        GameObject.Destroy(mWorkingTable.Find(mLastWorkingTableTarget).gameObject);

    //    GameObject working_canvas = Instantiate(DisplayRackFactory.GetFrameOnRack(DisplayRackFactory.DisplayRacks.ROOM_LEFT_BOARD, mTargetId).gameObject);
    //    working_canvas.transform.parent = mWorkingTable;
    //    mLastWorkingTableTarget = "SlicingPlane" + mTargetId;
    //    working_canvas.name = mLastWorkingTableTarget;
    //    working_canvas.transform.localPosition = new Vector3(0f, 0.03f, 0f);
    //    working_canvas.transform.localRotation = Quaternion.Euler(90.0f, .0f, .0f);
    //    working_canvas.transform.localScale = Vector3.one * 0.5f;
    //}
    //public void OnEnableWorkingTableTarget(in Transform table)
    //{
    //    mWorkingTable = table;
    //    mWorkingTableDirty = true;
    //    //CreateWorkingTableCanvas();
    //}
    //public void OnDisableWorkingTableTarget()
    //{
    //    mWorkingTable = null;
    //}

    //private void OnDestroy()
    //{
    //DisplayRackFactory.DeAttachFromRack(DisplayRackFactory.DisplayRacks.ROOM_LEFT_BOARD);
    //}
    protected override void OnChangeVisibilityStatus()
    {
        if (mTargetId < 0) return;
        base.OnChangeVisibilityStatus();
        mPreviewSlices[mTargetId].gameObject.SetActive(mIsVisibles[mTargetId]);
    }

    private void Update()
    {
        //Render Previews
        for (int fid = 0; fid < mPreviewSlices.Count; fid++)
        {
            if (!mIsVisibles[fid]) continue;
            
            mPreviewSliceMat[fid].DisableKeyword("OVERRIDE_MODEL_MAT");
            mPreviewSliceMat[fid].SetMatrix("_parentInverseMat",
                                            mSlicingPlanes[fid].mParentTransform ?
                                            mSlicingPlanes[fid].mParentTransform.worldToLocalMatrix : mSlicingPlanes[fid].transform.worldToLocalMatrix);

            mPreviewSliceMat[fid].SetMatrix("_planeMat", Matrix4x4.TRS(
                mSlicingPlanes[fid].transform.position,
                mSlicingPlanes[fid].transform.rotation,
                mSlicingPlanes[fid].mParentTransform ? mSlicingPlanes[fid].mParentTransform.lossyScale : mSlicingPlanes[fid].transform.lossyScale));

            mPreviewSlices[fid].GetComponentInChildren<Image>().material = mPreviewSliceMat[fid];
        }

        //DO NOT TOUCH!
        if (mTargetId < 0 || !VolumeObjectFactory.gHandGrabbleDirty) return;

        mHandGrabInteractableObjs[mTargetId].SetActive(false);
        mHandGrabInteractableObjs[mTargetId].SetActive(true);
    }
    //private void LateUpdate()
    //{
    //    if (mWorkingTableDirty)
    //    {
    //        UpdateWorkingTableCanvas();
    //        mWorkingTableDirty = false;
    //    }
    //}
}
