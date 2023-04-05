using UnityEngine;

public class GrabbaleAcuNeedle : MonoBehaviour
{
    public Camera FOVCamera;
    public GameObject BoundingObj;
    public Transform NeedleVisualObj;

    private Transform mLinkRoot;
    private Transform mUnlinkRoot;
    private NeedlingEdit mNeedlingManagePanel;

    private void Start()
    {
        mUnlinkRoot = transform.parent;
        mLinkRoot = VolumeObjectFactory.gTargetVolume.transform.parent;
            //.Find("LocatorRoot");
    }
    public void SetNeedlingManagerPanel(NeedlingEdit panel)
    {
        mNeedlingManagePanel = panel;
    }
    public void OnSelected()
    {
        if (BoundingObj.activeSelf) BoundingObj.SetActive(false);
    }
    public void OnReset()
    {
        if (!BoundingObj.activeSelf) BoundingObj.SetActive(true);
    }
    public void OnChangeVolumeLinkStatus(bool isLink) {
        transform.parent = isLink? mLinkRoot: mUnlinkRoot;
        transform.localScale = Vector3.one;
    }
    public void OnDeleteNeedle()
    {
        if (mNeedlingManagePanel)
            mNeedlingManagePanel.OnRemoveNeedleFromList(transform.name);
        GameObject.Destroy(this.gameObject);
    }
    public void OnChangeTarget(bool isOn)
    {
        FOVCamera.gameObject.SetActive(isOn);
    }
}
