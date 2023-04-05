using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityVolumeRendering;

public class PaletteHandler : RayInteractableHandler
{
    public override void OnReset()
    {
        ClearHandles();
        foreach (var point in VolumeObjectFactory.gTargetVolume.transferFunction.colourControlPoints)
            AddHandleAtPosition(
                "Prefabs/PaletteHandle",
                new Vector3(point.dataValue * mInnerSize.x, .0f, -0.001f),
                point.colourValue);
        //VolumeObjectFactory.gTargetVolume.transferFunction.GenerateTexture();
        TFDirty = true;
    }
    public void OnAddHandle()
    {
        if (!VolumeObjectFactory.gTargetVolume) return;

        Color newColour = Color.HSVToRGB(Random.Range(0.0f, 1.0f), 1.0f, 1.0f);

        VolumeObjectFactory.gTargetVolume.transferFunction.colourControlPoints.Add(new TFColourControlPoint(0.5f, newColour));
        //VolumeObjectFactory.gTargetVolume.transferFunction.GenerateTexture();
        TFDirty = true;
        AddHandleAtPosition(
            "Prefabs/PaletteHandle",
            new Vector3(0.5f * mInnerSize.x, .0f, -0.001f),
            newColour);
    }
    public void OnRemoveHandle()
    {
        if (mHightlightHandle < 0) return;
     VolumeObjectFactory.gTargetVolume.transferFunction.colourControlPoints.RemoveAt(mHandleIds.IndexOf(mHightlightHandle));

        //VolumeObjectFactory.gTargetVolume.transferFunction.GenerateTexture();
        TFDirty = true;
        RemoveHandleAt(mHightlightHandle);
        mHightlightHandle = -1;
    }
    public void OnResetHandles()
    {
        if (!VolumeObjectFactory.gTargetVolume) return;

        var tf = VolumeObjectFactory.gTargetVolume.transferFunction;
        tf.colourControlPoints.Clear();
        tf.colourControlPoints.Add(new TFColourControlPoint(0.5f, new Color(0.469f, 0.354f, 0.223f, 1.0f)));
        VolumeObjectFactory.gTargetVolume.SetTransferFunction(tf);
        OnReset();
    }
    //public void OnSelect()
    //{
    //    CheckHandleHit(true, false);
    //}
    //public void OnUnSelect()
    //{
    //    UnTargetHandle();
    //}

    void Update()
    {
        if (mTargetHandle < 0 || !CheckCursorPosInOuterCanvas(true, false)) return;

        var handleX = mCursorPosInCanvas.x - mHorizontalBoarderLimit.x;

        mHandles[mTargetHandle].GetComponent<RectTransform>().anchoredPosition = new Vector2(handleX, .0f);

        var tf = VolumeObjectFactory.gTargetVolume.transferFunction;
        TFColourControlPoint colPoint = tf.colourControlPoints[mTargetHandleIndex];
        colPoint.dataValue = Mathf.Clamp(handleX * mInnerSizeInv.x, 0.0f, 1.0f);
        tf.colourControlPoints[mTargetHandleIndex] = colPoint;
        TFDirty = true;
    }
}
