using UnityEngine;

namespace UnityVolumeRendering
{
    [ExecuteInEditMode]
    public class SlicingPlane : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        public Texture mDataTex;
        public Texture mTFTex;

        public Transform mParentTransform = null;
        public MeshRenderer mPlaneBoundaryRenderer = null;

        //public void AddAnnotationPlane(RenderTexture render_tex)
        //{
        //    displayCanvasMaterial.SetTexture("_DrawTex", render_tex);
        //    displayCanvasMaterial.EnableKeyword("ANNOTATION_ON");
        //}

        public void Initialized(string name, in Texture data_tex, in Texture tf_tex)
        {
            if(name.Length > 0)
                gameObject.name = name;

            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one * 0.1f;

            mDataTex = data_tex; mTFTex = tf_tex;

            meshRenderer = transform.GetComponent<MeshRenderer>();
            //sliceMeshRend.material = new Material(sliceMeshRend.sharedMaterial);

            //Material sliceMat = transform.GetComponent<MeshRenderer>().sharedMaterial;
            meshRenderer.material.SetTexture("_DataTex", data_tex);
            meshRenderer.material.SetTexture("_TFTex", tf_tex);

            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            meshRenderer.material.SetMatrix("_parentInverseMat",
                mParentTransform ? mParentTransform.worldToLocalMatrix : transform.worldToLocalMatrix);

            Matrix4x4 plane_mat = Matrix4x4.TRS(
                transform.position,
                transform.rotation,
                mParentTransform ? mParentTransform.lossyScale : transform.lossyScale);
            meshRenderer.material.EnableKeyword("OVERRIDE_MODEL_MAT");
            meshRenderer.material.SetMatrix("_planeMat", plane_mat);
            meshRenderer.material.SetMatrix("_planeModelMat", plane_mat*Matrix4x4.Scale(transform.localScale));
        }
    }
}
