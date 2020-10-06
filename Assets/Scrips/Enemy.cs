using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
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

    [SerializeField]
    Transform FireTransform;

    [SerializeField]
    GameObject Bullet;

    [SerializeField]
    float BulletSpeed = 1;

    Vector3 CurrentVelocity;

    float MoveStartTime = 0.0f;
    //float BattleStartTime = 0.0f;
    float LastBattleUpdateTime = 0.0f;

    [SerializeField]
    int FireRemainCount = 1;

    [SerializeField]
    int GamePoint = 10;

    // Start is called before the first frame update
    protected override void Initialize()
    {
        base.Initialize();
    }

    // Update is called once per frame
    protected override void UpdateActor()
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
        if(Time.time - LastBattleUpdateTime >= Mathf.Abs(1.0f))
        {
            if (FireRemainCount > 0)
            {
                Fire();
                FireRemainCount--;
            }
            else
            {
                Disappear(new Vector3(-15.0f, transform.position.y, transform.position.z));
            }

            LastBattleUpdateTime = Time.time;
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

        LastBattleUpdateTime = Time.time;

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

    public void Fire()
    {
        GameObject go = Instantiate(Bullet);

        //Debug.Log(FireTransform.position);

        Bullet bullet = go.GetComponent<Bullet>();
        bullet.Fire(OwnerSide.Enemy, FireTransform.position, -FireTransform.right, BulletSpeed, Damage);

    }

    protected override void OnDead(Actor killer)
    {
        base.OnDead(killer);
        SystemManager.Instance.GamePointAccumulator.Accumulate(GamePoint);
        CurrentState = State.Dead;
    }
}

