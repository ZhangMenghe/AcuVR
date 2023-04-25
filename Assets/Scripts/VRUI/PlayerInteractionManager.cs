using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractionManager : MonoBehaviour
{
    enum PlayerInteractionMethod
    {
        PLYI_RIGHT_HAND = 0,
        PLYI_LEFT_HAND,
        PLYI_RIGHT_HAND_CONTROLLER,
        PLYI_LEFT_HAND_CONTROLLER,
        PLYI_END
    }
    [SerializeField]
    private GameObject OVRHands;
    [SerializeField]
    private GameObject OVRControllerHands;
    [SerializeField]
    private Transform RightFingerTip;
    [SerializeField]
    private Transform RightHandControllerFingerTip;
    [SerializeField]
    private Transform LeftFingerTip;
    [SerializeField]
    private Transform LeftHandControllerFingerTip;

    public UnityEngine.UI.Button InterMethodBtn;

    public static Transform PlayerOPTip;
    private PlayerInteractionMethod mPlayerInter;
    private void Awake()
    {
        InterMethodBtn.onClick.AddListener(delegate {
            OnChangeHandInteraction();
        });
    }
    private void Start()
    {
        mPlayerInter = PlayerInteractionMethod.PLYI_LEFT_HAND;
        OnChangeHandInteraction();
    }
    private void OnChangeHandInteraction()
    {
        int index = ((int)mPlayerInter + 1) % 4;
        mPlayerInter = (PlayerInteractionMethod)index;
        switch (mPlayerInter)
        {
            case PlayerInteractionMethod.PLYI_RIGHT_HAND:
                PlayerOPTip = RightFingerTip;
                break;
            case PlayerInteractionMethod.PLYI_RIGHT_HAND_CONTROLLER:
                PlayerOPTip = RightHandControllerFingerTip;
                break;
            case PlayerInteractionMethod.PLYI_LEFT_HAND:
                PlayerOPTip = LeftFingerTip;
                break;
            case PlayerInteractionMethod.PLYI_LEFT_HAND_CONTROLLER:
                PlayerOPTip = LeftHandControllerFingerTip;
                break;
        }
        
        OVRHands.SetActive(index < 2);
        OVRControllerHands.SetActive(index > 1);
        

        //flip
        if (index %2 == 1)
        {
            InterMethodBtn.GetComponent<Image>().rectTransform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        else
        {
            VRUICommonUtils.SwapSprite(ref InterMethodBtn);
            InterMethodBtn.GetComponent<Image>().rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

}
