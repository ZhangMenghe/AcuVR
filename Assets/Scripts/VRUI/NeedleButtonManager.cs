using UnityEngine;
using UnityEngine.UI;
using UnityVolumeRendering;
public class NeedleButtonManager : MonoBehaviour
{
    public GameObject HandGrabInteractableObject;

    public Button LockBtn;

    private bool INITIAL_LOCK = false;

    private bool mLock;
    private bool mFirstTime;
    private void Awake()
    {
        mLock = INITIAL_LOCK;
        mFirstTime = true;
        LockBtn.onClick.AddListener(delegate {
            OnChangeLockStatus();
        });
    }
    public void OnGrabNeedle()
    {
        if (!mFirstTime) return;

        mLock = !mLock;
        VRUICommonUtils.SwapSprite(ref LockBtn);

        transform.parent.GetComponent<GrabbaleAcuNeedle>().OnChangeVolumeLinkStatus(mLock);

        StandardModelFactory.OnAddNeedle(transform.parent.gameObject);
    }
    public void OnReleaseNeedle()
    {
        if (mFirstTime)
        {
            HandGrabInteractableObject.SetActive(!mLock);
            mFirstTime = false;
        }
        else
        {
            OnChangeLockStatus();
        }
        StandardModelFactory.OnReleaseNeedle();
    }
    public void OnChangeLockStatus()
    {
        HandGrabInteractableObject.SetActive(mLock);
        mLock = !mLock;
        VRUICommonUtils.SwapSprite(ref LockBtn);

        transform.parent.GetComponent<GrabbaleAcuNeedle>().OnChangeVolumeLinkStatus(mLock);
    }
}