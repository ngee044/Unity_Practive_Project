﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCachableEffect : MonoBehaviour
{
    public string FilePath { set; get; }

    private void OnEnable()
    {
        StartCoroutine("CheckIfAlive");
    }

    IEnumerator CheckIfAlive()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!GetComponent<ParticleSystem>().IsAlive(true))
            {
                SystemManager.Instance.EffectManager.RemoveEffect(this);
                break;
            }
        }
    }
}