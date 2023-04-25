using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine.UI;
using UnityEngine;
using UnityVolumeRendering;

public class RayInteractableHandler : MonoBehaviour
{
    //public VolumeDataEdit VolumeManager;
    //public RayInteractorCursorVisual RightControllerCursor;
    //[SerializeField]
    //private Transform RightFingerTip;
    //[SerializeField]
    //private Transform RightHandControllerFingerTip;
    //public Transform LeftFingerTip;
    //public Transform LeftHandControllerFingerTip;

    protected Transform mContentCanvas;
    private Transform mMeshTransform;
    protected Mesh mMesh = null;
    protected Dictionary<int, Transform> mHandles = new Dictionary<int, Transform>();
    protected List<int> mHandleIds = new List<int>();

    protected int mTargetHandle = -1;
    protected int mTargetHandleIndex = -1;

    protected int mHightlightHandle;

    protected Vector2 mHorizontalBoarderLimit;
    protected Vector2 mVerticalBoarderLimit;

    protected Vector2 mOuterSize;
    protected Vector2 mInnerSize;
    protected Vector2 mInnerSizeInv;
    protected Vector2 mInnerScaleInv;

    protected Vector2 mCursorPosInCanvas;

    public static bool TFDirty = false;
    private int mTotalHandlerNum = 0;
    private void ChangeHighlightHandle(int index, bool highlight)
    {
        if (index < 0) return;
        mHandles[index].Find("highlight").gameObject.SetActive(highlight);
    }

    protected void UpdateHighlightHandle(int new_highlight)
    {
        ChangeHighlightHandle(mHightlightHandle, false);
        mHightlightHandle = new_highlight;
        ChangeHighlightHandle(mHightlightHandle, true);
    }
    protected void RemoveHandleAt(int index)
    {
        GameObject.Destroy(mHandles[index].gameObject);
        mHandles.Remove(index);
        mHandleIds.Remove(index);
    }

    public virtual void OnReset()
    {
        ClearHandles();
    }
    
    protected void ClearHandles()
    {
        mTargetHandle = -1; mTargetHandleIndex = -1; mHightlightHandle = -1; 
        mTotalHandlerNum = 0;
        foreach (Transform handle in mHandles.Values) GameObject.Destroy(handle.gameObject);
        mHandles.Clear();
        mHandleIds.Clear();
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
        handle.name = "handle " + mTotalHandlerNum;
        handle.GetComponent<InteractableHandlerButton>().Initialize(this, mTotalHandlerNum);
        mHandles.Add(mTotalHandlerNum, handle);
        mHandleIds.Add(mTotalHandlerNum);
        UpdateHighlightHandle(mTotalHandlerNum);
        mTotalHandlerNum++;
    }
    private void Awake()
    {
        mMeshTransform = this.transform.Find("Mesh");

        var outer_canvas = transform.Find("Canvas");
        mContentCanvas = outer_canvas.Find("Content");

        mOuterSize = outer_canvas.GetComponent<RectTransform>().rect.size;
        mInnerSize = mContentCanvas.GetComponent<RectTransform>().rect.size;

        var boarder_size = (mOuterSize - mInnerSize) * 0.5f;
        mHorizontalBoarderLimit = new Vector2(boarder_size.x, mOuterSize.x - boarder_size.x);
        mVerticalBoarderLimit = new Vector2(boarder_size.y, mOuterSize.y - boarder_size.y);

        mInnerScaleInv = new Vector2(1.0f / mMeshTransform.lossyScale.x, 1.0f/ mMeshTransform.lossyScale.y);
        mInnerSizeInv = new Vector2(1.0f / mInnerSize.x, 1.0f / mInnerSize.y);

        //mUseHand = RightFingerTip.gameObject.activeInHierarchy;
    }
    private void Start()
    {
        if (!VolumeObjectFactory.gTargetVolume) return;
        OnReset();
    }
    public void OnHandlerPressed(int target)
    {
        ChangeHighlightHandle(mHightlightHandle, false);
        ChangeHighlightHandle(target, true);
        mTargetHandle = mHightlightHandle = target;
        mTargetHandleIndex = mHandleIds.IndexOf(mTargetHandle);
    }
    public void OnHandlerReleased()
    {
        if (mTargetHandle < 0) return;
        mTargetHandle = -1; mTargetHandleIndex = -1;
    }
    private Vector2 GetCursorPos()
    {
        // Convert cursor position to local space of the mesh
        //Vector3 cursorInMesh = mMeshTransform.InverseTransformPoint(CylinderUI.UseHand?RightFingerTip.position:RightHandControllerFingerTip.position);
        Vector3 cursorInMesh = mMeshTransform.InverseTransformPoint(PlayerInteractionManager.PlayerOPTip.position);
        cursorInMesh.z = .0f;
        if (!mMesh)
            mMesh = mMeshTransform.GetComponent<MeshFilter>().mesh;
        if (mMesh.bounds.Contains(cursorInMesh))
        {
            // Compute the texture coordinates of the cursor position
            return new Vector2((cursorInMesh.x / mMesh.bounds.size.x + 0.5f)*mInnerSize.x, (cursorInMesh.y / mMesh.bounds.size.y + 0.5f) * mInnerSize.y);
        }
        return Vector2.zero;
    }
    //protected void CheckHandleHit(bool checkX = true, bool checkY = true)
    //{


    //    //Vector2 CursorPos = Vector2.zero;
    //    //Vector3 CursorInMesh = mMesh.transform.InverseTransformPoint(RightFingerTip.position);

    //    //if (checkX)
    //    //{
    //    //    CursorPos.x = (CursorInMesh.x * mInnerScaleInv.x + 0.5f) * mOuterSize.x;
    //    //    CursorPos.x += Comp * (mOuterSize.x * 0.5f - CursorPos.x);
    //    //}
    //    //if (checkY)
    //    //{
    //    //    CursorPos.y = (CursorInMesh.y * mInnerScaleInv.y + 0.5f) * mOuterSize.y;
    //    //    CursorPos.y += CompY * (mOuterSize.y * 0.5f - CursorPos.y);
    //    //}
    //    Vector2 CursorPos = GetCursorPos();

    //    //check if any handle hits
    //    for (int i=0; i<mHandles.Count; i++)
    //    {
    //        if (CursorHitObject(CursorPos, mHandles[i].GetComponent<RectTransform>(), checkX, checkY))
    //        {
    //            ChangeHighlightHandle(mHightlightHandle, false);
    //            ChangeHighlightHandle(i, true);
    //            mTargetHandle = mHightlightHandle = i;
    //            break;
    //        }
    //    }
    //}
    //protected void UnTargetHandle()
    //{
    //    if (mTargetHandle < 0) return;
    //    mTargetHandle = -1;
    //}
    protected bool CheckCursorPosInOuterCanvas(bool checkX, bool checkY)
    {
        mCursorPosInCanvas = GetCursorPos();

        if (checkX)
        {
            if (mCursorPosInCanvas.x < mHorizontalBoarderLimit.x || mCursorPosInCanvas.x > mHorizontalBoarderLimit.y) return false;
        }
        if (checkY)
        {
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