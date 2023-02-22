using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//[ExecuteInEditMode]
public class DisplayRackFactory//:MonoBehaviour
{
    public enum DisplayRacks
    {
        ROOM_LEFT_BOARD = 0,
        DISPLAY_RACK_END
    }
    //public static Transform LeftBoardObj;
    private static Transform[] DisplayRackObjs = new Transform[(int)DisplayRacks.DISPLAY_RACK_END];

    private static bool[] isDisplayRackAttached = new bool[(int)DisplayRacks.DISPLAY_RACK_END];
    private static List<bool>[] visibilityLists = Enumerable.Repeat(new List<bool>(), (int)DisplayRacks.DISPLAY_RACK_END).ToArray();
    private static List<Material>[] frameMaterials = Enumerable.Repeat(new List<Material>(), (int)DisplayRacks.DISPLAY_RACK_END).ToArray();
    private static List<Transform>[] frameList = Enumerable.Repeat(new List<Transform>(), (int)DisplayRacks.DISPLAY_RACK_END).ToArray();
    private static List<Transform> slicingPlaneList = new List<Transform>();

    private static readonly Vector3 LeftUpperCorner=new Vector3(-1.5f, -1.0f,  -0.1f);
    private static readonly Vector2 FrameGap = new Vector2(1.0f, -1.0f);

    //public static readonly Vector3[] mDisplayPoses = new Vector3[] {
    //        new Vector3(-0.6f, 0.4f, -0.1f), new Vector3(.0f, 0.4f, .0f), new Vector3(0.6f, 0.4f, .0f),
    //        new Vector3(-0.6f, -0.2f, .0f), new Vector3(.0f, -0.2f, .0f), new Vector3(0.6f, -0.2f, .0f)
    //    };
    public static readonly Vector3 mUnitScale = Vector3.one * 0.8f;

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
            if (!DisplayRackObjs[(int)rackId])
            {
                Transform StaticParent = GameObject.Find("Static").transform;
                DisplayRackObjs[(int)rackId] = StaticParent.Find("LeftBoard");
                if (!DisplayRackObjs[(int)rackId])
                {
                    DisplayRackObjs[(int)rackId] = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/LeftBoard")).transform;
                    DisplayRackObjs[(int)rackId].parent = GameObject.Find("Static").transform;
                    DisplayRackObjs[(int)rackId].name = "LeftBoard";
                }
            }

