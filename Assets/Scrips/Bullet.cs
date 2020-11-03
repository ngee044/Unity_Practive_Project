﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum OwnerSide : int
{
    Player = 0,
    Enemy
}

public class Bullet : NetworkBehaviour
{
    const float LifeTime = 8.0f;

    [SyncVar]
    [SerializeField]
    Vector3 MoveDirection = Vector3.zero;

    [SyncVar]
    [SerializeField]
    float Speed = 0.0f;

    [SyncVar]
    bool NeedMove = false;

    [SyncVar]
    bool Hited = false;

    [SyncVar]
    float FiredTime = 0.0f;

    [SyncVar]
    [SerializeField]
    int Damage = 1;
    [SyncVar]
    [SerializeField]
    int OwnerInstanceID;


    [SyncVar]
    [SerializeField]
    string filePath;

    public string FilePath
    {
        get { return filePath; }
        set { filePath = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)
        {
            InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
            transform.SetParent(inGameSceneMain.BulletManager.transform);
            inGameSceneMain.BulletCacheSystem.Add(FilePath, gameObject);
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ProcessDisapperCondition())
            return;


        UpdateMove();
    }

    private void UpdateMove()
    {
        if (!NeedMove)
            return;

        Vector3 moveVector = MoveDirection.normalized * Speed * Time.deltaTime;
        moveVector = AdjustMove(moveVector);
        this.transform.position += moveVector;
    }

    public void Fire(int ownerInstanceID, Vector3 firePosition, Vector3 direction, float speed, int damage)
    {
        OwnerInstanceID = ownerInstanceID;
        SetPosition(firePosition);
        this.transform.position = firePosition;
        MoveDirection = direction;
        Speed = speed;
        Damage = damage;

        FiredTime = Time.time;
        NeedMove = true;

        UpdateNetworkBullet();
    }

    Vector3 AdjustMove(Vector3 moveVector)
    {
        RaycastHit hitInfo;

        if (Physics.Linecast(this.transform.position, this.transform.position + moveVector, out hitInfo))
        {
            Actor actor = hitInfo.collider.GetComponentInParent<Actor>();
            if (actor && actor.IsDead)
                return moveVector;

            moveVector = hitInfo.point - transform.position;
            OnBulletCollision(hitInfo.collider);
        }

        return moveVector;
    }

    void OnBulletCollision(Collider collider)
    {
        if (Hited)
            return;

        if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyBullet")
            || collider.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            return;
        }

        Actor owner = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ActorManager.GetActor(OwnerInstanceID);

        Actor actor = collider.GetComponentInParent<Actor>();
        if (actor && actor.IsDead || actor.gameObject.layer == owner.gameObject.layer)
            return;

        actor.OnBulletHited(Damage, transform.position);

        Collider myCollider = GetComponentInChildren<Collider>();
        myCollider.enabled = false;

        Hited = true;
        NeedMove = false;

        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectManager.GenerateEffect(EffectManager.BulletDisappearFxIndex, transform.position);
        go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        Disapper();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("bullet collision pos = " + other.transform.position);
        OnBulletCollision(other);
    }

    bool ProcessDisapperCondition()
    {
        if (this.transform.position.x > 15.0f || this.transform.position.x < -15.0f
            || this.transform.position.y > 15.0f || this.transform.position.y < -15.0f)
        {
            Disapper();
            return true;
        }
        else if (Time.time - FiredTime > LifeTime)
        {
            Disapper();
            return true;
        }

        return false;
    }

    void Disapper()
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Remove(this);
    }

    [ClientRpc]
    public void RpcSetActive(bool value)
    {
        this.gameObject.SetActive(value);
        base.SetDirtyBit(1);
    }

    public void SetPosition(Vector3 position)
    {
        // 정상적으로 NetworkBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때
        //CmdSetPosition(position);

        // MonoBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때의 꼼수
        if (isServer)
        {
            RpcSetPosition(position);        // Host 플레이어인경우 RPC로 보내고
        }
        else
        {
            CmdSetPosition(position);        // Client 플레이어인경우 Cmd로 호스트로 보낸후 자신을 Self 동작
            if (isLocalPlayer)
                transform.position = position;
        }
    }

    [Command]
    public void CmdSetPosition(Vector3 position)
    {
        this.transform.position = position;
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcSetPosition(Vector3 position)
    {
        this.transform.position = position;
        base.SetDirtyBit(1);
    }

    public void UpdateNetworkBullet()
    {
        // 정상적으로 NetworkBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때
        //CmdUpdateNetworkBullet();

        // MonoBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때의 꼼수
        if (isServer)
        {
            RpcUpdateNetworkBullet();        // Host 플레이어인경우 RPC로 보내고
        }
        else
        {
            CmdUpdateNetworkBullet();        // Client 플레이어인경우 Cmd로 호스트로 보낸후 자신을 Self 동작
        }
    }

    [Command]
    public void CmdUpdateNetworkBullet()
    {
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcUpdateNetworkBullet()
    {
        base.SetDirtyBit(1);
    }
}
