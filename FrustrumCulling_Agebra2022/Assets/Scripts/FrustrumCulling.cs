using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrustrumCulling : MonoBehaviour
{
    // Maxima cantidad de vertices por plano, habiendo 6 planos
    const uint maxVertexPerPlane = 4;

    // Variables modificables en el editor
    [SerializeField] Color frustumLineColor = Color.green; // Color de lineas
    [SerializeField] Color frustumPlaneColor = Color.green; // Color de los planos

    // Creacion de los planos, 4 Vector3 = 4 vertices o puntos en el espacio por plano 
    Vector3[] frustumCornerFar = new Vector3[maxVertexPerPlane];
    Vector3[] frustumCornerNear = new Vector3[maxVertexPerPlane];
    Vector3[] frustumCornerLeft = new Vector3[maxVertexPerPlane];
    Vector3[] frustumCornerRight = new Vector3[maxVertexPerPlane];
    Vector3[] frustumCornerUp = new Vector3[maxVertexPerPlane];
    Vector3[] frustumCornerDown = new Vector3[maxVertexPerPlane];

    Camera camera;

    // Buscador de meshs (mallas) -> toma un mesh y los pasa al Mesh Renderer para que sea renderizado en la pantalla.
    MeshFilter[] filters = default;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main; // Camara principal
        filters = FindObjectsOfType<MeshFilter>();
        CalculateFrustum();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateFrustum();
        FrustumUpdate();
    }

    // Calcular el frustum
    private void CalculateFrustum()
    {
        // https://docs.unity3d.com/ScriptReference/Camera.CalculateFrustumCorners.html
        // Dadas coordenadas del viewport, se calculan los vectores que nacen del frustum hacia las cuatro esquinas
        //                                                                       Un "ojo" que indica con cual      
        //                             viewport              float z             ojo humano se renderiza o ninguno   Vector3 de esquinas
        camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornerFar);
        camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCornerNear);

        // Cada indice equivale a un punto en el espacio, entonces cada 4 vertices un plano
        // Se calcula los vertices del plano izquierdo del frustum
        frustumCornerLeft[0] = frustumCornerNear[1];
        frustumCornerLeft[1] = frustumCornerFar[1];
        frustumCornerLeft[2] = frustumCornerFar[0];
        frustumCornerLeft[3] = frustumCornerNear[0];

        // Se calcula los vertices del plano derecho del frustum
        frustumCornerRight[0] = frustumCornerNear[3];
        frustumCornerRight[1] = frustumCornerFar[3];
        frustumCornerRight[2] = frustumCornerFar[2];
        frustumCornerRight[3] = frustumCornerNear[2];

        // Se calcula los vertices del plano superior del frustum
        frustumCornerUp[0] = frustumCornerNear[2];
        frustumCornerUp[1] = frustumCornerFar[2];
        frustumCornerUp[2] = frustumCornerFar[1];
        frustumCornerUp[3] = frustumCornerNear[1];

        // Se calcula los vertices del plano inferior del frustum
        frustumCornerDown[0] = frustumCornerNear[0];
        frustumCornerDown[1] = frustumCornerFar[0];
        frustumCornerDown[2] = frustumCornerFar[3];
        frustumCornerDown[3] = frustumCornerNear[3];

        // Se recorre 4 veces en total, es decir por cada vertice en los planos
        for (int i = 0; i < maxVertexPerPlane; i++)
        {
            frustumCornerFar[i] = FromLocalToWolrd(frustumCornerFar[i], camera.transform);
            frustumCornerNear[i] = FromLocalToWolrd(frustumCornerNear[i], camera.transform);
            frustumCornerLeft[i] = FromLocalToWolrd(frustumCornerLeft[i], camera.transform);
            frustumCornerRight[i] = FromLocalToWolrd(frustumCornerRight[i], camera.transform);
            frustumCornerUp[i] = FromLocalToWolrd(frustumCornerUp[i], camera.transform);
            frustumCornerDown[i] = FromLocalToWolrd(frustumCornerDown[i], camera.transform);
        }
    }

    // Transformo las posiciones locales en globales
    private Vector3 FromLocalToWolrd(Vector3 point, Transform transformRef)
    {
        // Se multiplica cada valor en x, y, z por la escala local del tranform referenciado
        Vector3 result = new Vector3(point.x * transformRef.localScale.x, point.y * transformRef.localScale.y, point.z * transformRef.localScale.z);

        // Tambien se tiene en cuenta la rotacion
        result = transformRef.localRotation * result;

        return result + transformRef.position; // Se devuelve el resultado mas la posicion del transform
    }

    // Uso todos los datos de las mesh que estan dentro de la escena para hacer los respectivos calculos (prendo o apago el game object)
    private void FrustumUpdate()
    {
        // Por cada "item" (de tipo MeshFilter) en los filtros instanciados previamente
        foreach (var item in filters)
        {
            // Para calcular las normales necesito el indice de grupo de vertices, para saber cuales forman una cara
            // Siendo una normal el vector (recta) que es perpendicular al plano
            for (int i = 0; i < item.mesh.GetIndices(0).Length; i += 3) // Salto de a 3 vertices para mantener el orden
            {
                // Tomo los vertices ordenados proporcionados por unity

                Vector3 v1 = item.mesh.vertices[item.mesh.GetIndices(0)[i]];
                Vector3 v2 = item.mesh.vertices[item.mesh.GetIndices(0)[i + 1]];
                Vector3 v3 = item.mesh.vertices[item.mesh.GetIndices(0)[i + 2]];

                // Paso las coordenadas locales a globales...
                v1 = FromLocalToWolrd(v1, item.transform);
                v2 = FromLocalToWolrd(v2, item.transform);
                v3 = FromLocalToWolrd(v3, item.transform);

                if (IsVertexInFrustum(v1) || IsVertexInFrustum(v2) || IsVertexInFrustum(v3))
                {
                    if (!item.gameObject.activeSelf)
                    {
                        item.gameObject.SetActive(true);
                    }
                }
                else
                {
                    item.gameObject.SetActive(false); // "Apago el objeto"
                }

            }

        }
    }

    // Verifico si un vertice esta contenido en el frustum
    private bool IsVertexInFrustum(Vector3 vertex)
    {
        //                          vertice  centro de un plano dado                                
        return IsVertexInNormalPlane(vertex, CenterOfPlane(frustumCornerFar),   NormalFromPlane(frustumCornerFar))   &&
               IsVertexInNormalPlane(vertex, CenterOfPlane(frustumCornerNear),  -NormalFromPlane(frustumCornerNear)) &&
               IsVertexInNormalPlane(vertex, CenterOfPlane(frustumCornerLeft),  NormalFromPlane(frustumCornerLeft))  &&
               IsVertexInNormalPlane(vertex, CenterOfPlane(frustumCornerRight), NormalFromPlane(frustumCornerRight)) &&
               IsVertexInNormalPlane(vertex, CenterOfPlane(frustumCornerUp),    NormalFromPlane(frustumCornerUp))    &&
               IsVertexInNormalPlane(vertex, CenterOfPlane(frustumCornerDown),  NormalFromPlane(frustumCornerDown));
    }

    // Calculo la normal de un plano y la direccion desde el centro del plano al vertice, para saber si esta en la misma direccion de la normal del plano
    private bool IsVertexInNormalPlane(Vector3 vertex, Vector3 centerPlane, Vector3 normalPlane)
    {
        // Vector3.Dot devuele el resultado de la multiplacion de las magnitudes por el coseno del angulo entre estos vectores
        return Vector3.Dot((vertex - centerPlane).normalized, normalPlane) > 0; // Si es mayor a cero esta por delante de la cara del plano.
    }

    // Calculo el centro del plano en base a sus vertices
    Vector3 CenterOfPlane(Vector3[] plane)
    {
        float sumX = 0;
        float sumY = 0;
        float sumZ = 0;

        for (int i = 0; i < plane.Length; i++)
        {
            sumX += plane[i].x;
            sumY += plane[i].y;
            sumZ += plane[i].z;
        }

        // Se devuelve la suma de los vertices totales dividido el largo del plano
        return new Vector3(sumX / (float)plane.Length, sumY / (float)plane.Length, sumZ / (float)plane.Length);
    }

    // Calculo la normal de un plano en base a sus vertices
    Vector3 NormalFromPlane(Vector3[] plane)
    {
        // Vector3 dir = Vector3.Cross(b - a, c - a); solo tomamos 3 vertices, descartamos el cuarto
        Vector3 dir = Vector3.Cross(plane[1] - plane[0], plane[2] - plane[0]);
        Vector3 norm = Vector3.Normalize(dir);
        return norm;
    }

}
