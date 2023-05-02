using UnityEngine;
//[ExecuteInEditMode]
public class SingleSlicingPlane : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_Text idText;
    [SerializeField]
    private Transform TargetPlane;
    [SerializeField]
    private RectTransform IdCanvas;

    //[SerializeField]
    //private GrabbaleAcuNeedle targetNeedle;

    private Transform mTargetNeedleTip;
    private Transform mTargetNeedleEnd;

    private ProjectedNeedle mNeedlePlane;

    private bool mDrawNeedleProjection;
    private Vector3 mIdCanvasPos;

    public void Initialized(int id, Vector3 scale)
    {
        idText.SetText(id.ToString());
        //TargetPlane.localScale = scale;

        TargetPlane.localScale = Vector3.one * Mathf.Max(scale.x, scale.z);
        var uscale = TargetPlane.localScale;
        mIdCanvasPos = IdCanvas.anchoredPosition3D;
        IdCanvas.anchoredPosition3D = new Vector3(mIdCanvasPos.x * uscale.x, mIdCanvasPos.y, mIdCanvasPos.z * uscale.z);
        IdCanvas.localScale *= (uscale.x + uscale.z) * 0.5f;

        mNeedlePlane = GetComponentInChildren<ProjectedNeedle>();
        mNeedlePlane.Initialize();
        mDrawNeedleProjection = false;
    }

    public void onEnableNeedleProject(in Transform needleTip, in Transform needleEnd)
    {
        mDrawNeedleProjection = true;
        mTargetNeedleTip = needleTip;
        mTargetNeedleEnd = needleEnd;
    }

    public void OnDisableNeedleProject()
    {
        mDrawNeedleProjection = false;
    }

    public RenderTexture GetRenderTexture()
    {
        return mNeedlePlane.mRT;
    }
    private void Update()
    {
        if (mDrawNeedleProjection)
            mNeedlePlane.OnUpdate(mTargetNeedleTip.position, mTargetNeedleEnd.position);

    }
}
