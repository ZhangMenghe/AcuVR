using UnityEngine;
using UnityEngine.UI;
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
    //[HideInInspector]
    //public bool[] mVisibles;
    private Transform mHeadRoot;

    public void Initialized(in Transform OrganMenu, in Transform HeadRoot)
    {
        mHeadRoot = HeadRoot;
        //mVisibles = new bool[(int)StandardModelLayers.LAYER_END];
        
        GameObject template = OrganMenu.Find("template").gameObject;
        for(int i=0; i< (int)StandardModelLayers.LAYER_END; i++)
        {
            //mVisibles[i] = true;
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
                var target_obj = mHeadRoot.Find(layer.name).gameObject;
                target_obj.SetActive(!target_obj.activeSelf);
                //mHeadRoot.Find(layer.name).gameObject.SetActive(mVisibles[i]);
            });
        }
    }
}
