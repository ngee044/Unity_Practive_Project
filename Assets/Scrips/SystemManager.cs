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

    GamePointAccumulator gamePointAccumulator = new GamePointAccumulator();

    [SerializeField]
    EffectManager efftectManager;

    public Player Hero
    {
        get
        {
            return player;
        }
    }

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
