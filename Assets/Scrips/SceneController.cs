using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNameConstants
{
    public static string TitleScene = "TitleScene";
    public static string LoadingScene = "LoadingScene";
    public static string InGame = "InGame";
}

public class SceneController : MonoBehaviour
{
    private static SceneController instance = null;
   
    public static SceneController Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject go = GameObject.Find("SceneController");
                if(go == null)
                {
                    go = new GameObject("SceneController");
                    SceneController controller = go.AddComponent<SceneController>();
                    return controller;
                }
                else
                {
                    instance = go.GetComponent<SceneController>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Can't have two instance of singleton.");
            DestroyImmediate(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanaged;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, LoadSceneMode.Single));
    }

    public void LoadSceneAdditive(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName, LoadSceneMode.Additive));

    }

    IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

        while (!asyncOperation.isDone)
            yield return null;

        Debug.Log("LoadSceneAsync is complete");
    }

    public void OnActiveSceneChanaged(Scene scene0, Scene scene1)
    {
        Debug.Log("OnActiveSceneChanaged is called! scene 0 = " + scene0.name + ", scene1 = " + scene1.name);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log("OnActiveSceneChanaged is called! scene 0 = " + scene.name + ", scene1 = " + loadSceneMode.ToString());
        BaseSceneMain baseSceneMain = GameObject.FindObjectOfType<BaseSceneMain>();
        Debug.Log("OnSceneLoaded ! basesSceneMain.name = " + baseSceneMain.name);
        SystemManager.Instance.CurrentSceneMain = baseSceneMain;

    }

    public void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("OnActiveSceneChanaged is called! scene 0 = " + scene.name);
    }

}
