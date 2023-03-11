using UnityEngine;

public class RotationManipulation : MonoBehaviour
{
    enum TargetAxis
    {
        TARGET_AXIS_X = 0,
        TARGET_AXIS_Y,
        TARGET_AXIS_Z,
        TARGET_AXIS_None,
    }
    public Transform ControllerCursor;

    public Transform XAxis;
    public Transform YAxis;
    public Transform ZAxis;

    private TargetAxis mTargetAxis = TargetAxis.TARGET_AXIS_None;
    private Vector3 mLastCursorPos = Vector3.zero;

    private Transform mTargetTransform = null;
    private Transform mParentTransform = null;

    //private Vector3 mTargetLossyScaleSize;

    private readonly Color mHighlightColor = new Color(1.0f, 1.0f, .0f, 0.5f);

    private readonly Color[] AxisColors = {
        new Color(1.0f, 0.4f, 0.4f),
        new Color(0.52f, 0.8f, 0.32f),
        new Color(0.3f, 0.5f, 0.9f),
    };

    private readonly Vector3[] RotateAxis = { 
        Vector3.right,
        Vector3.up,
        Vector3.forward,
    };
    public void OnXAxisRotateStart()
    {
        transform.parent = null;

        mTargetAxis = TargetAxis.TARGET_AXIS_X;
        mLastCursorPos = ControllerCursor.position;
        XAxis.GetComponent<MeshRenderer>().material.SetColor("_Color", mHighlightColor);
    }
    public void OnXAxisRotateEnd()
    {
        XAxis.GetComponent<MeshRenderer>().material.SetColor("_Color", AxisColors[(int)mTargetAxis]);
        mTargetAxis = TargetAxis.TARGET_AXIS_None;
        transform.rotation = Quaternion.identity;
        transform.parent = mParentTransform;
    }
    public void OnYAxisRotateStart()
    {
        transform.parent = null;

        mTargetAxis = TargetAxis.TARGET_AXIS_Y;
        mLastCursorPos = ControllerCursor.position;
        YAxis.GetComponent<MeshRenderer>().material.SetColor("_Color", mHighlightColor);

    }
    public void OnYAxisRotateEnd()
    {
        YAxis.GetComponent<MeshRenderer>().material.SetColor("_Color", AxisColors[(int)mTargetAxis]);
        mTargetAxis = TargetAxis.TARGET_AXIS_None;
        transform.rotation = Quaternion.identity;
        transform.parent = mParentTransform;

    }
    public void OnZAxisRotateStart()
    {
        transform.parent = null;
        mTargetAxis = TargetAxis.TARGET_AXIS_Z;
        mLastCursorPos = ControllerCursor.position;
        ZAxis.GetComponent<MeshRenderer>().material.SetColor("_Color", mHighlightColor);
    }
    public void OnZAxisRotateEnd()
    {
        ZAxis.GetComponent<MeshRenderer>().material.SetColor("_Color", AxisColors[(int)mTargetAxis]);
        mTargetAxis = TargetAxis.TARGET_AXIS_None;
        transform.rotation = Quaternion.identity;
        transform.parent = mParentTransform;
    }

    public void Initialize()
    {
        mParentTransform = transform.parent;
        mTargetTransform = mParentTransform.Find("TargetObject");

        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
        //transform.localScale = Vector3.one * scale;
        //mTargetLossyScaleSize = mTargetTransform.lossyScale;

        //transform.localScale = Vector3.one * Mathf.Max(mTargetLossyScaleSize.x, mTargetLossyScaleSize.y, mTargetLossyScaleSize.z) * 1.5f;

        XAxis.GetComponent<MeshRenderer>().material.SetColor("_Color", AxisColors[0]);
        YAxis.GetComponent<MeshRenderer>().material.SetColor("_Color", AxisColors[1]);
        ZAxis.GetComponent<MeshRenderer>().material.SetColor("_Color", AxisColors[2]);
    }

    void Update()
    {
        if (!mTargetTransform) return;
        //if(mTargetLossyScaleSize!= mTargetTransform.lossyScale)
        //{
        //    mTargetLossyScaleSize = mTargetTransform.lossyScale;
        //    transform.localScale = Vector3.one * Mathf.Max(mTargetLossyScaleSize.x, mTargetLossyScaleSize.y, mTargetLossyScaleSize.z) * 1.5f;
        //}

        if (mTargetAxis == TargetAxis.TARGET_AXIS_None) return;
        float rotate_amount = Vector3.SignedAngle(mLastCursorPos - transform.position, ControllerCursor.position - transform.position, RotateAxis[(int)mTargetAxis]);

        mTargetTransform.RotateAround(mTargetTransform.position, RotateAxis[(int)mTargetAxis], rotate_amount);

        transform.rotation *= Quaternion.AngleAxis(rotate_amount, RotateAxis[(int)mTargetAxis]);

        mLastCursorPos = ControllerCursor.position;
    }
}
