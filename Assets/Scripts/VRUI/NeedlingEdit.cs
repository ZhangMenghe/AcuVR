using System.Collections.Generic;
using UnityEngine;
using UnityVolumeRendering;

public class NeedlingEdit : BasicMutipleTargetUI
{
    public VolumeDataEdit VolumeManager;

    //MENGHE: ENABLE/DISABLE Controller and ControllerHand?
    public Transform OVRController;
    public Transform OVRControllerHands;

    private Transform mMagViewDisplay = null;
    private MeshRenderer mMagViewRenderer = null;
    private List<Transform> mNeedleObjs = new List<Transform>();

    private Transform mAcuNeedleRoot;
    private void Start()
    {
        mAcuNeedleRoot = new GameObject("NeedleRoot").transform;
        Initialize();

        if (DisplayRackFactory.AttachToRack(DisplayRackFactory.DisplayRacks.CURVE_LEFT_BOARD, "Magnification View"))
        {
            var mDisplayRack = DisplayRackFactory.GetDisplayRack(DisplayRackFactory.DisplayRacks.CURVE_LEFT_BOARD);

            mMagViewDisplay = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;

            mMagViewDisplay.parent = mDisplayRack;
            mMagViewDisplay.localPosition = new Vector3(.0f, -0.32f, -0.01f);
            mMagViewDisplay.localScale = new Vector3(0.5f, 0.3f, 1.0f);
            mMagViewDisplay.localRotation = Quaternion.identity;

            mMagViewRenderer = mMagViewDisplay.GetComponent<MeshRenderer>();
            mMagViewRenderer.sharedMaterial = Resources.Load<Material>("RenderTextures/AcuNeedleRT");

            //mDisplayRack.gameObject.SetActive(false);
        }
    }

    public void OnAddNeedle()
    {
        mIsVisibles.Add(true);
        mTotalId++;

        var acuNeedleObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/AcuNeedle1")).transform;
        acuNeedleObj.name = "Needle " + mTotalId;
        //acuNeedleObj.position = new Vector3(-1.3f, 1.5f, -1.0f);
        acuNeedleObj.parent = mAcuNeedleRoot;
        acuNeedleObj.position = Camera.main.transform.position + new Vector3(.0f, .0f, 0.3f);
        acuNeedleObj.rotation = Quaternion.identity;
        acuNeedleObj.localScale = Vector3.one;

        mNeedleObjs.Add(acuNeedleObj);
        AddOptionToTargetDropDown(acuNeedleObj.name);
        DropdownValueChanged(mIsVisibles.Count);
    }
    public void OnRemoveNeedle()
    {
        if (mTargetId < 0) return;

        if (mTargetId < mNeedleObjs.Count)
        {
            Destroy(mNeedleObjs[mTargetId].gameObject);

            mNeedleObjs.RemoveAt(mTargetId);
        }
        RemoveTargetOptionFromDropDown();
    }
    public void OnChangePanelStatus(bool isOn)
    {
        DisplayRackFactory.SetDisplayRackVisibility(DisplayRackFactory.DisplayRacks.CURVE_LEFT_BOARD, isOn);
    }
    protected override void OnChangeVisibilityStatus()
    {
        if (mTargetId < 0) return;
        base.OnChangeVisibilityStatus();

        //mIsVisibles[mTargetId] = !mIsVisibles[mTargetId];
        //mNeedleObjs[mTargetId].gameObject.SetActive(mIsVisibles[mTargetId]);
        ////UpdateSprite(mIsVisibles[mTargetId]);
        //VRUICommonUtils.SwapSprite(ref VisibilityBtn);
    }
    protected override void UpdateSnapableObjectStatus(int value)
    {
        //maybe disable other needles?
    }

    private void OnDestroy()
    {
        DisplayRackFactory.DeAttachFromRack(DisplayRackFactory.DisplayRacks.CURVE_LEFT_BOARD);
    }

    private void Update()
    {
        if (VolumeObjectFactory.gTargetVolume)
        {
            for (int i = 0; i < mNeedleObjs.Count; i++)
                mNeedleObjs[i].localScale = Vector3.one * VolumeObjectFactory.gTargetVolume.GetVolumeUnifiedScale();
        }
    }
}
