using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InteractableHandlerButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    enum HandlerButtonType
    {
        HANDLER_1D,
        HANDLER_2D
    }
    // The button component
    private Button button;

    private RayInteractableHandler mHandler;
    private int mId;
    private HandlerButtonType mType = HandlerButtonType.HANDLER_1D;
    void Start()
    {
        // Get the button component
        button = GetComponent<Button>();
    }
    public void Initialize(in RayInteractableHandler handler, int id)
    {
        mHandler = handler;
        mId = id;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        mHandler.OnHandlerPressed(mId);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mHandler.OnHandlerReleased();
    }
}
