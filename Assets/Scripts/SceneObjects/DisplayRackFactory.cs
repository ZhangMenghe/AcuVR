using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DisplayRackFactory
{
    public enum DisplayRacks
    {
        ROOM_LEFT_BOARD = 0,
        DISPLAY_RACK_END
    }
    //public static Transform LeftBoardObj;
    public static Transform[] DisplayRackObjs;

    private static bool[] isDisplayRackAttached = new bool[(int)DisplayRacks.DISPLAY_RACK_END];
    private static List<bool>[] visibilityLists = Enumerable.Repeat(new List<bool>(), (int)DisplayRacks.DISPLAY_RACK_END).ToArray();
    private static List<Material>[] frameMaterials = Enumerable.Repeat(new List<Material>(), (int)DisplayRacks.DISPLAY_RACK_END).ToArray();

    public static readonly Vector3[] mDisplayPoses = new Vector3[] {
            new Vector3(-0.6f, 0.4f, .0f), new Vector3(.0f, 0.4f, .0f), new Vector3(0.6f, 0.4f, .0f),
            new Vector3(-0.6f, -0.2f, .0f), new Vector3(.0f, -0.2f, .0f), new Vector3(0.6f, -0.2f, .0f)
        };
    public static readonly Vector3 mUnitScale = Vector3.one * 0.5f;

    private static readonly Vector3[] RACK_POSITIONS = {
        new Vector3(2.5f, 2.0f, 1.3f),
        new Vector3(0.12f, 2.0f, 2.6f),
        new Vector3(2.4f, 2.0f, -1.3f),
    };
    private static readonly Quaternion[] RACK_ROTATES = {
        Quaternion.Euler(.0f, 60.0f, .0f),
        Quaternion.identity,
        Quaternion.Euler(.0f, 120.0f, .0f)
    };
    private static List<string> m_rack_names = new List<string>();
    public static GameObject CreateDisplayRack(string rack_name)
    {
        GameObject display_rack = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/DisplayRackPrefab"));
        display_rack.name = rack_name;

        display_rack.transform.localScale = Vector3.one;
        display_rack.transform.localPosition = RACK_POSITIONS[m_rack_names.Count % 3];
        display_rack.transform.localRotation = RACK_ROTATES[m_rack_names.Count % 3];


        var title = display_rack.transform.Find("Title");
        title.GetComponent<TMPro.TextMeshPro>().SetText(rack_name);

        m_rack_names.Add(rack_name);
        return display_rack;
    }
    public static GameObject CreateMagnificationViewRack(string rack_name, float ratio)
    {
        GameObject display_rack = CreateDisplayRack(rack_name);
        
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        
        quad.transform.parent = display_rack.transform;
        quad.transform.localPosition = new Vector3(.0f, 0.15f, .0f);
        quad.transform.localScale = new Vector3(ratio, 1.0f, 1.0f);
        quad.transform.localRotation = Quaternion.identity;

        quad.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("RenderTextures/AcuNeedleRT");
        return display_rack;
    }

    public static void AttachToRack(DisplayRacks rackId, string rack_name) {
        if(rackId == DisplayRacks.ROOM_LEFT_BOARD)
        {
            ref Transform LeftBoardObj = ref DisplayRackObjs[(int)rackId];
            if (!LeftBoardObj)
            {
                LeftBoardObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/LeftBoard")).transform;
                LeftBoardObj.parent = GameObject.Find("Static").transform;
            }

            isDisplayRackAttached[(int)rackId] = true;
            var title = LeftBoardObj.transform.Find("Title");
            title.GetComponent<TMPro.TextMeshPro>().SetText(rack_name);
        }
    }

    public static void DeAttachFromRack(DisplayRacks rackId)
    {
        if (isDisplayRackAttached[(int)rackId])
        {
            isDisplayRackAttached[(int)rackId] = false;
            GameObject.Destroy(DisplayRackObjs[(int)rackId].gameObject);
            DisplayRackObjs[(int)rackId] = null;
        }
    }

    public void AddFrame(DisplayRacks rackId, ref UnityVolumeRendering.SlicingPlane slicingPlane)
    {
        int curr_frame_num = visibilityLists[(int)rackId].Count;
        GameObject frame = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/SingleCanvas"));
        frame.name = "canvas" + curr_frame_num;
        frame.transform.parent = DisplayRackObjs[(int)rackId];
        frame.transform.localPosition = mDisplayPoses[curr_frame_num % 6];
        frame.transform.localRotation = Quaternion.identity;
        frame.transform.localScale = mUnitScale;

        var canvasRenderer = frame.GetComponent<MeshRenderer>();
        var frame_material = new Material(canvasRenderer.sharedMaterial);
        frame_material.SetTexture("_DataTex", slicingPlane.mDataTex);
        frame_material.SetTexture("_TFTex", slicingPlane.mTFTex);
        
        frameMaterials[(int)rackId].Add(frame_material);
        visibilityLists[(int)rackId].Add(true);
    }
}
