using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]s
public class TerrainGenerator : MonoBehaviour
{

    #region Variables

    #region PublicVariables

    [Header("UI")]
    public MaskableGraphic[] graphicWithColor0;
    public MaskableGraphic[] graphicWithColorN;

    [Header("Layers")]

    public Layer[] layers;

    public LayerColor[] layerColors;
    public string seed;

    [Header("Chunks")]
    public float chunkWidth;
    public int chunkCount;

    [Header("Rendering")]

    public Material material;

    //public Shader shader;
    //public Texture treeTexture;

    [Header("Debug")]

    public bool randomSeed;
    public bool autoGenerate;

    #endregion

    #region PrivateVariables

    static TerrainGenerator terrainGenerator;

    Chunk[] chunks;
    Material[] layerMaterials;

    int selectedLayerColorIndex = 0;

    #endregion

    #endregion

    #region Properties

    public static TerrainGenerator Instance
    {
        get
        {
            if (!terrainGenerator)
            {
                terrainGenerator = FindObjectOfType<TerrainGenerator>() as TerrainGenerator;
                if (!terrainGenerator)
                {
                    Debug.LogError("No actie instance of the TerrainGenerator script found in scene");
                }
            }
            return terrainGenerator;
        }
    }

    public static float ChunkWidth { get { return Instance.chunkWidth; } }
    public static int ChunkCount { get { return Instance.chunkCount; } }

    #endregion

    #region MonoBehaviourMethods

    void Start()
    {
        Initialize();
        GenerateTerrain();
        SetMaterialColor();
    }

    private void OnValidate()
    {
        foreach(LayerColor layerColor in layerColors)
        {
            if(layerColor.colors.Length != layers.Length+1){
                Color[] temp = new Color[layers.Length+1];
                for (int i = 0; i < ((layers.Length > layerColor.colors.Length) ? layerColor.colors.Length: layers.Length); i++){
                    temp[i] = layerColor.colors[i];
                }
                layerColor.colors = temp;
            }
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)){
            selectedLayerColorIndex++;
            selectedLayerColorIndex = selectedLayerColorIndex % layerColors.Length;
            SetMaterialColor();
        }
    }

    #endregion

    #region Methods

    #region PublicMethods

    public static Material GetMaterial(int index)
    {
        return Instance.layerMaterials[index];
    }

    public void GenerateTerrain()
    {
        chunks = new Chunk[chunkCount];

        if (randomSeed)
        {
            seed = System.DateTime.UtcNow.ToString();
        }
        Random.InitState(seed.GetHashCode());


        for (int i = 0; i < chunkCount; i++)
        {
            chunks[i] = new GameObject("Chunk " + i).AddComponent<Chunk>();
            chunks[i].transform.SetParent(transform);
            chunks[i].transform.localPosition = new Vector3(i * chunkWidth, 0);

            if (i == 0)
            {
                chunks[i].GenerateChunk(ref layers, chunkWidth);
            }
            else if (i < chunkCount - 1)
            {
                chunks[i].GenerateChunkWithStartPositions(ref layers, chunkWidth, chunks[i - 1].EndPositions);
            }
            else
            {
                chunks[i].GenerateChunkWithBothPositions(ref layers, chunkWidth, chunks[i - 1].EndPositions, chunks[0].StartPositions);
            }


        }
    }

    #endregion

    #region PrivateMethods

    void Initialize()
    {
        GenerateMaterials();
    }


    void GenerateMaterials()
    {
        layerMaterials = new Material[layers.Length];
        for (int i = 0; i < layerMaterials.Length; i++)
        {
            layerMaterials[i] = new Material(material)
            {
                color = layers[i].layerColor
            };
        }
    }

    void SetMaterialColor()
    {
        for (int i = 0; i < layerColors[selectedLayerColorIndex].colors.Length - 1;i++){
            layerMaterials[i].color = layerColors[selectedLayerColorIndex].colors[i];
        }
        foreach(MaskableGraphic graphic in graphicWithColor0){
            float alpha = graphic.color.a;
            Color color = layerColors[selectedLayerColorIndex].colors[0];
            color.a = alpha;
            graphic.color = color;
        }
        foreach (MaskableGraphic graphic in graphicWithColorN)
        {
            float alpha = graphic.color.a;
            Color color = layerColors[selectedLayerColorIndex].colors[layers.Length];
            color.a = alpha;
            graphic.color = color;
        }
        Camera.main.backgroundColor = layerColors[selectedLayerColorIndex].colors[layers.Length];
    }

    #endregion

    #endregion

    #region Coroutines

    #endregion
}

[System.Serializable]
public class Layer
{

    [HideInInspector]
    public string name;
    public int numberOfIterations;
    public Vector2 heightRange;
    public float roughness, verticalDisplacement;
    public float zPosition;
    public Color layerColor;
}

[System.Serializable]
public class LayerColor
{
    public string name;
    public Color[] colors;
}