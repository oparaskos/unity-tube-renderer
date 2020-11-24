using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Unity.TubeRenderer
{
    [RequireComponent(typeof(MeshFilter))]
    [ExecuteInEditMode]
    public class TubeRenderer : MonoBehaviour
    {
        [Min(1)]
        public int subdivisions = 3;
        [Min(0)]
        public int segments = 8;
        public Vector3[] positions;
        [Min(0)]
        public float startWidth = 1f;
        [Min(0)]
        public float endWidth = 1f;
        public bool showNodesInEditor = false;
        public Vector2 uvScale = Vector2.one;
        public bool inside = false;

        private MeshFilter meshFilter;
        private Mesh mesh = null;
        private float theta = 0f;
        private int lastUpdate = 0;

        public Vector3 GetPosition(float f)
        {
            int a = Math.Max(0, Math.Min(positions.Length, Mathf.FloorToInt(f)));
            int b = Math.Max(0, Math.Min(positions.Length, Mathf.CeilToInt(f)));
            float t = f - a;
            return Vector3.Lerp(positions[a], positions[b], t);
        }

        public Vector3 GetPosition(int index)
        {
            return positions[index];
        }

        public void SetPositions(Vector3[] positions)
        {
            this.positions = positions;
        }

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            if (mesh == null) mesh = new Mesh();
            meshFilter.mesh = CreateMesh();
            lastUpdate = PropHashCode();
        }


        private Mesh CreateMesh()
        {
            Vector3[] interpolatedPositions = Enumerable.Range(0, (positions.Length - 1) * subdivisions)
                .Select(i => ((float)i) / ((float)subdivisions))
                .Select(f => GetPosition(f))
                .Append(positions.Last())
                .ToArray();

            theta = (Mathf.PI * 2) / segments;

            Vector3[] verts = new Vector3[interpolatedPositions.Length * segments];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];
            int[] tris = new int[2 * 3 * verts.Length];

            for (int i = 0; i < interpolatedPositions.Length; i++)
            {
                float dia = Mathf.Lerp(startWidth, endWidth, (float)i / interpolatedPositions.Length);

                Vector3 localForward = GetVertexFwd(interpolatedPositions, i);
                Vector3 localUp = Vector3.Cross(localForward, Vector3.up);
                Vector3 localRight = Vector3.Cross(localForward, localUp);

                for (int j = 0; j < segments; ++j)
                {
                    float t = theta * j;
                    Vector3 vert = interpolatedPositions[i] + (Mathf.Sin(t) * localUp * dia) + (Mathf.Cos(t) * localRight * dia);
                    int x = i * segments + j;
                    verts[x] = vert;
                    uvs[x] = uvScale * new Vector2(t / (Mathf.PI * 2), ((float)i * positions.Length) / (float)subdivisions);
                    normals[x] = (vert - interpolatedPositions[i]).normalized;
                    if (i >= interpolatedPositions.Length - 1) continue;

                    if (inside) normals[x] = -normals[x];
                    if (inside)
                    {
                        tris[x * 6] = x;
                        tris[x * 6 + 1] = x + segments;
                        tris[x * 6 + 2] = x + 1;

                        tris[x * 6 + 3] = x;
                        tris[x * 6 + 4] = x + segments - 1;
                        tris[x * 6 + 5] = x + segments;
                    }
                    else
                    {
                        tris[x * 6] = x + 1;
                        tris[x * 6 + 1] = x + segments;
                        tris[x * 6 + 2] = x;

                        tris[x * 6 + 3] = x + segments;
                        tris[x * 6 + 4] = x + segments - 1;
                        tris[x * 6 + 5] = x;
                    }
                }
            }
            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        private Vector3 GetVertexFwd(Vector3[] positions, int i)
        {
            Vector3 lastPosition;
            Vector3 thisPosition;
            if (i <= 0)
            {
                lastPosition = positions[i];
            }
            else
            {
                lastPosition = positions[i - 1];
            }
            if (i < positions.Length - 1)
            {
                thisPosition = positions[i + 1];
            }
            else
            {
                thisPosition = positions[i];
            }
            return (lastPosition - thisPosition).normalized;
        }

        private void OnDrawGizmos()
        {
            if (showNodesInEditor)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < positions.Length; ++i)
                {
                    float dia = Mathf.Lerp(startWidth, endWidth, (float)i / positions.Length);
                    Gizmos.DrawSphere(transform.position + positions[i], dia);
                }
            }
        }

        private int PropHashCode()
        {
            return positions.Aggregate(0, (total, it) => total ^ it.GetHashCode()) ^ positions.GetHashCode() ^ segments.GetHashCode() ^ subdivisions.GetHashCode() ^ startWidth.GetHashCode() ^ endWidth.GetHashCode();
        }

        private void LateUpdate()
        {
            if (lastUpdate != PropHashCode())
            {
                meshFilter.mesh = CreateMesh();
            }
        }
    }
}