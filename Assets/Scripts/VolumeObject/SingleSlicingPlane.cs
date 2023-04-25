using UnityEngine;
//[ExecuteInEditMode]
public class SingleSlicingPlane : MonoBehaviour
{
    public TMPro.TMP_Text idText;

    //[SerializeField]
    //private GrabbaleAcuNeedle targetNeedle;

    private Transform mTargetNeedleTip;
    private Transform mTargetNeedleEnd;

    private ProjectedNeedle mNeedlePlane;

    private bool mDrawNeedleProjection;

    public void Initialized(int id)
    {
        idText.SetText(id.ToString());
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
