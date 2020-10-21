using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneMain : BaseSceneMain
{
    public void OnSartButton()
    {
        Debug.Log("clicked button");
        SceneController.Instance.LoadScene(SceneNameConstants.LoadingScene);
    }

}
