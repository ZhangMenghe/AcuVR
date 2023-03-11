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

        UpdateHighlightHandle(mHandles.Count - 1);
    }
    public void OnRemoveHandle()
    {
        if (mHightlightHandleIndex < 0) return;

        VolumeObjectFactory.gTargetVolume.transferFunction.colourControlPoints.RemoveAt(mHightlightHandleIndex);
        //VolumeObjectFactory.gTargetVolume.transferFunction.GenerateTexture();
        TFDirty = true;
        RemoveHandleAt(mHightlightHandleIndex);
        mHightlightHandleIndex = -1;
    }
    public void OnSelect()
    {
        CheckHandleHit(true, false);
    }
    public void OnUnSelect()
    {
        UnTargetHandle();
    }

    void Update()
    {
        if (mTargetHandle < 0 || !CheckCursorPosInOuterCanvas(true, false)) return;

        var handleX = mCursorPosInCanvas.x - mHorizontalBoarderLimit.x;

        mHandles[mTargetHandle].GetComponent<RectTransform>().anchoredPosition = new Vector2(handleX, .0f);

        var tf = VolumeObjectFactory.gTargetVolume.transferFunction;
        TFColourControlPoint colPoint = tf.colourControlPoints[mTargetHandle];
        colPoint.dataValue = Mathf.Clamp(handleX * mInnerSizeInv.x, 0.0f, 1.0f);
        tf.colourControlPoints[mTargetHandle] = colPoint;
        TFDirty = true;
    }
}
