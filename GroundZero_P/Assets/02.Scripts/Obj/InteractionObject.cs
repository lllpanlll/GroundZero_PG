﻿using UnityEngine;
using System.Collections;

public class InteractionObject : MonoBehaviour {

    public enum KindOfObj { Ap, Dp}
    public KindOfObj kindObj;

    float fDetructTime = 1;

    // Use this for initialization
    void OnEnable () {
        StartCoroutine(FallingObject());
        StartCoroutine(Destruction(fDetructTime));

    }

    IEnumerator FallingObject()
    {
        while (transform.position.y >= 1)
        {
            yield return new WaitForSeconds(0.01f);
            transform.Translate(-Vector3.up * 30 * Time.deltaTime);
        }
    }

    IEnumerator Destruction(float _time)
    {
        yield return new WaitForSeconds(_time);

        gameObject.SetActive(false);

    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag(Tags.Player))
        {
            T2.Manager mgr = coll.GetComponent<T2.Manager>();
            switch (kindObj)
            {
                case KindOfObj.Ap:
                    if ((mgr.GetAP() + 100) > T2.Stat.MAX_AP) mgr.SetAP(1000);
                    else mgr.SetAP(mgr.GetAP() + 100);
                    break;
                case KindOfObj.Dp:
                    mgr.SetDP(100);
                    break;
            }
        }
    }
}
