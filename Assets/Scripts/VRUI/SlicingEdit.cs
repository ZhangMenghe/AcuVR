using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityVolumeRendering;
public class SlicingEdit : BasicMutipleTargetUI
{
    private enum ScrollStatus
    {
        NONE,
        SCROLL_VIEW_ADD,
        SCROLL_VIEW_REMOVE,
        SCROLL_VIEW_CHANGE
    }
    public Transform PreviewContentParent;
    private List<SlicingPlane> mSlicingPlanes = new List<SlicingPlane>();
    private List<RectTransform> mPreviewSlices = new List<RectTransform>();
    private List<Material> mPreviewSliceMat = new List<Material>();
    private List<bool> mSliceVisibilities = new List<bool>();

    private RectTransform mScrollContentRect;
    private float mScrollWidth;
    private float mContentWidth;
    private float mSpacing = 2.0f;
    private float mLastScrollPosition = -10.0f;
    private bool mScrollConsumed = true;
    private GrabbaleAcuNeedle mTargetNeedle=null;
    //private Transform mWorkingTable;
    //private string mLastWorkingTableTarget = "";
    //private bool mWorkingTableDirty;
    private void Awake()
    {
        mPlaneColor = new Color(.15f, 0.5f, .25f, 1.0f);
        mPlaneColorInactive = new Color(.25f, 0.35f, .3f, 0.01f);
    }
    private void Start()
    {
        Initialize();
        var scroll_rect = transform.GetComponentInChildren<ScrollRect>();
        mScrollWidth = scroll_rect.GetComponent<RectTransform>().sizeDelta.x;
        mScrollContentRect = scroll_rect.content.GetComponent<RectTransform>();
    }
    public void DisableNeeldeProjection()
    {
        mTargetNeedle = null;
        int vid = 0;
        foreach(var plane in mTargetObjs)
        {
            if (mSliceVisibilities[vid++])
            {
                plane.GetComponent<SingleSlicingPlane>().OnDisableNeedleProject();
                mPreviewSlices[vid - 1].transform.Find("ProjNeedlePlane").gameObject.SetActive(false);
            }
        }
    }
    public void EnableNeedleProjection(in GrabbaleAcuNeedle needle)
    {
        mTargetNeedle = needle;
        int vid = 0;
        foreach (var plane in mTargetObjs)
        {
            if (mSliceVisibilities[vid++])
            {
                plane.GetComponent<SingleSlicingPlane>().onEnableNeedleProject(needle.NeedleTipTransform, needle.NeedleEndTransform);

                var proj_plane = mPreviewSlices[vid - 1].transform.Find("ProjNeedlePlane");
                proj_plane.gameObject.SetActive(true);
            }
        }
    }
    public void OnScrollValueChange(Vector2 scrollPosition)
    {
        if (!mScrollConsumed || mSliceVisibilities.Count==0) return;
        bool scroll_left = mLastScrollPosition < scrollPosition.x;
        int frontId, backId;
        for (frontId = 0; frontId < mSliceVisibilities.Count && !mSliceVisibilities[frontId]; frontId++) ;
        if(!scroll_left) frontId -= 1;

        if (frontId >= 0)
            mSliceVisibilities[frontId] = mPreviewSlices[frontId].anchoredPosition.x + mScrollContentRect.anchoredPosition.x > 0;

        for (backId = mSliceVisibilities.Count - 1; backId > frontId && !mSliceVisibilities[backId]; backId--);
        if (scroll_left) backId += 1;

        ////scroll left
        //if (mLastScrollPosition < scrollPosition.x)
        //{
        //    for (frontId = 0; frontId < mSliceVisibilities.Count && !mSliceVisibilities[frontId]; frontId++);
        //    //mSliceVisibilities[frontId] = mPreviewSlices[frontId].anchoredPosition.x + mScrollContentRect.anchoredPosition.x > 0;

        //    for (backId = mSliceVisibilities.Count - 1; backId > frontId && !mSliceVisibilities[backId]; backId--);
        //    backId += 1;

        //    //if (backId > frontId && backId < mSliceVisibilities.Count)
        //    //{
        //    //    mSliceVisibilities[backId] = mPreviewSlices[backId].anchoredPosition.x + mScrollContentRect.anchoredPosition.x < mScrollWidth - mContentWidth;
        //    //}
        //}
        //else
        //{
        //    for (frontId = 0; frontId < mSliceVisibilities.Count && !mSliceVisibilities[frontId]; frontId++) ;
        //    frontId -= 1;

        //    for (backId = mSliceVisibilities.Count - 1; backId > frontId && !mSliceVisibilities[backId]; backId--);
        //}
        
        if (backId > frontId && backId < mSliceVisibilities.Count)
        {
            mSliceVisibilities[backId] = mPreviewSlices[backId].anchoredPosition.x + mScrollContentRect.anchoredPosition.x < mScrollWidth - mContentWidth;
        }

        mLastScrollPosition = scrollPosition.x;
        mScrollConsumed = false;
    }

