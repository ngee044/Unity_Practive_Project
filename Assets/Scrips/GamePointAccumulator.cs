using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePointAccumulator
{
    int gamePoint = 0;

    public int GamePoint
    {
        set { gamePoint = value; }
        get { return gamePoint; }
    }

    public void Accumulate(int value)
    {
        gamePoint += value;

        PlayerStatePanel panel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        panel.SetScore(SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GamePointAccumulator.gamePoint);
    }

    public void Reset()
    {
        gamePoint = 0;
    }
}
