using UnityEngine;

public class GrabbaleAcuNeedle : MonoBehaviour
{
    public GameObject BoundingObj;
    public void OnSelected()
    {
        if (BoundingObj.activeSelf) BoundingObj.SetActive(false);
    }
}
