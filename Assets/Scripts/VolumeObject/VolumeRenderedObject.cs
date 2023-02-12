using System.Collections.Generic;
using UnityEngine;

namespace UnityVolumeRendering
{
    public class HelmVolumeParam
    {
        public bool mask_enabled = false;
        public bool body_enabled = true;
        public bool mask_highlight = true;
       
        public void ResetProperties(Material mat)
        {
            if (body_enabled) mat.EnableKeyword("SHOW_BODY");
            else mat.DisableKeyword("SHOW_BODY");

            if (mask_enabled) mat.EnableKeyword("SHOW_ORGAN");
            else mat.DisableKeyword("SHOW_ORGAN");

            if (mask_highlight) mat.EnableKeyword("SHOW_ORGAN_RECOLOR");
            else mat.DisableKeyword("SHOW_ORGAN_RECOLOR");
        }
    }
    [ExecuteInEditMode]
    public class VolumeRenderedObject : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        public TransferFunction transferFunction;

        [SerializeField, HideInInspector]
        public TransferFunction2D transferFunction2D;

        [SerializeField, HideInInspector]
        public VolumeDataset dataset;

        [SerializeField, HideInInspector]
        public MeshRenderer meshRenderer;

        [SerializeField, HideInInspector]
        private RenderMode renderMode;

        [SerializeField, HideInInspector]
        private TFRenderMode tfRenderMode;

        [SerializeField, HideInInspector]
        private bool lightingEnabled;

        [SerializeField, HideInInspector]
        private LightSource lightSource;

        [SerializeField, HideInInspector]
        private Vector3 contrastCutOffWindow = new Vector3(0.0f, 1.0f, 0.5f);
        private Vector3 contrastAdjustWindow = new Vector3(.0f, 1.0f, 0.5f);
        private bool contrastCutoffEnabled = true;

        private ColorTransferScheme colorScheme;

        [SerializeField, HideInInspector]
        private bool rayTerminationEnabled = true;

        [SerializeField, HideInInspector]
        private bool dvrBackward = false;

        private HelmVolumeParam helm_params = new HelmVolumeParam();

        private List<SlicingPlane> m_sl_planes = new List<SlicingPlane>();
        private readonly Vector3[] m_sl_display_poses = new Vector3[] {
            new Vector3(-0.6f, 0.4f, .0f), new Vector3(.0f, 0.4f, .0f), new Vector3(0.6f, 0.4f, .0f),
            new Vector3(-0.6f, -0.2f, .0f), new Vector3(.0f, -0.2f, .0f), new Vector3(0.6f, -0.2f, .0f)
        };
        private readonly Vector3 m_sl_display_scale = Vector3.one * 0.5f;

        //TODO:
        private List<Transform> m_cs_planes = new List<Transform>();
        private static int MAX_CS_PLANE_NUM = 5;
        private Matrix4x4[] m_cs_plane_matrices = new Matrix4x4[MAX_CS_PLANE_NUM];

