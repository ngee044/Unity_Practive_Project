using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OwnerSide : int
{
    Player = 0,
    Enemy
}

public class Bullet : MonoBehaviour
{
    const float LifeTime = 5.0f;


    [SerializeField]
    Vector3 MoveDirection = Vector3.zero;

    [SerializeField]
    float Speed = 0.0f;

    bool NeedMove = false;
    bool Hited = false;

    float FiredTime = 0.0f;

    [SerializeField]
    int Damage = 1;

    Actor Owner;

    public string FilePath { get; set; }

    // Start is called before the first frame update
    void Start()
    {

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

    public void Fire(Actor FireOwner, Vector3 firePosition, Vector3 direction, float speed, int damage)
    {
        Owner = FireOwner;
        this.transform.position = firePosition;
        MoveDirection = direction;
        Speed = speed;
        Damage = damage;

        FiredTime = Time.time;
        NeedMove = true;
    }

    Vector3 AdjustMove(Vector3 moveVector)
    {
        RaycastHit hitInfo;

        if (Physics.Linecast(this.transform.position, this.transform.position + moveVector, out hitInfo))
        {
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

        Actor actor = collider.GetComponentInParent<Actor>();
        if (actor.IsDead && actor || actor == Owner)
            return;

        actor.OnBulletHited(Owner, Damage, this.transform.position);

        Collider myCollider = GetComponentInChildren<Collider>();
        myCollider.enabled = false;

        Hited = true;
        NeedMove = false;

        GameObject go = SystemManager.Instance.EffectManager.GenerateEffect(EffectManager.BulletDisappearFxIndex, transform.position);
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
        SystemManager.Instance.BulletManager.Remove(this);
    }

}
