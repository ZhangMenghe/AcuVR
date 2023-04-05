using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkingTableController : MonoBehaviour
{
    [SerializeField]
    private Oculus.Interaction.RayInteractable rayInteractable;

    private bool mKeepChanging = false;
    private bool mIsUp;

    private readonly float ChangeSpeed = 0.0005f;

    public void OnCanvasSelected()
    {
        float hitHeight = transform.InverseTransformPoint(rayInteractable.SurfaceHit.Point).y;

        if (Mathf.Abs(hitHeight) > 0.095f || Mathf.Abs(hitHeight) < 0.005f) return;

        mKeepChanging = true;
        mIsUp = hitHeight > 0;
    }
    public void OnCanvasDeSelected()
    {
        mKeepChanging = false;
    }
    private void Update()
    {
        if (mKeepChanging)
            transform.localPosition += new Vector3(.0f, mIsUp ? ChangeSpeed : -ChangeSpeed, .0f);
    }
}
