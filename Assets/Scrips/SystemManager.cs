using System.Collections;
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
    Player player;

    [SerializeField]
    EffectManager efftectManager;

    [SerializeField]
    EnemyManager enemyManager;

    [SerializeField]
    BulletManager bulletManager;

    [SerializeField]
    DamageManager damageManager;

    [SerializeField]
    PrefabCacheSystem enemyCacheSystem = new PrefabCacheSystem();

    [SerializeField]
    PrefabCacheSystem bulletCacheSystem = new PrefabCacheSystem();

    [SerializeField]
    PrefabCacheSystem effectCacheSystem = new PrefabCacheSystem();

    [SerializeField]
    PrefabCacheSystem damageCacheSystem = new PrefabCacheSystem();

    public PrefabCacheSystem DamageCacheSystem
    {
        get { return damageCacheSystem; }
    }

    public DamageManager DamageManager
    {
        get { return damageManager; }
    }

    public EnemyManager EnemyManager
    {
        get { return enemyManager; }
    }

    public BulletManager BulletManager
    {
        get { return bulletManager; }
    }

    public Player Hero
    {
        get
        {
            return player;
        }
    }

    GamePointAccumulator gamePointAccumulator = new GamePointAccumulator();
    public GamePointAccumulator GamePointAccumulator
    {
        get
        {
            return gamePointAccumulator;
        }
    }

    public EffectManager EffectManager
    {
        get
        {
            return efftectManager;
        }
    }

    public PrefabCacheSystem EnemyCacheSystem
    {
        get
        {
            return enemyCacheSystem;
        }
    }

    public PrefabCacheSystem BulletCacheSystem
    {
        get
        {
            return bulletCacheSystem;
        }
    }

    public PrefabCacheSystem EffectCacheSystem
    {
        get
        {
            return effectCacheSystem;
        }
    }

    [SerializeField]
    EnemyTable enemyTable;

    public EnemyTable EnemyTable
    {
        get { return enemyTable; }
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
