using UnityEngine;

namespace Assets.Scripts.Data
{
    [System.Serializable]
    public class TerrainAttributes
    {
        [Header("Grid Attributes")]
        [Tooltip("Every grid has object count of grid size square magnitude.\n(Example grid size 4 means, every grid hase 16 objects on it.)")]
        public int gridSize = 4;
        [Tooltip("Maximum distance of active grids, it will also effect the active grid count by square magnitude.\n(15 means there will be 225 active grids on the scene every frame)")]
        public int distancePlane = 15;
        [Tooltip("The change of spawning trees.\n(10 means there is a 10/1000 = %1 chance of every object being a tree)")]
        public int treeDensity = 10;
        [Tooltip("The change of spawning rocks.\n(15 means there is a 15/1000 = %1.5 chance of every object being a rocks)")]
        public int rockDensity = 15;

        [Header("Object Type Prefabs")]
        public GameObject Tree;
        public GameObject Rock;
        public GameObject Grass;
    }
}
