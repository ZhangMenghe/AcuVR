using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRUICommonUtils
{
    public static void SwapSprite(ref Button targetBtn)
    {
        var tmp = targetBtn.spriteState.disabledSprite;
        SpriteState spriteState = targetBtn.spriteState;
        spriteState.disabledSprite = targetBtn.image.sprite;
        targetBtn.image.sprite = tmp;
        targetBtn.spriteState = spriteState;
    }
}
