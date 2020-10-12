using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public const int PlayerBulletIndex = 0;
    public const int EnemyBulletIndex = 1;

    [SerializeField]
    PrefabCacheData[] BulletFiles;

    Dictionary<string, GameObject> FileCache = new Dictionary<string, GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        Prepare();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject Load(string resourcePath)
    {
        GameObject go = null;

        if (FileCache.ContainsKey(resourcePath))
        {
            go = FileCache[resourcePath];
        }
        else
        {

            // 캐시 없을 때 로드
            go = Resources.Load<GameObject>(resourcePath);
            if (go == false)
            {
                Debug.LogError("Load Error! path = " + resourcePath);
                return null;
            }
            FileCache.Add(resourcePath, go);
        }

        return go;
    }

    public void Prepare()
    {
        for(int i = 0; i < BulletFiles.Length; i++)
        {
            GameObject go = Load(BulletFiles[i].filePath);
            SystemManager.Instance.BulletCacheSystem.GenerateCache(BulletFiles[i].filePath, go, BulletFiles[i].cacheCount);
        }
        
    }

    public Bullet Generate(int index)
    {
        string path = BulletFiles[index].filePath;
        GameObject go = SystemManager.Instance.BulletCacheSystem.Archive(path);

        Bullet bullet = go.GetComponent<Bullet>();
        bullet.FilePath = path;

        return bullet;
    }

    public bool Remove(Bullet bullet)
    {
        SystemManager.Instance.BulletCacheSystem.Restore(bullet.FilePath, bullet.gameObject);
        return true;
    }
}
