﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField]
    protected int MaxHp = 100;

    [SerializeField]
    protected int CurrentHp;

    [SerializeField]
    protected int Damage = 1;

    [SerializeField]
    protected int crashDamage = 100;

    [SerializeField]
    bool isDead = false;

    public bool IsDead
    {
        get { return isDead; }
    }

    protected int CrashDamage
    {
        get { return crashDamage; }
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        if (CurrentHp == 0)
        {
            CurrentHp = MaxHp;
        }
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateActor();
    }

    protected virtual void UpdateActor()
    {

    }

    public virtual void OnBulletHited(Actor attacker, int damage)
    {
        Debug.Log("OnBulletHited damage= " + damage);
        DecreaseHP(attacker, damage);
    }

    public virtual void OnCrash(Actor attacker, int damage)
    {
        Debug.Log("OnBulletHited damage= " + damage);
        DecreaseHP(attacker, damage);
    }

    void DecreaseHP(Actor attacker, int value)
    {
        if (isDead)
            return;

        CurrentHp -= value;

        if (CurrentHp < 0)
            CurrentHp = 0;

        if (CurrentHp == 0)
        {
            OnDead(attacker);
        }
    }

    protected virtual void OnDead(Actor killer)
    {
        isDead = true;
        Debug.Log("OnDead() killer = " + killer.name);

        //Destroy(this.gameObject);
    }
}
