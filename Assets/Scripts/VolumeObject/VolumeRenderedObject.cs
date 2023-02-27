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

        //TODO:
        public static readonly int MAX_CS_PLANE_NUM = 5;
        private Matrix4x4[] m_cs_plane_matrices = new Matrix4x4[MAX_CS_PLANE_NUM];

        private float m_unified_scale = 1.0f;
        private Vector3 m_real_scale;

        public List<Transform> m_cs_planes { get; set; } = new List<Transform>();
        public List<Transform> SlicingPlaneList { get; set; } = new List<Transform>();

        public static bool isSnapAble = true;

        public void CreateCrossSectionPlane()
        {
            Transform cross_plane;
            if (isSnapAble)
            {
                cross_plane = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/AAASnapCrossSectionPlane")).transform;

                cross_plane.parent = transform;
                cross_plane.localRotation = Quaternion.identity;
                cross_plane.localPosition = Vector3.zero;
                cross_plane.localScale = Vector3.one;

                //cross_plane.rotation = transform.rotation;
                //cross_plane.position = transform.position;
                //cross_plane.localScale = transform.localScale;

                Transform csplane_mesh = cross_plane.Find("CSPlaneObj").Find("CSPlaneMesh");
                csplane_mesh.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                csplane_mesh.localPosition = Vector3.zero;
                csplane_mesh.localScale = Vector3.one * 1.2f;
                m_cs_planes.Add(csplane_mesh);
            }
            else
            {
                cross_plane = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/CrossSectionPlane")).transform;
                cross_plane.parent = transform;

                m_cs_planes.Add(cross_plane);

                cross_plane.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                cross_plane.localPosition = Vector3.zero;
                cross_plane.localScale = Vector3.one * 1.2f;
            }
            //GameObject quad = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/CrossSectionPlane"));
            cross_plane.name = "CSPlane" + m_cs_planes.Count;
            meshRenderer.sharedMaterial.EnableKeyword("CUTOUT_PLANE");
        }
        public void DeleteCrossSectionPlaneAt(int TargetId)
        {
            if (TargetId < m_cs_planes.Count)
            {
                Destroy(isSnapAble? m_cs_planes[TargetId].parent.parent.gameObject:
                    m_cs_planes[TargetId].gameObject);
                m_cs_planes.RemoveAt(TargetId);
            }
        }

        private void check_slicing_planes() {
            for (int i = SlicingPlaneList.Count - 1; i >= 0; i--)
            {
                if (SlicingPlaneList[i] == null) { SlicingPlaneList.Remove(SlicingPlaneList[i]); continue; }
            }
            //MENGHE: JUST FOR DEBUG
            //if(SlicingPlaneList.Count == 0)
            //    DisplayRackFactory.DeAttachFromRack(DisplayRackFactory.DisplayRacks.ROOM_LEFT_BOARD);

        }
        public void CreateSlicingPlane()
        {
            //check the list
            if(!isSnapAble)check_slicing_planes();

            Transform slicePlaneObj;
            SlicingPlane slicing_plane;
            if (isSnapAble)
            {
                slicePlaneObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/AAASnapSlicingPlaneBounded")).transform;

                slicePlaneObj.name = "SLPlane" + SlicingPlaneList.Count;
                slicePlaneObj.parent = transform;

                //AAASnapSlicingPlane
                slicePlaneObj.localRotation = Quaternion.identity;
                slicePlaneObj.localPosition = Vector3.zero;
                slicePlaneObj.localScale = Vector3.one;

                slicing_plane = slicePlaneObj.Find("SlicingPlaneObj").Find("SlicingPlaneMesh").gameObject.AddComponent<SlicingPlane>();
                slicing_plane.mPlaneBoundaryRenderer = slicing_plane.transform.Find("CollidingBBox").GetComponent<MeshRenderer>();
            }
            else
            {
                slicePlaneObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/SlicingPlane")).transform;
                slicePlaneObj.parent = transform;
                slicing_plane = slicePlaneObj.gameObject.AddComponent<SlicingPlane>();
            }
            slicing_plane.mParentTransform = transform;

            slicing_plane.Initialized(
                isSnapAble?"":"SLPlane" + SlicingPlaneList.Count,
                dataset.GetDataTexture(), 
                transferFunction.GetTexture()
                );
            SlicingPlaneList.Add(slicing_plane.transform);
            DisplayRackFactory.AddFrame(DisplayRackFactory.DisplayRacks.ROOM_LEFT_BOARD, slicing_plane);
            ////MENGHE: add drawing plane at the right place
            ////slicePlane.AddAnnotationPlane(Resources.Load<RenderTexture>("RenderTextures/DrawRT"));
        }

        public void DeleteSlicingPlaneAt(int TargetId)
        {
            if (TargetId < SlicingPlaneList.Count)
            {
                Destroy(isSnapAble ? SlicingPlaneList[TargetId].parent.parent.gameObject :
    SlicingPlaneList[TargetId].gameObject);

                SlicingPlaneList.RemoveAt(TargetId);
            }
            //MENGHE: REMOVE THE ONE ON SHLF
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
            if (m_real_scale.x == 0f && m_real_scale.y == 0f && m_real_scale.z == 0f) m_real_scale = transform.localScale;

            if (new_scale != m_unified_scale)
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
            //MENGHE! ONLY FOR DEBUG!!!!
            DisplayRackFactory.RenderFrames();
            //MENGHE: DO NOT CHECK EVERYTIME
            bool need_update = false;
            for(int i=m_cs_planes.Count-1; i>=0; i--) { 
                if (m_cs_planes[i] == null) { m_cs_planes.Remove(m_cs_planes[i]); continue; }
                if (m_cs_planes[i].hasChanged) { need_update = true;}
            }
            if (m_cs_planes.Count == 0) { meshRenderer.sharedMaterial.DisableKeyword("CUTOUT_PLANE"); need_update = false; }
            if (need_update)
            {
                int plane_count = Mathf.Min(m_cs_planes.Count, MAX_CS_PLANE_NUM);
                int active_count = plane_count;
                for (int i = 0; i < plane_count; i++)
                {
                    if (!m_cs_planes[i].gameObject.activeSelf){
                        active_count--; continue; 
                    }

                    m_cs_plane_matrices[i] = m_cs_planes[i].worldToLocalMatrix * transform.localToWorldMatrix;
                }
                
                meshRenderer.sharedMaterial.EnableKeyword("CUTOUT_PLANE");
                meshRenderer.sharedMaterial.SetInteger("_CrossSectionNum", active_count);
                meshRenderer.sharedMaterial.SetMatrixArray("_CrossSectionMatrices", m_cs_plane_matrices);
            }
        }
    }
}
