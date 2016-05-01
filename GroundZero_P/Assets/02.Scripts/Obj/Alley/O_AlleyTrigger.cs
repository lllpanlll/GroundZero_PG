using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Alley Trigger
/// 
/// *코멘트
///     <<추가>>  몬스터 대기 / 순찰 때에도 플레이어 골목 여부 전달
/// </summary>



public class O_AlleyTrigger : MonoBehaviour
{
    private M_AICore m_Core;                            //몬스터 AI


    public int alleyIndex;                              //골목 고유 Index

    public GameObject[] gateTrigger;                    //골목 통로 Trigger

    public bool isPatrolLooping;                        //순찰 경로가 순환 가능한가                
    public Transform[] patrolWayPoints;                 //골목 순찰 경로



    //Awake
    void Awake()
    {
        m_Core = GameObject.FindGameObjectWithTag(Tags.Monster).GetComponent<M_AICore>();
    }



    //골목 진입
    void OnTriggerEnter(Collider coll)
    {
        //Debug.Log("Trigger Enter " + coll.gameObject.tag);

        if (coll.gameObject.tag.Equals(Tags.Player))                //트리거에 플레이어 진입 시
        {
            if (m_Core.monState.Equals(M_TopState.Attack))          //몬스터가 공격 상태라면 (플레이어를 인식하고 있다면)
            {
                //몬스터의 상태를 Alley로 바꾸고 Alley에 필요한 정보 제공 
                M_Attack.instance.ChangeStateAttackToAlley();
                M_Alley.instance.EnterAlley(this);
            }
            

            //몬스터가 대기 / 순찰 상태일 때, 플레이어가 골목에 있다 알림
            else if (m_Core.monState.Equals(M_TopState.Idle))    
            {
                M_Idle.instance.IsPlayerInAlley = true;
                M_Alley.instance.EnterAlley(this);
            }
            else if (m_Core.monState.Equals(M_TopState.Patrol))  
            {
                M_Patrol.instance.IsPlayerInAlley = true;
                M_Alley.instance.EnterAlley(this);
            }
        }
    }

    //골목 이탈
    void OnTriggerExit(Collider coll)
    {
        //Debug.Log("Trigger Exit " + coll.gameObject.tag);

        if (coll.gameObject.tag.Equals(Tags.Player))                //트리거에 플레이어 이탈 시
        {
            if (m_Core.monState.Equals(M_TopState.Alley))           //몬스터가 골목 상태라면
            {
                //몬스터의 상태를 바꿔주는 Alley 함수 호출
                M_Alley.instance.ExitAlley();
            }


            //몬스터가 대기 / 순찰 상태일 때, 플레이어가 골목에 없다 알림
            else if (m_Core.monState.Equals(M_TopState.Idle))
            { M_Idle.instance.IsPlayerInAlley = false; }
            else if (m_Core.monState.Equals(M_TopState.Patrol))
            { M_Patrol.instance.IsPlayerInAlley = false; }
        }
    }
}
