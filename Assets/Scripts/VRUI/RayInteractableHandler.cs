using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine.UI;
using UnityEngine;
using UnityVolumeRendering;

public class RayInteractableHandler : MonoBehaviour
{
    public VolumeDataEdit VolumeManager;
    public RayInteractorCursorVisual RightControllerCursor;

    protected Transform mContentCanvas;
    protected Transform mMesh;
    protected List<Transform> mHandles = new List<Transform>();

    protected int mTargetHandle = -1;
    protected int mHightlightHandleIndex;

    protected Vector2 mHorizontalBoarderLimit;
    protected Vector2 mVerticalBoarderLimit;

    protected Vector2 mOuterSize;
    protected Vector2 mInnerSize;
    protected Vector2 mInnerSizeInv;
    protected Vector2 mInnerScaleInv;

    protected Vector2 mCursorPosInCanvas;

    public static bool TFDirty = false;

    //FENGYIN!! PLEASE DO NOT TOUCH, OTHERWISE IT WON'T ALIGN
    protected readonly float Comp = 0.06f;
    protected readonly float CompY = -0.45f;

    private void ChangeHighlightHandle(int index, bool highlight)
    {
        if (index < 0 || index > mHandles.Count - 1) return;
        mHandles[index].Find("highlight").gameObject.SetActive(highlight);
    }

    protected void UpdateHighlightHandle(int new_highlight)
    {
        ChangeHighlightHandle(mHightlightHandleIndex, false);
        mHightlightHandleIndex = new_highlight;
        ChangeHighlightHandle(mHightlightHandleIndex, true);
    }
    protected void RemoveHandleAt(int index)
    {
        GameObject.Destroy(mHandles[index].gameObject);
        mHandles.RemoveAt(index);
    }

    public virtual void OnReset()
    {
        ClearHandles();
    }
    
    protected void ClearHandles()
    {
        mTargetHandle = -1; mHightlightHandleIndex = -1;
        foreach (Transform handle in mHandles) GameObject.Destroy(handle.gameObject);
        mHandles.Clear();
    }

    protected bool CursorHitObject(Vector2 cursorPos, in RectTransform rt, bool checkX, bool checkY)
    {
        if (checkX)
        {            
            float rt_anchorx = rt.anchoredPosition.x + mHorizontalBoarderLimit.x;
            float sz = rt.rect.width * 0.5f;
            if (cursorPos.x < rt_anchorx - sz || cursorPos.x > rt_anchorx + sz) return false;
        }
        if (checkY)
        {
            float rt_anchory = rt.anchoredPosition.y + mVerticalBoarderLimit.x;
            float sz = rt.rect.height * 0.5f;
            if (cursorPos.y < rt_anchory - sz || cursorPos.y > rt_anchory + sz) return false;
        }
        return true;
    }

    //protected void AddHandleAtPosition(float pos, Color color)
    protected void AddHandleAtPosition(string HandlePrefabPath, Vector3 anchorPos, Color color)
    {
        var handle = GameObject.Instantiate(Resources.Load<GameObject>(HandlePrefabPath)).transform;
        handle.parent = mContentCanvas;

        handle.GetComponent<RectTransform>().anchoredPosition3D = anchorPos;//;
        handle.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        handle.GetComponent<RectTransform>().localScale = Vector3.one;
        float H, S, V;
        Color.RGBToHSV(color, out H, out S, out V);
        handle.GetComponent<Image>().color = Color.HSVToRGB(H, S, V * 0.5f);
        handle.name = "handle " + mHandles.Count;
        
        mHandles.Add(handle);
    }

    private void Awake()
    {
        mMesh = this.transform.Find("Mesh");

        var outer_canvas = transform.Find("Canvas");
        mContentCanvas = outer_canvas.Find("Content");

        mOuterSize = outer_canvas.GetComponent<RectTransform>().rect.size;
        mInnerSize = mContentCanvas.GetComponent<RectTransform>().rect.size;

        var boarder_size = (mOuterSize - mInnerSize) * 0.5f;
        mHorizontalBoarderLimit = new Vector2(boarder_size.x, mOuterSize.x - boarder_size.x);
        mVerticalBoarderLimit = new Vector2(boarder_size.y, mOuterSize.y - boarder_size.y);

        mInnerScaleInv = new Vector2(1.0f / mMesh.lossyScale.x, 1.0f/mMesh.lossyScale.y);
        mInnerSizeInv = new Vector2(1.0f / mInnerSize.x, 1.0f / mInnerSize.y);
    }
    private void Start()
    {
        if (!VolumeObjectFactory.gTargetVolume) return;
        OnReset();
    }

    protected void CheckHandleHit(bool checkX = true, bool checkY = true)
    {
        Vector2 CursorPos = Vector2.zero;
        Vector3 CursorInMesh = mMesh.transform.InverseTransformPoint(RightControllerCursor.transform.position);
        if (checkX)
        {
            CursorPos.x = (CursorInMesh.x * mInnerScaleInv.x + 0.5f) * mOuterSize.x;
            CursorPos.x += Comp * (mOuterSize.x * 0.5f - CursorPos.x);
        }
        if (checkY)
        {
            CursorPos.y = (CursorInMesh.y * mInnerScaleInv.y + 0.5f) * mOuterSize.y;
            CursorPos.y += CompY * (mOuterSize.y * 0.5f - CursorPos.y);
        }

        //check if any handle hits
        for(int i=0; i<mHandles.Count; i++)
        {
            if (CursorHitObject(CursorPos, mHandles[i].GetComponent<RectTransform>(), checkX, checkY))
            {
                ChangeHighlightHandle(mHightlightHandleIndex, false);
                ChangeHighlightHandle(i, true);
                mTargetHandle = mHightlightHandleIndex = i;
                break;
            }
        }
    }
    protected void UnTargetHandle()
    {
        if (mTargetHandle < 0) return;
        mTargetHandle = -1;
    }

    protected bool CheckCursorPosInOuterCanvas(bool checkX, bool checkY)
    {
        if (checkX)
        {
            mCursorPosInCanvas.x = (mMesh.transform.InverseTransformPoint(RightControllerCursor.transform.position).x * mInnerScaleInv.x + 0.5f) * mOuterSize.x;
            mCursorPosInCanvas.x += Comp * (mOuterSize.x * 0.5f - mCursorPosInCanvas.x);
            if (mCursorPosInCanvas.x < mHorizontalBoarderLimit.x || mCursorPosInCanvas.x > mHorizontalBoarderLimit.y) return false;
        }
        if (checkY)
        {
            mCursorPosInCanvas.y = (mMesh.transform.InverseTransformPoint(RightControllerCursor.transform.position).y * mInnerScaleInv.y + 0.5f) * mOuterSize.y;
            mCursorPosInCanvas.y += CompY * (mOuterSize.y * 0.5f - mCursorPosInCanvas.y);
            if (mCursorPosInCanvas.y < mVerticalBoarderLimit.x || mCursorPosInCanvas.y > mVerticalBoarderLimit.y) return false;
        }
        return true;
    }
    private void LateUpdate()
    {
        if (TFDirty)
        {
            VolumeObjectFactory.gTargetVolume.transferFunction.GenerateTexture();
            TFDirty = false;
        }
    }
}