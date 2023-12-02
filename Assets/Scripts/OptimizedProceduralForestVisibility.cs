using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class OptimizedProceduralForestVisibility : MonoBehaviour
{
    public LayerMask groundLayer;
    public Transform player;
    public Camera mainCamera;
    public int gridSize = 25;
    public int distancePlane = 4;
    public int treeDensity = 20;
    public int rockDensity = 5;

    public GameObject Tree;
    public GameObject Rock;
    public GameObject Grass;
    private GameObject TreePoolParent;
    private GameObject RockPoolParent;
    private GameObject GrassPoolParent;


    private Dictionary<(int, int), List<TerrainObject>> _terrainObjectGrids = new();
    private HashSet<(int, int)> _neighBourGrids = new();
    HashSet<(int, int)> _visibleGrids = new();


    Stack<GameObject> _treePool = new Stack<GameObject>();
    Stack<GameObject> _rockPool = new Stack<GameObject>();
    Stack<GameObject> _grassPool = new Stack<GameObject>();

    void Awake()
    {
        GenerateForestData();
    }

    private void GenerateForestData()
    {
        var terrain = Terrain.activeTerrain;
        var terrainWidth = (int)terrain.terrainData.size.x;
        var terrainLenght = (int)terrain.terrainData.size.z;

        for (int i = 0; i <= terrainWidth / gridSize; i++)
        {
            for (int j = 0; j <= terrainLenght / gridSize; j++)
            {
                _terrainObjectGrids.Add((i, j), new List<TerrainObject>());
            }
        }
        TreePoolParent = new GameObject("TreePoolParent");
        GrassPoolParent = new GameObject("GrassPoolParent");
        RockPoolParent = new GameObject("RockPoolparent");

        List<TerrainObject> terrainObjects = new List<TerrainObject>();
        for (int i = 0; i <= terrainWidth; i++)
        {
            for (int j = 0; j <= terrainLenght; j++)
            {
                var randomType = Random.Range(0, 1000);
                var posXZ = new Vector2(i, j);
                var objAbsPos = new Vector3(posXZ.x, 1, posXZ.y);
                var posXYZ = new Vector3(posXZ.x, terrain.SampleHeight(new Vector3(posXZ.x, 1, posXZ.y)) + 0.2f, posXZ.y);

                var newTerrainObject = new TerrainObject()
                {
                    position = posXYZ,
                    type = randomType <= treeDensity ? TerrainObjectType.Tree : randomType <= treeDensity + rockDensity ? TerrainObjectType.Rock : TerrainObjectType.Grass
                };
                terrainObjects.Add(newTerrainObject);
                _terrainObjectGrids[(Mathf.FloorToInt(objAbsPos.x / gridSize), Mathf.FloorToInt(objAbsPos.z / gridSize))].Add(newTerrainObject);
            }
        }
    }

    

    private void Update()
    {
        CalculateNeighbourGrids();
        CheckVisibleTargets();
    }
    private void CalculateNeighbourGrids()
    {
        var playerGridPositionX = Mathf.FloorToInt((player.transform.position.x) / gridSize);
        var playerGridPositionY = Mathf.FloorToInt((player.transform.position.z) / gridSize);
        var currentGridTuple = (playerGridPositionX, playerGridPositionY);

        HashSet<(int, int)> newNeighbourGrids = new();
        if (_terrainObjectGrids.ContainsKey(currentGridTuple))
        {
            for (int i = -distancePlane; i <= distancePlane; i++)
            {
                for (int j = -distancePlane; j <= distancePlane; j++)
                {
                    var keyValue = (currentGridTuple.playerGridPositionX + i, currentGridTuple.playerGridPositionY + j);
                    if (_terrainObjectGrids.ContainsKey(keyValue))
                    {
                        newNeighbourGrids.Add(keyValue);
                    }

                }
            }
            _neighBourGrids = newNeighbourGrids;
        }
    }

    public void CheckVisibleTargets()
    {
        foreach ((int, int) visibleCoord in _visibleGrids) 
        {
            if (!_neighBourGrids.Contains(visibleCoord))
            {
                MakeUnvisible(_terrainObjectGrids[(visibleCoord.Item1, visibleCoord.Item2)]);
            }

        }
        _visibleGrids = _neighBourGrids;

        foreach ((int, int) visibleCoord in _visibleGrids)
        {
            MakeGridVisible(_terrainObjectGrids[(visibleCoord.Item1, visibleCoord.Item2)]);
        }
    }

    private void MakeGridVisible(List<TerrainObject> terrainObjects)
    {
        foreach (TerrainObject terrainObject in terrainObjects)
        {
            if (terrainObject.visual != null)
            {
                terrainObject.visual.SetActive(true);
            }
            else
            {
                var go = TryPopPoolObject(terrainObject.type);
                if (go != null)
                {
                    MakeTerrainObjectVisible(go, terrainObject);
                }
                else
                {
                    MakeTerrainObjectVisible(CreateNewPoolObject(terrainObject.type), terrainObject);
                }
            }

        }
    }

    private void MakeUnvisible(List<TerrainObject> terrainObjects)
    {
        foreach (TerrainObject terrainObject in terrainObjects)
        {
            var visual = terrainObject.visual;
            visual.SetActive(false);
            PushPoolObjectBack(visual, terrainObject.type);
            terrainObject.visual = null;
        }
    }

    private GameObject TryPopPoolObject(TerrainObjectType type)
    {
        switch (type)
        {
            case TerrainObjectType.Grass:
                _grassPool.TryPop(out var grassObj);
                return grassObj;
            case TerrainObjectType.Tree:
                _treePool.TryPop(out var treeObj);
                return treeObj;
            case TerrainObjectType.Rock:
                _rockPool.TryPop(out var rockObj);
                return rockObj;
        }
        return null;
    }

    private void PushPoolObjectBack(GameObject obj,TerrainObjectType type)
    {
        switch (type)
        {
            case TerrainObjectType.Grass:
                _grassPool.Push(obj);
                break;
            case TerrainObjectType.Tree:
                _treePool.Push(obj);
                break;
            case TerrainObjectType.Rock:
                _rockPool.Push(obj);
                break;
        }
    }

    private GameObject CreateNewPoolObject(TerrainObjectType type)
    {
        switch (type)
        {
            case TerrainObjectType.Grass:
                var newGrassObj = Instantiate(Grass);
                newGrassObj.transform.parent = GrassPoolParent.transform;
                return newGrassObj;
            case TerrainObjectType.Tree:
                var newTreeObj = Instantiate(Tree);
                newTreeObj.transform.parent = TreePoolParent.transform;
                return newTreeObj;
            case TerrainObjectType.Rock:
                var newRockObj = Instantiate(Rock);
                newRockObj.transform.parent = RockPoolParent.transform;
                return newRockObj;
        }
        return null;
    }

    private void MakeTerrainObjectVisible(GameObject visual, TerrainObject terrainObject)
    {
        terrainObject.visual = visual;
        visual.transform.position = new Vector3(terrainObject.position.x, terrainObject.position.y, terrainObject.position.z);
        if (!terrainObject.isRotationSet)
        {
            terrainObject.rotation = GetSurfaceAllignment(visual.transform);
            terrainObject.isRotationSet = true;
        }
        visual.transform.rotation = Quaternion.Euler(terrainObject.rotation);
        visual.SetActive(true);
    }

    public Vector3 GetSurfaceAllignment(Transform objTransform)
    {
        Ray ray = new Ray(objTransform.position, -objTransform.up);
        RaycastHit info = new RaycastHit();
        Quaternion RotationRef = Quaternion.Euler(0f, 0f, 0f);

        if (Physics.Raycast(ray, out info, groundLayer))
        {
            RotationRef = Quaternion.FromToRotation(Vector3.up, info.normal);
            return new Vector3(RotationRef.eulerAngles.x, RotationRef.eulerAngles.y, RotationRef.eulerAngles.z);
        }
        return Vector3.zero;
    }
}

public class TerrainObject
{
    public Vector3 position;
    public Vector3 rotation;
    public GameObject visual;
    public TerrainObjectType type;
    public bool isRotationSet;
}

public enum TerrainObjectType
{
    Grass,
    Tree,
    Rock
}