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
        if (Input.GetKeyDown(KeyCode.L))
        {
            GenerateEnemy(0, new Vector3(15.0f, 0.0f, 0.0f));
        }
    }

    public bool GenerateEnemy(int index, Vector3 position)
    {
        Debug.Log("create Enemy type = " + index);
        string filePath = enemyFiles[index].filePath;
        GameObject go = SystemManager.Instance.EnemyCacheSystem.Archive(filePath);
        if (!go)
        {
            Debug.LogError("GenerateEnemy Error");
            return false;
        }

        go.transform.position = position;

        Enemy enemy = go.GetComponent<Enemy>();
        enemy.Appear(new Vector3(7.0f, 0.0f, 0.0f));
        enemy.FilePath = filePath;
        v_enemy.Add(enemy);
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
        SystemManager.Instance.EnemyCacheSystem.Restore(enemy.FilePath, enemy.gameObject);
        return true;
    }

    public void Prepare()
    {
        for (int i = 0; i < enemyFiles.Length; i++)
        {
            GameObject go = enemyFactory.Load(enemyFiles[i].filePath);
            SystemManager.Instance.EnemyCacheSystem.GenerateCache(enemyFiles[i].filePath, go, enemyFiles[i].cacheCount);
        }
    }


}
