using UnityEngine;
using UnityVolumeRendering;

public class VolumeObjectFactory
{
    public static VolumeRenderedObject gTargetVolume { get; set; } = null;
    public static HandGrabVolume gHandGrabVolume { get; set; } = null;
    public static bool gHandGrabbleDirty = false;

    public static readonly Vector3 DEFAULT_VOLUME_POSITION = new Vector3(.0f, 1.5f, .3f);
    public static bool VolumeForwad;
    private static readonly Quaternion DEFAULT_VOLUME_ROTATION = Quaternion.Euler(-90.0f, 0.0f, 180.0f);
    public static readonly int MAX_CS_PLANE_NUM = 5;
    public static string DataVolumePart;

    private static CylinderUI mMainUI;
    private static Vector3 mSavePos;
    private static Quaternion mSaveRot;
    private static Transform mVolumeRoot;
    //public static void OnWorkingTableChange(bool isOn) {
    //    ResetTargetVolume();
    //    GetVolumeTargetObject().gameObject.GetComponent<Rigidbody>().isKinematic = !isOn;
    //}
    public static void OnTargetNeedleChange(int id, in GrabbaleAcuNeedle targetNeedle)
    {
        if (id < 0) mMainUI.SlicingPanel.GetComponent<SlicingEdit>().DisableNeeldeProjection();

        else
        {
            mMainUI.SlicingPanel.GetComponent<SlicingEdit>().EnableNeedleProjection(targetNeedle);
        }
    }
    public static void SetUIManager(in CylinderUI mainUI)
    {
        mMainUI = mainUI;
    }
    public static void GatherObjectsInScene()
    {
        var volumes = GameObject.FindGameObjectsWithTag("VolumeRenderingObject");
        if (volumes.Length > 0)
        {
            gTargetVolume = volumes[0].GetComponent<VolumeRenderedObject>();
            mVolumeRoot = gTargetVolume.transform.parent.parent;
            gHandGrabVolume = mVolumeRoot.GetComponent<HandGrabVolume>();
            DataVolumePart = mVolumeRoot.name.Contains("pelvis") ? "pelvis" : "head";
        }
    }
    public static void SaveCurrentTransform()
    {
        mSavePos = mVolumeRoot.position;
        mSaveRot = mVolumeRoot.rotation;
    }
    public static void RestorTransform(Transform followUpTransform)
    {
        mVolumeRoot.rotation = mSaveRot;
        followUpTransform.rotation = mSaveRot;

        var translation = mSavePos - mVolumeRoot.position;
        mVolumeRoot.position = mSavePos;
        followUpTransform.position += translation;
        followUpTransform.localScale = Vector3.one;
    }
    //MENGHE:SNAPABLE T ONLY IN VR
    public static void ResetTargetVolume()
    {
        if (!gTargetVolume) return;

        ResetTargetVolumeTransform();
        gTargetVolume.OnReset();
        mMainUI.OnTargetVolumeReset();
        StandardModelFactory.OnTargetVolumeReset();
    }
    public static void ResetTargetVolumeTransform()
    {
        if (!gTargetVolume) return;
        mVolumeRoot.rotation = Quaternion.identity;
        mVolumeRoot.position = DEFAULT_VOLUME_POSITION;
        mVolumeRoot.localScale = Vector3.one;

        //gTargetVolume.transform.parent.rotation = Quaternion.identity;
        //gTargetVolume.transform.parent.position = DEFAULT_VOLUME_POSITION;
    }

