using System;
using System.Collections.Generic;
// using Unity.VisualScripting;
using UnityEngine;

public class RoadMeshGeneratorStarter : MonoBehaviour
{
    public SplinePathTemp splinePath;
    public float width = 3f;
    public int rungs = 10;
    MeshFilter _meshFilter;
    MeshRenderer _meshRenderer;
    public Material material;
    public float TextureRepeatLength = 2f;
    public bool UpdateEveryFrame = false;
    public float[] bankAngles;
    void Start()
    {
        _meshFilter = gameObject.AddComponent<MeshFilter>();
        _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        _meshRenderer.material = material;
        BuildRoadMesh();
    }

    void Update()
    {
        if (UpdateEveryFrame)
        {
            BuildRoadMesh();
        }
    }

    void BuildRoadMesh()
    {
        List<Vector3> verts = new();
        List<Vector2> UVs = new();
        Array.Resize(ref bankAngles, splinePath.SegmentCount +1);
        for (int i = 0; i < rungs; i++)
        {
            float fraction = i / (float)(rungs - 1);
            float distance = splinePath.TotalLength * fraction;
            float u = splinePath.ParameterAtDistance(distance);
            Vector3 centerPoint = splinePath.SamplePoint(u);
            Vector3 tangent = splinePath.SampleTangent(u);
            
            int segment = (int)Math.Min(u,splinePath.SegmentCount-1);
            float t = u - segment;
            float bankAngle = Mathf.SmoothStep(bankAngles[segment], bankAngles[segment + 1], t);
            
            Vector3 right = Vector3.Cross(Vector3.up, tangent).normalized;
            right = Quaternion.AngleAxis(bankAngle, tangent) * right;

            float halfwidth = width * 0.5f;
            float lift = Mathf.Sin(bankAngle * Mathf.Deg2Rad)* halfwidth;
            centerPoint += Vector3.up*Math.Abs(lift);
            Vector3 leftVert = centerPoint - right * halfwidth;
            Vector3 rightVert = centerPoint + right * halfwidth;
            verts.Add(leftVert);
            verts.Add(rightVert);
            UVs.Add(new Vector2(0f,distance/TextureRepeatLength));
            UVs.Add(new Vector2(1f,distance/TextureRepeatLength));
            
            
        }

        List<int> triangles = new();
        for (int i = 0; i < rungs - 1; i++)
        {
            int BottomLeft = i*2;
            int BottomRight = BottomLeft + 1;
            int TopLeft = (i + 1) * 2;
            int TopRight = TopLeft + 1;
            
            triangles.Add(BottomLeft);
            triangles.Add(TopLeft);
            triangles.Add(BottomRight);
            
            triangles.Add(TopLeft);
            triangles.Add(TopRight);
            triangles.Add(BottomRight);
        }

        Mesh _mesh = new();
        _mesh.vertices = verts.ToArray();
        _mesh.triangles = triangles.ToArray();
        _mesh.uv = UVs.ToArray();
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _meshFilter.mesh = _mesh;
    }
}