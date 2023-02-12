using UnityEngine;
using UnityEditor;

namespace UnityVolumeRendering
{
    [CustomEditor(typeof(VolumeRenderedObject))]
    public class VolumeRenderedObjectCustomInspector : Editor
    {
        private bool contrastSetting = true;
        private bool tfSettings = true;
        private bool maskSettings = false;
        private bool lightSettings = false;
        private bool otherSettings = false;

        public override void OnInspectorGUI()
        {
            VolumeRenderedObject volrendObj = (VolumeRenderedObject)target;

            // Render mode
            RenderMode oldRenderMode = volrendObj.GetRenderMode();
            RenderMode newRenderMode = (RenderMode)EditorGUILayout.EnumPopup("Render mode", oldRenderMode);
            if (newRenderMode != oldRenderMode)
                volrendObj.SetRenderMode(newRenderMode);

            EditorGUILayout.Space();

            volrendObj.SetVolumeUnifiedScale(
                EditorGUILayout.Slider("Volume Scale",
                volrendObj.GetVolumeUnifiedScale(), .0f, 10.0f, new GUILayoutOption[] { GUILayout.Width(500.0f) })
                );
            EditorGUILayout.Space();

            // Visibility window
            contrastSetting = EditorGUILayout.Foldout(contrastSetting, "Contrast");
            if (contrastSetting)
            {
                bool cutoff_enabled = GUILayout.Toggle(volrendObj.GetContrastCutoffEnabled(), " Enable Value Cut-Off");
                volrendObj.SetContrastCutoffEnabled(cutoff_enabled);

                Vector3 contrast = volrendObj.GetVisibilityWindow();
                EditorGUILayout.MinMaxSlider("Contrast Value Range", ref contrast.x, ref contrast.y, 0.0f, 1.0f);
                if (!cutoff_enabled)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Brightness");
                    contrast.z = EditorGUILayout.Slider(contrast.z, .0f, 1.0f, new GUILayoutOption[] { GUILayout.Width(300.0f) });
                    EditorGUILayout.EndHorizontal();
                }
                volrendObj.SetVisibilityWindow(contrast);
            }


            // Transfer function settings
            EditorGUILayout.Space();
            tfSettings = EditorGUILayout.Foldout(tfSettings, "Transfer function");
            if (tfSettings)
            {
                // Transfer function type
                TFRenderMode tfMode = (TFRenderMode)EditorGUILayout.EnumPopup("Transfer function type", volrendObj.GetTransferFunctionMode());
                if (tfMode != volrendObj.GetTransferFunctionMode())
                    volrendObj.SetTransferFunctionMode(tfMode);

                // Show TF button
                if (GUILayout.Button("Edit transfer function"))
                {
                    if (tfMode == TFRenderMode.TF1D)
                        TransferFunctionEditorWindow.ShowWindow(volrendObj);
                    else
                        TransferFunction2DEditorWindow.ShowWindow();
                }

                //Color Transfer function
                volrendObj.SetColorScheme((ColorTransferScheme)EditorGUILayout.EnumPopup("Color Transfer Function", volrendObj.GetColorScheme()));
            }

            // Lighting settings
            EditorGUILayout.Space();
            lightSettings = EditorGUILayout.Foldout(lightSettings, "Lighting");
            if (lightSettings)
            {
                if (volrendObj.GetRenderMode() == RenderMode.DirectVolumeRendering)
                    volrendObj.SetLightingEnabled(GUILayout.Toggle(volrendObj.GetLightingEnabled(), "Enable lighting"));
                else
                    volrendObj.SetLightingEnabled(false);

                if (volrendObj.GetLightingEnabled() || volrendObj.GetRenderMode() == RenderMode.IsosurfaceRendering)
                {
                    LightSource oldLightSource = volrendObj.GetLightSource();
                    LightSource newLightSource = (LightSource)EditorGUILayout.EnumPopup("Light source", oldLightSource);
                    if (newLightSource != oldLightSource)
                        volrendObj.SetLightSource(newLightSource);
                }
            }

            //Mask setting
            EditorGUILayout.Space();
            maskSettings = EditorGUILayout.Foldout(maskSettings, "Organs");
            if (maskSettings)
            {
                volrendObj.SetOrganMaskEnabled(GUILayout.Toggle(volrendObj.GetOrganMaskEnabled(), " Enable Organ Mask"));
                volrendObj.SetOrganHighlightEnabled(GUILayout.Toggle(volrendObj.GetOrganHighlightEnabled(), " Highlight Organ"));
                volrendObj.SetBodyEnabled(GUILayout.Toggle(volrendObj.GetBodyEnabled(), " Enable Body"));
            }

            // Other settings for direct volume rendering
            if (volrendObj.GetRenderMode() == RenderMode.DirectVolumeRendering)
            {
                GUILayout.Space(10);
                otherSettings = EditorGUILayout.Foldout(otherSettings, "Other Settings");
                if (otherSettings)
                {
                    // Temporary back-to-front rendering option
                    volrendObj.SetDVRBackwardEnabled(GUILayout.Toggle(volrendObj.GetDVRBackwardEnabled(), "Enable Back-to-Front Direct Volume Rendering"));

                    // Early ray termination for Front-to-back DVR
                    if (!volrendObj.GetDVRBackwardEnabled())
                    {
                        volrendObj.SetRayTerminationEnabled(GUILayout.Toggle(volrendObj.GetRayTerminationEnabled(), "Enable early ray termination"));
                    }
                }
            }
        }
    }
}
