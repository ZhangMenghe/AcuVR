//using UnityEngine;

//namespace UnityVolumeRendering
//{
//    /// <summary>
//    /// Cross section plane.
//    /// Used for cutting a model (cross section view).
//    /// </summary>
//    [ExecuteInEditMode]
//    public class CrossSectionPlane : MonoBehaviour
//    {
//        /// <summary>
//        /// Volume dataset to cross section.
//        /// </summary>
//        public VolumeRenderedObject targetObject;

//        private void Start()
//        {
//            if (targetObject != null)
//                targetObject.meshRenderer.sharedMaterial.EnableKeyword("CUTOUT_PLANE");
//        }
//        private void OnEnable()
//        {
//            if (targetObject != null)
//                targetObject.meshRenderer.sharedMaterial.EnableKeyword("CUTOUT_PLANE");
//        }
//        private void OnDisable()
//        {
//            if (targetObject != null)
//                targetObject.meshRenderer.sharedMaterial.DisableKeyword("CUTOUT_PLANE");
//        }
//        private void Update()
//        {
//            if (targetObject == null || !transform.hasChanged)
//                return;
//            Debug.LogError(this.gameObject.name + "_update");
//            Material mat = targetObject.meshRenderer.sharedMaterial;
//            mat.SetMatrix("_CrossSectionMatrix", transform.worldToLocalMatrix * targetObject.transform.localToWorldMatrix);
//        }
//    }
//}
