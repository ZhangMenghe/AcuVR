using UnityEngine;
using UnityVolumeRendering;
[ExecuteInEditMode]
public class AcuNeedle : MonoBehaviour
{
    private VolumeRenderedObject m_target_volume;
    private float m_unified_scale = -1.0f;
    private GameObject display_rack;
    private Transform m_needle;

    private void Start()
    {
        //MENGHE: WHERE TO DEFINE THE RATIO?
        m_needle = this.transform.Find("needle");
        this.transform.Find("FOVCamera").GetComponent<Camera>().aspect = 1.5f;
        display_rack = DisplayRackFactory.CreateMagnificationViewRack("Magnification View",
            1.5f);
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
