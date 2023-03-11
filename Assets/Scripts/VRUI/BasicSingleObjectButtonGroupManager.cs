using UnityEngine;
using UnityEngine.UI;
using UnityVolumeRendering;

public class BasicSingleObjectButtonGroupManager : MonoBehaviour
{
    public GameObject TargetObject;
    public GameObject HandGrabInteractableObject;
    public Oculus.Interaction.RayInteractable rayInteractable;

    public Button LockBtn;
    public Button VisibleBtn;
    public Button ResetBtn;

    private static readonly bool INITIAL_VISIBLE = true;
    private static readonly bool INITIAL_LOCK = false;

    protected bool mIsVisible = INITIAL_VISIBLE;
    protected bool mLock = INITIAL_LOCK;

    private void Awake()
    {
        LockBtn.onClick.AddListener(delegate {
            OnChangeLockStatus();
        });
        VisibleBtn.onClick.AddListener(delegate {
            OnChangeVisibilityStatus();
        });
        ResetBtn.onClick.AddListener(delegate {
            OnReset();
        });

        OnReset();
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
        rayInteractable.enabled = mLock;
        if (mLock) VolumeObjectFactory.gHandGrabbleDirty = true;

        mLock = !mLock;
        VRUICommonUtils.SwapSprite(ref LockBtn);
    }
}
