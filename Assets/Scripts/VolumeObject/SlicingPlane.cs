using UnityEngine;

namespace UnityVolumeRendering
{
    [ExecuteInEditMode]
    public class SlicingPlane : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        private MeshRenderer canvasRenderer = null;
        private Material displayCanvasMaterial;

        public void CreatePolySlicingPlane(Transform parent_transform, 
            Texture data_tex, Texture trans_tex, int cid, 
            Vector3 local_position, Vector3 local_scale)
        {
            GameObject PolySlicingPlane = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/SingleCanvas"));
            PolySlicingPlane.name = "canvas" + cid;
            PolySlicingPlane.transform.parent = parent_transform;
            PolySlicingPlane.transform.localPosition = local_position;
            PolySlicingPlane.transform.localRotation = Quaternion.identity;
            PolySlicingPlane.transform.localScale = local_scale;

            canvasRenderer = PolySlicingPlane.GetComponent<MeshRenderer>();

            displayCanvasMaterial = new Material(canvasRenderer.sharedMaterial);
            displayCanvasMaterial.SetTexture("_DataTex", data_tex);
            displayCanvasMaterial.SetTexture("_TFTex", trans_tex);
        }

        public void AddAnnotationPlane(RenderTexture render_tex)
        {
            displayCanvasMaterial.SetTexture("_DrawTex", render_tex);
            displayCanvasMaterial.EnableKeyword("ANNOTATION_ON");
        }

        public void RemoveChildren()
        {
            //if(m_display_canvas != null) GameObject.DestroyImmediate(m_display_canvas.gameObject);
        }
        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            meshRenderer.sharedMaterial.SetMatrix("_parentInverseMat", 
                transform.parent ? transform.parent.worldToLocalMatrix : transform.worldToLocalMatrix);

            Matrix4x4 plane_mat = Matrix4x4.TRS(
                transform.position,
                transform.rotation,
                transform.parent ? transform.parent.lossyScale : transform.lossyScale);
            meshRenderer.sharedMaterial.EnableKeyword("OVERRIDE_MODEL_MAT");
            meshRenderer.sharedMaterial.SetMatrix("_planeMat", plane_mat);
            meshRenderer.sharedMaterial.SetMatrix("_planeModelMat", plane_mat*Matrix4x4.Scale(transform.localScale));

            if (canvasRenderer != null)
            {
                displayCanvasMaterial.DisableKeyword("OVERRIDE_MODEL_MAT");
                displayCanvasMaterial.SetMatrix("_parentInverseMat", transform.parent.worldToLocalMatrix);
                displayCanvasMaterial.SetMatrix("_planeMat", plane_mat);
                canvasRenderer.sharedMaterial = displayCanvasMaterial;
            }
        }
    }
}