            isDisplayRackAttached[(int)rackId] = true;
            var title = DisplayRackObjs[(int)rackId].Find("Title");
            title.GetComponent<TMPro.TextMeshPro>().SetText(rack_name);
        }
    }

    public static void DeAttachFromRack(DisplayRacks rackId)
    {
        if (isDisplayRackAttached[(int)rackId])
        {
            isDisplayRackAttached[(int)rackId] = false;
            if (DisplayRackObjs[(int)rackId])
            {
                GameObject.Destroy(DisplayRackObjs[(int)rackId].gameObject);
                DisplayRackObjs[(int)rackId] = null;
            }
        }
    }

    public static void AddFrame(DisplayRacks rackId, in UnityVolumeRendering.SlicingPlane slicingPlane)
    {
        if (!isDisplayRackAttached[(int)rackId]) return;
        int curr_frame_num = visibilityLists[(int)rackId].Count;
        Transform frame = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/SingleCanvas")).transform;
        frame.name = "canvas" + curr_frame_num;
        frame.parent = DisplayRackObjs[(int)rackId];
        //MENGHE:FIX THIS WITH PAGES
        curr_frame_num = curr_frame_num % 8;
        frame.localPosition = LeftUpperCorner + new Vector3((curr_frame_num % 4)*FrameGap.x, (curr_frame_num /4)*FrameGap.y, .0f);
        //frame.localPosition = mDisplayPoses[curr_frame_num % 4];
        frame.localRotation = Quaternion.identity;
        frame.localScale = mUnitScale;

        var canvasRenderer = frame.GetComponent<MeshRenderer>();
        var frame_material = new Material(canvasRenderer.sharedMaterial);
        frame_material.SetTexture("_DataTex", slicingPlane.mDataTex);
        frame_material.SetTexture("_TFTex", slicingPlane.mTFTex);

        frameList[(int)rackId].Add(frame);
        frameMaterials[(int)rackId].Add(frame_material);
        visibilityLists[(int)rackId].Add(true);

        if (rackId == DisplayRacks.ROOM_LEFT_BOARD)
            slicingPlaneList.Add(slicingPlane.transform);
    }
    public static void RemoveFrame(DisplayRacks rackId, int targetId)
    {
        if (!isDisplayRackAttached[(int)rackId]) return;

        GameObject.Destroy(frameList[(int)rackId][targetId]);
        frameList[(int)rackId].RemoveAt(targetId);
        frameMaterials[(int)rackId].RemoveAt(targetId);
        visibilityLists[(int)rackId].RemoveAt(targetId);
        if (rackId == DisplayRacks.ROOM_LEFT_BOARD)
        {
            GameObject.Destroy(slicingPlaneList[targetId]);
            slicingPlaneList.RemoveAt(targetId);
        }
    }
    public static void ChangeFrameVisibilityStatus(DisplayRacks rackId, int targetId, bool isOn)
    {
        if (!isDisplayRackAttached[(int)rackId] || visibilityLists[(int)rackId][targetId]==isOn) return;
        visibilityLists[(int)rackId][targetId] = isOn;
        //frameList[(int)rackId][targetId];
        var targetFrame = frameList[(int)rackId][targetId];

        if (isOn)
        {
            if (targetFrame.childCount > 0) {
                for (int i = 0; i < targetFrame.childCount; i++)
                {
                    targetFrame.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (targetFrame.childCount > 0)
            {
                for (int i = 0; i < targetFrame.childCount; i++)
                {
                    targetFrame.GetChild(i).gameObject.SetActive(true);
                }
            }
            else
            {
                var invisible_quad = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/InvisibleShadeCanvas")).transform;
                invisible_quad.parent = targetFrame;
                invisible_quad.localPosition = new Vector3(.0f, .0f, -0.01f);
                invisible_quad.localRotation = Quaternion.identity;
                invisible_quad.localScale = Vector3.one;
            }
        }
    }
    public static void RenderFrames()
    {
        for (int i=0; i<(int)DisplayRacks.DISPLAY_RACK_END; i++)
        {
            if (!isDisplayRackAttached[i]) continue;

            var currFrameList = frameList[i];
            var currFrameMats = frameMaterials[i];
            var currVisibilities = visibilityLists[i];

            for (int fid=0; fid<frameList[i].Count; fid++)
            {
                var canvasRenderer = currFrameList[fid].GetComponent<MeshRenderer>();
                if (canvasRenderer != null)
                {
                    if(currVisibilities[fid]) currFrameMats[fid].DisableKeyword("INVISIBLE");
                    else currFrameMats[fid].EnableKeyword("INVISIBLE");

                    currFrameMats[fid].DisableKeyword("OVERRIDE_MODEL_MAT");
                    currFrameMats[fid].SetMatrix("_parentInverseMat", slicingPlaneList[fid].parent.worldToLocalMatrix);
                    currFrameMats[fid].SetMatrix("_planeMat", Matrix4x4.TRS(
                        slicingPlaneList[fid].position,
                        slicingPlaneList[fid].rotation,
                        slicingPlaneList[fid].parent ? slicingPlaneList[fid].parent.lossyScale : slicingPlaneList[fid].lossyScale));
                    canvasRenderer.sharedMaterial = currFrameMats[fid];
                }
            }
        }

    }
    //private void Update()
    //{
    //    RenderFrames();
    //}
}
