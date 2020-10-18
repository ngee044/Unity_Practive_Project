using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SquadronData
{
    public float SquadronGenerateTime;
    public Squadron squdron;
}

public class SquadronManager : MonoBehaviour
{
    float GameStartTime;
    int SquadronIndex;

    [SerializeField]
    SquadronData[] squadronDatas;

    bool running = false;

    // Start is called before the first frame update
    void Start()
    {
        
    } 

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartGame();
        }

        CheckSquadronGeneratings();
    }

    public void StartGame()
    {
        GameStartTime = Time.time;
        SquadronIndex = 0;
        running = true;
        Debug.Log("Game Started");
    }

    void CheckSquadronGeneratings()
    {
        if (!running)
            return;

        if (Time.time - GameStartTime >= squadronDatas[SquadronIndex].SquadronGenerateTime)
        {
            GenerateSquadron(squadronDatas[SquadronIndex]);
            SquadronIndex++;

            if(SquadronIndex >= squadronDatas.Length)
            {
                AllSquadronGenerated();
                return;
            }
        }
    }

    void GenerateSquadron(SquadronData data)
    {
        data.squdron.GenerateAllData();
        Debug.Log("GenerateSquadron");
    }

    void AllSquadronGenerated()
    {
        Debug.Log("AllSquadronGenerated");
        running = false;
    }
}
