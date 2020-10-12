﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    public const string EnemyPath = "Prefabs/Enemy";

    Dictionary<string, GameObject> EnemyFileCache = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject Load(string resourcePath)
    {
        GameObject gobj = null;
        if(EnemyFileCache.ContainsKey(resourcePath))
        {
            gobj = EnemyFileCache[resourcePath];
        }
        else
        {
            gobj = Resources.Load<GameObject>(resourcePath);
            if(gobj == false)
            {
                Debug.LogError("Load error! path = " + resourcePath);
                return null;
            }

            EnemyFileCache.Add(resourcePath, gobj);
        }

        //GameObject InstanceGameObject = Instantiate<GameObject>(gobj);

        return gobj;
    }

}
