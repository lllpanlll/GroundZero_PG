using UnityEngine;
using System.Collections;

public class ToFlash : MonoBehaviour {

    // NOTE : 보스의 시야를 0으로 만들어서 플레이어 인식을 
    // 못하게 했었는데, 지금은 안됨. - 20160322 -
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
        if (coll.gameObject.tag == Tags.Player)
        {
            StartCoroutine(UseFlashing());
        }
        //else
            //sForObject.OnTriggerDamage(coll, gameObject, iHp);
    }

    //public float sightLengthRange = 15.0f;          // 보스 시야 거리
    //public float sightAngleRange = 40.0f;           // 보스 시야각

    IEnumerator UseFlashing()
    {
        GameObject oMon = GameObject.Find("Monster");
        Animator animMon = oMon.GetComponentInChildren<Animator>();
        MonsterCtrl monsterCtrl = oMon.GetComponent<MonsterCtrl>();

        //animMon.Stop();
        animMon.Play("Breath");
        monsterCtrl.sightLengthRange = 0.0f;

        yield return new WaitForSeconds(3f);

        monsterCtrl.sightLengthRange = 15.0f;
        yield return null;
    }
}
