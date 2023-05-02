using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSectionPlane : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_Text idText;
    [SerializeField]
    private Transform TargetPlane;
    [SerializeField]
    private RectTransform IdCanvas;

    private Vector3 mIdCanvasPos;

    public void Initialized(int id, Vector3 scale)
    {
        idText.SetText(id.ToString());
        //TargetPlane.localScale = scale;
        
        TargetPlane.localScale = Vector3.one * Mathf.Max(scale.x, scale.z);
        var uscale = TargetPlane.localScale;
        mIdCanvasPos = IdCanvas.anchoredPosition3D;
        IdCanvas.anchoredPosition3D = new Vector3(mIdCanvasPos.x * uscale.x, mIdCanvasPos.y, mIdCanvasPos.z * uscale.z);
        IdCanvas.localScale  *= (uscale.x+ uscale.z)*0.5f;
    }
    public void OnPlaneSelect()
    {
        VolumeObjectFactory.gTargetVolume.UpdateCrossSectionPlane(name, true);
        StandardModelFactory.OnUpdateCrossSectionPlane(name, true);
    }
    public void OnPlaneReleased()
    {
        VolumeObjectFactory.gTargetVolume.UpdateCrossSectionPlane(name, false);
        StandardModelFactory.OnUpdateCrossSectionPlane(name, false);
    }

}
