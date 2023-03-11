using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkingTableController : MonoBehaviour
{
    public GameObject Visuals;
    private readonly float ChangeSpeed = 0.0005f;
    private bool mKeepChanging = false;
    private bool mIsUp;
    [SerializeField]
    private Oculus.Interaction.RayInteractable rayInteractable;

    public void OnCanvasSelected()
    {
        mKeepChanging = true;
        
        Debug.LogWarning("======"+ transform.InverseTransformPoint(rayInteractable.SurfaceHit.Point));
    }
    public void OnCanvasDeSelected()
    {
        mKeepChanging = false;

        transform.localPosition += Visuals.transform.localPosition;
        Visuals.transform.localPosition = Vector3.zero;
    }
    public void OnUpButtonClicked()
    {
        //mIsUp = true;
    }
    public void OnDownButtonClicked()
    {
        //mIsUp = false;
    }
    private void Update()
    {
        //if (mKeepChanging)
            //Visuals.transform.localPosition += new Vector3(.0f, mIsUp ? ChangeSpeed : -ChangeSpeed, .0f);
    }
}
