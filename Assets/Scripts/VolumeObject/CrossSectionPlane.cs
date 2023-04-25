using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSectionPlane : MonoBehaviour
{
    public TMPro.TMP_Text idText;
    public void Initialized(int id)
    {

        idText.SetText(id.ToString());
    }
    public void OnPlaneSelect()
    {
        VolumeObjectFactory.gTargetVolume.UpdateCrossSectionPlane(name, true);
    }
    public void OnPlaneReleased()
    {
        VolumeObjectFactory.gTargetVolume.UpdateCrossSectionPlane(name, false);
    }

}
