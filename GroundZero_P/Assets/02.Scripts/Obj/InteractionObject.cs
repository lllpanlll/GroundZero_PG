using UnityEngine;
using System.Collections;

public class InteractionObject : MonoBehaviour {

    public enum KindOfObj { Ap, Dp}
    public KindOfObj kindObj;
    public int iApValue = 100;

    float fDetructTime = 2f;

    void OnEnable () {
        StartCoroutine(FallingObject());
        StartCoroutine(Destruction(fDetructTime));
    }

    IEnumerator FallingObject()
    {
        while (true)
        {
            if (transform.position.y >= 1)
            {
                transform.Translate(-Vector3.up * 30 * Time.deltaTime);
            }
            yield return new WaitForSeconds(0.01f);
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

            if (mgr.GetAP() < T2.Stat.MAX_AP || mgr.GetDP() < T2.Stat.MAX_DP)
                switch (kindObj)
                {
                    case KindOfObj.Ap:
                        if ((mgr.GetAP() + iApValue) > T2.Stat.MAX_AP)
                            mgr.SetAP(1000);
                        else
                            mgr.SetAP(mgr.GetAP() + iApValue);
                        // 사용 하고 나면 뭔가 변화가 일어나고 기능도 없어져야함
                        break;
                    case KindOfObj.Dp:
                        mgr.SetDP(100); // dp는 기획서 다시 봐야겠다.
                        // 사용 하고 나면 뭔가 변화가 일어나고 기능도 없어져야함
                        break;
                }
        }
    }
}
