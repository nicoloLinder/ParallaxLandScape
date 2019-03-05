using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{

    #region Variables

    #region PublicVariables

    #endregion

    #region PrivateVariables

    #endregion

    #endregion

    #region Properties

    #endregion

    #region Methods

    #region PublicMethods

    public static Mesh GenerateMesh(Vector2[] points, int layer)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[points.Length * 2];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[(points.Length * 2 - 2) * 3];

        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int i = 0; i < points.Length; i++)
        {
            vertices[vertexIndex] = points[i];
            uvs[vertexIndex] = new Vector2(((float)i / points.Length) * 5 * (layer + 1), 1);
            if (layer >2){
                uvs[vertexIndex] = Vector2.zero;
            }

            vertices[vertexIndex + points.Length] = points[i] - Vector2.up * 100;

            uvs[vertexIndex + points.Length] = new Vector2(((float)i / points.Length) * 5 * (layer + 1), -2 - layer);
            if (layer > 2)
            {
                uvs[vertexIndex] = Vector2.zero;
            }


            vertexIndex++;
        }

        for (int i = 0; i < points.Length - 1; i++)
        {
            triangles[triangleIndex++] = i;
            triangles[triangleIndex++] = i + 1;
            triangles[triangleIndex++] = i + points.Length + 1;

            triangles[triangleIndex++] = i;
            triangles[triangleIndex++] = i + points.Length + 1;
            triangles[triangleIndex++] = i + points.Length;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        return mesh;
    }

    #endregion

    #region PrivateMethods

    #endregion

    #endregion

    #region Coroutines

    #endregion
}
