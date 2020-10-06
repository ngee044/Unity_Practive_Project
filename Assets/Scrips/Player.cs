using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    GameObject Bullet;

    [SerializeField]
    float BulletSpeed = 1;

    // Start is called before the first frame update
    protected override void Initialize()
    {
        base.Initialize();
        if (Speed == 0)
            Speed = 0.5f;
    }

    // Update is called once per frame
    protected override void UpdateActor()
    {
        if(moveVector.sqrMagnitude == 0)
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

        if(res.x - boxCollider.size.x * 0.5f < -MainBGQuadTransform.localScale.x * 0.5f)
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
            enemy.OnCrash(this);
        }
    }

    public void OnCrash(Enemy enermy)
    {
        Debug.Log("enermy OnCrash");
    }

    public void Fire()
    {
        GameObject go = Instantiate(Bullet);

        //Debug.Log(FireTransform.position);

        Bullet bullet = go.GetComponent<Bullet>();
        bullet.Fire(OwnerSide.Player, FireTransform.position, FireTransform.right, BulletSpeed, Damage);

    }

    protected virtual void OnDead(Actor killer)
    {
        base.OnDead(killer);
        SystemManager.Instance.GamePointAccumulator.Reset();
        Debug.Log("Game Over");
    }
}
