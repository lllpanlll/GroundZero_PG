using UnityEngine;
using System.Collections;

public class BuildingObjectCtrl : MonoBehaviour {

    public int HP = 50;
    
    //충돌하면
    void OnCollisionEnter(Collision coll)
    {
        //몬스터 공격이면 데미지만큼 HP 차감
        if (coll.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterAttkCollider))
        {
            HP -= coll.gameObject.GetComponent<MonsterAttkCtrl>().GetDamage();

            //HP 0 이하 시 파괴
            if (HP <= 0)
                Destroy(gameObject);
        }
    }

    //충돌하면
    void OnTriggerEnter(Collider coll)
    {
        //몬스터 공격이면 데미지만큼 HP 차감
        if (coll.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterAttkCollider))
        {
            HP -= coll.gameObject.GetComponent<MonsterAttkCtrl>().GetDamage();

            //HP 0 이하 시 파괴
            if (HP <= 0)
                Destroy(gameObject);
        }
    }
}