        private float m_unified_scale = 1.0f;
        private Vector3 m_real_scale;
        public void CreateCrossSectionPlane()
        {
            GameObject quad = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/CrossSectionPlane"));
            quad.name = "CSPlane" + m_cs_planes.Count;

            quad.transform.parent = transform;
            quad.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            quad.transform.localPosition = Vector3.zero;
            quad.transform.localScale = Vector3.one * 1.2f;

            m_cs_planes.Add(quad.transform);
            meshRenderer.sharedMaterial.EnableKeyword("CUTOUT_PLANE");
        }
        private void check_slicing_planes() {
            for (int i = m_sl_planes.Count - 1; i >= 0; i--)
            {
                if (m_sl_planes[i] == null) { m_sl_planes.Remove(m_sl_planes[i]); continue; }
            }
        }
        public void CreateSlicingPlane()
        {
            //check the list
            check_slicing_planes();

            GameObject sliceRenderingPlane = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/SlicingPlane"));
            sliceRenderingPlane.name = "SLPlane" + m_sl_planes.Count;

            sliceRenderingPlane.transform.parent = transform;
            sliceRenderingPlane.transform.localRotation = Quaternion.identity;
            sliceRenderingPlane.transform.localPosition = Vector3.zero;
            sliceRenderingPlane.transform.localScale = Vector3.one * 0.1f;

            MeshRenderer sliceMeshRend = sliceRenderingPlane.GetComponent<MeshRenderer>();
            sliceMeshRend.material = new Material(sliceMeshRend.sharedMaterial);
            Material sliceMat = sliceRenderingPlane.GetComponent<MeshRenderer>().sharedMaterial;
            sliceMat.SetTexture("_DataTex", dataset.GetDataTexture());
            sliceMat.SetTexture("_TFTex", transferFunction.GetTexture());

            var slicing_plane_rack = GameObject.Find("Slicing Planes");
            if (slicing_plane_rack == null)
            {
                slicing_plane_rack = DisplayRackFactory.CreateDisplayRack("Slicing Planes");
            }

            SlicingPlane sl_plane = sliceRenderingPlane.GetComponent<SlicingPlane>();
            sl_plane.CreatePolySlicingPlane(
                slicing_plane_rack.transform,
                dataset.GetDataTexture(), transferFunction.GetTexture(), 
                m_sl_planes.Count,
                m_sl_display_poses[m_sl_planes.Count % 5], m_sl_display_scale);
            //MENGHE: add drawing plane at the right place
            sl_plane.AddAnnotationPlane(Resources.Load<RenderTexture>("RenderTextures/DrawRT"));
            m_sl_planes.Add(sl_plane);
        }

        public void SetRenderMode(RenderMode mode)
        {
            if (renderMode != mode)
            {
                renderMode = mode;
                //SetVisibilityWindow(0.0f, 1.0f, 0.5f); // reset visibility window
                UpdateMaterialProperties();
            }
        }
        public void SetColorScheme(ColorTransferScheme color)
        {
            if (colorScheme == color) return;
            colorScheme = color;
            if (colorScheme == ColorTransferScheme.None)
                meshRenderer.sharedMaterial.DisableKeyword("TFCOLOR_SPECTRUM");
            else
            {
                meshRenderer.sharedMaterial.EnableKeyword("TFCOLOR_SPECTRUM");
                meshRenderer.sharedMaterial.SetInteger("_Color", ((int)colorScheme) - 1);
            }
        }
        
        public void SetOrganMaskEnabled(bool enabled)
        {
            if(helm_params.mask_enabled != enabled)
            {
                helm_params.mask_enabled = enabled;
                if (enabled) meshRenderer.sharedMaterial.EnableKeyword("SHOW_ORGAN");
                else meshRenderer.sharedMaterial.DisableKeyword("SHOW_ORGAN");
            }
        }

        public void SetOrganHighlightEnabled(bool enabled)
        {
            if (helm_params.mask_highlight != enabled)
            {
                helm_params.mask_highlight = enabled;
                if (enabled) meshRenderer.sharedMaterial.EnableKeyword("SHOW_ORGAN_RECOLOR");
                else meshRenderer.sharedMaterial.DisableKeyword("SHOW_ORGAN_RECOLOR");
            }
        }

        public void SetBodyEnabled(bool enabled)
        {
            if (helm_params.body_enabled != enabled)
            {
                helm_params.body_enabled = enabled;
                if (enabled) meshRenderer.sharedMaterial.EnableKeyword("SHOW_BODY");
                else meshRenderer.sharedMaterial.DisableKeyword("SHOW_BODY");
            }
        }
        public void SetContrastCutoffEnabled(bool cutoff_enabled) {
            if(cutoff_enabled != contrastCutoffEnabled)
            {
                contrastCutoffEnabled = cutoff_enabled;
                if (cutoff_enabled) meshRenderer.sharedMaterial.EnableKeyword("CONTRAST_CUTOFF");
                else meshRenderer.sharedMaterial.DisableKeyword("CONTRAST_CUTOFF");
            }
        }

