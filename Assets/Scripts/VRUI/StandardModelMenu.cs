using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class StandardModelMenu
{
    //enum StandardModelLayers
    //{
    //    LAYER_SKIN = 0,
    //    LAYER_MUSCULAR,
    //    LAYER_CIRCULATORY,
    //    LAYER_DIGESTIVE,
    //    LAYER_EYE,
    //    LAYER_LYMPHATIC,
    //    LAYER_NERVOUS,
    //    LAYER_RESPIRATORY,
    //    LAYER_SKELETAL,
    //    LAYER_END
    //}
    //private readonly string[] mLayerNames = {
    //    "Skin", "Muscular", "Circulatory", "Digestive",
    //    "Eye", "Lymphatic", "Nervous", "Respiratory", "Skeletal"
    //};
    private Transform mModelRoot;
    private Dictionary<string, bool> mVisibilities = new Dictionary<string, bool>();

    private List<Button> mLayerButtons = new List<Button>();
    //private List<string> mOrganNames = new List<string>();

    public void Initialized(in Transform OrganMenu, in Transform HeadRoot)
    {
        mModelRoot = HeadRoot;
        
        GameObject template = OrganMenu.Find("template").gameObject;


        for(int i=0; i< HeadRoot.childCount; i++)
        {
            string organ_name = HeadRoot.GetChild(i).name;
            //mOrganNames.Add(organ_name);

            GameObject layer = GameObject.Instantiate(template);
            layer.transform.parent = OrganMenu;

            var rect_trans = layer.GetComponent<RectTransform>();

            rect_trans.localScale = Vector3.one;
            rect_trans.localPosition = new Vector3(10.0f, -10.0f, .0f);
            rect_trans.localRotation = Quaternion.identity;

            layer.SetActive(true);
            layer.name = organ_name;
            layer.GetComponentInChildren<TMPro.TMP_Text>().SetText(organ_name);
            var button = layer.GetComponentInChildren<Button>();
            button.onClick.AddListener(delegate {
                VRUICommonUtils.SwapSprite(ref button);
                //var target_obj = mHeadRoot.Find(layer.name).gameObject;
                //target_obj.SetActive(!target_obj.activeSelf);
                mVisibilities[layer.name] = !mVisibilities[layer.name];
                mModelRoot.Find(layer.name).gameObject.SetActive(mVisibilities[layer.name]);
            });
            mLayerButtons.Add(button);
            mVisibilities.Add(organ_name, true);
        }
    }
    public void onReset()
    {
        int id = 0;
        foreach (KeyValuePair<string, bool> vis in mVisibilities)
        {
            if (!vis.Value)
            {
                var button = mLayerButtons[id];
                VRUICommonUtils.SwapSprite(ref button);
                mModelRoot.Find(vis.Key).gameObject.SetActive(true);
            }
            id++;
        }
        Dictionary<string, bool> dictCopy = new Dictionary<string, bool>(mVisibilities);

        foreach (string key in dictCopy.Keys)
        {
            mVisibilities[key] = true;
        }
    }
}
