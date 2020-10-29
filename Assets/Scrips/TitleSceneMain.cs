using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneMain : BaseSceneMain
{
    public void OnSartButton()
    {
        PanelManager.GetPanel(typeof(NetworkConfigPanel)).Show();
    }

    public void GotoNextScene()
    {
        SceneController.Instance.LoadScene(SceneNameConstants.LoadingScene);
    }
}
