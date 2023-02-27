using UnityEngine;

public class DrawMeshBounds : MonoBehaviour
{
    public void OnDrawBoundEnable()
    {
        this.gameObject.SetActive(true);
    }
    public void OnDrawBoundDisable()
    {
        this.gameObject.SetActive(false);
    }
}