        public void SetTransferFunctionMode(TFRenderMode mode)
        {
            tfRenderMode = mode;
            if (tfRenderMode == TFRenderMode.TF1D && transferFunction != null)
                transferFunction.GenerateTexture();
            else if (transferFunction2D != null)
                transferFunction2D.GenerateTexture();
            UpdateMaterialProperties();
        }
        public void SetOriginalScale(Vector3 scale) { 
            m_real_scale = scale;
            transform.localScale = m_real_scale * m_unified_scale;
        }
        public void SetVolumeUnifiedScale(float new_scale)
        {
            if(new_scale != m_unified_scale)
            {
                m_unified_scale = new_scale;
                transform.localScale = m_real_scale * new_scale;
            }
        }

        public float GetVolumeUnifiedScale()
        {
            return m_unified_scale;
        }
        public TFRenderMode GetTransferFunctionMode()
        {
            return tfRenderMode;
        }

        public RenderMode GetRenderMode()
        {
            return renderMode;
        }

        public ColorTransferScheme GetColorScheme() { return colorScheme; }
        public bool GetOrganMaskEnabled() { return helm_params.mask_enabled; }
        public bool GetOrganHighlightEnabled() { return helm_params.mask_highlight; }
        public bool GetBodyEnabled() { return helm_params.body_enabled; }
        public bool GetContrastCutoffEnabled() { return contrastCutoffEnabled; }
        public bool GetLightingEnabled()
        {
            return lightingEnabled;
        }

        public LightSource GetLightSource()
        {
            return lightSource;
        }

        public void SetLightingEnabled(bool enable)
        {
            if (enable != lightingEnabled)
            {
                lightingEnabled = enable;
                UpdateMaterialProperties();
            }
        }

        public void SetLightSource(LightSource source)
        {
            lightSource = source;
            UpdateMaterialProperties();
        }

        public void SetVisibilityWindow(float min, float max, float brightness)
        {
            SetVisibilityWindow(new Vector3(min, max, brightness));
        }

        public void SetVisibilityWindow(Vector3 window)
        {
            if(contrastCutoffEnabled && window != contrastCutOffWindow)
            {
                contrastCutOffWindow = window;
                meshRenderer.sharedMaterial.SetVector("_ContrastCutoff", contrastCutOffWindow);

            }
            else if(!contrastCutoffEnabled && window != contrastAdjustWindow)
            {
                contrastAdjustWindow = window;
                meshRenderer.sharedMaterial.SetVector("_ContrastAdjust", contrastAdjustWindow);

            }
        }

        public Vector3 GetVisibilityWindow()
        {
            return contrastCutoffEnabled ? contrastCutOffWindow : contrastAdjustWindow;
        }

        public bool GetRayTerminationEnabled()
        {
            return rayTerminationEnabled;
        }

        public void SetRayTerminationEnabled(bool enable)
        {
            if (enable != rayTerminationEnabled)
            {
                rayTerminationEnabled = enable;
                UpdateMaterialProperties();
            }
        }

        public bool GetDVRBackwardEnabled()
        {
            return dvrBackward;
        }

        public void SetDVRBackwardEnabled(bool enable)
        {
            if (enable != dvrBackward)
            {
                dvrBackward = enable;
                UpdateMaterialProperties();
            }
        }

        public void SetTransferFunction(TransferFunction tf)
        {
            this.transferFunction = tf;
            UpdateMaterialProperties();
        }