    public static VolumeRenderedObject CreateObject(VolumeDataset dataset, bool Snapable = true)
    {
        Transform VolumeObject;
        Transform meshContainer;
        if (Snapable)
        {
            mVolumeRoot = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/AAHVolume")).transform;
            mVolumeRoot.name = "VolumeRenderedObject_" + dataset.datasetName;
            mVolumeRoot.rotation = Quaternion.identity;
            mVolumeRoot.position = DEFAULT_VOLUME_POSITION;

            VolumeObject = mVolumeRoot.Find("TargetObject").Find("VolumeObject");
            VolumeObject.localRotation = DEFAULT_VOLUME_ROTATION;
            VolumeObject.localPosition = Vector3.zero;

            meshContainer = VolumeObject.Find("Mesh");
        }
        else
        {
            VolumeObject = new GameObject("VolumeRenderedObject_" + dataset.datasetName).transform;
            VolumeObject.tag = "VolumeRenderingObject";
            VolumeObject.rotation = DEFAULT_VOLUME_ROTATION;
            //MENGHE: where to put?
            VolumeObject.position = DEFAULT_VOLUME_POSITION;

            meshContainer = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/VolumeContainer")).transform;
            meshContainer.parent = VolumeObject;
            meshContainer.localScale = Vector3.one;
            meshContainer.localPosition = Vector3.zero;
            meshContainer.localRotation = Quaternion.identity;
        }
        VolumeRenderedObject volObj = VolumeObject.gameObject.AddComponent<VolumeRenderedObject>();

        MeshRenderer meshRenderer = meshContainer.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(meshRenderer.sharedMaterial);

        volObj.meshRenderer = meshRenderer;
        volObj.dataset = dataset;

        const int noiseDimX = 512;
        const int noiseDimY = 512;
        Texture2D noiseTexture = NoiseTextureGenerator.GenerateNoiseTexture(noiseDimX, noiseDimY);

        TransferFunction tf = TransferFunctionDatabase.CreateTransferFunction();
        Texture2D tfTexture = tf.GetTexture();
        volObj.transferFunction = tf;

        TransferFunction2D tf2D = TransferFunctionDatabase.CreateTransferFunction2D();
        volObj.transferFunction2D = tf2D;

        meshRenderer.sharedMaterial.EnableKeyword("UNITY_SINGLE_PASS_STEREO");

        meshRenderer.sharedMaterial.SetTexture("_DataTex", dataset.GetDataTexture());
        meshRenderer.sharedMaterial.SetTexture("_MaskTex", dataset.maskTexture);
        meshRenderer.sharedMaterial.SetTexture("_GradientTex", null);
        meshRenderer.sharedMaterial.SetTexture("_NoiseTex", noiseTexture);
        meshRenderer.sharedMaterial.SetTexture("_TFTex", tfTexture);

        meshRenderer.sharedMaterial.EnableKeyword("MODE_DVR");
        meshRenderer.sharedMaterial.DisableKeyword("MODE_MIP");
        meshRenderer.sharedMaterial.DisableKeyword("MODE_SURF");

        if(dataset.scaleX != 0.0f && dataset.scaleY != 0.0f && dataset.scaleZ != 0.0f)
        {
            volObj.SetOriginalScale(new Vector3(dataset.scaleX, dataset.scaleY, dataset.scaleZ) * 0.001f);
            //float maxScale = Mathf.Max(dataset.scaleX, dataset.scaleY, dataset.scaleZ);
            //volObj.transform.localScale = new Vector3(dataset.scaleX / maxScale, dataset.scaleY / maxScale, dataset.scaleZ / maxScale);
            //volObj.transform.localScale = new Vector3(dataset.scaleX, dataset.scaleY, dataset.scaleZ) * 0.001f;
        }

        return volObj;
    }

    public static void SpawnCutoutBox(VolumeRenderedObject volobj)
    {
        GameObject obj = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/CutoutBox"));
        obj.transform.rotation = Quaternion.Euler(270.0f, 0.0f, 0.0f);
        CutoutBox cbox = obj.gameObject.GetComponent<CutoutBox>();
        cbox.targetObject = volobj;
        obj.transform.position = volobj.transform.position;

#if UNITY_EDITOR
        UnityEditor.Selection.objects = new UnityEngine.Object[] { obj };
#endif
    }
}
