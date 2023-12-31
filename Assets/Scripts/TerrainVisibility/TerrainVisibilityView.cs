using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Data;
using ServiceLocator;

public class TerrainVisibilityView : MonoBehaviour
{
    [Header("References")]
    public LayerMask groundLayer;
    public Transform player;
    public Camera mainCamera;

    [Header("Terrain Attributes")]
    public TerrainAttributes terrainAttributes;

    private GameObject TreePoolParent;
    private GameObject RockPoolParent;
    private GameObject GrassPoolParent;

    private TerrainVisibilityModel _terrainData;
    private PoolService _poolService;

    void Awake()
    {
        TreePoolParent = new GameObject("TreePoolParent");
        GrassPoolParent = new GameObject("GrassPoolParent");
        RockPoolParent = new GameObject("RockPoolparent");
        _poolService = Locator.Instance.Get<PoolService>(); 

        _terrainData = new TerrainVisibilityModel(
            player,
            terrainAttributes);
        _terrainData._makeGridVisibleEvent += MakeGridVisible;
        _terrainData._makeGridUnvisibleEvent += MakeGridUnvisible;
    }

    private void Update()
    {
        _terrainData.CalculateNeighbourGrids();
        _terrainData.CheckVisibleTargets();
    }

    private void MakeGridVisible(object sender,List<TerrainObjectData> terrainObjects)
    {
        foreach (TerrainObjectData terrainObject in terrainObjects)
        {
            if (terrainObject.visual != null)
            {
                terrainObject.visual.SetActive(true);
            }
            else
            {
                var go = _poolService.TryPop(terrainObject.type.ToString());
                if (go != null)
                {
                    MakeTerrainObjectVisible(go, terrainObject);
                }
                else
                {
                    MakeTerrainObjectVisible(CreateNewTerrainObject(terrainObject.type), terrainObject);
                }
            }

        }
    }

    private void MakeGridUnvisible(object sender,List<TerrainObjectData> terrainObjects)
    {
        foreach (TerrainObjectData terrainObject in terrainObjects)
        {
            var visual = terrainObject.visual;
            visual.SetActive(false);
            _poolService.Push(terrainObject.type.ToString(), visual);
            terrainObject.visual = null;
        }
    }

    private GameObject CreateNewTerrainObject(TerrainObjectType type)
    {
        switch (type)
        {
            case TerrainObjectType.Grass:
                var newGrassObj = Instantiate(terrainAttributes.Grass);
                newGrassObj.transform.parent = GrassPoolParent.transform;
                return newGrassObj;
            case TerrainObjectType.Tree:
                var newTreeObj = Instantiate(terrainAttributes.Tree);
                newTreeObj.transform.parent = TreePoolParent.transform;
                return newTreeObj;
            case TerrainObjectType.Rock:
                var newRockObj = Instantiate(terrainAttributes.Rock);
                newRockObj.transform.parent = RockPoolParent.transform;
                return newRockObj;
        }
        return null;
    }

    private void MakeTerrainObjectVisible(GameObject visual, TerrainObjectData terrainObject)
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
        if (Physics.Raycast(ray, out var info, groundLayer))
        {
            var RotationRef = Quaternion.FromToRotation(Vector3.up, info.normal);
            return new Vector3(RotationRef.eulerAngles.x, RotationRef.eulerAngles.y, RotationRef.eulerAngles.z);
        }
        return Vector3.zero;
    }
}
