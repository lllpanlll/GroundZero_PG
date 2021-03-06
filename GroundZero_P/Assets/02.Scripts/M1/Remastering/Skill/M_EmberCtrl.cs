﻿using UnityEngine;
using System.Collections;

public class M_EmberCtrl : MonoBehaviour
{
    private float curTime = 2.0f;        //잔불 유지시간
    public float CurTime { set{ curTime = value; } }

    private bool isStart = false;       //초기화 여부

    void OnEnable()
    {
            StartCoroutine(WaitForDestroy());
    }

    IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(0.03f);     //curTime이 외부에서 무사히 설정되길 대기

        yield return new WaitForSeconds(curTime);

        gameObject.SetActive(false);
    }
}
