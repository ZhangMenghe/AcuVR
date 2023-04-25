using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class StandardModelMenu
{
    enum StandardModelLayers
    {
        LAYER_SKIN = 0,
        LAYER_MUSCULAR,
        LAYER_CIRCULATORY,
        LAYER_DIGESTIVE,
        LAYER_EYE,
        LAYER_LYMPHATIC,
        LAYER_NERVOUS,
        LAYER_RESPIRATORY,
        LAYER_SKELETAL,
        LAYER_END
    }
    private readonly string[] mLayerNames = {
        "Skin", "Muscular", "Circulatory", "Digestive",
        "Eye", "Lymphatic", "Nervous", "Respiratory", "Skeletal"
    };
    private Transform mHeadRoot;
    private Dictionary<string, bool> mVisibilities = new Dictionary<string, bool>();

    private List<Button> mLayerButtons = new List<Button>();
    public void Initialized(in Transform OrganMenu, in Transform HeadRoot)
    {
        mHeadRoot = HeadRoot;
        
        GameObject template = OrganMenu.Find("template").gameObject;
        for(int i=0; i< (int)StandardModelLayers.LAYER_END; i++)
        {
            GameObject layer = GameObject.Instantiate(template);
            layer.transform.parent = OrganMenu;

            var rect_trans = layer.GetComponent<RectTransform>();

            rect_trans.localScale = Vector3.one;
            rect_trans.localPosition = new Vector3(10.0f, -10.0f, .0f);
            rect_trans.localRotation = Quaternion.identity;

            layer.SetActive(true);
            layer.name = mLayerNames[i];
            layer.GetComponentInChildren<TMPro.TMP_Text>().SetText(mLayerNames[i]);
            var button = layer.GetComponentInChildren<Button>();
            button.onClick.AddListener(delegate {
                VRUICommonUtils.SwapSprite(ref button);
                //var target_obj = mHeadRoot.Find(layer.name).gameObject;
                //target_obj.SetActive(!target_obj.activeSelf);
                mVisibilities[layer.name] = !mVisibilities[layer.name];
                mHeadRoot.Find(layer.name).gameObject.SetActive(mVisibilities[layer.name]);
            });
            mLayerButtons.Add(button);
            mVisibilities.Add(mLayerNames[i], true);
        }
    }
    public void onReset()
    {
        for (int i = 0; i < (int)StandardModelLayers.LAYER_END; i++)
        {
            if (!mVisibilities[mLayerNames[i]])
            {
                var button = mLayerButtons[i];
                VRUICommonUtils.SwapSprite(ref button);
                mHeadRoot.Find(mLayerNames[i]).gameObject.SetActive(true);
                mVisibilities[mLayerNames[i]] = true;
            }
        }
    }
}
