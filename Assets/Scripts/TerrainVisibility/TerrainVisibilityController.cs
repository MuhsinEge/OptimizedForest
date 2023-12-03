using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Data;
public class TerrainVisibilityController
{
    private Transform _player;
    private int _gridSize;
    private int _distancePlane;

    private Dictionary<(int, int), List<TerrainObjectData>> _terrainObjectGrids = new();
    private HashSet<(int, int)> _neighBourGrids = new();
    private HashSet<(int, int)> _visibleGrids = new();

    public EventHandler<List<TerrainObjectData>> _makeGridUnvisibleEvent;
    public EventHandler<List<TerrainObjectData>> _makeGridVisibleEvent;

    public TerrainVisibilityController(Transform player,TerrainAttributes terrainAttributes)
    {
        _player = player;
        _gridSize = terrainAttributes.gridSize;
        _distancePlane = terrainAttributes.distancePlane;

        var terrain = Terrain.activeTerrain;
        var terrainWidth = (int)terrain.terrainData.size.x;
        var terrainLenght = (int)terrain.terrainData.size.z;

        for (int i = 0; i <= terrainWidth / _gridSize; i++)
        {
            for (int j = 0; j <= terrainLenght / _gridSize; j++)
            {
                _terrainObjectGrids.Add((i, j), new List<TerrainObjectData>());
            }
        }
        
        List<TerrainObjectData> terrainObjects = new List<TerrainObjectData>();
        for (int i = 0; i <= terrainWidth; i++)
        {
            for (int j = 0; j <= terrainLenght; j++)
            {
                var randomType = UnityEngine.Random.Range(0, 1000);
                var posXZ = new Vector2(i, j);
                var objAbsPos = new Vector3(posXZ.x, 1, posXZ.y);
                var posXYZ = new Vector3(posXZ.x, terrain.SampleHeight(new Vector3(posXZ.x, 1, posXZ.y)) + 0.2f, posXZ.y);

                var newTerrainObject = new TerrainObjectData()
                {
                    position = posXYZ,
                    type = randomType <= terrainAttributes.treeDensity ? TerrainObjectType.Tree : randomType <= terrainAttributes.treeDensity + terrainAttributes.rockDensity ? TerrainObjectType.Rock : TerrainObjectType.Grass
                };
                terrainObjects.Add(newTerrainObject);
                _terrainObjectGrids[(Mathf.FloorToInt(objAbsPos.x /_gridSize), Mathf.FloorToInt(objAbsPos.z / _gridSize))].Add(newTerrainObject);
            }
        }
    }

    public void CalculateNeighbourGrids()
    {
        var playerGridPositionX = Mathf.FloorToInt((_player.transform.position.x) / _gridSize);
        var playerGridPositionY = Mathf.FloorToInt((_player.transform.position.z) / _gridSize);
        var currentGridTuple = (playerGridPositionX, playerGridPositionY);

        HashSet<(int, int)> newNeighbourGrids = new();
        if (_terrainObjectGrids.ContainsKey(currentGridTuple))
        {
            for (int i = -_distancePlane; i <= _distancePlane; i++)
            {
                for (int j = -_distancePlane; j <= _distancePlane; j++)
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
                _makeGridUnvisibleEvent?.Invoke(this,_terrainObjectGrids[(visibleCoord.Item1, visibleCoord.Item2)]);
            }

        }
        _visibleGrids = _neighBourGrids;

        foreach ((int, int) visibleCoord in _visibleGrids)
        {
            _makeGridVisibleEvent?.Invoke(this,_terrainObjectGrids[(visibleCoord.Item1, visibleCoord.Item2)]);
        }
    }

}

