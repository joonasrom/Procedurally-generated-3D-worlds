using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
   private const int MESH_SCALE = 50;

    [SerializeField] private GameObject[] objects;
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private Gradient gradient;
    [SerializeField] private int xSize = 100;
    [SerializeField] private int zSize = 100;
    [SerializeField] private float scale = 50f;
    [SerializeField] private int octaves = 10;
    [SerializeField] private float lacunarity = 1.75f;

    private int seed;
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;
    private float minTerrainheight;
    private float maxTerrainheight;
    private float lastNoiseHeight;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateNewMap();
    }

    private void SetNullProperties()
    {
        xSize = Mathf.Max(100, xSize);
        zSize = Mathf.Max(100, zSize);
        octaves = Mathf.Max(10, octaves);
        lacunarity = Mathf.Max(1.75f, lacunarity);
        scale = Mathf.Max(50f, scale);
    }

    public void CreateNewMap()
    {
        CreateMeshShape();
        CreateTriangles();
        ColorMap();
        FormMesh();
    }

   private void CreateMeshShape()
    {
        // Create seed
        Vector2[] octaveOffsets = GetOffsetSeed();

        if (scale <= 0f) {
            scale = 0.0001f;
        }

        // Create vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {
                // Set height of vertices
                float noiseHeight = GenerateNoiseHeight(z, x, octaveOffsets);
                SetMinMaxHeights(noiseHeight);
                vertices[i] = new Vector3(x, noiseHeight, z);
                i++;
            }
        }
    }

    private Vector2[] GetOffsetSeed()
    {
        seed = Random.Range(0, 1000);

        // Change area of map
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int o = 0; o < octaves; o++) {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[o] = new Vector2(offsetX, offsetY);
        }

        return octaveOffsets;
    }

    private float GenerateNoiseHeight(int z, int x, Vector2[] octaveOffsets)
{
    float amplitude = 20;
    float frequency = 1;
    float persistence = 0.5f;
    float noiseHeight = 1;

    // loop over octaves
    for (int y = 0; y < octaves; y++)
    {
        float mapZ = z / scale * frequency + octaveOffsets[y].y;
        float mapX = x / scale * frequency + octaveOffsets[y].x;

        float perlinValue = (Mathf.PerlinNoise(mapZ, mapX)) * 2 - 1;
        noiseHeight += heightCurve.Evaluate(perlinValue) * amplitude;
        frequency *= lacunarity;
        amplitude *= persistence;
    }
    return noiseHeight;
}

    private void SetMinMaxHeights(float noiseHeight)
{
    maxTerrainheight = Mathf.Max(maxTerrainheight, noiseHeight);
    minTerrainheight = Mathf.Min(minTerrainheight, noiseHeight);
}


    private void CreateTriangles() 
    {
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < xSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    private void ColorMap()
{
    colors = new Color[vertices.Length];

    for (int i = 0; i < vertices.Length; i++)
    {
        float height = Mathf.InverseLerp(minTerrainheight, maxTerrainheight, vertices[i].y);
        colors[i] = gradient.Evaluate(height);
    }
}

    private void SummonObjects() 
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            // find actual position of vertices in the game
            Vector3 worldPt = transform.TransformPoint(mesh.vertices[i]);
            var noiseHeight = worldPt.y;
            // Stop generation if height difference between 2 vertices is too steep
            if(System.Math.Abs(lastNoiseHeight - worldPt.y) < 25)
            {
                // min height for object generation
                if (noiseHeight > 150)
                {
                    // Chance to generate
                    if (Random.Range(1, 5) == 1)
                    {
                        GameObject objectToSpawn = objects[Random.Range(0, objects.Length)];
                        var spawnAboveTerrainBy = noiseHeight * 1;

                        if (noiseHeight > 400 && objectToSpawn != objects[0] && objectToSpawn != objects[1] && objectToSpawn != objects[2] && objectToSpawn != objects[3]) { 
                        // Dont do anything
                        } else {
                            Instantiate(objectToSpawn, new Vector3(mesh.vertices[i].x * MESH_SCALE, spawnAboveTerrainBy, mesh.vertices[i].z * MESH_SCALE), Quaternion.identity);
                    }
                    }
                }
            }
            lastNoiseHeight = noiseHeight;
        }
    }

    private void SummonObjects2() 
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            // find actual position of vertices in the game
            Vector3 worldPt = transform.TransformPoint(mesh.vertices[i]);
            var noiseHeight = worldPt.y;
            // Stop generation if height difference between 2 vertices is too steep
            if(System.Math.Abs(lastNoiseHeight - worldPt.y) < 25)
            {
                // min height for object generation
                if (noiseHeight > 0 && noiseHeight < 150)
                {
                    // Chance to generate
                    if (Random.Range(1, 15) == 1)
                    {
                        GameObject objectToSpawn = objects[Random.Range(12, objects.Length)];
                        var spawnAboveTerrainBy = noiseHeight * 1;
                            Instantiate(objectToSpawn, new Vector3(mesh.vertices[i].x * MESH_SCALE, spawnAboveTerrainBy, mesh.vertices[i].z * MESH_SCALE), Quaternion.identity);
                        
                    }
                }
            }
            lastNoiseHeight = noiseHeight;
        }
    }

    private void FormMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        GetComponent<MeshCollider>().sharedMesh = mesh;
        gameObject.transform.localScale = new Vector3(MESH_SCALE, MESH_SCALE, MESH_SCALE);

        SummonObjects();
        SummonObjects2();
    }

}
