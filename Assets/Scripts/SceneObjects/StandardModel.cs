using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardModel : MonoBehaviour
{
    public Transform CrossPlaneRoot;
    public HeadLocator LocatorRoot;
    public Transform TargetObjectRoot;

    //private MeshRenderer meshRenderer;
    private MeshRenderer[] meshRenderers = new MeshRenderer[0];
    private List<Transform> mCrossPlanes = new List<Transform>();

    private Matrix4x4[] mCrossPlaneMatrices = new Matrix4x4[5];
    protected List<bool> mCrossPlaneVisibles = new List<bool>();
        
    private readonly int MAX_CROSS_PLANES = 5;
    private HeadLocator mRefVolumeLocator;

    private int mTargetCrossPlane = -1;
    private Transform mRefCrossPlane;
    private bool mIsLinked = false;

    private List<Transform> mNeedleObjs = new List<Transform>();
    private int mTargetNeedle = -1;
    private Transform mRefNeedleTransform = null;
    private GameObject mNaiveNeedleObj;
    private bool mIsHoldingNeedle = false;

    private void Awake()
    {
        mNaiveNeedleObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/AcuNeedleNaive"));
        mNaiveNeedleObj.SetActive(false);
        LocatorRoot.CreateVirtualMidObjs();
    }
    private void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>(false);
    }

    public void LinkVolume(in Transform volume, float unified_scale)
    {
        mRefVolumeLocator = volume.parent.GetComponentInChildren<HeadLocator>(true);
        mRefVolumeLocator.Initialize();
        LocatorRoot.Initialize();

        //Visual Group Scale
        transform.parent.localScale = LocatorRoot.GetRealSize(mRefVolumeLocator.worldSize)/ unified_scale;

        LocatorRoot.AlignLocators(TargetObjectRoot.parent, mRefVolumeLocator);

        TargetObjectRoot.localScale = Vector3.one * unified_scale;

        //MENGHE:MOVE THE LINKED OBJECTS MOVE TOGETHER WITHOUT ATTACHMENT
        //TargetObjectRoot.parent.parent = volume.parent;
        mIsLinked = true;
    }
    public void UnLinkVolume()
    {
        mRefVolumeLocator = null;
        //TargetObjectRoot.parent.parent = null;
        transform.parent.localScale = Vector3.one; //VisualGroup
        TargetObjectRoot.localScale = Vector3.one;
        mIsLinked = false;
        //LocatorRoot.ResetScale();

        //remove all the cross-section planes
        foreach(var plane in mCrossPlanes)
        {
            Destroy(plane.parent.gameObject);
        }
        mCrossPlanes.Clear();
        mCrossPlaneVisibles.Clear();
        foreach (MeshRenderer meshRenderer in meshRenderers)
            meshRenderer.material.SetInteger("_CrossSectionNum", 0);
        mTargetCrossPlane = -1;
        mRefCrossPlane = null;
    }

    public void SyncCrossSectionPlanes(in CrossSectionEdit CrossSectionManager)
    {
        if (CrossSectionManager.mIsVisibles.Count < 1) return;
        mTargetCrossPlane = -1;
        mRefCrossPlane = null;
        int pid = 0;
        foreach(var refPlane in CrossSectionManager.mTargetObjs)
        {
            if (pid < mCrossPlanes.Count)
            {
                mCrossPlanes[pid].parent.localPosition = Vector3.Scale(refPlane.localPosition, refPlane.lossyScale);
                mCrossPlanes[pid].parent.localRotation = refPlane.localRotation;
            }
            else
            {
                AddCrossSectionPlane(refPlane.gameObject, pid ==CrossSectionManager.mTargetId);
            }
            mCrossPlaneVisibles[pid] = CrossSectionManager.mIsVisibles[pid];

            if (!mCrossPlaneVisibles[pid])
                mCrossPlanes[pid].parent.gameObject.SetActive(false);

            pid++;
        }
    }

    public void AddCrossSectionPlane(in GameObject refPlaneRoot, bool targetOn)
    {
        if (!mIsLinked || mCrossPlanes.Count > MAX_CROSS_PLANES) return;

        Transform planeRoot = new GameObject("CSPlaneRoot").transform;
        planeRoot.parent = CrossPlaneRoot;
        planeRoot.localPosition = Vector3.zero;
        planeRoot.localRotation = Quaternion.identity;
        planeRoot.localScale = Vector3.one * 0.3f;

        Transform cross_plane = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/CrossSectionFollowPlane")).transform;
        cross_plane.parent = planeRoot;
        cross_plane.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        cross_plane.localPosition = Vector3.zero;
        cross_plane.localScale = Vector3.one * 1.2f;

        mCrossPlanes.Add(cross_plane);
        mCrossPlaneVisibles.Add(true);

        if (targetOn)
        {
            mTargetCrossPlane = mCrossPlanes.Count - 1;
            mRefCrossPlane = refPlaneRoot.transform;
        }
    }

    public void DeleteCrossSectionPlaneAt(int TargetId)
    {
        if (!mIsLinked || TargetId >= mCrossPlanes.Count) return;
            
        Destroy(mCrossPlanes[TargetId].parent.gameObject);
        mCrossPlanes.RemoveAt(TargetId);
        mCrossPlaneVisibles.RemoveAt(TargetId);
        mTargetCrossPlane = -1;
        mRefCrossPlane = null;
    }
    public void OnChangeCrossSectionTarget(int target, in Transform refPlaneRoot)
    {
        if (!mIsLinked) return;
        mTargetCrossPlane = target;
        mRefCrossPlane = refPlaneRoot;
    }
    public void OnChangeVisibility(int target, bool visible)
    {
        if (!mIsLinked) return;

        mCrossPlanes[target].parent.gameObject.SetActive(visible);
        mCrossPlaneVisibles[target] = visible;
    }

    public void OnAddNeedle(in GameObject needle)
    {
        var acuNeedleObj = GameObject.Instantiate(mNaiveNeedleObj).transform;
        acuNeedleObj.gameObject.SetActive(true);
        acuNeedleObj.name = needle.name + "-follow";
        acuNeedleObj.parent = TargetObjectRoot;
        //acuNeedleObj.parent = LocatorRoot.transform;//.parent;//.Find("Locators");//.Find("head-top");
        acuNeedleObj.localScale = Vector3.one;
        acuNeedleObj.localRotation = Quaternion.identity;
        acuNeedleObj.localPosition = Vector3.zero;

        mNeedleObjs.Add(acuNeedleObj);
        mTargetNeedle = mNeedleObjs.Count - 1;
        mRefNeedleTransform = needle.transform;
        mIsHoldingNeedle = true;
    }

    public void OnDeleteNeedle(int target)
    {
        if (target < 0 || target >= mNeedleObjs.Count) return;
        GameObject.Destroy(mNeedleObjs[target].gameObject);
        mNeedleObjs.RemoveAt(target);
        if (target == mTargetNeedle) mRefNeedleTransform = null;
        mTargetNeedle = -1;
    }

    public void OnReleaseNeedle()
    {
        mIsHoldingNeedle = false;
    }

    private void Update()
    {
        if (!mIsLinked) return;

        if (VolumeObjectFactory.gVolumeScaleDirty)
            TargetObjectRoot.localScale = Vector3.one * VolumeObjectFactory.gTargetVolume.GetVolumeUnifiedScale();
        if (mRefCrossPlane)
        {
            mCrossPlanes[mTargetCrossPlane].parent.localPosition = Vector3.Scale(mRefCrossPlane.localPosition, mRefCrossPlane.lossyScale);
            mCrossPlanes[mTargetCrossPlane].parent.localRotation = mRefCrossPlane.localRotation;
        }

        int activePlane = 0;
        for (int i = 0; i < mCrossPlanes.Count; i++)
        {
            if(!mCrossPlaneVisibles[i]) continue;
            mCrossPlaneMatrices[activePlane++] = mCrossPlanes[i].worldToLocalMatrix * transform.localToWorldMatrix;
        }

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material.SetInteger("_CrossSectionNum", activePlane);

            meshRenderer.material.SetMatrixArray("_CrossSectionMatrices", mCrossPlaneMatrices);

        }

        if (mRefNeedleTransform && mIsHoldingNeedle)
        {
            mNeedleObjs[mTargetNeedle].position = LocatorRoot.GetAlignedPosition(mRefVolumeLocator, mRefNeedleTransform.position);
            mNeedleObjs[mTargetNeedle].localRotation = mRefNeedleTransform.localRotation;
            ////mNeedleObjs[mTargetNeedle].localScale = mRefNeedleTransform.localScale;
        }
    }
}
