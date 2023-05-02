using UnityEngine;
using UnityEngine.UI;
public class StandardModelButtonManager : MonoBehaviour
{
    [SerializeField]
    private Transform RootTransform;
    [SerializeField]
    private StandardModel StandardModelData;
    [SerializeField]
    private GameObject ControlBtnGroup;
    [SerializeField]
    private GameObject HandGrabInteractableObject;

    [SerializeField]
    private Button MoreBtn;
    [SerializeField]
    private Button ShowListBtn;
    [SerializeField]
    private Button LockBtn;
    [SerializeField]
    private Button ResetBtn;
    [SerializeField]
    private Button LinkBtn;
    [SerializeField]
    private GameObject OrganMenuObject;

    private bool mLock;
    private bool mIsLinked;
    private bool mIsMenuOn;
    private bool mIsControlBtnGroupOn;

    private StandardModelMenu mOrganMenu;

    private bool mLastTimeControlForward;
    private void Awake()
    {
        ShowListBtn.onClick.AddListener(delegate {
            OnChangeExtensiveUI();
        });

        LockBtn.onClick.AddListener(delegate {
            OnChangeLockStatus();
        });


        ResetBtn.onClick.AddListener(delegate {
            OnReset();
        });

        LinkBtn.onClick.AddListener(delegate {
            OnChangeLinkStatus();
        });

        MoreBtn.onClick.AddListener(delegate {
            OnChangeMoreStatus();
        });

        mLock = false;
        mIsLinked = false;
        mIsMenuOn = false;
        mIsControlBtnGroupOn = true;
        mLastTimeControlForward = true;
        StandardModelData.OnReleaseLinkVolume();

        //mMainMenuObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/StandardModelUI"));
        //var StandardModelUI = mMainMenuObj.transform;
        //StandardModelUI.parent = transform;
        //StandardModelUI.localScale = Vector3.one;
        //StandardModelUI.localPosition = new Vector3(-0.3f, .0f, -0.15f);
        //StandardModelUI.localRotation = Quaternion.Euler(.0f, -30.0f, .0f);

        mOrganMenu = new StandardModelMenu();
        mOrganMenu.Initialized(OrganMenuObject.transform.Find("OrganMenu"), StandardModelData.transform);
        OrganMenuObject.SetActive(mIsMenuOn);
    }
    private void OnReset()
    {
        if (mLock) OnChangeLockStatus();
        if (mIsLinked) OnChangeLinkStatus();
       
        mOrganMenu.onReset();
        OrganMenuObject.SetActive(false);
        mIsMenuOn = false;
        if (!mLastTimeControlForward) flipButtonPanel();
        StandardModelFactory.OnResetModel();
    }
    private void OnChangeLockStatus()
    {
        HandGrabInteractableObject.SetActive(mLock);
        //rayInteractable.enabled = mLock;
        if (mLock) VolumeObjectFactory.gHandGrabbleDirty = true;

        mLock = !mLock;
        VRUICommonUtils.SwapSprite(ref LockBtn);
    }
    private void OnChangeLinkStatus()
    {
        mIsLinked = !mIsLinked;
        if (mIsLinked && VolumeObjectFactory.gTargetVolume)
        {
            StandardModelFactory.OnLinkStandardModelWithVolume();
            if (!mLock) OnChangeLockStatus();
        }else if (!mIsLinked)
        {
            StandardModelData.OnReleaseLinkVolume();
            if (mLock) OnChangeLockStatus();
        }

        VRUICommonUtils.SwapSprite(ref LinkBtn);
    }
    private void OnChangeExtensiveUI()
    {
        mIsMenuOn = !mIsMenuOn;
        OrganMenuObject.SetActive(mIsMenuOn);
    }
    private void OnChangeMoreStatus()
    {
        mIsControlBtnGroupOn = !mIsControlBtnGroupOn;
        ControlBtnGroup.SetActive(mIsControlBtnGroupOn);
        //VRUICommonUtils.SwapSprite(ref MoreBtn);
    }
    private void flipButtonPanel()
    {
        transform.localPosition = new Vector3(
            -transform.localPosition.x,
            transform.localPosition.y,
            transform.localPosition.z);
        var rot = transform.localRotation.eulerAngles;
        transform.localRotation = Quaternion.Euler(0, (rot.y + 180) % 360, 0);
        mLastTimeControlForward = !mLastTimeControlForward;
    }

    public void OnReleaseObject()
    {
        if (Vector3.Dot(RootTransform.forward, Camera.main.transform.forward) > .0f != mLastTimeControlForward) flipButtonPanel();
    }
}