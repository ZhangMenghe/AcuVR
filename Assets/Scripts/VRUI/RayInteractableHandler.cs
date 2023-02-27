using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine.UI;
using UnityEngine;
using UnityVolumeRendering;

public class RayInteractableHandler : MonoBehaviour
{
    public CylinderUI RootUIManager;
    public RayInteractorCursorVisual RightControllerCursor;
    private Transform paletteCanvas;
    private Transform mMesh;

    private List<Transform> mHandles = new List<Transform>();
    private int mTargetHandle = -1;
    private float mPaletteScaleInv;
    private Vector2 mBoarderLimit;
    private float mOuterSize;
    private float mPaletteSizeInv;
    private float HANDLE_WIDTH = -1.0f;
    private int mHightlightHandleIndex;
    //FENGYIN!! PLEASE DO NOT TOUCH, OTHERWISE IT WON'T ALIGN
    private readonly float Comp = 0.06f;
    private readonly float HandleStartPos = 400.0f;
    private void ChangeHighlightHandle(int index, bool highlight)
    {
        if (index < 0 || index > mHandles.Count - 1) return;
        mHandles[index].Find("highlight").gameObject.SetActive(highlight);
    }
    public void OnAddHandle()
    {
        if (!RootUIManager.mTargetVolume) return;
        
        Color newColour = Color.HSVToRGB(Random.Range(0.0f, 1.0f), 1.0f, 1.0f);

        RootUIManager.mTargetVolume.transferFunction.colourControlPoints.Add(new TFColourControlPoint(Mathf.Clamp(HandleStartPos* mPaletteSizeInv, 0.0f, 1.0f), newColour));
        RootUIManager.mTargetVolume.transferFunction.GenerateTexture();
        AddHandleAtPosition(HandleStartPos, newColour);

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
            AddHandleAtPosition(point.dataValue / mPaletteSizeInv, point.colourValue);
        RootUIManager.mTargetVolume.transferFunction.GenerateTexture();
    }

    private bool CursorHitObject(Vector3 world_pos, in RectTransform rt)
    {
        float xPosNormalized = (mMesh.transform.InverseTransformPoint(world_pos).x * mPaletteScaleInv +0.5f)* mOuterSize;
        xPosNormalized += Comp * (mOuterSize * 0.5f - xPosNormalized);

        float rt_anchorx = rt.anchoredPosition.x + mBoarderLimit.x;
        float sz = rt.rect.width * 0.5f;
        return (xPosNormalized >= rt_anchorx-sz) && (xPosNormalized <= rt_anchorx + sz);
    }

    private void AddHandleAtPosition(float pos, Color color)
    {
        var handle = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/PaletteHandle")).transform;
        if (HANDLE_WIDTH < 0) HANDLE_WIDTH = handle.GetComponent<RectTransform>().rect.width;
        handle.parent = paletteCanvas;

        handle.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(pos, .0f, -0.001f);
        handle.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        handle.GetComponent<RectTransform>().localScale = Vector3.one;
        handle.GetComponent<Image>().color = color;
        handle.name = "handle " + mHandles.Count;
        
        mHandles.Add(handle);
    }

    private void Awake()
    {
        mMesh = this.transform.Find("Mesh");

        var outer_canvas = transform.Find("Canvas");
        paletteCanvas = outer_canvas.Find("Palette");

        mOuterSize = outer_canvas.GetComponent<RectTransform>().rect.width;
        var inner_size = paletteCanvas.GetComponent<RectTransform>().rect.width;

        float boarder_size = (mOuterSize - inner_size) * 0.5f;
        mBoarderLimit = new Vector2(boarder_size, mOuterSize - boarder_size);
        mPaletteScaleInv = 1.0f / mMesh.lossyScale.x;
        mPaletteSizeInv = 1.0f / inner_size;
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

        float xpos = (mMesh.transform.InverseTransformPoint(RightControllerCursor.transform.position).x * mPaletteScaleInv + 0.5f) * mOuterSize;
        xpos += Comp * (mOuterSize * 0.5f - xpos);

        if (xpos < (mBoarderLimit.x + HANDLE_WIDTH * 0.5f) || xpos > (mBoarderLimit.y-HANDLE_WIDTH * 0.5f)) return;

        var handle_pos = xpos - mBoarderLimit.x;
        mHandles[mTargetHandle].GetComponent<RectTransform>().anchoredPosition = new Vector2(handle_pos, .0f);

        TFColourControlPoint colPoint = RootUIManager.mTargetVolume.transferFunction.colourControlPoints[mTargetHandle];
        colPoint.dataValue = Mathf.Clamp(handle_pos * mPaletteSizeInv, 0.0f, 1.0f);
        RootUIManager.mTargetVolume.transferFunction.colourControlPoints[mTargetHandle] = colPoint;
        RootUIManager.mTargetVolume.transferFunction.GenerateTexture();
    }
}
