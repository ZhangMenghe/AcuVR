using UnityEngine;

namespace UnityVolumeRendering
{
    [ExecuteInEditMode]
    public class SlicingPlane : MonoBehaviour
    {
        private MeshRenderer meshRenderer;
        public Texture mDataTex;
        public Texture mTFTex;

        //public void AddAnnotationPlane(RenderTexture render_tex)
        //{
        //    displayCanvasMaterial.SetTexture("_DrawTex", render_tex);
        //    displayCanvasMaterial.EnableKeyword("ANNOTATION_ON");
        //}

        public void Initialized(string name, in Texture data_tex, in Texture tf_tex)
        {
            gameObject.name = name;

            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one * 0.1f;

            mDataTex = data_tex; mTFTex = tf_tex;

            MeshRenderer sliceMeshRend = transform.GetComponent<MeshRenderer>();
            sliceMeshRend.material = new Material(sliceMeshRend.sharedMaterial);
            
            Material sliceMat = transform.GetComponent<MeshRenderer>().sharedMaterial;
            sliceMat.SetTexture("_DataTex", data_tex);
            sliceMat.SetTexture("_TFTex", tf_tex);

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
        }
    }
}
