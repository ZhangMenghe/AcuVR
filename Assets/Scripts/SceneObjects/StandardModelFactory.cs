using UnityEngine;

public class StandardModelFactory
{
    private static readonly Vector3 DEFAULT_MODEL_POSITION = new Vector3(-0.3f, 1.5f, 0.3f);
    private static GameObject StandardModel = null;
    public static StandardModel StandardModelData;
    private static CrossSectionEdit CrossSectionManager;

    public static void OnChangeStandardModelStatus()
    {
        if (!StandardModel)
        {
            //Add to scene
            StandardModel = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/AAHStandardModel"));
            StandardModelData = StandardModel.GetComponentInChildren<StandardModel>();
            //if (VolumeObjectFactory.gTargetVolume)
            //    StandardModelData.LinkVolume(VolumeObjectFactory.gTargetVolume.transform);
        }
        else
        {
            //StandardModelData.UnLinkVolume();
            //GameObject.Destroy(StandardModel);
            //StandardModel = null;
            StandardModel.SetActive(!StandardModel.activeSelf);
        }
    }
    public static void setCrossSectionManager(in CrossSectionEdit manager)
    {
        CrossSectionManager = manager;
    }
    public static void OnLinkStandardModelWithVolume()
    {
        //MENGHE:FIND A BETTER WAY THAN RESET!!
        VolumeObjectFactory.ResetTargetVolume();
        OnResetModel();

        StandardModelData.LinkVolume(VolumeObjectFactory.gTargetVolume.transform, VolumeObjectFactory.gTargetVolume.GetVolumeUnifiedScale());

        StandardModelData.SyncCrossSectionPlanes(CrossSectionManager);
    }
    public static void OnResetModel()
    {
        if (!StandardModel) return;
        StandardModel.transform.rotation = Quaternion.identity;
        StandardModel.transform.position = DEFAULT_MODEL_POSITION;
        StandardModel.transform.localScale = Vector3.one;
        //StandardModelData.OnReset();
    }
    public static void AddCrossSectionPlane(in GameObject planeRoot, bool targetOn = true)
    {
        if (!StandardModelData) return;
        StandardModelData.AddCrossSectionPlane(planeRoot, targetOn);
    }
    public static void DeleteCrossSectionPlaneAt(int target)
    {
        if (!StandardModelData) return;
        StandardModelData.DeleteCrossSectionPlaneAt(target);
    }
    public static void OnChangeCrossSectionTarget(int target, in Transform refPlaneRoot)
    {
        if (!StandardModelData) return;
        StandardModelData.OnChangeCrossSectionTarget(target, refPlaneRoot);
    }
    public static void OnChangeVisibility(int target, bool visible)
    {
        if (!StandardModelData) return;
        StandardModelData.OnChangeVisibility(target, visible);
    }

    public static void OnAddNeedle(in GameObject needle)
    {
        if (!StandardModelData) return;
        StandardModelData.OnAddNeedle(needle);
    }
    public static void OnDeleteNeedle(int target)
    {
        if (!StandardModelData) return;
        StandardModelData.OnDeleteNeedle(target);
    }
    public static void OnReleaseNeedle()
    {
        if (!StandardModelData) return;
        StandardModelData.OnReleaseNeedle();

    }
}