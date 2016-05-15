using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Monster Patrol State
/// 도로 순찰 상태.
/// 
/// *코멘트
///     <<추가완료>>  인식했을 때 플레이어가 골목인지 아닌지 판단 후 골목/공격 상태 변환 결정
///     <<추가완료>>  경직 적용      
/// </summary>



public class M_Patrol : M_FSMState
{
    #region SingleTon

    public static M_Patrol instance = null;

    //컴포넌트로 연결되어있으면 이렇게 가능(MonoBehavior 상속 시) 
    void Awake()
    {
        instance = this; //<-지금은 상위 클래스인 M_FSMState가 MonoBehavior을 사용하기 때문에 이거 사용
    }

    #endregion


    private M_PatrolState patrolState = M_PatrolState.None;     //순찰 상태  

    public Transform[] patrolWayPoints;                         //순찰 경로

    public int nowPatrolWayPointIndex;                          //현재 선택한 순찰 지점 인덱스

    public float patrolTimeLimit = 70.0f;                       //순찰 최대 제한시간
    public float patrolTimeCounter = 0.0f;                      //순찰 시간 카운트

    public float waitTimeToArrive = 1.0f;                       //순찰 지점 도착 시 대기시간

    private bool isPlayerInAlley = false;                       //현재 플레이어가 골목에 있는지
    public bool IsPlayerInAlley { set { isPlayerInAlley = value; } }


    public float lookRotationTime = 0.5f;                       //플레이어를 바라볼 회전 시간



    //상태 초기화
    public override void FSMInitialize()
    {
        topState = M_TopState.Patrol;                           //이 상태는 Patrol입니다
    }

    //상태 Update
    public override void FSMUpdate()
    {
        m_Core.delayTime = 0.1f;                                //업데이트 주기 설정


        #region 상태 판단

        if ((m_Core.CheckSight().isPlayerInSight)               //시야에 플레이어가 있거나
            || (m_Core.CheckAuditoryField().isHearing))         //청역범위에 플레이어가 있으면 
            ChangeStateToRecognition();                         //상태 변경
           
        #endregion


        #region 하위 행동 FSM

        switch (patrolState)
        {
            case M_PatrolState.None:
                Debug.LogError("NonePatrolState!!!");
                break;

            case M_PatrolState.RandomPatrol:
                RandomPatrol();
                break;

            case M_PatrolState.GoToPlayerPatrol:
                GoToPlayerPatrol();
                break;
        }
        
        #endregion
    }


    //상태 매 프레임 Update
    public override void FSMMustUpdate()
    {
        //None
    }



    #region 하위 상태

    //랜덤 위치로 이동
    void RandomPatrol()
    {
        patrolTimeCounter += (Time.deltaTime + m_Core.delayTime);     //시간 계속 카운트

        //순찰 시간 제한을 넘기면 플레이어 쪽으로 이동
        if (patrolTimeCounter > patrolTimeLimit)                    
            patrolState = M_PatrolState.GoToPlayerPatrol;


        //현재 선택된 순찰 지점에 도착하면
        if (Vector3.Distance(m_Core.Tr.position, patrolWayPoints[nowPatrolWayPointIndex].position) < 1.0f)
        {
            m_Core.delayTime = waitTimeToArrive;                                        //다음 움직임까지 일정 시간 대기

            int tempPrevIndex = nowPatrolWayPointIndex;                                 //이전 인덱스 저장 

            //다음에 갈 인덱스를 랜덤으로 선정 (이전 인덱스와 겹치지 않도록 한다)
            while (true)
            {
                Random.seed = (int)System.DateTime.Now.Ticks;                           //랜덤값의 시드를 무작위로 만든다
                nowPatrolWayPointIndex = Random.Range(0, patrolWayPoints.Length - 1);   

                if (!nowPatrolWayPointIndex.Equals(tempPrevIndex))                      
                    break;                                                                                                                      
            }
        }

        //지정 순찰 위치까지 이동
        m_Core.NvAgent.Resume();
        m_Core.NvAgent.destination = patrolWayPoints[nowPatrolWayPointIndex].position;
        m_Core.Animator.SetBool("IsRunning", true);
    }
    
    //플레이어 위치로 이동
    void GoToPlayerPatrol()
    {
        //플레이어 위치까지 이동
        m_Core.NvAgent.Resume();
        m_Core.NvAgent.destination = m_Core.PlayerTr.position;
        m_Core.Animator.SetBool("IsRunning", true);
    }

    #endregion



    //인식 상태로 체인지
    public void ChangeStateToRecognition()
    {
        if (isPlayerInAlley)                                    //플레이어가 골목 안에 있다면
        {
            M_Alley.instance.IsStartToIdleOrPatrol = true;
            m_Core.ChangeState(M_Alley.instance);               //골목상태로 변경
        }
        else
            m_Core.ChangeState(M_Attack.instance);              //공격상태로 변경
    }



    //몬스터 경직
    public override void MonRigid()
    {
        base.MonRigid();
    }



    //상태 진입
    public override void Enter()
    {
        m_Core.monState = topState;                                                     //몬스터의 현재 상태는 Patrol입니다
        patrolState = M_PatrolState.RandomPatrol;                                       //시작은 랜덤부터

        Random.seed = (int)System.DateTime.Now.Ticks;                                   //랜덤값의 시드를 무작위로 만든다
        nowPatrolWayPointIndex = Random.Range(0, patrolWayPoints.Length - 1);           //인덱스 랜덤 선택
        patrolTimeCounter = 0.0f;

        //////Debug.Log("Enter Patrol");
    }

    //상태 탈출                   
    public override void Exit()
    {
        //플레이어를 바라보기
        StartCoroutine(RotateToPoint(m_Core.transform, m_Core.PlayerTr.position, lookRotationTime));

        //////Debug.Log("Exit Patrol");
    }
}
