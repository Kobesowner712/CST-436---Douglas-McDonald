using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoadMeshGeneratorStarter : MonoBehaviour
{
    private MeshFilter _meshFilter;

    private MeshRenderer _meshRenderer;
    public Material material;

    public SplinePathTemp splinePath;

    public float textureRepeatLength = 2f;

    public bool rebuildEveryFrame = false;

    public float[] bankAngles;
    
    private int rungs = 10;
    private int width = 3;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _meshFilter = gameObject.AddComponent<MeshFilter>();
        _meshRenderer = gameObject.AddComponent<MeshRenderer>();

        BuildRoadMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (rebuildEveryFrame)
        {
            BuildRoadMesh();
        }
    }

    void BuildRoadMesh()
    {
        List<Vector3> verts = new();
        List<Vector2> uvs = new();
        Array.Resize(ref bankAngles, splinePath.SegmentCount + 1);
        for (int i = 0; i < rungs; i++)
        {
            float fraction = i / (rungs - 1);
            float distance = splinePath.TotalLength;

            float u = splinePath.ParameterAtDistance(distance);

            Vector3 centerPoint = splinePath.SamplePoint(u);
            Vector3 tangent = splinePath.SampleTangent(u);

            int segment = (int)Math.Min(u, splinePath.SegmentCount - 1);
            float t = u - segment;
            float bankAngle = Mathf.SmoothStep(bankAngles[segment], bankAngles[segment + 1], t);
            
            
            float halfWidth = width * 0.5f;
            float lift = Mathf.Sin(bankAngle * Mathf.Deg2Rad) * halfWidth;
            
            Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;
            right = Quaternion.AngleAxis(bankAngle, tangent) * right;

            centerPoint += Vector3.up * Math.Abs(lift);
            
            
            Vector3 leftVert = centerPoint - right * halfWidth;
            Vector3 rightVert = centerPoint + right * halfWidth;

            verts.Add(leftVert);
            verts.Add(rightVert);
            
            uvs.Add(new Vector2(0f, distance / textureRepeatLength));
            uvs.Add(new Vector2(1f, distance / textureRepeatLength));
            
        }
        
        List<int> triangles = new();
        for (int i = 0; i < rungs; i++)
        {
            int bottomLeft = i * 2;
            int bottomRight = bottomLeft + 1;
            int topLeft = (i + 1) * 2;
            int topRight = topLeft + 1;
            
            triangles.Add(bottomLeft);
            triangles.Add(topLeft);
            triangles.Add(bottomRight);

            triangles.Add(topLeft);
            triangles.Add(topRight);
            triangles.Add(bottomRight);
        }

        Mesh mesh = new();
        mesh.vertices = verts.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        _meshFilter.mesh = mesh;
        _meshRenderer.material = material;
    }
}
