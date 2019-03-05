using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    #region Variables

    #region PublicVariables

    #endregion

    #region PrivateVariables

    MeshFilter[] meshFilters;
    MeshRenderer[] meshRenderers;

    GameObject[] layerGameObjects;

    Vector2[] startPositions;
    Vector2[] endPositions;

    Vector2[][] noiseMaps;

    #endregion

    #endregion

    #region Properties

    public MeshFilter[] MeshFilter { get { return meshFilters; } }
    public MeshRenderer[] MeshRenderer { get { return meshRenderers; } }

    public Vector2[] StartPositions { get { return startPositions; } }
    public Vector2[] EndPositions { get { return endPositions; } }

    #endregion

    #region MonoBehaviourMethods

    void Awake()
    {
        EventManager.StartListening(EventName.TILT_PHONE, TiltChunk);
        EventManager.StartListening(EventName.SCROLLING, MoveChunk);
    }

    #endregion

    #region Methods

    #region PublicMethods

    public Chunk GenerateChunk(ref Layer[] layers, float width)
    {

        PrepareChunk(ref layers);

        startPositions = new Vector2[layers.Length];
        endPositions = new Vector2[layers.Length];

        for (int i = 0; i < layers.Length; i++)
        {
            startPositions[i] = new Vector2(0, Random.Range(layers[i].heightRange.x, layers[i].heightRange.y));
            endPositions[i] = new Vector2(width, Random.Range(layers[i].heightRange.x, layers[i].heightRange.y));

            GenerateLayer(ref layers[i], i);
        }

        return this;
    }

    public Chunk GenerateChunkWithStartPositions(ref Layer[] layers, float width, Vector2[] _startPositions)
    {

        PrepareChunk(ref layers);

        startPositions = endPositions = new Vector2[layers.Length]; ;
        endPositions = new Vector2[layers.Length];

        for (int i = 0; i < _startPositions.Length; i++)
        {
            startPositions[i] = new Vector2(0, _startPositions[i].y);
        }

        for (int i = 0; i < layers.Length; i++)
        {
            endPositions[i] = new Vector2(width, Random.Range(layers[i].heightRange.x, layers[i].heightRange.y));

            GenerateLayer(ref layers[i], i);
        }

        return this;
    }

    public Chunk GenerateChunkWithEndPositions(ref Layer[] layers, float width, Vector2[] _endPositions)
    {

        PrepareChunk(ref layers);

        startPositions = new Vector2[layers.Length];
        endPositions = new Vector2[layers.Length];

        for (int i = 0; i < _endPositions.Length; i++)
        {
            endPositions[i] = new Vector2(width, _endPositions[i].y);
        }

        for (int i = 0; i < layers.Length; i++)
        {
            startPositions[i] = new Vector2(width, Random.Range(layers[i].heightRange.x, layers[i].heightRange.y));

            GenerateLayer(ref layers[i], i);


        }

        return this;
    }

    public Chunk GenerateChunkWithBothPositions(ref Layer[] layers, float width, Vector2[] _startPositions, Vector2[] _endPositions)
    {

        PrepareChunk(ref layers);

        startPositions = _startPositions;
        endPositions = _endPositions;

        for (int i = 0; i < _startPositions.Length; i++)
        {
            startPositions[i] = new Vector2(0, _startPositions[i].y);
            endPositions[i] = new Vector2(width, _endPositions[i].y);
        }

        for (int i = 0; i < layers.Length; i++)
        {
            GenerateLayer(ref layers[i], i);
        }

        return this;
    }

    public Vector2 GetStartPosition(int index)
    {
        return startPositions[index];
    }

    public Vector2 GetEndPosition(int index)
    {
        return endPositions[index];
    }

    public void MoveChunk()
    {
        Vector3 direction = ((Mathf.Abs(Input.mouseScrollDelta.x) > Mathf.Abs(Input.mouseScrollDelta.y)) ? Input.mouseScrollDelta.x : Input.mouseScrollDelta.y) * Vector2.right;
        transform.position += direction;
        if (transform.position.x < 0)
        {
            transform.position += TerrainGenerator.ChunkCount * TerrainGenerator.ChunkWidth * Vector3.right;
        }
        else if (transform.position.x > TerrainGenerator.ChunkCount * TerrainGenerator.ChunkWidth)
        {
            transform.position += TerrainGenerator.ChunkCount * TerrainGenerator.ChunkWidth * Vector3.left;
        }
    }

    public void TiltChunk()
    {
        Vector2 direction = InputManager.Direction();

        foreach (GameObject layer in layerGameObjects)
        {
            layer.transform.localPosition = new Vector3(direction.x * layer.transform.localPosition.z * CameraController.Instance.tiltFactor.x, direction.y * layer.transform.localPosition.z * CameraController.Instance.tiltFactor.y, layer.transform.localPosition.z);
        }
    }

    #endregion

    #region PrivateMethods

    void PrepareChunk(ref Layer[] layers)
    {
        noiseMaps = new Vector2[layers.Length][];

        layerGameObjects = new GameObject[layers.Length];
        meshFilters = new MeshFilter[layers.Length];
        meshRenderers = new MeshRenderer[layers.Length];
    }

    /// <summary>
    /// Generate a Layer gameObject (noise map, gameObject, position, meshFilter, meshRenderer);
    /// </summary>
    /// <param name="layers">Layers array with all infromation.</param>
    /// <param name="index">Layer index</param>
    void GenerateLayer(ref Layer layer, int index)
    {
        //  Create noise map
        noiseMaps[index] = NoiseGenerator.GenerateNoiseMap(layer.numberOfIterations, startPositions[index], endPositions[index], layer.roughness, layer.verticalDisplacement);

        //  Create Layer gameObject
        layerGameObjects[index] = new GameObject("Layer " + index);

        layerGameObjects[index].transform.SetParent(transform);

        layerGameObjects[index].transform.localPosition = new Vector3(0, 0, layer.zPosition);

        meshFilters[index] = layerGameObjects[index].AddComponent<MeshFilter>();
        meshRenderers[index] = layerGameObjects[index].AddComponent<MeshRenderer>();

        //  Create mesh

        meshFilters[index].mesh = MeshGenerator.GenerateMesh(noiseMaps[index], index);

        meshRenderers[index].material = TerrainGenerator.GetMaterial(index);
    }

    #endregion

    #endregion

    #region Coroutines

    #endregion
}
