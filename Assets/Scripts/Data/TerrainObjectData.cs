using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class TerrainObjectData
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
}