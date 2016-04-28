using UnityEngine;
using System.Collections;

public class MonsterHitCtrl : MonoBehaviour {
    public bool canBroken = false;
    public int brokenHP = 50;
    public float receiveDamageRatio = 0.5f;


    #region <공격자용 피격 함수>
    public void OnHitMonster(int pDamage)
    {
        //가져온 데미지에 데미지 적용 비율 계산하여 최종 데미지 산출
        int hitDamage = (int)(pDamage * receiveDamageRatio);

        //부서질 수 있는 것이라면 부서트림
        if (canBroken)
        {
            brokenHP -= hitDamage;

            if (brokenHP < 0)
            {
                //부서진 부위의 하위 메쉬는 안보임 처리 -> 메쉬나 메터리얼의 구분 필요...!!!!! 
            }
        }

        //가져온 데미지만큼 총 HP 차감
        GameObject.FindWithTag(Tags.Monster).GetComponent<M_FSMTest>().HP -= hitDamage;

        //공격의 상태값 가져와서 상태 적용

        //붉게 출력
        StartCoroutine(this.ColoringRed());
    }
    #endregion

    #region <콜리더 충돌>
    void OnCollisionEnter(Collision coll)
    {
        //플레이어의 공격일 때만
        if(coll.gameObject.tag == "PLAYERATTK")
        {
            //플레이어에게서 데미지 가져옴
            int playerDamage = 10;

            //가져온 데미지에 데미지 적용 비율 계산하여 최종 데미지 산출
            int hitDamage = (int)(playerDamage * receiveDamageRatio);

            //부서질 수 있는 것이라면 부서트림
            if (canBroken)
            {
                brokenHP -= hitDamage;

                if (brokenHP < 0)
                {
                    //부서진 부위의 하위 메쉬는 안보임 처리 -> 메쉬나 메터리얼의 구분 필요...!!!!! 
                }
            }

            //가져온 데미지만큼 총 HP 차감
            GameObject.FindWithTag(Tags.Monster).GetComponent<M_FSMTest>().HP -= hitDamage;

            //공격의 상태값 가져와서 상태 적용

            //붉게 출력
            StartCoroutine(this.ColoringRed());
        }
    }
    #endregion

    #region <트리거 충돌>
    void OnTriggerEnter(Collider coll)
    {
        //플레이어의 공격일 때만
        if (coll.gameObject.tag == "PLAYERATTK")
        {
            //플레이어에게서 데미지 가져옴
            int playerDamage = 10;

            //가져온 데미지에 데미지 적용 비율 계산하여 최종 데미지 산출
            int hitDamage = (int)(playerDamage * receiveDamageRatio);

            //부서질 수 있는 것이라면 부서트림
            if (canBroken)
            {
                brokenHP -= hitDamage;

                if (brokenHP < 0)
                {
                    //부서진 부위의 하위 메쉬는 안보임 처리 -> 메쉬나 메터리얼의 구분 필요...!!!!! 
                }
            }

            //가져온 데미지만큼 총 HP 차감
            GameObject.FindWithTag(Tags.Monster).GetComponent<M_FSMTest>().HP -= hitDamage;

            //공격의 상태값 가져와서 상태 적용

            //붉게 출력
            StartCoroutine(this.ColoringRed());
        }
    }
    #endregion

    #region <붉게 출력>
    IEnumerator ColoringRed()
    {
        //////피격 시 붉게 출력됨(피격 부위만? 아니면 전체?)
        GameObject.Find("MonsterModel").GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
        
        yield return new WaitForSeconds(0.2f);

        GameObject.Find("MonsterModel").GetComponent<SkinnedMeshRenderer>().material.color = Color.black;
    }
    #endregion
}
