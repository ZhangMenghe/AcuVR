using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrabbaleAcuNeedle : MonoBehaviour
{
    public GameObject GlowObj;
    public Transform NeedleTipTransform;
    public Transform NeedleEndTransform;

    [SerializeField]
    private Camera FOVCamera;
    [SerializeField]
    private GameObject BoundingObj;
    [SerializeField]
    private Transform NeedleVisualObj;
    [SerializeField]
    private TMP_Text IDTextField;

    [SerializeField]
    private GameObject HandGrabInteractableObject;
    public bool IsLock{get; private set;}

    //private Transform mLinkRoot;
    //private Transform mUnlinkRoot;

    private BoxCollider mTargetVolumeCollider;
    private BoxCollider mNeedleCollider;
    private Transform mUnAttachedRoot;
    private Transform mAttachedRoot;

    private bool mFirstTimeGrab;
    //private NeedlingEdit mNeedlingManagePanel;
    //private bool mIsGlowing;
    //private void Start()
    //{
    //    mIsGlowing = false;
    //    mUnlinkRoot = transform.parent;
    //    mLinkRoot = VolumeObjectFactory.gTargetVolume.transform.parent;
    //        //.Find("LocatorRoot");
    //}
    //public void SetNeedlingManagerPanel(NeedlingEdit panel)
    //{
    //    mNeedlingManagePanel = panel;
    //}
    public void OnInitialized(int id)
    {
        IsLock = false;
        IDTextField.SetText(id.ToString());
        //mIsGlowing = false;
        //mUnlinkRoot = transform.parent;
        //mLinkRoot = VolumeObjectFactory.gTargetVolume.transform.parent;
        mTargetVolumeCollider = VolumeObjectFactory.gTargetVolume.GetComponentInChildren<BoxCollider>();
        mNeedleCollider = GetComponentInChildren<BoxCollider>();
        mAttachedRoot = VolumeObjectFactory.gTargetVolume.transform.parent;
        mUnAttachedRoot = transform.parent;
        mFirstTimeGrab = true;
    }
    public void OnGrabNeedle()
    {
        if (BoundingObj.activeSelf) BoundingObj.SetActive(false);
        if (mFirstTimeGrab)
        {
            StandardModelFactory.OnAddNeedle(gameObject);
            mFirstTimeGrab = false;
        }
        else
        {
            StandardModelFactory.OnGrabNeedle(name);
        }
    }

    public void OnReleaseNeedle()
    {
        //if needle is closed to the target area, attach it to the target volume 
        transform.parent = mTargetVolumeCollider.bounds.Intersects(mNeedleCollider.bounds)? mAttachedRoot : mUnAttachedRoot;

        //Contains(transform.position)? mAttachedRoot: mUnAttachedRoot;
        StandardModelFactory.OnReleaseNeedle();
    }

    public void OnReset()
    {
        transform.position = Camera.main.transform.position + new Vector3(.0f, .0f, 0.3f);
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        if (!BoundingObj.activeSelf) BoundingObj.SetActive(true);
        if (IsLock) OnLockStatusChange();
    }
    public void OnLockStatusChange()
    {
        HandGrabInteractableObject.SetActive(IsLock);
        IsLock = !IsLock;
        //OnChangeVolumeLinkStatus(IsLock);
    }
    public void OnChangeVolumeLinkStatus(bool isLink) {
        //transform.parent = isLink? mLinkRoot: mUnlinkRoot;
        //transform.localScale = Vector3.one;
    }
    //public void OnDeleteNeedle()
    //{
    //    if (mNeedlingManagePanel)
    //        mNeedlingManagePanel.OnRemoveNeedleFromList(transform.name);
    //    GameObject.Destroy(this.gameObject);
    //}
    public void OnChangeTarget(bool isOn)
    {
        FOVCamera.gameObject.SetActive(isOn);
    }
    //public void OnGlowChange()
    //{
    //    mIsGlowing = !mIsGlowing;
    //    GlowObj.SetActive(mIsGlowing);
    //    VRUICommonUtils.SwapSprite(ref GlowBtn);
    //}
}
