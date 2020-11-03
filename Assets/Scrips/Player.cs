using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Player : Actor
{
    const string PlayerHUDPath = "Prefabs/PlayerHUD";

    [SerializeField]
    [SyncVar]
    Vector3 moveVector = Vector3.zero;

    [SerializeField]
    NetworkIdentity NetworkIdentity = null;

    [SerializeField]
    float Speed = 0;

    [SerializeField]
    BoxCollider boxCollider;

    [SerializeField]
    Transform FireTransform;

    [SerializeField]
    float BulletSpeed = 1;

    InputController inputController = new InputController();

    [SerializeField]
    [SyncVar]
    bool Host = false;  // Host 플레이어인지 여부

    [SerializeField]
    Material ClientPlayerMaterial;

    [SerializeField]
    [SyncVar]
    int UsableItemCount = 0;

    public int ItemCount
    {
        get
        {
            return UsableItemCount;
        }
    }

    // Start is called before the first frame update
    protected override void Initialize()
    {
        base.Initialize();

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();

        if (isLocalPlayer)
        {
            inGameSceneMain.Hero = this;
        }

        if (isServer && isLocalPlayer)
        {
            Host = true;
            RpcSetHost();
        }

        Transform startTransform;
        if (Host)
        {
            startTransform = inGameSceneMain.PlayerStartTransform1;
        }
        else
        {
            startTransform = inGameSceneMain.PlayerStartTransform2;
            MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
            meshRenderer.material = ClientPlayerMaterial;
        }

        SetPosition(startTransform.position);

        if (actorInstanceID != 0)
            inGameSceneMain.ActorManager.Regist(actorInstanceID, this);

        InitializePlayerHUD();
    }

    void InitializePlayerHUD()
    {
        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        GameObject go = Resources.Load<GameObject>(PlayerHUDPath);
        GameObject goInstance = Instantiate<GameObject>(go, inGameSceneMain.DamageManager.CanvasTransform);
        PlayerHUD playerHUD = goInstance.GetComponent<PlayerHUD>();
        playerHUD.Initialize(this);
    }

    [ClientCallback]
    public void UpdateInput()
    {
        inputController.UpdateInput();
    }

    public void UpdateMove()
    {
        if (moveVector.sqrMagnitude == 0)
        {
            return;
        }

        if (isServer)
        {
            RpcMove(moveVector);
        }
        else
        {
            CmdMove(moveVector);
            if (isLocalPlayer)
            {
                transform.position += AdjustMoveVetor(moveVector);
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("OnStartClient");
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log("OnStartLocalPlayer");
    }


    // Update is called once per frame
    protected override void UpdateActor()
    {
        if (!isLocalPlayer)
            return;

        base.UpdateActor();
        UpdateInput();
        UpdateMove();
    }

    [ClientRpc]
    public void RpcMove(Vector3 mv)
    {
        this.moveVector = mv;
        transform.position += AdjustMoveVetor(this.moveVector);
        base.SetDirtyBit(1);
        this.moveVector = Vector3.zero;
    }


    [Command]
    public void CmdMove(Vector3 mv)
    {
        this.moveVector = mv;
        transform.position += mv;
        base.SetDirtyBit(1);
        this.moveVector = Vector3.zero;
    }

    public void ProcessInput(Vector3 moveDirection)
    {
        if (!isLocalPlayer)
            return;

        moveVector = moveDirection * Speed * Time.deltaTime;
    }

    Vector3 AdjustMoveVetor(Vector3 moveVector)
    {
        Transform mainBGQuadTransform = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().MainBGQuadTransform;
        Vector3 res = Vector3.zero;

        res = boxCollider.transform.position + boxCollider.center + moveVector;

        if (res.x - boxCollider.size.x * 0.5f < -mainBGQuadTransform.localScale.x * 0.5f)
        {
            moveVector.x = 0;
        }

        if (res.x + boxCollider.size.x * 0.5f > mainBGQuadTransform.localScale.x * 0.5f)
        {
            moveVector.x = 0;
        }

        if (res.y - boxCollider.size.y * 0.5f < -mainBGQuadTransform.localScale.y * 0.5f)
        {
            moveVector.y = 0;
        }

        if (res.y + boxCollider.size.y * 0.5f > mainBGQuadTransform.localScale.y * 0.5f)
        {
            moveVector.y = 0;
        }

        return moveVector;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("other = " + other.name);
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy)
        {
            if (!enemy.IsDead)
            {
                BoxCollider box = (BoxCollider)other;
                Vector3 crashPos = enemy.transform.position + box.center;
                crashPos.x += box.size.x * 0.5f;

                enemy.OnCrash(crashDamage, crashPos);
            }
        }
    }

    public void Fire()
    {
        if (Host)
        {
            Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletManager.PlayerBulletIndex);
            bullet.Fire(actorInstanceID, FireTransform.position, FireTransform.right, BulletSpeed, Damage);
        }
        else
        {
            CmdFire(actorInstanceID, FireTransform.position, FireTransform.right, BulletSpeed, Damage);
        }
    }

    public void FireBomb()
    {
        if (UsableItemCount <= 0)
            return;

        if (Host)
        {
            Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletManager.PlayerBombIndex);
            bullet.Fire(actorInstanceID, FireTransform.position, FireTransform.right, BulletSpeed, Damage);
        }
        else
        {
            CmdFireBomb(actorInstanceID, FireTransform.position, FireTransform.right, BulletSpeed, Damage);
        }
        DecreaseUsableItemCount();
    }

    [Command]
    public void CmdFire(int ownerInstanceID, Vector3 firePosition, Vector3 direction, float speed, int damage)
    {
        Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletManager.PlayerBulletIndex);
        bullet.Fire(ownerInstanceID, firePosition, direction, speed, damage);
        base.SetDirtyBit(1);
    }

    protected override void DecreaseHP(int value, Vector3 damagePos)
    {
        base.DecreaseHP(value, damagePos);

        Vector3 damagePoint = damagePos + Random.insideUnitSphere * 0.5f;
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.Generate(DamageManager.PlayerDamageIndex, damagePoint, value, Color.red);
    }

    protected override void OnDead()
    {
        base.OnDead();
        gameObject.SetActive(false);
    }

    [ClientRpc]
    public void RpcSetHost()
    {
        Host = true;
        base.SetDirtyBit(1);
    }

    [Command]
    public void CmdFireBomb(int ownerInstanceID, Vector3 firePosition, Vector3 direction, float speed, int damage)
    {
        Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletManager.PlayerBombIndex);
        bullet.Fire(ownerInstanceID, firePosition, direction, speed, damage);
        base.SetDirtyBit(1);
    }

    void DecreaseUsableItemCount()
    {
        // 정상적으로 NetworkBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때
        //CmdDecreaseUsableItemCount();

        // MonoBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때의 꼼수
        if (isServer)
        {
            RpcDecreaseUsableItemCount();        // Host 플레이어인경우 RPC로 보내고
        }
        else
        {
            CmdDecreaseUsableItemCount();        // Client 플레이어인경우 Cmd로 호스트로 보낸후 자신을 Self 동작
            if (isLocalPlayer)
                UsableItemCount--;
        }
    }

    protected virtual void InternalIncreaseHP(int value)
    {
        if (isDead)
            return;

        CurrentHP += value;

        if (CurrentHP > MaxHP)
            CurrentHP = MaxHP;
    }

    public virtual void IncreaseHP(int value)
    {
        if (isDead)
            return;

        CmdIncreaseHP(value);
    }

    [Command]
    public void CmdIncreaseHP(int value)
    {
        InternalIncreaseHP(value);
        base.SetDirtyBit(1);
    }

    [Command]
    public void CmdDecreaseUsableItemCount()
    {
        UsableItemCount--;
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcDecreaseUsableItemCount()
    {
        UsableItemCount--;
        base.SetDirtyBit(1);
    }
}
