using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StandardModel : MonoBehaviour
{
    public Transform CrossPlaneRoot;
    public HeadLocator LocatorRoot;
    public Transform TargetObjectRoot;
    [SerializeField]
    private StandardModelButtonManager PanelButtonManager;

    private MeshRenderer[] meshRenderers = new MeshRenderer[0];

    private List<Transform> mCrossPlanes = new List<Transform>();
    private List<bool> mCSPlanesActive = new List<bool>();
    private int mCSUpdatingId = -1, mCSUpdatingMatrixId = -1;

    private List<Matrix4x4> mCSPlaneMatrices = new List<Matrix4x4>();
    private List<float> mCSPlaneInBounds = new List<float>();
    private Matrix4x4[] mCSPlaneMatriceArray = new Matrix4x4[VolumeObjectFactory.MAX_CS_PLANE_NUM];
    private float[] mCSPlaneInBoundArray = new float[VolumeObjectFactory.MAX_CS_PLANE_NUM];

    /*-----------------------------------------------------------*/

    private HeadLocator mRefVolumeLocator;

    private int mTargetCrossPlane = -1;
    private Transform mRefCrossPlane;
    private bool mIsLinked = false;

    private List<Transform> mNeedleObjs = new List<Transform>();
    private NeedlingEdit mRefNeedleManager;
    private int mHoldNeedleId = -1;
    private Transform mRefNeedleTransform;


    private GameObject mNaiveNeedleObj;
    private BoxCollider mCollider;

    private void Awake()
    {
        mNaiveNeedleObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/AcuNeedleNaive"));
        mNaiveNeedleObj.SetActive(false);
        LocatorRoot.CreateVirtualMidObjs();
    }

    private void Start()
    {
        mCollider = GetComponent<BoxCollider>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>(false);
        foreach (MeshRenderer meshRenderer in meshRenderers)
            meshRenderer.material.SetInteger("_CrossSectionNum", 0);
    }
    public void OnTargetVolumeReset()
    {
        if (!mIsLinked) return;
        //reset scale
        TargetObjectRoot.localScale = Vector3.one * VolumeObjectFactory.gTargetVolume.GetVolumeUnifiedScale();
    }
    public void OnTargetVolumeTransformationChanged()
    {
        if (!mIsLinked) return;

        //scale
        TargetObjectRoot.localScale = Vector3.one * VolumeObjectFactory.gTargetVolume.GetVolumeUnifiedScale();
    }
    public void OnLinkVolume(in Transform volume, float unified_scale)
    {
        mRefVolumeLocator = volume.parent.GetComponentInChildren<HeadLocator>(true);
        mRefVolumeLocator.Initialize();
        LocatorRoot.Initialize();

        //Visual Group Scale
        transform.parent.localScale = LocatorRoot.GetRealSize(mRefVolumeLocator.worldSize)/ unified_scale;

        LocatorRoot.AlignLocators(TargetObjectRoot.parent, mRefVolumeLocator);

        TargetObjectRoot.localScale = Vector3.one * unified_scale;
        mIsLinked = true;
    }
    public void PostLinkSync(in CrossSectionEdit CrossSectionManager, in NeedlingEdit NeedleManager)
    {
        SyncCrossSectionPlanes(CrossSectionManager);
        SyncNeedles(NeedleManager);
        //SyncButtonGroup
        PanelButtonManager.OnReleaseObject();
    }
    public void OnReleaseLinkVolume()
    {
        mRefVolumeLocator = null;

        TargetObjectRoot.parent.localScale = Vector3.one;
        TargetObjectRoot.localScale = Vector3.one;
        transform.parent.localScale = Vector3.one; //VisualGroup
        mIsLinked = false;
        PanelButtonManager.OnReleaseObject();
        //LocatorRoot.ResetScale();

        //remove all the cross-section planes
        foreach(var plane in mCrossPlanes)
        {
            Destroy(plane.parent.gameObject);
        }
        mCrossPlanes.Clear();
        mCSPlanesActive.Clear();
        mCSPlaneMatrices.Clear();
        mCSPlaneInBounds.Clear();

        foreach (MeshRenderer meshRenderer in meshRenderers)
            meshRenderer.material.SetInteger("_CrossSectionNum", 0);
        mTargetCrossPlane = -1;
        mRefCrossPlane = null;

        //remove all needles
        foreach(var needle in mNeedleObjs)
            Destroy(needle.gameObject);
        mNeedleObjs.Clear();
        mHoldNeedleId = -1;
    }

    private void SyncCrossSectionPlanes(in CrossSectionEdit CrossSectionManager)
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
                AddCrossSectionPlane(refPlane, pid ==CrossSectionManager.mTargetId);
            }
            mCSPlanesActive[pid] = CrossSectionManager.mIsVisibles[pid];

            if (!mCSPlanesActive[pid])
                mCrossPlanes[pid].parent.gameObject.SetActive(false);

            pid++;
        }
    }

    public void AddCrossSectionPlane(in Transform refPlaneRoot, bool targetOn)
    {
        if (!mIsLinked || mCrossPlanes.Count > VolumeObjectFactory.MAX_CS_PLANE_NUM) return;

        Transform planeRoot = new GameObject("CSPlaneRoot").transform;
        planeRoot.parent = CrossPlaneRoot;
        planeRoot.localPosition = Vector3.Scale(refPlaneRoot.localPosition, refPlaneRoot.lossyScale);
        //Vector3.zero;
        planeRoot.localRotation = refPlaneRoot.localRotation;//Quaternion.identity;
        planeRoot.localScale = Vector3.one * 0.3f;

        Transform cross_plane = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/CrossSectionFollowPlane")).transform;
        cross_plane.parent = planeRoot;
        cross_plane.name = refPlaneRoot.name;
        cross_plane.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        cross_plane.localPosition = Vector3.zero;
        cross_plane.localScale = Vector3.one * 1.2f;

        mCrossPlanes.Add(cross_plane);
        mCSPlanesActive.Add(true);
        mCSPlaneInBounds.Add(1.0f);
        mCSPlaneMatrices.Add(cross_plane.worldToLocalMatrix * transform.localToWorldMatrix);
        UpdateCrossSectionMatrics();

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
        
        if (mCSPlanesActive[TargetId])
        {
            int matrixIdx = mCSPlanesActive.Take(TargetId).Count(b => b);
            mCSPlaneMatrices.RemoveAt(matrixIdx);
            mCSPlaneInBounds.RemoveAt(matrixIdx);
        }

        mCrossPlanes.RemoveAt(TargetId);
        mCSPlanesActive.RemoveAt(TargetId);

        mTargetCrossPlane = -1;
        mRefCrossPlane = null;

        UpdateCrossSectionMatrics();
    }
    public void OnChangeCrossSectionTarget(int target, in Transform refPlaneRoot)
    {
        if (!mIsLinked) return;
        mTargetCrossPlane = target;
        mRefCrossPlane = refPlaneRoot;
    }
    public void OnChangeVisibility(int targetId, bool isVisible)
    {
        if (!mIsLinked) return;

        mCrossPlanes[targetId].parent.gameObject.SetActive(isVisible);

        if (targetId < 0 || targetId >= mCSPlanesActive.Count || isVisible == mCSPlanesActive[targetId]) return;


        int matrixIdx = mCSPlanesActive.Take(targetId).Count(b => b);
        if (isVisible)
        {
            var inbound = mCollider.bounds.Intersects(mCrossPlanes[targetId].GetComponent<BoxCollider>().bounds);
            mCSPlaneMatrices.Insert(matrixIdx, mCrossPlanes[targetId].worldToLocalMatrix * transform.localToWorldMatrix);
            mCSPlaneInBounds.Insert(matrixIdx, inbound ? 1.0f : -1.0f);
        }
        else
        {
            mCSPlaneMatrices.RemoveAt(matrixIdx);
            mCSPlaneInBounds.RemoveAt(matrixIdx);
        }
        mCSPlanesActive[targetId] = isVisible;
        UpdateCrossSectionMatrics();
    }

    private void SyncNeedles(in NeedlingEdit needleManager) {
        mRefNeedleManager = needleManager;

        foreach (var needle in needleManager.mGrabbableNeedles)
        {
            var acuNeedleObj = GameObject.Instantiate(mNaiveNeedleObj).transform;
            acuNeedleObj.gameObject.SetActive(true);
            acuNeedleObj.name = needle.name + "-follow";
            acuNeedleObj.parent = TargetObjectRoot;
            acuNeedleObj.localScale = Vector3.one;

            acuNeedleObj.position = LocatorRoot.GetAlignedPosition(mRefVolumeLocator, needle.transform.position);
            acuNeedleObj.localRotation = needle.transform.localRotation;
            mNeedleObjs.Add(acuNeedleObj);
        }
    }

    public void OnAddNeedle(in GameObject needle)
    {
        var acuNeedleObj = GameObject.Instantiate(mNaiveNeedleObj).transform;
        acuNeedleObj.gameObject.SetActive(true);
        acuNeedleObj.name = needle.name + "-follow";
        acuNeedleObj.parent = TargetObjectRoot;
        acuNeedleObj.localScale = Vector3.one;

        mNeedleObjs.Add(acuNeedleObj);
        mHoldNeedleId = mRefNeedleManager.mNeedleNames.IndexOf(needle.name);
        mRefNeedleTransform = needle.transform;
    }

    public void OnDeleteNeedle(int target)
    {
        if (target < 0 || target >= mNeedleObjs.Count) return;
        GameObject.Destroy(mNeedleObjs[target].gameObject);
        mNeedleObjs.RemoveAt(target);
    }
    public void OnGrabNeedle(string name)
    {
        mHoldNeedleId = mRefNeedleManager.mNeedleNames.IndexOf(name);
        if (mHoldNeedleId >= 0)
            mRefNeedleTransform = mRefNeedleManager.mGrabbableNeedles[mHoldNeedleId].transform;
    }
    public void OnReleaseNeedle()
    {
        mHoldNeedleId = -1;
        mRefNeedleTransform = null;
    }

    private void Update()
    {
        if (!mIsLinked) return;

        //if (VolumeObjectFactory.gVolumeScaleDirty)
        //    TargetObjectRoot.localScale = Vector3.one * VolumeObjectFactory.gTargetVolume.GetVolumeUnifiedScale();
        
        if (mRefCrossPlane && mCSUpdatingId >= 0)
        {
            mCrossPlanes[mTargetCrossPlane].parent.localPosition = Vector3.Scale(mRefCrossPlane.localPosition, mRefCrossPlane.lossyScale);
            mCrossPlanes[mTargetCrossPlane].parent.localRotation = mRefCrossPlane.localRotation;


            bool inBound = mCollider.bounds.Intersects(mCrossPlanes[mCSUpdatingId].GetComponent<BoxCollider>().bounds);

            mCSPlaneInBounds[mCSUpdatingMatrixId] = inBound ? 1.0f : -1.0f;
            mCSPlaneInBoundArray[mCSUpdatingMatrixId] = mCSPlaneInBounds[mCSUpdatingMatrixId];

            if (inBound)
            {
                mCSPlaneMatrices[mCSUpdatingMatrixId] = mCrossPlanes[mCSUpdatingId].worldToLocalMatrix * transform.localToWorldMatrix;

                mCSPlaneMatriceArray[mCSUpdatingMatrixId] = mCSPlaneMatrices[mCSUpdatingMatrixId];
                foreach (MeshRenderer meshRenderer in meshRenderers)
                    meshRenderer.sharedMaterial.SetMatrixArray("_CrossSectionMatrices", mCSPlaneMatriceArray);

            }
            foreach (MeshRenderer meshRenderer in meshRenderers)
                meshRenderer.sharedMaterial.SetFloatArray("_CrossSectionInBounds", mCSPlaneInBoundArray);
        }

        if (mHoldNeedleId >= 0)
        {
            mNeedleObjs[mHoldNeedleId].position = LocatorRoot.GetAlignedPosition(mRefVolumeLocator, mRefNeedleTransform.position);
            mNeedleObjs[mHoldNeedleId].localRotation = mRefNeedleTransform.localRotation;
        }
    }
    private void UpdateCrossSectionMatrics()
    {
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.sharedMaterial.SetInteger("_CrossSectionNum", mCSPlaneMatrices.Count);

            for (int i = 0; i < mCSPlaneMatrices.Count; i++)
            {
                mCSPlaneMatriceArray[i] = mCSPlaneMatrices[i];
                mCSPlaneInBoundArray[i] = mCSPlaneInBounds[i];
            }

            meshRenderer.sharedMaterial.SetMatrixArray("_CrossSectionMatrices", mCSPlaneMatriceArray);
            meshRenderer.sharedMaterial.SetFloatArray("_CrossSectionInBounds", mCSPlaneInBoundArray);
        }
    }

    public void UpdateCrossSectionPlane(string targetName, bool updating)
    {
        if (!updating) { mCSUpdatingId = -1; return; }
        mCSUpdatingId = mCrossPlanes.FindIndex(a => a.name == targetName);
        mCSUpdatingMatrixId = mCSPlanesActive.Take(mCSUpdatingId + 1).Count(b => b) - 1;
    }
}
