using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class OptimizedProceduralForestVisibility : MonoBehaviour
{
    public Transform player;
    public Camera mainCamera;
    public int x = 500;
    public int z = 500;
    public int Smoothness = 25;
    public int distancePlane = 4;
    HashSet<TerrainObject> terrainObjects = new HashSet<TerrainObject>();
    Dictionary<(int, int), HashSet<TerrainObject>> terrainObjectGrids = new();
    Dictionary<(int, int), HashSet<TerrainObject>> neighBourGrids = new();
    Stopwatch timer = new Stopwatch();
    static readonly object _lock = new object();
    void Awake()
    {
        for (int i = 0; i < x; i++)
        {

            for (int j = 0; j < z; j++)
            {
                terrainObjects.Add(new TerrainObject() { position = new Vector3(i - (x / 2), 1, j - (z / 2)) });
            }
        }

        foreach (var terrainObject in terrainObjects)
        {
            var keyX = Mathf.FloorToInt(terrainObject.position.x / Smoothness);
            var keyZ = Mathf.FloorToInt(terrainObject.position.z / Smoothness);
            terrainObjectGrids.TryGetValue((keyX, keyZ), out var objectSet);
            if (objectSet == null)
            {
                terrainObjectGrids.Add((keyX, keyZ), new HashSet<TerrainObject>());
            }
            terrainObjectGrids[(keyX, keyZ)].Add(terrainObject);
        }
        Test();
    }

    public void Test()
    {

        CalculateNeighbourGrids();
        CheckVisibleTargets();
    }

    private async void CalculateNeighbourGrids()
    {
        Monitor.Enter(_lock);
        timer.Restart();

        var currentGridPosX = Mathf.FloorToInt(player.transform.position.x / Smoothness);
        var currentGridPosZ = Mathf.FloorToInt(player.transform.position.z / Smoothness);
        var currentGridTuple = (currentGridPosX, currentGridPosZ);

        Dictionary<(int, int), HashSet<TerrainObject>> newNeighbourGrids = new();
        if (terrainObjectGrids.ContainsKey(currentGridTuple))
        {

            for (int i = -distancePlane; i <= distancePlane; i++)
            {
                for (int j = -distancePlane; j <= distancePlane; j++)
                {
                    var possibleNeighbourTuple = (currentGridPosX + j, currentGridPosZ + i);
                    if (terrainObjectGrids.TryGetValue(possibleNeighbourTuple, out var objectSet))
                    {
                        var gridGroupX = (possibleNeighbourTuple.Item1 * Smoothness) + (Smoothness / 2);
                        var gridGroupY = (possibleNeighbourTuple.Item2 * Smoothness) + (Smoothness / 2);
                        newNeighbourGrids.Add((gridGroupX, gridGroupY), objectSet);
                    }

                }
            }
            neighBourGrids = newNeighbourGrids;
        }
        timer.Stop();
        UnityEngine.Debug.Log("Ellapsed Miliseconds On Radius Check :" + timer.ElapsedMilliseconds);
        Monitor.Exit(_lock);
        await UniTask.DelayFrame(30);
        CalculateNeighbourGrids();
    }
    public async void CheckVisibleTargets()
    {

        Monitor.Enter(_lock);
        timer.Restart();
        foreach ((int, int) coordTuple in neighBourGrids.Keys)
        {
            var centerPosition = new Vector3(coordTuple.Item1, 1, coordTuple.Item2);
            var ViewPortPoint = mainCamera.WorldToViewportPoint(centerPosition);
            if (ViewPortPoint.x >= 0 && ViewPortPoint.x <= 1 && ViewPortPoint.y >= 0 && ViewPortPoint.y <= 1 && ViewPortPoint.z > 0)
            {
                foreach (TerrainObject terrainObject in neighBourGrids[(coordTuple.Item1, coordTuple.Item2)])
                {
                    MakeVisible(player.transform.position, terrainObject.position);
                }

            }
        }
        timer.Stop();
        UnityEngine.Debug.Log("Ellapsed Miliseconds On Visible Check :" + timer.ElapsedMilliseconds);
        Monitor.Exit(_lock);
        await UniTask.DelayFrame(5);
        CheckVisibleTargets();
    }

    public void MakeVisible(Vector3 start, Vector3 end)
    {
        UnityEngine.Debug.DrawLine(start, end, Color.green, 0.2f);
    }

    public void MakeUnvisible()
    {

    }
}

public class TerrainObject
{
    public Vector3 position;
}