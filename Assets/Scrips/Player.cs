using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Actor
{
    [SerializeField]
    Vector3 moveVector = Vector3.zero;

    [SerializeField]
    float Speed = 0;

    [SerializeField]
    BoxCollider boxCollider;

    [SerializeField]
    Transform MainBGQuadTransform;

    [SerializeField]
    Transform FireTransform;

    [SerializeField]
    float BulletSpeed = 1;

    // Start is called before the first frame update
    protected override void Initialize()
    {
        base.Initialize();
        PlayerStatePanel playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        playerStatePanel.SetHP(CurrentHp, MaxHp);
        if (Speed == 0)
            Speed = 0.5f;
    }

    // Update is called once per frame
    protected override void UpdateActor()
    {
        base.UpdateActor();
        if (moveVector.sqrMagnitude == 0)
        {
            return;
        }

        moveVector = AdjustMoveVetor(moveVector);

        UpdateMove();
    }

    void UpdateMove()
    {
        transform.position += moveVector;
    }

    public void ProcessInput(Vector3 moveDirection)
    {
        moveVector = moveDirection * Speed * Time.deltaTime;
    }

    Vector3 AdjustMoveVetor(Vector3 moveVector)
    {
        Vector3 res = Vector3.zero;

        res = boxCollider.transform.position + boxCollider.center + moveVector;

        if (res.x - boxCollider.size.x * 0.5f < -MainBGQuadTransform.localScale.x * 0.5f)
        {
            moveVector.x = 0;
        }

        if (res.x + boxCollider.size.x * 0.5f > MainBGQuadTransform.localScale.x * 0.5f)
        {
            moveVector.x = 0;
        }

        if (res.y - boxCollider.size.y * 0.5f < -MainBGQuadTransform.localScale.y * 0.5f)
        {
            moveVector.y = 0;
        }

        if (res.y + boxCollider.size.y * 0.5f > MainBGQuadTransform.localScale.y * 0.5f)
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

                enemy.OnCrash(this, crashDamage, crashPos);
            }
        }
    }

    public override void OnCrash(Actor enermy, int damage, Vector3 crashPos)
    {
        base.OnCrash(enermy, damage, crashPos);
        Debug.Log("player to enermy OnCrash");
    }

    public void Fire()
    {
#if false
        GameObject go = Instantiate(Bullet);
        Bullet bullet = go.GetComponent<Bullet>();
        //Debug.Log(FireTransform.position);
#endif
        Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletManager.PlayerBulletIndex);
        bullet.Fire(this, FireTransform.position, FireTransform.right, BulletSpeed, Damage);

    }

    protected override void DecreaseHP(Actor attacker, int value, Vector3 damagePos)
    {
        base.DecreaseHP(attacker, value, damagePos);
        PlayerStatePanel playerStatePanel = PanelManager.GetPanel(typeof(PlayerStatePanel)) as PlayerStatePanel;
        playerStatePanel.SetHP(CurrentHp, MaxHp);

        Vector3 damagePoint = damagePos + UnityEngine.Random.insideUnitSphere * 0.5f;
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.Generate(DamageManager.PlayerDamageIndex, damagePoint, value, Color.red);
    }

    protected override void OnDead(Actor killer)
    {
        base.OnDead(killer);
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GamePointAccumulator.Reset();
        Debug.Log("Game Over");
        gameObject.SetActive(false);
    }
}
