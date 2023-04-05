using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityVolumeRendering;

public class HistHandler : RayInteractableHandler
{
    public override void OnReset()
    {
        ClearHandles();
        foreach (var point in VolumeObjectFactory.gTargetVolume.transferFunction.alphaControlPoints)
            AddHandleAtPosition(
                "Prefabs/HistControllPoint",
                new Vector3(point.dataValue * mInnerSize.x, point.alphaValue * mInnerSize.y, -0.001f), Color.red); 
        TFDirty = true;
    }
    public void OnAddHandle()
    {
        if (!VolumeObjectFactory.gTargetVolume) return;

        VolumeObjectFactory.gTargetVolume.transferFunction.alphaControlPoints.Add(new TFAlphaControlPoint(0.5f, 0.5f));
        TFDirty = true;

        AddHandleAtPosition(
            "Prefabs/HistControllPoint",
            new Vector3(0.5f * mInnerSize.x, 0.5f * mInnerSize.y, -0.001f),
            Color.red);
    }
    public void OnRemoveHandle()
    {
        if (mHightlightHandle < 0) return;

        VolumeObjectFactory.gTargetVolume.transferFunction.alphaControlPoints.RemoveAt(mHandleIds.IndexOf(mHightlightHandle));
        TFDirty = true;
        RemoveHandleAt(mHightlightHandle);
        mHightlightHandle = -1;
    }
    public void OnResetHandles()
    {
        if (!VolumeObjectFactory.gTargetVolume) return;

        var tf = VolumeObjectFactory.gTargetVolume.transferFunction;
        tf.alphaControlPoints.Clear();
        tf.alphaControlPoints.Add(new TFAlphaControlPoint(0.2f, 0.0f));
        tf.alphaControlPoints.Add(new TFAlphaControlPoint(0.8f, 1.0f));
        if (VolumeObjectFactory.gTargetVolume)
            VolumeObjectFactory.gTargetVolume.SetTransferFunction(tf);
        VolumeObjectFactory.gTargetVolume.SetTransferFunction(tf);
        OnReset();
    }

    //public void OnSelect()
    //{
    //    CheckHandleHit(true, true);
    //}
    //public void OnUnSelect()
    //{
    //    UnTargetHandle();
    //}

    void Update()
    {
        if (mTargetHandle < 0 || !CheckCursorPosInOuterCanvas(true, true)) return;

        var handle = new Vector2(mCursorPosInCanvas.x - mHorizontalBoarderLimit.x,
            mCursorPosInCanvas.y - mVerticalBoarderLimit.x);

        mHandles[mTargetHandle].GetComponent<RectTransform>().anchoredPosition = handle;

        //ref TransferFunction tf = ref VolumeObjectFactory.gTargetVolume.transferFunction;
        TFAlphaControlPoint alphaPoint = VolumeObjectFactory.gTargetVolume.transferFunction.alphaControlPoints[mTargetHandleIndex];
        alphaPoint.dataValue = Mathf.Clamp(handle.x * mInnerSizeInv.x, 0.0f, 1.0f);
        alphaPoint.alphaValue = Mathf.Clamp(handle.y * mInnerSizeInv.y, 0.0f, 1.0f);
        VolumeObjectFactory.gTargetVolume.transferFunction.alphaControlPoints[mTargetHandleIndex] = alphaPoint;
        TFDirty = true;
    }
}