        private void UpdateMaterialProperties()
        {
            bool useGradientTexture = tfRenderMode == TFRenderMode.TF2D || renderMode == RenderMode.IsosurfaceRendering || lightingEnabled;
            meshRenderer.sharedMaterial.SetTexture("_GradientTex", useGradientTexture ? dataset.GetGradientTexture() : null);

            if (tfRenderMode == TFRenderMode.TF2D)
            {
                meshRenderer.sharedMaterial.SetTexture("_TFTex", transferFunction2D.GetTexture());
                meshRenderer.sharedMaterial.EnableKeyword("TF2D_ON");
            }
            else
            {
                meshRenderer.sharedMaterial.SetTexture("_TFTex", transferFunction.GetTexture());
                meshRenderer.sharedMaterial.DisableKeyword("TF2D_ON");
            }


            if (lightingEnabled)
                meshRenderer.sharedMaterial.EnableKeyword("LIGHTING_ON");
            else
                meshRenderer.sharedMaterial.DisableKeyword("LIGHTING_ON");

            if (lightSource == LightSource.SceneMainLight)
                meshRenderer.sharedMaterial.EnableKeyword("USE_MAIN_LIGHT");
            else
                meshRenderer.sharedMaterial.DisableKeyword("USE_MAIN_LIGHT");

            switch (renderMode)
            {
                case RenderMode.DirectVolumeRendering:
                    {
                        meshRenderer.sharedMaterial.EnableKeyword("MODE_DVR");
                        meshRenderer.sharedMaterial.DisableKeyword("MODE_MIP");
                        meshRenderer.sharedMaterial.DisableKeyword("MODE_SURF");
                        break;
                    }
                case RenderMode.MaximumIntensityProjectipon:
                    {
                        meshRenderer.sharedMaterial.DisableKeyword("MODE_DVR");
                        meshRenderer.sharedMaterial.EnableKeyword("MODE_MIP");
                        meshRenderer.sharedMaterial.DisableKeyword("MODE_SURF");
                        break;
                    }
                case RenderMode.IsosurfaceRendering:
                    {
                        meshRenderer.sharedMaterial.DisableKeyword("MODE_DVR");
                        meshRenderer.sharedMaterial.DisableKeyword("MODE_MIP");
                        meshRenderer.sharedMaterial.EnableKeyword("MODE_SURF");
                        break;
                    }
            }
            if (contrastCutoffEnabled) meshRenderer.sharedMaterial.EnableKeyword("CONTRAST_CUTOFF");
            else meshRenderer.sharedMaterial.DisableKeyword("CONTRAST_CUTOFF");
            //meshRenderer.sharedMaterial.SetVector("_Contrast", contrastWindow);
            //meshRenderer.sharedMaterial.SetInteger("_u_maskbits", helm_params.mask_bit);

            if (rayTerminationEnabled) meshRenderer.sharedMaterial.EnableKeyword("RAY_TERMINATE_ON");
            else meshRenderer.sharedMaterial.DisableKeyword("RAY_TERMINATE_ON");

            if (dvrBackward) meshRenderer.sharedMaterial.EnableKeyword("DVR_BACKWARD_ON");
            else meshRenderer.sharedMaterial.DisableKeyword("DVR_BACKWARD_ON");

            helm_params.ResetProperties(meshRenderer.sharedMaterial);
        }

        private void Start()
        {
            UpdateMaterialProperties();
        }

        private void Update()
        {
            //MENGHE: DO NOT CHECK EVERYTIME
            bool need_update = false;
            for(int i=m_cs_planes.Count-1; i>=0; i--) { 
                if (m_cs_planes[i] == null) { m_cs_planes.Remove(m_cs_planes[i]); continue; }
                if (m_cs_planes[i].hasChanged) { need_update = true;}
            }
            if (m_cs_planes.Count == 0) { meshRenderer.sharedMaterial.DisableKeyword("CUTOUT_PLANE"); need_update = false; }
            if (need_update)
            {
                //TODO: HIDE THE GAME OBJS?
                int plane_count = Mathf.Min(m_cs_planes.Count, MAX_CS_PLANE_NUM);
                for (int i = 0; i < plane_count; i++)
                    m_cs_plane_matrices[i] = m_cs_planes[i].worldToLocalMatrix * transform.localToWorldMatrix;

                meshRenderer.sharedMaterial.EnableKeyword("CUTOUT_PLANE");
                meshRenderer.sharedMaterial.SetInteger("_CrossSectionNum", plane_count);
                meshRenderer.sharedMaterial.SetMatrixArray("_CrossSectionMatrices", m_cs_plane_matrices);
            }
        }
    }
}
