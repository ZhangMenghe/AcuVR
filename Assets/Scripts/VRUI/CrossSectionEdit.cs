using UnityEngine;
using UnityEngine.UI;
using UnityVolumeRendering;
public class CrossSectionEdit : BasicMutipleTargetUI
{
    //public VolumeDataEdit VolumeManager;
    //public Button ManipulationBtn;
    //public Transform ControllerCursor;
    private readonly bool mTargetOnNewPlane = true;
    private void Awake()
    {
        mPlaneColor = new Color(1.0f, 0.6f, .0f, 1.0f);
        //mPlaneColorInactive = new Color(0.7f, 0.4f, 0.15f);
        mPlaneColorInactive = new Color(0.9f, 0.9f, 0.9f,0.01f);
    }
    private void Start()
    {
        //ManipulationBtn.onClick.AddListener(delegate {
        //    OnChangeTransformManipulator();
        //    ManipulationBtn.image.sprite = mManipulateMode ? ManulSprite : ManipulateSprite;
        //});
        Initialize();
        //InitializeManipulator();
    }
    /*
    protected override void ResetManipulator()
    {
        if (mTargetId < 0)
        {
            if (mManipulateMode) HideManipulator();
            return;
        }

        if (mManipulateMode)
        {
            mHandGrabInteractableObjs[mTargetId].SetActive(false);
            mHandGrabInteractableObjs[mTargetId].transform.parent.Find("TargetObject").GetComponent<Oculus.Interaction.RayInteractable>().enabled = false;

            var targetTransform = mHandGrabInteractableObjs[mTargetId].transform.parent.Find("TargetObject");
            var targetLossyScaleSize = targetTransform.lossyScale;
            mManipulator.transform.localScale = Vector3.one * Mathf.Max(targetLossyScaleSize.x, targetLossyScaleSize.y, targetLossyScaleSize.z) * 1.5f;
            
            mManipulator.transform.parent = mHandGrabInteractableObjs[mTargetId].transform.parent;
            mManipulator.SetActive(true);
            mManipulator.GetComponent<RotationManipulation>().ControllerCursor = ControllerCursor;
            mManipulator.GetComponent<RotationManipulation>().Initialize();
        }
        else
        {
            HideManipulator();

            mHandGrabInteractableObjs[mTargetId].SetActive(true);
            mHandGrabInteractableObjs[mTargetId].transform.parent.Find("TargetObject").GetComponent<Oculus.Interaction.RayInteractable>().enabled = true;

            //GameObject.Destroy(mHandGrabInteractableObjs[mTargetId].transform.parent.Find("RotationManipulator").gameObject);
        }
    }
    */
    public void OnAddCrossSectionPlane()
    {
        if (!VolumeObjectFactory.gTargetVolume || mIsVisibles.Count > VolumeObjectFactory.MAX_CS_PLANE_NUM) return;

        Transform planeRoot = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/AAASnapCrossSectionPlane")).transform;
        planeRoot.name = "CSPlane" + (++mTotalId);
        planeRoot.parent = VolumeObjectFactory.gHandGrabVolume.SlicingPlaneRoot;
        planeRoot.localRotation = VolumeObjectFactory.VolumeForwad?Quaternion.identity: Quaternion.Euler(.0f, .0f, 180.0f);
        planeRoot.localPosition = Vector3.zero;
        planeRoot.localScale = Vector3.one;

        planeRoot.GetComponent<CrossSectionPlane>().Initialized(mTotalId, VolumeObjectFactory.gTargetVolume.transform.localScale);

        var mesh_renderer = planeRoot.GetComponentInChildren<MeshRenderer>();
        mesh_renderer.material.SetColor("_Color", mPlaneColor);
        mesh_renderer.name = planeRoot.name;
        mBBoxRenderer.Add(mesh_renderer);

        mTargetObjs.Add(planeRoot);
        mHandGrabInteractableObjs.Add(planeRoot.Find("HandGrabInteractable").gameObject);
        mIsVisibles.Add(true);

        AddOptionToTargetDropDown(planeRoot.name, mTargetOnNewPlane);
        DropdownValueChanged(mIsVisibles.Count);

        VolumeObjectFactory.gTargetVolume.AddCrossSectionPlane(mesh_renderer.transform);
        StandardModelFactory.AddCrossSectionPlane(planeRoot, mTargetOnNewPlane);
    }
    public void OnRemoveCrossSectionPlane()
    {
        if (!VolumeObjectFactory.gTargetVolume || mTargetId < 0) return;
        Destroy(mTargetObjs[mTargetId].gameObject);
        //VolumeObjectFactory.gTargetVolume.DeleteCrossSectionPlaneAt(mTargetId);
        VolumeObjectFactory.gTargetVolume.RemoveCrossSectionPlaneAt(mTargetId);
        StandardModelFactory.DeleteCrossSectionPlaneAt(mTargetId);
        OnRemoveTarget();
    }
    protected override void DropdownChangeFinalize() {
        StandardModelFactory.OnChangeCrossSectionTarget(mTargetId, mTargetId<0?null:mTargetObjs[mTargetId]);
    }
    protected override void OnChangeVisibilityStatus()
    {
        if (mTargetId < 0) return;
        base.OnChangeVisibilityStatus();
        VolumeObjectFactory.gTargetVolume.ChangeCrossSectionPlaneActiveness(mTargetId, mIsVisibles[mTargetId]);
        StandardModelFactory.OnChangeVisibility(mTargetId, mIsVisibles[mTargetId]);
    }

    private void Update()
    {
        //DO NOT TOUCH!
        if (mTargetId < 0 || !VolumeObjectFactory.gHandGrabbleDirty) return;

        mHandGrabInteractableObjs[mTargetId].SetActive(false);
        mHandGrabInteractableObjs[mTargetId].SetActive(true);
    }
}
