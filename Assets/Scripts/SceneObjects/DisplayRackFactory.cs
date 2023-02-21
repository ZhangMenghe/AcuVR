using System.Collections.Generic;
using UnityEngine;


public class DisplayRackFactory
{
    public static Transform LeftBoardObj;
    private static bool isLeftBoardAttached = false;
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

    public static void AttachToRoomLeftBoard(string rack_name) {
        if (!LeftBoardObj)
        {
            LeftBoardObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/LeftBoard")).transform;
            LeftBoardObj.parent = GameObject.Find("Static").transform;
        }

        isLeftBoardAttached = true;
        var title = LeftBoardObj.transform.Find("Title");
        title.GetComponent<TMPro.TextMeshPro>().SetText(rack_name);

        m_rack_names.Add(rack_name);
    }

    public static void DeAttachToRoomLeftBoard()
    {
        if (isLeftBoardAttached)
        {
            isLeftBoardAttached = false;
            GameObject.Destroy(LeftBoardObj.gameObject);
            LeftBoardObj = null;
        }
    }
}
