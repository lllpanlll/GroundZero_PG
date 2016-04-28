using UnityEngine;
using System.Collections;

public class ToChargeDP : MonoBehaviour {
    // 오브젝트 근처로 플레이어가 오면 DP를 100으로 채워준다,

    GameObject oForObject;
    //ForObject sForObject;
    public int iHp;

    void Awake()
    {
        oForObject = transform.parent.gameObject;
        //sForObject = oForObject.GetComponent<ForObject>();
    }
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UsePlayer()
    {

    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == Tags.Player)
        {
        }
        //else
            //sForObject.OnCollisionDamage(coll, gameObject, iHp);
    }

    void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == Tags.Player)
        {
            T2.Manager mgr = coll.GetComponent<T2.Manager>();
            mgr.SetDP(100);
        }
        //else
            //sForObject.OnTriggerDamage(coll, gameObject, iHp);
    }
}
