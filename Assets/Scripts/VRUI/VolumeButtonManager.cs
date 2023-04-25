using UnityEngine;
using UnityVolumeRendering;
public class VolumeButtonManager : BasicSingleObjectButtonGroupManager
{
    private void Awake()
    {
        INITIAL_VISIBLE = true;
        INITIAL_LOCK = false;
        OnAwake();
    }
    protected override void OnReset()
    {
        base.OnReset();

        TargetObject.transform.parent.rotation = Quaternion.identity;
        TargetObject.transform.parent.position = VolumeObjectFactory.DEFAULT_VOLUME_POSITION;
        TargetObject.GetComponent<VolumeRenderedObject>().onReset();
    }
}
