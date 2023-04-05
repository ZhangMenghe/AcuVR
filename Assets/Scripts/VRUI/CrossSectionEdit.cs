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
        mPlaneColor = new Color(1.0f, 0.6f, .0f);
        mPlaneColorInactive = new Color(0.7f, 0.4f, 0.15f);
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
        if (!VolumeObjectFactory.gTargetVolume || mIsVisibles.Count > VolumeRenderedObject.MAX_CS_PLANE_NUM) return;

        Transform planeRoot = VolumeObjectFactory.gTargetVolume.CreateCrossSectionPlane();
        var box_renderer = VolumeObjectFactory.gTargetVolume.m_cs_planes[mIsVisibles.Count].GetComponent<MeshRenderer>();
        box_renderer.material.SetColor("_Color", mPlaneColor);
        mBBoxRenderer.Add(box_renderer);

        mTargetObjs.Add(planeRoot);
        mHandGrabInteractableObjs.Add(planeRoot.Find("HandGrabInteractable").gameObject);
        mIsVisibles.Add(true);

        AddOptionToTargetDropDown("CSPlane " + (++mTotalId), mTargetOnNewPlane);
        DropdownValueChanged(mIsVisibles.Count);

        StandardModelFactory.AddCrossSectionPlane(planeRoot.gameObject, mTargetOnNewPlane);


        //if (mManipulateMode) ResetManipulator();
    }
    public void OnRemoveCrossSectionPlane()
    {
        if (!VolumeObjectFactory.gTargetVolume || mTargetId < 0) return;
        VolumeObjectFactory.gTargetVolume.DeleteCrossSectionPlaneAt(mTargetId);
        StandardModelFactory.DeleteCrossSectionPlaneAt(mTargetId);
        OnRemoveTarget();
    }
    protected override void DropdownChangeFinalize() {
        StandardModelFactory.OnChangeCrossSectionTarget(mTargetId, mTargetId<0?null:mTargetObjs[mTargetId]);
    }
    protected override void OnChangeVisibilityStatus()
    {
        base.OnChangeVisibilityStatus();
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