    public void OnAddSlicingPlane()
    {
        if (!VolumeObjectFactory.gTargetVolume) return;

        Transform planeRoot = VolumeObjectFactory.gTargetVolume.CreateSlicingPlane();
        planeRoot.GetComponent<SingleSlicingPlane>().Initialized(++mTotalId);



        var slicePlane = VolumeObjectFactory.gTargetVolume.SlicingPlaneList[mIsVisibles.Count];
        var box_renderer = slicePlane.Find("CollidingBBox").GetComponent<MeshRenderer>();
        box_renderer.material.SetColor("_Color", mPlaneColor);
        //box_renderer.material.EnableKeyword("DEBUG");
        mBBoxRenderer.Add(box_renderer);
        mTargetObjs.Add(planeRoot);
        mHandGrabInteractableObjs.Add(planeRoot.Find("HandGrabInteractable").gameObject);
        mIsVisibles.Add(true);

        AddOptionToTargetDropDown("SlicingPlane " + mTotalId);
        DropdownValueChanged(mIsVisibles.Count);

        var previewRoot = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/SlicingPlanePreview"));
        var preview_plane = previewRoot.GetComponent<RectTransform>();
        preview_plane.parent = PreviewContentParent;

        preview_plane.localRotation = Quaternion.identity;
        preview_plane.localScale = Vector3.one;
        preview_plane.localPosition = Vector3.zero;

        mContentWidth = preview_plane.sizeDelta.x;

        var slicingPlaneData = slicePlane.GetComponent<SlicingPlane>();
        mSlicingPlanes.Add(slicingPlaneData);
        var frame_material = new Material(preview_plane.GetComponentInChildren<Image>().material);

        frame_material.SetTexture("_DataTex", slicingPlaneData.mDataTex);
        frame_material.SetTexture("_TFTex", slicingPlaneData.mTFTex);
        mPreviewSliceMat.Add(frame_material);

        preview_plane.Find("Title").GetComponent<TMPro.TMP_Text>().SetText("Slice " + mTotalId);
        mPreviewSlices.Add(preview_plane);

        var scount = mPreviewSlices.Count;
        if (scount > 1)
            mSliceVisibilities.Add(mPreviewSlices[scount - 2].anchoredPosition.x + mScrollContentRect.anchoredPosition.x < mScrollWidth - mContentWidth * 2 - mSpacing);
        else
            mSliceVisibilities.Add(mScrollContentRect.anchoredPosition.x>(-mContentWidth));
       
        previewRoot.transform.Find("ProjNeedlePlane").GetComponent<Image>().material.mainTexture = planeRoot.GetComponent<SingleSlicingPlane>().GetRenderTexture();

        if (mTargetNeedle)
        {
            planeRoot.GetComponent<SingleSlicingPlane>().onEnableNeedleProject(mTargetNeedle.NeedleTipTransform, mTargetNeedle.NeedleEndTransform);


            //previewRoot.GetComponent<SingleSlicingPlane>().onEnableNeedleProject(mTargetNeedle.NeedleTipTransform, mTargetNeedle.NeedleEndTransform);
        }
    }
    public void OnRemoveSlicingPlane()
    {
        if (!VolumeObjectFactory.gTargetVolume || mTargetId < 0) return;
        VolumeObjectFactory.gTargetVolume.DeleteSlicingPlaneAt(mTargetId);
        GameObject.Destroy(mPreviewSlices[mTargetId].gameObject);
        mPreviewSlices.RemoveAt(mTargetId);
        mSliceVisibilities.RemoveAt(mTargetId);
        mPreviewSliceMat.RemoveAt(mTargetId);
        mSlicingPlanes.RemoveAt(mTargetId);
        //adjust scroll view
        for(int i=mTargetId; i<mPreviewSlices.Count; i++)
        {
            var anx = mPreviewSlices[i].anchoredPosition.x + mScrollContentRect.anchoredPosition.x - mContentWidth - mSpacing;
            mSliceVisibilities[i] = (anx > 0) && (anx < mScrollWidth - mContentWidth);
            //mPreviewSlices[i].gameObject.SetActive(mSliceVisibilities[i]);
        }
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
    private void UpdateScrollViewVisibility()
    {
        int scount = mPreviewSlices.Count;
        //if(mScrollStatus == ScrollStatus.SCROLL_VIEW_ADD)
        //{
        //    //check the last one
        //    mSliceVisibilities[scount - 1] = mPreviewSlices[scount - 1].anchoredPosition.x + mScrollContentRect.rect.x < mScrollContentRect.sizeDelta.x - mContentWidth;
        //}
        //else if(mScrollStatus == ScrollStatus.SCROLL_VIEW_REMOVE)
        //{
        //    //Check all items following
        //    for(int i=mRemoveTarget; i<scount-1; i++)
        //    {
        //        var anchor_x = mPreviewSlices[i].GetComponent<RectTransform>().anchoredPosition.x;

        //        mSliceVisibilities[i] = anchor_x
        //    }
        //}
        //transform.GetComponentInChildren<ScrollRect>().content.GetComponent<RectTransform>()

        //if (front)
        //{
        //    for (int i = 0; i<mSliceVisibilities.Count; i++)
        //    {
        //        if (mPreviewSlices[i].GetComponent<RectTransform>().anchoredPosition.x < .0f) mSliceVisibilities[i] = false;
        //        else
        //            break;
        //    }
        //}
        //if (back)
        //{
        //    for (int i = mSliceVisibilities.Count - 1; i >= 0 && !mSliceVisibilities[i]; i--)
        //    {
        //        if (mPreviewSlices[i].GetComponent<RectTransform>().anchoredPosition.x < mScrollContentRect.sizeDelta.x - mContentWidth) mSliceVisibilities[i] = true;
        //    }
        //}
    }
    protected override void OnChangeVisibilityStatus()
    {
        if (mTargetId < 0) return;
        base.OnChangeVisibilityStatus();
        mPreviewSlices[mTargetId].gameObject.SetActive(mIsVisibles[mTargetId]);
        if (mTargetNeedle && mIsVisibles[mTargetId])
        {
           mTargetObjs[mTargetId].GetComponent<SingleSlicingPlane>().onEnableNeedleProject(mTargetNeedle.NeedleTipTransform, mTargetNeedle.NeedleEndTransform);
           //mPreviewSlices[mTargetId].GetComponent<SingleSlicingPlane>().onEnableNeedleProject(mTargetNeedle.NeedleTipTransform, mTargetNeedle.NeedleEndTransform);
        }
    }

    private void Update()
    {
        //if (mScrollStatus!=ScrollStatus.NONE)
        //{
        //    UpdateScrollViewVisibility();
        //    mScrollStatus = ScrollStatus.NONE;
        //}
        //Render Previews
        for (int fid = 0; fid < mPreviewSlices.Count; fid++)
        {
            if (!mIsVisibles[fid] || !mSliceVisibilities[fid])
                mPreviewSliceMat[fid].EnableKeyword("DISCARD_ALL");
            else
            {
                mPreviewSliceMat[fid].DisableKeyword("DISCARD_ALL");
                mPreviewSliceMat[fid].DisableKeyword("OVERRIDE_MODEL_MAT");
                mPreviewSliceMat[fid].SetMatrix("_parentInverseMat",
                                                mSlicingPlanes[fid].mParentTransform ?
                                                mSlicingPlanes[fid].mParentTransform.worldToLocalMatrix : mSlicingPlanes[fid].transform.worldToLocalMatrix);

                mPreviewSliceMat[fid].SetMatrix("_planeMat", Matrix4x4.TRS(
                    mSlicingPlanes[fid].transform.position,
                    mSlicingPlanes[fid].transform.rotation,
                    mSlicingPlanes[fid].mParentTransform ? mSlicingPlanes[fid].mParentTransform.lossyScale : mSlicingPlanes[fid].transform.lossyScale));
            }


            mPreviewSlices[fid].GetComponentInChildren<Image>().material = mPreviewSliceMat[fid];
        }
        mScrollConsumed = true;

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
