using UnityEngine;
public class WristUI : MonoBehaviour
{
    public Transform _wristUICanvas;
    public Transform HandTracker;

    readonly private Vector3 CONST_WRIST_MENU_OFFSET = new Vector3(-0.06f, -0.012f, -0.005f);
    private void Update()
    {
        _wristUICanvas.rotation = HandTracker.rotation * Quaternion.AngleAxis(90.0f, Vector3.right);
        _wristUICanvas.position = HandTracker.position + CONST_WRIST_MENU_OFFSET;
    }
    public void onMoreButtonClicked()
    {
        Debug.Log("================You have clicked the button!");
    }
}
