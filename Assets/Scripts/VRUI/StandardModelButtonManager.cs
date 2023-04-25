using UnityEngine;
using UnityEngine.UI;
public class StandardModelButtonManager : MonoBehaviour
{
    public StandardModel StandardModelData;
    public GameObject HandGrabInteractableObject;
    //public Oculus.Interaction.RayInteractable rayInteractable;

    public Button MoreBtn;
    public Button ShowListBtn;
    public Button LockBtn;
    public Button ResetBtn;
    public Button LinkBtn;

    [SerializeField]
    private GameObject ControlBtnGroup;

    private GameObject mMainMenuObj;

    private bool mLock;
    private bool mIsLinked;
    private bool mIsMenuOn;
    private bool mIsControlBtnGroupOn;

    private StandardModelMenu mOrganMenu;
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
        StandardModelData.UnLinkVolume();

        mMainMenuObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/StandardModelUI"));
        var StandardModelUI = mMainMenuObj.transform;
        StandardModelUI.parent = transform;
        StandardModelUI.localScale = Vector3.one;
        StandardModelUI.localPosition = new Vector3(-0.3f, .0f, -0.15f);
        StandardModelUI.localRotation = Quaternion.Euler(.0f, -30.0f, .0f);

        mOrganMenu = new StandardModelMenu();
        mOrganMenu.Initialized(StandardModelUI.Find("OrganMenu"), StandardModelData.transform);
        mMainMenuObj.SetActive(mIsMenuOn);
    }
    private void OnReset()
    {
        if (mLock) OnChangeLockStatus();
        if (mIsLinked) OnChangeLinkStatus();
       
        mOrganMenu.onReset();
        mMainMenuObj.SetActive(false);
        mIsMenuOn = false;

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
            StandardModelData.UnLinkVolume();
            if (mLock) OnChangeLockStatus();
        }

        VRUICommonUtils.SwapSprite(ref LinkBtn);
    }
    private void OnChangeExtensiveUI()
    {
        mIsMenuOn = !mIsMenuOn;
        mMainMenuObj.SetActive(mIsMenuOn);
    }
    private void OnChangeMoreStatus()
    {
        mIsControlBtnGroupOn = !mIsControlBtnGroupOn;
        ControlBtnGroup.SetActive(mIsControlBtnGroupOn);
        //VRUICommonUtils.SwapSprite(ref MoreBtn);
    }
}