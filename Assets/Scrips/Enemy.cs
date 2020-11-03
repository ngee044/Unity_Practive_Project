using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
    [SyncVar]
    State CurrentState = State.None;

    const float MaxSpeed = 10.0f;
    const float MaxSpeedTime = 0.5f;

    [SerializeField]
    [SyncVar]
    Vector3 TargetPosition;

    [SerializeField]
    [SyncVar]
    float CurrentSpeed;

    [SerializeField]
    [SyncVar]
    Transform FireTransform;

    [SerializeField]
    [SyncVar]
    GameObject Bullet;

    [SerializeField]
    [SyncVar]
    float BulletSpeed = 1;

    [SyncVar]
    Vector3 CurrentVelocity;

    [SyncVar]
    Vector3 AppearPoint;

    [SyncVar]
    Vector3 DisappearPoint;

    [SyncVar]
    float MoveStartTime = 0.0f;
    //float BattleStartTime = 0.0f;

    [SyncVar]
    float LastActionUpdateTime = 0.0f;

    [SerializeField]
    [SyncVar]
    int FireRemainCount = 3;

    [SerializeField]
    [SyncVar]
    int GamePoint = 10;

    [SyncVar]
    [SerializeField]
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

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)
        {
            transform.SetParent(inGameSceneMain.EnemyManager.transform);
            inGameSceneMain.EnemyCacheSystem.Add(FilePath, gameObject);
            gameObject.SetActive(false);
        }

        if (actorInstanceID != 0)
        {
            inGameSceneMain.ActorManager.Regist(actorInstanceID, this);
        }
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
        // 정상적으로 NetworkBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때
        //CmdReset(data);

        // MonoBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때의 꼼수
        if (isServer)
        {
            RpcReset(data);        // Host 플레이어인경우 RPC로 보내고
        }
        else
        {
            CmdReset(data);        // Client 플레이어인경우 Cmd로 호스트로 보낸후 자신을 Self 동작
            if (isLocalPlayer)
                ResetData(data);
        }
    }

    void ResetData(SquadronMemberStruct data)
    {
        EnemyStruct enemyStruct = SystemManager.Instance.EnemyTable.GetEnemy(data.EnemyID);

        CurrentHP = MaxHP = enemyStruct.MaxHP;             // CurrentHP까지 다시 입력
        Damage = enemyStruct.Damage;                       // 총알 데미지
        crashDamage = enemyStruct.CrashDamage;             // 충돌 데미지
        BulletSpeed = enemyStruct.BulletSpeed;             // 총알 속도
        FireRemainCount = enemyStruct.FireRemainCount;     // 발사할 총알 갯수
        GamePoint = enemyStruct.GamePoint;                 // 파괴시 얻을 점수

        AppearPoint = new Vector3(data.AppearPointX, data.AppearPointY, 0);             // 입장시 도착 위치 
        DisappearPoint = new Vector3(data.DisappearPointX, data.DisappearPointY, 0);    // 퇴장시 목표 위치

        CurrentState = State.Ready;
        LastActionUpdateTime = Time.time;
        //
        isDead = false;      // Enemy는 재사용되므로 초기화시켜줘야 함
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
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.RemoveEnemy(this);
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

                player.OnCrash(crashDamage, crashPos);
            }
        }
    }

    public override void OnCrash(int damage, Vector3 crashPos)
    {
        base.OnCrash( damage, crashPos);
        Debug.Log("Enumy to Player OnCrash");
    }

    public void Fire()
    {
#if false
        GameObject go = Instantiate(Bullet);
        Bullet bullet = go.GetComponent<Bullet>();

        //Debug.Log(FireTransform.position);
#else
        Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletManager.EnemyBulletIndex);
        bullet.Fire(actorInstanceID, FireTransform.position, -FireTransform.right, BulletSpeed, Damage);
#endif
    }

    protected override void OnDead()
    {
        base.OnDead();

        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GamePointAccumulator.Accumulate(GamePoint);
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.RemoveEnemy(this);

        CurrentState = State.Dead;
    }

    protected override void DecreaseHP(int value, Vector3 damagePos)
    {
        base.DecreaseHP(value, damagePos);

        Vector3 damagePoint = damagePos + Random.insideUnitSphere * 0.5f;
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.Generate(DamageManager.EnemyDamageIndex, damagePoint, value, Color.magenta);
    }

    [Command]
    public void CmdReset(SquadronMemberStruct data)
    {
        ResetData(data);
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcReset(SquadronMemberStruct data)
    {
        ResetData(data);
        base.SetDirtyBit(1);
    }
}

