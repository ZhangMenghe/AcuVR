//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using System.Linq;
//namespace UnityVolumeRendering
//{
//    [ExecuteInEditMode]
//    public class PolySlicingPlane : MonoBehaviour
//    {
//        public Vector3 ForwardAxis;
//        public Vector3 VolumeScale;

//        private MeshRenderer meshRenderer;
//        private Mesh mesh;
//        readonly int[] m_single_indices_data = new int[12]{
//            0,1,2,0,2,3,0,3,4,0,4,5
//        };
//        Vector3 old_pp = new Vector3(-1000.0f, -1000.0f, -1000.0f);
//        Quaternion old_rot = Quaternion.identity;


//        public void Initialize()
//        {
//            gameObject.AddComponent<MeshFilter>();
//            gameObject.AddComponent<MeshRenderer>();
//            meshRenderer = GetComponent<MeshRenderer>();
//            mesh = new Mesh(); 
//            GetComponent<MeshFilter>().sharedMesh = mesh;

//            Material mat = Resources.Load<Material>("PolySliceMaterial");
//            meshRenderer.sharedMaterial = mat;
//            //drawTriangles();

//            //UpdateVertices(Vector3.zero, Vector3.forward);
//        }


//        private void Update()
//        {
//            //meshRenderer.sharedMaterial.SetMatrix("_parentInverseMat", transform.parent.worldToLocalMatrix);
//            //meshRenderer.sharedMaterial.SetMatrix("_planeMat", Matrix4x4.TRS(transform.position, transform.rotation, transform.parent.lossyScale));

//        }
//        public void UpdateTransform(Vector3 position, Quaternion rotation)
//        {
//            if(!position.Equals(old_pp) || !rotation.Equals(old_rot)){
//                UpdateVertices(position, rotation * new Vector3(.0f, 1.0f, .0f));
//                old_pp = position; old_rot = rotation;
//            }
//        }
//        float RayPlane(Vector3 ro, Vector3 rd, Vector3 pp, Vector3 pn)
//        {
//            float d = Vector3.Dot(pn, rd);
//            float t = Vector3.Dot(pp - ro, pn);
//            return Mathf.Abs(d) > 1e-5 ? (float)(t / d) : (float)(t > .0 ? 1e5 : -1e5);
//        }

//        void PlaneBox(Vector3 aabb_min, Vector3 aabb_max, Vector3 pp, Vector3 pn, ref List<Vector3> out_points)
//        {
//            float t;
//            Vector3 rd;
//            //axis-x
//            rd = new Vector3(aabb_max.x - aabb_min.x, .0f, .0f);
//            Vector3[] ro_x = new Vector3[4]{
//                    new Vector3(aabb_min.x, aabb_min.y, aabb_min.z),
//                    new Vector3(aabb_min.x, aabb_max.y, aabb_min.z),
//                    new Vector3(aabb_min.x, aabb_min.y, aabb_max.z),
//                    new Vector3(aabb_min.x, aabb_max.y, aabb_max.z)
//            };

//            foreach (Vector3 ro in ro_x)
//            {
//                t = RayPlane(ro, rd, pp, pn);
//                if (t >= .0 && t < 1.0f) out_points.Add(ro + rd * t);
//            }

//            //axis-y
//            rd = new Vector3(.0f, aabb_max.y - aabb_min.y, .0f);
//            Vector3[] ro_y = new Vector3[4]{
//                    new Vector3(aabb_min.x, aabb_min.y, aabb_min.z),
//                    new Vector3(aabb_max.x, aabb_min.y, aabb_min.z),
//                    new Vector3(aabb_min.x, aabb_min.y, aabb_max.z),
//                    new Vector3(aabb_max.x, aabb_min.y, aabb_max.z)
//            };
//            foreach (Vector3 ro in ro_y)
//            {
//                t = RayPlane(ro, rd, pp, pn);
//                if (t >= .0 && t < 1.0f) out_points.Add(ro + rd * t);
//            }

//            //axis-z
//            rd = new Vector3(.0f, .0f, aabb_max.z - aabb_min.z);
//            Vector3[] ro_z = new Vector3[4]{
//                    new Vector3(aabb_min.x, aabb_min.y, aabb_min.z),
//                    new Vector3(aabb_max.x, aabb_min.y, aabb_min.z),
//                    new Vector3(aabb_min.x, aabb_max.y, aabb_min.z),
//                    new Vector3(aabb_max.x, aabb_max.y, aabb_min.z)
//            };
//            foreach (Vector3 ro in ro_z)
//            {
//                t = RayPlane(ro, rd, pp, pn);
//                if (t >= .0 && t < 1.0f) out_points.Add(ro + rd * t);
//            }
//        }
//        void SortPoints(ref List<Vector3> points, Vector3 pn)
//        {
//            if (points.Count == 0) return;
//            var origin = points[0];
//            points.Sort((lhs, rhs) => 
//                Vector3.Dot(Vector3.Cross(Vector3.Normalize(lhs - origin), Vector3.Normalize(rhs - origin)), pn).CompareTo(.0f)
//            );
//        }

//        private bool UpdateVertices(Vector3 pp, Vector3 pn)
//        {
//            mesh.Clear();
//            pn.z = Mathf.Abs(pn.z);


//            Vector3 aabb_min= new Vector3(-0.5f, -0.5f, -0.5f), aabb_max = new Vector3(0.5f, 0.5f, 0.5f);

//            List<Vector3> polygon_points = new List<Vector3>();
//            PlaneBox(aabb_min, aabb_max, pp, pn, ref polygon_points);

//            if (polygon_points.Count < 3) return false;
            
//            SortPoints(ref polygon_points, pn);
//            mesh.vertices = polygon_points.ToArray();
//            int[] trianglesArray = new int[3 * (polygon_points.Count - 2)];
//            Array.Copy(m_single_indices_data, 0, trianglesArray, 0, trianglesArray.Length);
//            mesh.triangles = trianglesArray;

//            //update transform
//            //TODO:quad rotation?
//            transform.rotation = Quaternion.FromToRotation(pn, ForwardAxis);
//            List<Vector3> rot_poly = new List<Vector3>();
//            foreach (Vector3 p in polygon_points)
//                rot_poly.Add(transform.rotation* p);
//            transform.position = -rot_poly.Aggregate(new Vector3(0, 0, 0), (s, v) => s + v) / (float)rot_poly.Count;
//            transform.position+= transform.parent.transform.position;
//            transform.localScale = VolumeScale;
//            //transform.rotation *= Quaternion.AngleAxis(-45.0f, Vect.up);

//            return true;
//        }
//        private void drawTriangles()
//        {
//            //We need two arrays one to hold the vertices and one to hold the triangles
//            Vector3[] VerteicesArray = new Vector3[3];
//            int[] trianglesArray = new int[3];

//            //lets add 3 vertices in the 3d space
//            VerteicesArray[0] = new Vector3(0, 0, 0);
//            VerteicesArray[1] = new Vector3(0, 1, 0);
//            VerteicesArray[2] = new Vector3(1, 1, 0);

//            //define the order in which the vertices in the VerteicesArray shoudl be used to draw the triangle
//            trianglesArray[0] = 0;
//            trianglesArray[1] = 1;
//            trianglesArray[2] = 2;

//            //add these two triangles to the mesh
//            mesh.vertices = VerteicesArray;
//            mesh.triangles = trianglesArray;
//            mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
//        }
//    }
//}
