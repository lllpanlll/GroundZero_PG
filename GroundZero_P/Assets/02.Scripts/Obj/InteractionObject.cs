using UnityEngine;
using System.Collections;

public class InteractionObject : MonoBehaviour {

    public enum KindOfObj { Ap, Dp}
    public KindOfObj kindObj;

	// Use this for initialization
	void Start () {
        StartCoroutine(FallingObject());
	}

    IEnumerator FallingObject()
    {
        while (transform.position.y >= 1)
        {
            yield return new WaitForSeconds(0.01f);
            transform.Translate(-Vector3.up * 30 * Time.deltaTime);
        }
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
