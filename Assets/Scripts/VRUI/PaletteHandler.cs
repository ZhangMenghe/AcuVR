using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaletteHandler : MonoBehaviour
{
    public GameObject paletteCanvas;
    public void OnHover()
    {
        paletteCanvas.GetComponent<Image>().color = new Color(1.0f, 1.0f, .0f);
    }
    public void OnUnHover()
    {
        paletteCanvas.GetComponent<Image>().color = new Color(.0f, 1.0f, 1.0f);
    }
    public void OnSelect()
    {
        paletteCanvas.GetComponent<Image>().color = new Color(1.0f, .0f, .0f);
    }
    public void OnUnSelect()
    {
        paletteCanvas.GetComponent<Image>().color = new Color(.0f, 1.0f, .0f);
    }
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
