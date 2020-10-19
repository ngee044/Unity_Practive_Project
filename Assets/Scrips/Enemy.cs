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
    Vector3 AppearPoint;
    Vector3 DisappearPoint;

    float MoveStartTime = 0.0f;
    //float BattleStartTime = 0.0f;
    float LastActionUpdateTime = 0.0f;

    [SerializeField]
    int FireRemainCount = 3;

    [SerializeField]
    int GamePoint = 10;

    private string filePath;

    public string FilePath
    {
        set
        {
            filePath = value;
        }
        get
        {
            return filePath;
        }
    }
    
    // Start is called before the first frame update
    protected override void Initialize()
    {
        base.Initialize();
    }

    // Update is called once per frame
    protected override void UpdateActor()
    {
        base.UpdateActor();
        switch(CurrentState)
        {
            case State.None:
                break;
            case State.Ready:
                UpdateReady();
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
            default:
                break;
        }
    }

    private void UpdateBattle()
    {
        if(Time.time - LastActionUpdateTime > Mathf.Abs(3.0f))
        {
            if (FireRemainCount > 0)
            {
                Fire();
                FireRemainCount--;
            }
            else
            {
                Disappear(DisappearPoint);
            }

            LastActionUpdateTime = Time.time;
        }
    }

    public void Reset(SquadronMemberStruct data)
    {
        EnemyStruct enemyStruct = SystemManager.Instance.EnemyTable.GetEnemy(data.EnemyID);

        CurrentHp = MaxHp = enemyStruct.MaxHP;
        Damage = enemyStruct.Damage;
        crashDamage = enemyStruct.CrashDamage;
        BulletSpeed = enemyStruct.BulletSpeed;
        FireRemainCount = enemyStruct.FireRemainCount;
        GamePoint = enemyStruct.GamePoint;

        AppearPoint = new Vector3(data.AppearPointX, data.AppearPointY, 0);
        DisappearPoint = new Vector3(data.DisappearPointX, data.DisappearPointY, 0);

        CurrentState = State.Ready;
        LastActionUpdateTime = Time.time;
    }

    void UpdateReady()
    {
        if (Time.time - LastActionUpdateTime > 1.0f)
        {
            Appear(AppearPoint);
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
            LastActionUpdateTime = Time.time;
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

        LastActionUpdateTime = Time.time;

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
            if (!player.IsDead)
            {
                BoxCollider box = ((BoxCollider)other);
                Vector3 crashPos = player.transform.position + box.center;
                crashPos.x += box.size.x * 0.5f;

                player.OnCrash(this, crashDamage, crashPos);
            }
        }
    }

    public override void OnCrash(Actor player, int damage, Vector3 crashPos)
    {
        base.OnCrash(player, damage, crashPos);
        Debug.Log("Enumy to Player OnCrash");
    }

    public void Fire()
    {
#if false
        GameObject go = Instantiate(Bullet);
        Bullet bullet = go.GetComponent<Bullet>();

        //Debug.Log(FireTransform.position);
#else
        Bullet bullet = SystemManager.Instance.BulletManager.Generate(BulletManager.EnemyBulletIndex);
        bullet.Fire(this, FireTransform.position, -FireTransform.right, BulletSpeed, Damage);
#endif
    }

    protected override void OnDead(Actor killer)
    {
        base.OnDead(killer);
        SystemManager.Instance.GamePointAccumulator.Accumulate(GamePoint);
        
        SystemManager.Instance.EnemyManager.RemoveEnemy(this);
        CurrentState = State.Dead;
    }

    protected override void DecreaseHP(Actor attacker, int value, Vector3 damagePos)
    {
        base.DecreaseHP(attacker, value, damagePos);
        Vector3 damagePoint = damagePos + UnityEngine.Random.insideUnitSphere * 0.5f;
        SystemManager.Instance.DamageManager.Generate(DamageManager.EnemyDamageIndex, damagePoint, value, Color.magenta);
    }
}

