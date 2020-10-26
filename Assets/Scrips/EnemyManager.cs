using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    EnemyFactory enemyFactory;

    List<Enemy> v_enemy = new List<Enemy>();

    [SerializeField]
    PrefabCacheData[] enemyFiles;

    public List<Enemy> Enemies
    {
        get
        {
            return v_enemy;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Prepare();
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    public bool GenerateEnemy(SquadronMemberStruct data)
    {
        string FilePath = SystemManager.Instance.EnemyTable.GetEnemy(data.EnemyID).FilePath;
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyCacheSystem.Archive(FilePath);
        go.transform.position = new Vector3(data.GeneratePointX, data.GeneratePointY, 0);

        Enemy enemy = go.GetComponent<Enemy>();
        enemy.FilePath = FilePath;

        enemy.Reset(data);
        Enemies.Add(enemy);

        return true;
    }

    public bool RemoveEnemy(Enemy enemy)
    {
        if(v_enemy.Contains(enemy) == false)
        {
            Debug.LogError("No exist Enemy");
            return false;
        }

        v_enemy.Remove(enemy);
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyCacheSystem.Restore(enemy.FilePath, enemy.gameObject);
        return true;
    }

    public void Prepare()
    {
        for (int i = 0; i < enemyFiles.Length; i++)
        {
            GameObject go = enemyFactory.Load(enemyFiles[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyCacheSystem.GenerateCache(enemyFiles[i].filePath, go, enemyFiles[i].cacheCount);
        }
    }


}
