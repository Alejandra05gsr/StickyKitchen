using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class NormalVisualizer : MonoBehaviour
{
    [Header("Configuración de Depuración")]
    [Tooltip("Longitud de la línea de la normal.")]
    public float normalLength = 0.5f;

    [Tooltip("Color de la línea de la normal.")]
    public Color normalColor = Color.yellow;

    [Tooltip("¿Mostrar normales solo cuando el objeto esté seleccionado?")]
    public bool showOnlyWhenSelected = true;

    private void OnDrawGizmos()
    {
        if (showOnlyWhenSelected) return;
        DrawMeshNormals();
    }

    private void OnDrawGizmosSelected()
    {
        if (!showOnlyWhenSelected) return;
        DrawMeshNormals();
    }

    private void DrawMeshNormals()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null) return;

        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        Gizmos.color = normalColor;

        // Recorrer cada vértice y dibujar su normal asociada
        for (int i = 0; i < vertices.Length; i++)
        {
            // Convertir la posición del vértice de espacio local a espacio mundial
            Vector3 worldPos = transform.TransformPoint(vertices[i]);

            // Convertir la dirección de la normal de espacio local a espacio mundial
            Vector3 worldNormal = transform.TransformDirection(normals[i]);

            // Dibujar la línea desde el vértice hacia afuera
            Gizmos.DrawLine(worldPos, worldPos + worldNormal * normalLength);
        }
    }
}