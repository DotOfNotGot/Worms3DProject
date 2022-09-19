using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder.Shapes;

[RequireComponent(typeof(SphereCollider))]
public class TerrainDestruction : MonoBehaviour
{

    /*
    [SerializeField]
    private ProBuilderMesh terrainToDestroy;

    [SerializeField]
    private int terrainVertexCount;

    private SphereCollider _sphereCollider;

    [SerializeField]
    private List<Vector3> overLapPositions;
    [SerializeField]
    private List<Face> terrainFaces;
    [SerializeField]
    private List<Edge> terrainEdges;
    private Dictionary<Face, List<Edge>> _faceEdges;

    // Start is called before the first frame update
    void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetTerrainToDestroy(GameObject terrainMesh) 
    {

        terrainToDestroy = terrainMesh.GetComponent<ProBuilderMesh>();
        terrainVertexCount = terrainToDestroy.vertexCount;
        terrainFaces = new List<Face>();
        _faceEdges = new Dictionary<Face, List<Edge>>();
        terrainEdges = new List<Edge>();
        Collider terrainCollider = terrainToDestroy.GetComponent<Collider>();
        Bounds test = OverlapArea(terrainCollider.bounds, _sphereCollider.bounds);

        foreach (var position in overLapPositions)
        {
            terrainToDestroy.InsertVertexInMesh(position, Vector3.up);
        }
        foreach (var face in terrainToDestroy.faces)
        {
            terrainFaces.Add(face);
        }
        foreach (var face in terrainFaces)
        {
            _faceEdges.Add(face, new List<Edge>(face.edges));
            for (int i = 0; i < face.edges.Count; i++)
            {
                terrainEdges.Add(face.edges[i]);
            }
            
        }
        terrainVertexCount = terrainToDestroy.vertexCount;
    }

    private Bounds OverlapArea(Bounds a, Bounds b)
    {

        overLapPositions = new List<Vector3>();

        Vector3 min;
        Vector3 max;

        var minA = a.min;
        var maxA = a.max;
        var minB = b.min;
        var maxB = b.max;

        min.x = Mathf.Max(minA.x, minB.x);
        min.y = Mathf.Max(minA.y, minB.y);
        min.z = Mathf.Max(minA.z, minB.z);

        max.x = Mathf.Min(maxA.x, maxB.x);
        max.y = Mathf.Min(maxA.y, maxB.y);
        max.z = Mathf.Min(maxA.z, maxB.z);

        Bounds overLapResult = new Bounds();

        overLapResult.SetMinMax(min, max);

        // frontBottomLeft
        overLapPositions.Add(min);
        // frontBottomRight
        overLapPositions.Add(new Vector3(max.x, min.y, min.z));
        // frontTopLeft
        overLapPositions.Add(new Vector3(max.x, max.y, min.z));
        // frontTopRight
        overLapPositions.Add(new Vector3(max.x, max.y, min.z));
        // backBottomLeft
        overLapPositions.Add(new Vector3(min.x, min.y, max.z));
        // backBottomRight
        overLapPositions.Add(new Vector3(max.x, min.y, max.z));
        // backTopLeft
        overLapPositions.Add(new Vector3(min.x, max.y, max.z));
        // backTopRight
        overLapPositions.Add(max);

        return overLapResult;

        
    }
    */


}
