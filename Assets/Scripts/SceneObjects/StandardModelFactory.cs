using UnityEngine;

public class StandardModelFactory
{
    private static readonly Vector3 DEFAULT_MODEL_POSITION = new Vector3(-0.3f, 1.5f, 0.3f);
    private static GameObject StandardModel = null;
    public static StandardModel StandardModelData;
    private static CrossSectionEdit CrossSectionManager;
    private static NeedlingEdit NeedleManager;
    public static void OnChangeStandardModelStatus()
    {
        if (!StandardModel)
        {
            string prefab_name = (VolumeObjectFactory.DataVolumePart == "head") ? "Prefabs/AAHStandardModel" : "Prefabs/AAAFemalePelvis";
            //Add to scene
            //StandardModel = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/AAHStandardModel"));
            StandardModel = GameObject.Instantiate(Resources.Load<GameObject>(prefab_name));

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
    public static void OnTargetVolumeTransformationChanged()
    {
        if (!StandardModelData) return;
        StandardModelData.OnTargetVolumeTransformationChanged();
    }
    public static void OnTargetVolumeReset()
    {
        if (!StandardModelData) return;
        StandardModelData.OnTargetVolumeReset();
    }
    public static void setCrossSectionManager(in CrossSectionEdit manager)
    {
        CrossSectionManager = manager;
    }
    public static void setNeedlingManager(in NeedlingEdit manager)
    {
        NeedleManager = manager;
    }
    public static void OnLinkStandardModelWithVolume()
    {
        //VolumeObjectFactory.SaveCurrentTransform();
        //VolumeObjectFactory.ResetTargetVolumeTransform();
        //OnResetModel();
        var refTransform = VolumeObjectFactory.gTargetVolume.transform;
        var volumeRoot = refTransform.parent.parent;
        var mSavedRotation = volumeRoot.rotation;
        var mSavedScale = volumeRoot.localScale;

        volumeRoot.rotation = Quaternion.identity;
        volumeRoot.localScale = Vector3.one;
        StandardModel.transform.rotation = Quaternion.identity;
        
        StandardModelData.OnLinkVolume(refTransform, VolumeObjectFactory.gTargetVolume.GetVolumeUnifiedScale());
        StandardModel.transform.parent = volumeRoot;

        volumeRoot.rotation = mSavedRotation;
        volumeRoot.localScale = mSavedScale;
        StandardModel.transform.parent = null;

        StandardModelData.PostLinkSync(CrossSectionManager, NeedleManager);
    }
    public static void OnResetModel()
    {
        if (!StandardModel) return;
        StandardModel.transform.rotation = Quaternion.identity;
        StandardModel.transform.position = DEFAULT_MODEL_POSITION;
        StandardModel.transform.localScale = Vector3.one;
        //StandardModelData.OnReset();
    }
    public static void AddCrossSectionPlane(in Transform planeRoot, bool targetOn = true)
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

    public static void OnUpdateCrossSectionPlane(string targetName, bool updating)
    {
        if (!StandardModelData) return;
        StandardModelData.UpdateCrossSectionPlane(targetName, updating);
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
    public static void OnGrabNeedle(string name)
    {
        if (!StandardModelData) return;
        StandardModelData.OnGrabNeedle(name);
    }
    public static void OnReleaseNeedle()
    {
        if (!StandardModelData) return;
        StandardModelData.OnReleaseNeedle();
    }
}