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
    OwnerSide ownerSid = OwnerSide.Player;

    [SerializeField]
    Vector3 MoveDirection = Vector3.zero;

    [SerializeField]
    float Speed = 0.0f;

    bool NeedMove = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMove();
    }

    private void UpdateMove()
    {
        if (!NeedMove)
            return;

        Vector3 moveVector = MoveDirection.normalized * Speed * Time.deltaTime;
        this.transform.position += moveVector;
    }

    public void Fire(OwnerSide FireOwner, Vector3 firePosition, Vector3 direction, float speed)
    {
        ownerSid = FireOwner;
        this.transform.position = firePosition;
        MoveDirection = direction;
        Speed = speed;

        NeedMove = true;
    }
}
