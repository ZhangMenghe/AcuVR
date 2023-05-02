using UnityEngine;
using UnityEngine.UI;
using UnityVolumeRendering;

public class BasicSingleObjectButtonGroupManager : MonoBehaviour
{
    public GameObject TargetObject;
    public GameObject HandGrabInteractableObject;
    //public Oculus.Interaction.RayInteractable rayInteractable;

    public Button MoreBtn;
    public Button LockBtn;
    public Button VisibleBtn;
    public Button ResetBtn;

    [SerializeField]
    private GameObject ControlBtnGroup;

    protected bool INITIAL_VISIBLE;
    protected bool INITIAL_LOCK;

    protected bool mIsVisible;
    protected bool mLock;
    protected bool mIsControlBtnGroupOn;

    protected void OnAwake()
    {
        if (MoreBtn)
            MoreBtn.onClick.AddListener(delegate {
                OnChangeMoreStatus();
            });

        if (LockBtn)
        LockBtn.onClick.AddListener(delegate {
            OnChangeLockStatus();
        });
        if (VisibleBtn)
            VisibleBtn.onClick.AddListener(delegate {
            OnChangeVisibilityStatus();
        });
        if (ResetBtn)
            ResetBtn.onClick.AddListener(delegate {
            OnReset();
        });

        mLock = INITIAL_LOCK;
        mIsVisible = INITIAL_VISIBLE;
        mIsControlBtnGroupOn = true;
    }
    protected virtual void OnReset()
    {
        if (mLock != INITIAL_LOCK) OnChangeLockStatus();
        if (mIsVisible != INITIAL_VISIBLE) OnChangeVisibilityStatus();
    }

    protected virtual void OnChangeVisibilityStatus()
    {
        mIsVisible = !mIsVisible;
        TargetObject.SetActive(mIsVisible);
        VRUICommonUtils.SwapSprite(ref VisibleBtn);
    }
    protected virtual void OnChangeLockStatus()
    {
        HandGrabInteractableObject.SetActive(mLock);
        //rayInteractable.enabled = mLock;
        if (mLock) VolumeObjectFactory.gHandGrabbleDirty = true;

        mLock = !mLock;
        VRUICommonUtils.SwapSprite(ref LockBtn);
    }
    protected virtual void OnChangeMoreStatus()
    {
        mIsControlBtnGroupOn = !mIsControlBtnGroupOn;
        ControlBtnGroup.SetActive(mIsControlBtnGroupOn);
        //VRUICommonUtils.SwapSprite(ref MoreBtn);
    }
}
