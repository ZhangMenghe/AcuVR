using UnityEngine;
public class VolumeButtonManager : BasicSingleObjectButtonGroupManager
{
    [SerializeField]
    private Transform RootTransform;
    private bool mLastTimeControlForward;
    private void Awake()
    {
        INITIAL_VISIBLE = true;
        INITIAL_LOCK = false;
        mLastTimeControlForward = true;
        VolumeObjectFactory.VolumeForwad = mLastTimeControlForward;
        OnAwake();
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
        VolumeObjectFactory.VolumeForwad = mLastTimeControlForward;
    }
    protected override void OnReset()
    {
        base.OnReset();
        if (!mLastTimeControlForward) flipButtonPanel();
        VolumeObjectFactory.ResetTargetVolume();
    }
    public void OnReleaseObject()
    {
        if (Vector3.Dot(RootTransform.forward, Camera.main.transform.forward) > .0f != mLastTimeControlForward) flipButtonPanel();
    }
}