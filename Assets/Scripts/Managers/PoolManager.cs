using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager 
{
    private Dictionary<string, Stack<GameObject>> poolStacks;

    public PoolManager()
    {
        poolStacks = new Dictionary<string, Stack<GameObject>>();
    }

    public GameObject TryPop(string key)
    {
        if (!poolStacks.ContainsKey(key))
        {
            poolStacks.Add(key, new Stack<GameObject>());
            return null;
        }

        if (poolStacks[key].Count > 0)
        {
            return poolStacks[key].Pop();
        }

        return null;
    }

    public void Push(string key, GameObject go)
    {
        if (!poolStacks.ContainsKey(key))
        {
            poolStacks.Add(key, new Stack<GameObject>());
        }

        poolStacks[key].Push(go);
    }
}
