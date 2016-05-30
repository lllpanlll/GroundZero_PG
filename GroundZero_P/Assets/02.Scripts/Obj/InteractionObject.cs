using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InteractionObject : MonoBehaviour {

    T2.Manager mgr;
    public enum KindOfObj { Ap, Dp}
    public KindOfObj kindObj;
    float fSpeed = 20f;

    public int iMaxApCapacity = 1000;
    public int iMaxDpCapacity = 100;
    int iApCapacity;
    int iDpCapacity;
    public int iIncreaseDp = 1;
    bool bOutDp = false;

    void OnEnable()
    {
        StartCoroutine(FallObject());
    }

    void OnDisable()
    {
        iApCapacity = iMaxApCapacity;
        iDpCapacity = iMaxDpCapacity;
    }

    IEnumerator FallObject()
    {
        bool _bChk = true;
        while(_bChk)
        {
            Ray _ray = new Ray(transform.position, -Vector3.up);
            RaycastHit _hit = new RaycastHit();
            float _fDist = fSpeed * Time.deltaTime;

            transform.Translate(-Vector3.up * fSpeed * Time.deltaTime);

            if(Physics.Raycast(_ray, out _hit, _fDist, 1 << 14))
            {
                transform.position = _hit.point + Vector3.up;
                _bChk = false;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag(Tags.Player))
        {
            mgr = coll.GetComponent<T2.Manager>();
            switch (kindObj)
            {
                case KindOfObj.Dp:

                    bOutDp = true;
                    StartCoroutine(ChagingDp(mgr));

                    break;
            }
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.CompareTag(Tags.Player))
        {
            switch (kindObj)
            {
                case KindOfObj.Ap:

                    int iNeedAp = Mathf.Clamp((T2.Stat.MAX_AP - mgr.GetAP()), 0, iApCapacity);
                    mgr.SetAP(mgr.GetAP() + iNeedAp);

                    iApCapacity -= iNeedAp;

                    if (iApCapacity.Equals(0))
                        gameObject.SetActive(false);
                    break;
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag(Tags.Player))
        {
            switch (kindObj)
            {
                case KindOfObj.Dp:

                    bOutDp = false;

                    break;
            }
        }
    }

    IEnumerator ChagingDp(T2.Manager _mgr)
    {
        while (bOutDp)
        {

            if (T2.Stat.MAX_DP > _mgr.GetDP() && iDpCapacity > 0)
            {
                _mgr.SetDP(_mgr.GetDP() + iIncreaseDp);
                iDpCapacity -= iIncreaseDp;
            }

            if (iDpCapacity.Equals(0))
                gameObject.SetActive(false);

            yield return new WaitForSeconds(0.1f);
        }
    }
}
