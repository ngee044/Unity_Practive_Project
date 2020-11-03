using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public const int BulletDisappearFxIndex = 0;
    public const int ActorDeadFxIndex = 1;

    [SerializeField]
    PrefabCacheData[] effectPrefabFiles;

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

    public GameObject GenerateEffect(int index, Vector3 position)
    {
        if (index < 0 || index >= effectPrefabFiles.Length)
        {
            Debug.LogError("GenerateEffect error! out of range! index = " + index);
            return null;
        }

        string filePath = effectPrefabFiles[index].filePath;
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectCacheSystem.Archive(filePath);
        go.transform.position = position;

        AutoCachableEffect effect = go.GetComponent<AutoCachableEffect>();
        effect.FilePath = filePath;

        return go;
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
            go = Resources.Load<GameObject>(resourcePath);
            if (!go)
            {
                Debug.LogError("Load Error path = " + resourcePath);
                return null;
            }

            FileCache.Add(resourcePath, go);
        }

        return go;
    }

    public void Prepare()
    {
        for (int i = 0; i < effectPrefabFiles.Length; ++i)
        {
            GameObject go = Load(effectPrefabFiles[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectCacheSystem.GenerateCache(effectPrefabFiles[i].filePath, go, effectPrefabFiles[i].cacheCount, this.transform);
        }
    }

    public bool RemoveEffect(AutoCachableEffect effect)
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectCacheSystem.Restore(effect.FilePath, effect.gameObject);
        return true;
    }
}
