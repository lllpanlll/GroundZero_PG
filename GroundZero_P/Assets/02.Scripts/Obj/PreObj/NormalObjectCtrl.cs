//using UnityEngine;
//using System.Collections;

//public class NormalObjectCtrl : MonoBehaviour {

//    //public int HP = 50;

//    ////공격자용 피격 함수
//    //public void OnHitObj(int pDamage)
//    //{
//    //    //가져온 데미지만큼 총 HP 차감
//    //    HP -= pDamage;

//    //    //HP 0 이하 시 파괴
//    //    if (HP <= 0)
//    //        Destroy(gameObject);
//    //}

//    ////충돌하면
//    //void OnCollisionEnter(Collision coll)
//    //{
//    //    //몬스터 공격이면
//    //    if (coll.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterAttkCollider))
//    //    {
//    //        //몬스터의 점프공격이나 브레스 공격이면 즉시파괴
//    //        if ((coll.gameObject.GetComponent<MonsterAttkCtrl>().attkState == MonsterState.BreathAttk)
//    //            ||(coll.gameObject.GetComponent<MonsterAttkCtrl>().attkState == MonsterState.JumpAttk))
//    //        {
//    //            HP = 0;
//    //        }
//    //        //그 외 데미지만큼 HP차감
//    //        else
//    //        {
//    //            HP -= coll.gameObject.GetComponent<MonsterAttkCtrl>().GetDamage();
//    //        }
//    //    }
//    //    //플레이어 공격이면
//    //    else if(coll.gameObject.tag == "PLAYERATTK")
//    //    {

//    //    }

//    //    //HP 0 이하 시 파괴
//    //    if (HP <= 0)
//    //        Destroy(gameObject);
//    //}

//    ////트리거 충돌하면
//    //void OnTriggerEnter(Collider coll)
//    //{
//    //    //몬스터 공격이면
//    //    if (coll.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterAttkCollider))
//    //    {
//    //        //몬스터의 점프공격이나 브레스 공격이면 즉시파괴
//    //        if ((coll.gameObject.GetComponent<MonsterAttkCtrl>().attkState == MonsterState.BreathAttk)
//    //            || (coll.gameObject.GetComponent<MonsterAttkCtrl>().attkState == MonsterState.JumpAttk))
//    //        {
//    //            HP = 0;
//    //        }
//    //        //그 외 데미지만큼 HP차감
//    //        else
//    //        {
//    //            HP -= coll.gameObject.GetComponent<MonsterAttkCtrl>().GetDamage();
//    //        }
//    //    }
//    //    //플레이어 공격이면
//    //    else if (coll.gameObject.tag == "PLAYERATTK")
//    //    {

//    //    }

//    //    //HP 0 이하 시 파괴
//    //    if (HP <= 0)
//    //        Destroy(gameObject);
//    }
//}
