using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityVolumeRendering;
using Oculus.Interaction;
public class VolumeDataEdit : BasicMutipleTargetUI
{
    //Lock Button
    public Transform ControllerCursor;
    public Button LockBtn;
    //public Button ManipulationBtn;

    private List<bool> mLockStatus = new List<bool>();
    private List<GameObject> mVolumeObjs = new List<GameObject>();

    private Sprite UnlockSprite;
    private Sprite LockSprite;

    private float mManipulatorLossyScale;
    private void Awake()
    {
        Initialize();
        VolumeRenderedObject.isSnapAble = true;

        //MENGHE:!WORK ON THAT TO ENABLE MORE OBJS!!!!
        try
        {
            mVolumeObjs = GameObject.FindGameObjectsWithTag("VolumeRenderingObject").ToList();
            if(mVolumeObjs.Count > 0)
            {
                foreach(var vol in mVolumeObjs)
                {
                    mHandGrabInteractableObjs.Add(vol.transform.parent.parent.Find("HandGrabInteractable").gameObject);
                    AddVolumeToList(true);
                }
                VolumeObjectFactory.gTargetVolume = mVolumeObjs[mTargetId].GetComponent<VolumeRenderedObject>();
            }

        }
        catch (System.Exception e)
        {
            Debug.Log("No Volume Object now in the scene: " + e.Message);
        }
    }

    private void Start()
    {
        LockSprite = Resources.Load<Sprite>("icons/lock");
        UnlockSprite = Resources.Load<Sprite>("icons/unlock");
        LockBtn.onClick.AddListener(delegate {
            OnChangeLockStatus();
        });
        //ManipulationBtn.onClick.AddListener(delegate {
        //    OnChangeTransformManipulator();
        //    ManipulationBtn.image.sprite = mManipulateMode ? ManulSprite : ManipulateSprite;
        //});
        //InitializeManipulator();
    }

    public void OnAddVolume()
    {
        //int old_target = mTargetId;
        //LoadDataFromFileSystem();
        //AddVolumeToList(true);

        //if (mTargetId != old_target)
        //   VolumeObjectFactory.gTargetVolume = mVolumeObjs[mTargetId].GetComponent<VolumeRenderedObject>();

    }
    public void OnRemoveVolume()
    {
        if (mTargetId >= 0)
        {
            //if (mManipulateMode) HideManipulator();

            GameObject.Destroy(mVolumeObjs[mTargetId].transform.parent.parent.gameObject);
            mVolumeObjs.RemoveAt(mTargetId);
            mHandGrabInteractableObjs.RemoveAt(mTargetId);

            VolumeObjectFactory.gTargetVolume = null;
            RemoveTargetOptionFromDropDown();
        }
    }
    public void OnResetVolume()
    {
        VolumeObjectFactory.ResetTargetVolume();
    }
    protected override void OnChangeVisibilityStatus()
    {
        if (mTargetId < 0) return;
        base.OnChangeVisibilityStatus();
        //mIsVisibles[mTargetId] = !mIsVisibles[mTargetId];
        //mVolumeObjs[mTargetId].transform.parent.parent.gameObject.SetActive(mIsVisibles[mTargetId]);
        ////UpdateSprite(mIsVisibles[mTargetId]);
        //VRUICommonUtils.SwapSprite(ref VisibilityBtn);
    }

    protected override void UpdateSnapableObjectStatus(int value)
    {
        //MENGHE: SOME VISUAL CHANGES WHEN CHANGE TARGET VOLUME
        VolumeObjectFactory.gTargetVolume = (value >= 0)?mVolumeObjs[value].GetComponent<VolumeRenderedObject>():null;


        ////disable current one
        //if (mTargetId >= 0)
        //{
        //    VolumeObjectFactory.gTargetVolume.m_cs_planes[mTargetId].GetComponent<MeshRenderer>().material.SetTexture("_MainTex", unTargetTex);
        //    VolumeObjectFactory.gTargetVolume.m_cs_planes[mTargetId].parent.parent.Find("HandGrabInteractable").gameObject.SetActive(false);
        //}
        ////enable the new one
        //if (value >= 0)
        //{
        //    VolumeObjectFactory.gTargetVolume.m_cs_planes[value].GetComponent<MeshRenderer>().material.SetTexture("_MainTex", TargetTex);
        //    VolumeObjectFactory.gTargetVolume.m_cs_planes[value].parent.parent.Find("HandGrabInteractable").gameObject.SetActive(true);
        //}
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
            //If use Manipulator, lock the volume
            if (!mLockStatus[mTargetId])
                OnChangeLockStatus();
            var targetTransform = mHandGrabInteractableObjs[mTargetId].transform.parent.Find("TargetObject");
            var targetLossyScaleSize = targetTransform.lossyScale;
            mManipulatorLossyScale = Mathf.Max(targetLossyScaleSize.x, targetLossyScaleSize.y, targetLossyScaleSize.z);
            mManipulator.transform.localScale = Vector3.one * mManipulatorLossyScale * 1.5f* VolumeObjectFactory.gTargetVolume.GetVolumeUnifiedScale();
            
            mManipulator.transform.parent = mVolumeObjs[mTargetId].transform.parent;
            mManipulator.SetActive(true);
            mManipulator.GetComponent<RotationManipulation>().ControllerCursor = ControllerCursor;
            mManipulator.GetComponent<RotationManipulation>().Initialize();
        }
        else
        {
            HideManipulator();
            if(mLockStatus[mTargetId]) OnChangeLockStatus();
        }
    }
    */
    private void OnChangeLockStatus()
    {
        if (mTargetId < 0) return;
        mLockStatus[mTargetId] = !mLockStatus[mTargetId];

        mHandGrabInteractableObjs[mTargetId].SetActive(!mLockStatus[mTargetId]);
        mVolumeObjs[mTargetId].GetComponent<RayInteractable>().enabled = !mLockStatus[mTargetId];

        if(!mLockStatus[mTargetId])
            VolumeObjectFactory.gHandGrabbleDirty = true;

        //UpdateLockSprite(mLockStatus[mTargetId]);
        LockBtn.image.sprite = mLockStatus[mTargetId] ? LockSprite : UnlockSprite;
    }
    private void UpdateLockSprite(bool isChecked)
    {
        SpriteState spriteState = LockBtn.spriteState;

        if (isChecked)
        {
            LockBtn.image.sprite = LockSprite;
        }
        else
        {
            LockBtn.image.sprite = UnlockSprite;
        }

        LockBtn.spriteState = spriteState;
    }

    private void LoadDataFromFileSystem()
    {
        //MENGHE: LOAD DATA FROM SYSTEM
    }
    private void AddVolumeToList(bool targetOnNew)
    {
        mIsVisibles.Add(true);
        mLockStatus.Add(false);

        //MENGHE: USE LOADED NAME
        AddOptionToTargetDropDown("Volume " + mIsVisibles.Count.ToString(), targetOnNew);
        DropdownValueChanged(mIsVisibles.Count);
    }
    private void Update()
    {
        if (VolumeObjectFactory.gVolumeScaleDirty)
        {
            //var targetTransform = mHandGrabInteractableObjs[mTargetId].transform.parent.Find("TargetObject");
            //var targetLossyScaleSize = targetTransform.lossyScale;
            //mManipulator.transform.localScale = Vector3.one * mManipulatorLossyScale * 1.5f * VolumeObjectFactory.gTargetVolume.GetVolumeUnifiedScale();
        }
    }
    private void LateUpdate()
    {
        VolumeObjectFactory.gHandGrabbleDirty = false;
        VolumeObjectFactory.gVolumeScaleDirty = false;
    }

}
