using TMPro;
using UnityEngine;

public class whiteboard_contents : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.Find("Canvas").Find("Title").GetComponent<TextMeshPro>().SetText("Slicing Planes");
    }
}
