using UnityEngine;

[ExecuteInEditMode]
public class MeshBoxGizmo : MonoBehaviour
{
    public struct MyBounds
    {
        public Vector3 max;
        public Vector3 min;

        public void ApplyTransform(Transform t)
        {
            min = t.TransformPoint(min);
            max = t.TransformPoint(max);
        }
    }

    private void OnDrawGizmos()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.sharedMesh;
            if (mesh != null)
            {
                Gizmos.color = Color.green; // Color del Gizmo
                //Bounds b = mesh.bounds;
                //b.max = transform.TransformPoint(b.max);
                //b.min = transform.TransformPoint(b.min);

                MyBounds bounds = new MyBounds() { max = mesh.bounds.max, min = mesh.bounds.min };
                foreach (Vector3 vertex in mesh.vertices)
                {
                    //Vector3 vertex = transform.TransformPoint(v);
                    //vertex = transform.rotation * vertex;
                    Vector3 newMin = bounds.min;

                    if (bounds.min.x >= vertex.x)
                        newMin.x = vertex.x;
                    if (bounds.min.y >= vertex.y)
                        newMin.y = vertex.y;
                    if (bounds.min.z >= vertex.z)
                        newMin.z = vertex.z;
                    bounds.min = newMin;

                    Vector3 newMax = bounds.max;

                    if (bounds.max.x <= vertex.x)
                        newMax.x = vertex.x;
                    if (bounds.min.y <= vertex.y)
                        newMax.y = vertex.y;
                    if (bounds.min.z <= vertex.z)
                        newMax.z = vertex.z;
                    bounds.max = newMax;

                }
                bounds.ApplyTransform(transform);
                //Gizmos.DrawWireCube(transform.position, new Vector3(Mathf.Abs(bounds.min.x - bounds.max.x), Mathf.Abs(bounds.min.y - bounds.max.y), Mathf.Abs(bounds.min.z - bounds.max.z)));

                //Gizmos.DrawLine(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z), new Vector3(bounds.max.x, bounds.max.y, bounds.max.z));
                Vector3 aabbMin = bounds.min;
                Vector3 aabbMax = bounds.max;
                Gizmos.DrawLine(new Vector3(aabbMin.x, aabbMin.y, aabbMin.z), new Vector3(aabbMax.x, aabbMin.y, aabbMin.z));
                Gizmos.DrawLine(new Vector3(aabbMax.x, aabbMin.y, aabbMin.z), new Vector3(aabbMax.x, aabbMax.y, aabbMin.z));
                Gizmos.DrawLine(new Vector3(aabbMax.x, aabbMax.y, aabbMin.z), new Vector3(aabbMin.x, aabbMax.y, aabbMin.z));
                Gizmos.DrawLine(new Vector3(aabbMin.x, aabbMax.y, aabbMin.z), new Vector3(aabbMin.x, aabbMin.y, aabbMin.z));

                Gizmos.DrawLine(new Vector3(aabbMin.x, aabbMin.y, aabbMax.z), new Vector3(aabbMax.x, aabbMin.y, aabbMax.z));
                Gizmos.DrawLine(new Vector3(aabbMax.x, aabbMin.y, aabbMax.z), new Vector3(aabbMax.x, aabbMax.y, aabbMax.z));
                Gizmos.DrawLine(new Vector3(aabbMax.x, aabbMax.y, aabbMax.z), new Vector3(aabbMin.x, aabbMax.y, aabbMax.z));
                Gizmos.DrawLine(new Vector3(aabbMin.x, aabbMax.y, aabbMax.z), new Vector3(aabbMin.x, aabbMin.y, aabbMax.z));

                Gizmos.DrawLine(new Vector3(aabbMin.x, aabbMin.y, aabbMin.z), new Vector3(aabbMin.x, aabbMin.y, aabbMax.z));
                Gizmos.DrawLine(new Vector3(aabbMax.x, aabbMin.y, aabbMin.z), new Vector3(aabbMax.x, aabbMin.y, aabbMax.z));
                Gizmos.DrawLine(new Vector3(aabbMax.x, aabbMax.y, aabbMin.z), new Vector3(aabbMax.x, aabbMax.y, aabbMax.z));
                Gizmos.DrawLine(new Vector3(aabbMin.x, aabbMax.y, aabbMin.z), new Vector3(aabbMin.x, aabbMax.y, aabbMax.z));


                //Vector3[] corners = new Vector3[8];
                //  corners[0] = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);
                //  corners[1] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
                //  corners[2] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
                //  corners[3] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
                //  corners[4] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
                //  corners[5] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
                //  corners[6] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
                //  corners[7] = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
                //
                //  Gizmos.DrawLine(corners[0], corners[1]);
                //  Gizmos.DrawLine(corners[0], corners[2]);
                //  Gizmos.DrawLine(corners[0], corners[4]);
                //  Gizmos.DrawLine(corners[1], corners[3]);
                //  Gizmos.DrawLine(corners[1], corners[5]);
                //  Gizmos.DrawLine(corners[2], corners[3]);
                //  Gizmos.DrawLine(corners[2], corners[6]);
                //  Gizmos.DrawLine(corners[3], corners[7]);
                //  Gizmos.DrawLine(corners[4], corners[5]);
                //  Gizmos.DrawLine(corners[4], corners[6]);
                //  Gizmos.DrawLine(corners[5], corners[7]);
                //  Gizmos.DrawLine(corners[6], corners[7]);


            }
        }
    }
}
