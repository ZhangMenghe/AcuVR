using UnityEngine;

public class ProjectedNeedle : MonoBehaviour
{
    public ComputeShader CS;
    public Transform SlicingPlane;

    [HideInInspector]
    public RenderTexture mRT;

    //private Mesh mesh;
    private Vector3 mMeshMinBound;
    private Vector3 mMeshSizeInv;

    public void Initialize()
    {
        mRT = new RenderTexture(512, 512, 0);
        mRT.enableRandomWrite = true;
        mRT.Create();
        CS.SetTexture(0, "surface", mRT);
        GetComponent<Renderer>().material.mainTexture = mRT;

        var mesh_bound = GetComponent<MeshFilter>().mesh.bounds;
        mMeshMinBound = mesh_bound.center - mesh_bound.extents;
        var mesh_size = mesh_bound.extents * 2.0f;
        mMeshSizeInv = new Vector3(
            1.0f/ mesh_size.x,
            .0f,
            1.0f / mesh_size.z);

    }
    public void OnUpdate(Vector3 startPt, Vector3 endPt)
    {
        //project line on plane

        Plane plane = new Plane(SlicingPlane.up, SlicingPlane.position);

        Vector3 projectedStart = Vector3.ProjectOnPlane(startPt - plane.ClosestPointOnPlane(startPt), SlicingPlane.up) + plane.ClosestPointOnPlane(startPt);
        Vector3 projectedEnd = Vector3.ProjectOnPlane(endPt - plane.ClosestPointOnPlane(endPt), SlicingPlane.up) + plane.ClosestPointOnPlane(endPt);

        // Transform the projected points to local space
        Vector3 localStart = SlicingPlane.InverseTransformPoint(projectedStart);
        Vector3 localEnd = SlicingPlane.InverseTransformPoint(projectedEnd);

        var nv_start = (localStart - mMeshMinBound) * mMeshSizeInv.x;
        var nv_end = (localEnd - mMeshMinBound) * mMeshSizeInv.z;

        CS.SetVector("startTexcoord", new Vector2(1.0f - nv_start.x, 1.0f - nv_start.z));
        CS.SetVector("endTexcoord", new Vector2(1.0f - nv_end.x, 1.0f - nv_end.z));

        CS.Dispatch(0, mRT.width / 8, mRT.height / 8, 1);
    }
}
