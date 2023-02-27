using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine.UI;
using UnityEngine;
using UnityVolumeRendering;

public class RayInteractableHandler : MonoBehaviour
{
    public CylinderUI RootUIManager;
    public RayInteractorCursorVisual RightControllerCursor;

    private Transform mContentCanvas;
    private Transform mMesh;
    private List<Transform> mHandles = new List<Transform>();

    private int mTargetHandle = -1;
    private int mHightlightHandleIndex;

    private Vector2 mHorizontalBoarderLimit;
    private Vector2 mVerticalBoarderLimit;

    private Vector2 mOuterSize;
    private Vector2 mInnerSize;
    private Vector2 mInnerSizeInv;
    private Vector2 mInnerScaleInv;
    private Vector2 mHandleSize = Vector2.zero;

    //FENGYIN!! PLEASE DO NOT TOUCH, OTHERWISE IT WON'T ALIGN
    protected readonly float Comp = 0.06f;
    protected void ChangeHighlightHandle(int index, bool highlight)
    {
        if (index < 0 || index > mHandles.Count - 1) return;
        mHandles[index].Find("highlight").gameObject.SetActive(highlight);
    }
    public void OnAddHandle()
    {
        if (!RootUIManager.mTargetVolume) return;
        
        Color newColour = Color.HSVToRGB(Random.Range(0.0f, 1.0f), 1.0f, 1.0f);

        RootUIManager.mTargetVolume.transferFunction.colourControlPoints.Add(new TFColourControlPoint(Mathf.Clamp(0.5f, 0.0f, 1.0f), newColour));
        RootUIManager.mTargetVolume.transferFunction.GenerateTexture();
        AddHandleAtPosition(0.5f * mInnerSize.x, newColour);

        //reset last highlight handle
        ChangeHighlightHandle(mHightlightHandleIndex, false);
        mHightlightHandleIndex = mHandles.Count - 1;
        ChangeHighlightHandle(mHightlightHandleIndex, true);
    }
    public void OnRemoveHandle()
    {
        if (mHightlightHandleIndex < 0) return;

        RootUIManager.mTargetVolume.transferFunction.colourControlPoints.RemoveAt(mHightlightHandleIndex);
        RootUIManager.mTargetVolume.transferFunction.GenerateTexture();
        GameObject.Destroy(mHandles[mHightlightHandleIndex].gameObject);
        mHandles.RemoveAt(mHightlightHandleIndex);
        mHightlightHandleIndex = -1;
    }

    public void OnReset()
    {
        mTargetHandle = -1; mHightlightHandleIndex = -1;
        foreach (Transform handle in mHandles) GameObject.Destroy(handle.gameObject);
        mHandles.Clear();

        foreach (var point in RootUIManager.mTargetVolume.transferFunction.colourControlPoints)
            AddHandleAtPosition(point.dataValue * mInnerSize.x, point.colourValue);
        RootUIManager.mTargetVolume.transferFunction.GenerateTexture();
    }

    private bool CursorHitObject(Vector3 world_pos, in RectTransform rt)
    {
        float xPosNormalized = (mMesh.transform.InverseTransformPoint(world_pos).x * mInnerScaleInv.x + 0.5f)* mOuterSize.x;
        xPosNormalized += Comp * (mOuterSize.x * 0.5f - xPosNormalized);

        float rt_anchorx = rt.anchoredPosition.x + mHorizontalBoarderLimit.x;
        float sz = rt.rect.width * 0.5f;
        return (xPosNormalized >= rt_anchorx-sz) && (xPosNormalized <= rt_anchorx + sz);
    }

    private void AddHandleAtPosition(float pos, Color color)
    {
        var handle = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/PaletteHandle")).transform;
        if (mHandleSize.x == 0) mHandleSize = handle.GetComponent<RectTransform>().rect.size;
        handle.parent = mContentCanvas;

        handle.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(pos, .0f, -0.001f);
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
        mContentCanvas = outer_canvas.Find("Palette");

        mOuterSize = outer_canvas.GetComponent<RectTransform>().rect.size;
        mInnerSize = mContentCanvas.GetComponent<RectTransform>().rect.size;

        float boarder_size = (mOuterSize.x - mInnerSize.x) * 0.5f;
        mHorizontalBoarderLimit = new Vector2(boarder_size, mOuterSize.x - boarder_size);
        mInnerScaleInv = new Vector2(1.0f / mMesh.lossyScale.x, 1.0f/mMesh.lossyScale.y);
        mInnerSizeInv = new Vector2(1.0f / mInnerSize.x, 1.0f / mInnerSize.y);
    }
    private void Start()
    {
        if (!RootUIManager.mTargetVolume) return;
        OnReset();
    }
    public void OnHover()
    {
        //paletteCanvas.GetComponent<Image>().color = new Color(1.0f, 1.0f, .0f);
    }
    public void OnUnHover()
    {
        //paletteCanvas.GetComponent<Image>().color = new Color(.0f, 1.0f, 1.0f);
    }
    public void OnSelect()
    {
        for(int i=0; i<mHandles.Count; i++)
        {
            if (CursorHitObject(RightControllerCursor.transform.position, mHandles[i].GetComponent<RectTransform>()))
            {
                ChangeHighlightHandle(mHightlightHandleIndex, false);
                ChangeHighlightHandle(i, true);
                mTargetHandle = mHightlightHandleIndex = i;
                break;
            }
        }
    }
    public void OnUnSelect()
    {
        if (mTargetHandle < 0) return;
        //ChangeHighlightHandle(mTargetHandle, false);
        mTargetHandle = -1;
    }
    private void Update()
    {
        if (mTargetHandle < 0) return;

        float xpos = (mMesh.transform.InverseTransformPoint(RightControllerCursor.transform.position).x * mInnerScaleInv.x + 0.5f) * mOuterSize.x;
        xpos += Comp * (mOuterSize.x * 0.5f - xpos);

        if (xpos < (mHorizontalBoarderLimit.x + mHandleSize.x * 0.5f) || xpos > (mHorizontalBoarderLimit.y- mHandleSize.x * 0.5f)) return;

        var handle_pos = xpos - mHorizontalBoarderLimit.x;
        mHandles[mTargetHandle].GetComponent<RectTransform>().anchoredPosition = new Vector2(handle_pos, .0f);

        TFColourControlPoint colPoint = RootUIManager.mTargetVolume.transferFunction.colourControlPoints[mTargetHandle];
        colPoint.dataValue = Mathf.Clamp(handle_pos * mInnerSizeInv.x, 0.0f, 1.0f);
        RootUIManager.mTargetVolume.transferFunction.colourControlPoints[mTargetHandle] = colPoint;
        RootUIManager.mTargetVolume.transferFunction.GenerateTexture();
    }
}