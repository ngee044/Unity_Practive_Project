﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    static SystemManager instance = null;
    public static SystemManager Instance
    {
        get
        {
            return instance;
        }
    }

   

    [SerializeField]
    EnemyTable enemyTable;

    public EnemyTable EnemyTable
    {
        get { return enemyTable; }
    }

    BaseSceneMain currentSceneMain;

    public BaseSceneMain CurrentSceneMain
    {
        set
        {
            currentSceneMain = value;
        }
    }

    [SerializeField]
    NetworkConnectionInfo connectionInfo = new NetworkConnectionInfo();

    public NetworkConnectionInfo ConnectionInfo
    {
        get
        {
            return connectionInfo;
        }
    }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("SystemManager error! Singleton Error");
            Destroy(gameObject);
            return;
        }

        instance = this;

        //Scene 이동간에 사라지지 않도록 처리
        DontDestroyOnLoad(this.gameObject);
    }
    //

    // Start is called before the first frame update
    void Start()
    {

        BaseSceneMain baseSceneMain = GameObject.FindObjectOfType<BaseSceneMain>();
        Debug.Log("OnSceneLoaded ! baseSceneMain.name = " + baseSceneMain.name);
        SystemManager.Instance.CurrentSceneMain = baseSceneMain;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public T GetCurrentSceneMain<T>()
     where T : BaseSceneMain
    {
        return currentSceneMain as T;
    }
}
