using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Monster Hit
/// 
/// *코멘트
///     <<추가>>  공격 설정이 경직일 때 경직 적용
///     <<추가>>  몬스터 대기 / 순찰 때 피격 시 플레이어 인식 후 상태 변경
/// </summary>



public class M_HitCtrl : MonoBehaviour
{
    private M_AICore m_Core;                                         //몬스터 AI


    public bool canBroken = false;                                      //부위 파괴 가능여부
    public int brokenHP = 50;                                           //파괴 부위 HP

    public float receiveDamageRatio = 0.5f;                             //데미지 받는 비율



    //Awake
    void Awake()
    {
        m_Core = GameObject.FindGameObjectWithTag(Tags.Monster).GetComponent<M_AICore>();
    }

   

    //피격 함수
    public void OnHitMonster(int pDamage, bool pRigid)          //추후 공격의 상태값이 추가된다면 여기에
    {
        
        int hitDamage = (int)(pDamage * receiveDamageRatio);    //가져온 데미지에 데미지 적용 비율 계산하여 최종 데미지 산출

        if (canBroken)                                          //파괴 가능 시 파괴
        {
            brokenHP -= hitDamage;

            if (brokenHP < 0) { }                               //부서진 부위의 하위 메쉬는 안보임 처리 -> 메쉬나 메터리얼의 구분/애니메이션 요구됨
        }
        
        if (pRigid && (!m_Core.IsRigid))                        //현재 공격이 경직이고 몬스터가 이미 경직상태인 것이 아니라면
        {
            m_Core.RigidMon();                                  //경직 적용
        }


        m_Core.HP -= hitDamage;                                 //계산된 데미지만큼 총 HP 차감
        m_Core.IsRigid = true;                                  //경직 적용



        StartCoroutine(this.ColoringRed());                     //붉게 출력


        
        //몬스터가 대기 / 순찰 상태일 때 피격 시, 플레이어 인식에 들어감
        if (m_Core.monState.Equals(M_TopState.Idle))
        { M_Idle.instance.ChangeStateToRecognition(); }
        else if (m_Core.monState.Equals(M_TopState.Patrol))
        { M_Patrol.instance.ChangeStateToRecognition(); }

    }



    //붉게 출력
    IEnumerator ColoringRed()
    {
        //피격 시 붉게 출력됨(피격 부위만? 아니면 전체?)
        GameObject.Find("MonsterModel").GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GameObject.Find("MonsterModel").GetComponent<SkinnedMeshRenderer>().material.color = Color.black;
    }



    //피격
    void OnCollisionEnter(Collision coll)
    {
        //플레이어의 공격일 때만
        if (coll.gameObject.tag == "PLAYERATTK")
        {
            OnHitMonster(coll.gameObject.GetComponent<T2.Manager>().GetAP(), false);        //플레이어 공격의 데미지/상태 가져와 피격 처리 (Damage는 임시)
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        //플레이어의 공격일 때만
        if (coll.gameObject.tag == "PLAYERATTK")
        {
            OnHitMonster(coll.gameObject.GetComponent<T2.Manager>().GetAP(), false);       //플레이어 공격의 데미지/상태 가져와 피격 처리 (Damage는 임시)
        }
    }
}
