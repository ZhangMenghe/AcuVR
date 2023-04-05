using UnityEngine;
using UnityVolumeRendering;
//ONLY IN EDIT MODE
[ExecuteInEditMode]
public class AcuNeedle : MonoBehaviour
{
    private VolumeRenderedObject m_target_volume;
    private float m_unified_scale = -1.0f;
    //private GameObject display_rack;
    private Transform m_needle;

    private void Start()
    {
        m_needle = this.transform.Find("needle");
        this.transform.Find("FOVCamera").GetComponent<Camera>().aspect = 1.5f;

        if(DisplayRackFactory.AttachToRack(DisplayRackFactory.DisplayRacks.CURVE_LEFT_BOARD, "Magnification View"))
        {
            var display_rack = DisplayRackFactory.GetDisplayRack(DisplayRackFactory.DisplayRacks.CURVE_LEFT_BOARD);

            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;

            quad.parent = display_rack;
            quad.localPosition = new Vector3(.0f, -0.32f, -0.01f);
            quad.localScale = new Vector3(0.5f, 0.3f, 1.0f);
            quad.localRotation = Quaternion.identity;

            quad.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("RenderTextures/AcuNeedleRT");
        }
    }
    public void Initialize(VolumeRenderedObject volObj)
    {
        m_target_volume = volObj;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_target_volume && m_target_volume.GetVolumeUnifiedScale() != m_unified_scale)
        {
            m_unified_scale = m_target_volume.GetVolumeUnifiedScale();
            m_needle.localScale = Vector3.one * 0.001f * m_unified_scale;
        }
    }
}
