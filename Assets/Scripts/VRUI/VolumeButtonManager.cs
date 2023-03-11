using UnityEngine;
using UnityVolumeRendering;
public class VolumeButtonManager : BasicSingleObjectButtonGroupManager
{
    private void Start()
    {
        Vector3 sz = TargetObject.transform.localScale;
        transform.localPosition = new Vector3(-sz.x * 0.5f, sz.x * 0.5f, -sz.y * 0.5f);
    }
    protected override void OnReset()
    {
        base.OnReset();

        TargetObject.transform.parent.rotation = Quaternion.identity;
        TargetObject.transform.parent.position = VolumeObjectFactory.DEFAULT_VOLUME_POSITION;
        TargetObject.GetComponent<VolumeRenderedObject>().onReset();
    }
}
