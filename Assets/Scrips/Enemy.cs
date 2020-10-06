using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        None = -1,
        Ready = 0,
        Appear,
        Battle,
        Dead,
        Disappear,
    }

    [SerializeField]
    State CurrentState = State.None;

    const float MaxSpeed = 10.0f;
    const float MaxSpeedTime = 0.5f;

    [SerializeField]
    Vector3 TargetPosition;

    [SerializeField]
    float CurrentSpeed;

    Vector3 CurrentVelocity;

    float MoveStartTime = 0.0f;
    float BattleStartTime = 0.0f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch(CurrentState)
        {
            case State.None:
            case State.Ready:
                break;
            case State.Dead:
                break;
            case State.Appear:
            case State.Disappear:
                UpdateSpeed();
                UpdateMove();
                break;

            case State.Battle:
                UpdateBattle();
                break;
        }
    }

    private void UpdateBattle()
    {
        if(Time.time - BattleStartTime >= Mathf.Abs(3.0f))
        {
            Disappear(new Vector3(-15.0f, transform.position.y, transform.position.z));
        }
    }

    void UpdateSpeed()
    {
        CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, (Time.time - MoveStartTime) / MaxSpeedTime);
    }

    void UpdateMove()
    {
        float distance = Vector3.Distance(TargetPosition, transform.position);
        if (distance == 0)
        {
            Arrived();
            return;
        }

        CurrentVelocity = (TargetPosition - transform.position).normalized * CurrentSpeed;

        transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref CurrentVelocity, distance / CurrentSpeed, MaxSpeed);
    }

    void Arrived()
    {
        CurrentSpeed = 0.0f;
        if(CurrentState == State.Appear)
        {
            CurrentState = State.Battle;
        }
        else //if (CurrentState == State.Disappear)
        {
            CurrentState = State.None;
        }

    }

    public void Appear(Vector3 targetPose)
    {
        TargetPosition = targetPose;
        CurrentSpeed = MaxSpeed;

        BattleStartTime = Time.time;

        CurrentState = State.Appear;
        MoveStartTime = Time.time;
    }

    void Disappear(Vector3 targetPos)
    {
        TargetPosition = targetPos;
        CurrentSpeed = 0;

        CurrentState = State.Disappear;
        MoveStartTime = Time.time;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("other = " + other.name);
        Player player = other.GetComponentInParent<Player>();
        if(player)
        {
            player.OnCrash(this);
        }
    }

    public void OnCrash(Player player)
    {
        Debug.Log("Player OnCrash");
    }
}

