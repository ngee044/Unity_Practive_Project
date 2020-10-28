using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneMain : BaseSceneMain
{
    const float NextSceneInterval = 3.0f;
    const float TextUpdateIntval = 0.15f;
    const string LoadingTextValue = "Loading.....";

    [SerializeField]
    Text LoadingText;

    int TextIndex = 0;
    float LastUpdateTime = 0.0f;

    float SceneStartTime;
    bool NextSceneCall = false;

    protected override void OnStart()
    {
        base.OnStart();
        SceneStartTime = Time.time;
    }

    protected override void UpdateScene()
    {
        base.UpdateScene();

        float currentTime = Time.time;
        if(currentTime - LastUpdateTime > TextUpdateIntval)
        {
            LoadingText.text = LoadingTextValue.Substring(0, TextIndex + 1);
            TextIndex++;
            if(TextIndex >= LoadingTextValue.Length)
            {
                TextIndex = 0;
            }

            LastUpdateTime = currentTime;
        }
        if(currentTime - SceneStartTime > NextSceneInterval)
        {
            if (!NextSceneCall)
                GotoNextScene();
        }
    }

    void GotoNextScene()
    {
        //SceneController.Instance.LoadScene(SceneNameConstants.InGame);
        FWNetworkManager.singleton.StartHost();
        NextSceneCall = true;
    }
}
