using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Monster Alley State
/// 플레이어가 골목 안에 있을 때, 그에 대응하는 상태.
/// 
/// *코멘트
///     <<추가완료>>  경직 적용
///     <<추가완료>>  순찰 시 청역 안(?)에 플레이어 감지하면 Tracing으로 전환
///     <<추가완료>>  시야 안 공격 시 확률에 따라 마법 1과 마법 3 중 1 택
///     <<추가완료>>  대기2 시 확률에 따라 브레스 1과 점프 어택 중 택 1
///     <<추가완료>>  플레이어가 일정 시간동안 보였다(InSight) 안보였다(Tracing) 반복하면 브레스 2 발동
///     <<추가완료>>  브레스 2 마법 3 구현 완료
///     <<추가완료>>  대기하는 시간이 일정 시간 이상 되면 골목 안쪽을 바라봄
///     코루틴 함수를 가져와 쓰는거랑 IEmurator 변수 가져와 쓰는거랑 왜 다른걸까?
/// </summary>


public enum M_AlleySkillState           //골목 상태 사용 스킬
{
    None = 0,
    ForeFootPress,
    Magic_1,
    Magic_3,
    AlleyBreath_1,
    JumpAttack,
    Breath
}



public class M_Alley : M_FSMState
{
    #region SingleTon

    public static M_Alley instance = null;

    void Awake()
    { instance = this; }

    #endregion


    private M_AlleyState alleyState = M_AlleyState.None;                    //골목 상태  

    private M_AlleySkillState alleySkillState = M_AlleySkillState.None;     //사용 중인 스킬 상태
    public M_AlleySkillState AlleySkillState { set { alleySkillState = value; } }


    private bool isAlleyEnterCheck = false;                                 //골목 진입 체크

    private O_AlleyTrigger alleyBigTrigger;                                 //현재 위치 골목 Trigger 스크립트

    private int nowInAlleyIndex = -1;                                       //현재 위치한 골목 Index

    private Vector3 alleyGateTriggerPos;                                    //연락 온 골목 Trigger위치
    private GameObject alleyGateTrigger;                                    //연락 온 골목 Trigger GameObject
    public GameObject AlleyGateTrigger { get{ return alleyGateTrigger; } }
    private Vector3 alleyGateForwardPos;                                    //연락 온 골목 Trigger에 연결된 도로측 위치
    private Vector3 alleyInCornerPos;                                       //연락 온 골목 Trigger에 연결된 골목 안 코너 위치
    public Vector3 AlleyInCornerPos { get{ return alleyInCornerPos; } }     


    private bool isStartToIdleOrPatrol = false;                             //대기 / 순찰 상태에서 진입했는가
    public bool IsStartToIdleOrPatrol { set{ isStartToIdleOrPatrol = value; } }


    private float originSpeed;                                              //몬스터 원래 속도
    public float alleyTracingSpeed = 30.0f;                                 //골목 추격 시 몬스터 속도
    
    public float alleyForeFootAttackDist = 8.0f;                            //골목 추격 거리 제한

    public float alleyBreathInCornerDist = 10.0f;                           //골목 브레스 코너 거리 제한
    
    private bool isCheckingBreath_2 = false;                                //브레스 2 사용을 검토중인가
    public float checkTimeToBreath_2 = 3.0f;                                //브레스 2 사용 검토의 시간 한계
    private bool isTrunOfCountInSight = false;                              //시야 안 카운트를 할 시점인가
    public int maxTrun = 3;                                                 //브레스 2 사용을 위해 시야 안밖을 왔다갔다 해야 하는 횟수
    private int countTurn = 0;                                              //시야 안밖을 왔다갔다한 횟수 (2번을 1회로 친다)

    private bool isCheckTracing = false;                                    //추격 검토중


    private Vector3 targetGateForwardPos;                                   //추격할 골목 문 앞 위치
    private Vector3 targetGatePos;                                          //추격할 골목 문 위치
    private Vector3 targetCornerPos;                                        //추격할 골목 문 코너 위치
    public float finTraceDist = 30.0f;                                      //추격 제한 거리
    private bool isFinTrace = false;                                        //골목 내 추격 완료 및 대기중 여부
    private float finTraceWaitTimeCounter = 0.0f;                           //대기 시간 카운터
    public float finTraceWaitTimeLimit = 10.0f;                             //대기 한계 시간


    private bool isStartPatroling = false;                                  //Patrol 첫 지점을 통과했는가
    private int nowPatrolingPointIndex = -1;                                //현재 Patrol 중인 waypoint Index
    private Vector3 nowPatrolingPos;                                        //현재 Patrol 목표 waypoint
    private bool isPatrolingClockwise;                                      //시계방향 (+ 방향)으로 Ptarol 하는가
    private int patrolingStartIndex = -1;                                   //Patrol 시작 Index
    private int patrolingEndIndex = -1;                                     //Patrol 끝 Index 

    public float lookRotationTime = 0.5f;                                   //플레이어를 바라볼 회전 시간



    //상태 초기화
    public override void FSMInitialize()
    {
        topState = M_TopState.Alley;                                        //이 상태는 Alley입니다
    }


    //상태 Update
    public override void FSMUpdate()
    {
        m_Core.delayTime = 0.1f;                                            //업데이트 주기 설정


        #region 하위 FSM

        switch (alleyState)
        {
            case M_AlleyState.None:                                         //골목 상태가 설정되지 않은 경우 에러 출력
                Debug.LogError("NoneAlleyState!!!");
                break;

            case M_AlleyState.Starting:                                     //골목 상태 시작
                Starting();
                break;

            case M_AlleyState.InSight:                                      //골목 안쪽으로 플레이어가 보임
                InSight();
                break;

            case M_AlleyState.Tracing:                                      //플레이어가 보이지 않아 추격
                Tracing();
                break;

            case M_AlleyState.NoWait:                                       //대기가 오래되어 골목 안쪽을 바라봄
                NoWait();
                break;

            case M_AlleyState.Patrol:                                       //골목 순찰
                Patrol();
                break;
        }

        #endregion
    }



    #region 하위 상태 

    //골목 상태 시작
    void Starting()
    {
        if (Vector3.Distance(m_Core.Tr.position, alleyGateForwardPos) < 1.0f)  //도착지점과의 거리가 충분히 가까워지면
        {
            m_Core.NvAgent.speed = originSpeed;

            m_Core.NvAgent.Stop();
            m_Core.SetDestinationRealtime(false, null);
            m_Core.Animator.SetBool("IsRunning", false);

            StartCoroutine(RotateToPoint(m_Core.transform, alleyInCornerPos, lookRotationTime));

            alleyState = M_AlleyState.InSight;                              //시야 안으로 상태 변경
        }
        else                                                                //도착지점까지의 거리가 충분히 가깝지 않다면
        {
            //골목으로 플레이어를 추격
            m_Core.NvAgent.speed = alleyTracingSpeed;

            m_Core.NvAgent.Resume();
            m_Core.NvAgent.destination = alleyGateForwardPos;
            m_Core.SetDestinationRealtime(false, null);
            m_Core.Animator.SetBool("IsRunning", true);
        }
    }

    //골목 안쪽으로 플레이어가 보일 때
    void InSight()
    {
        //플레이어가 직선거리 상에 위치하고 있을 때, InSight 행동 실행
        if (m_Core.CheckSight().isPlayerInStraightLine)
        {
            if (!isCheckingBreath_2)                                    //브레스 2 사용 체크중이 아닐 때만                   
            {
                //안쪽 지점에 플레이어가 충분히 가깝다면 브레스 사용 가능 거리
                if (Vector3.Distance(alleyInCornerPos, m_Core.PlayerTr.position) < alleyBreathInCornerDist) 
                {
                    alleySkillState = M_AlleySkillState.AlleyBreath_1;
                    StartCoroutine(M_AlleyBreath_1.instance.UseSkill(alleyInCornerPos));                    //브레스 1 사용
                }

                //플레이어가 앞발찍기 사거리 안 일 때는
                else if (Vector3.Distance(m_Core.Tr.position, m_Core.PlayerTr.position) < alleyForeFootAttackDist) 
                {
                    alleySkillState = M_AlleySkillState.ForeFootPress;
                    StartCoroutine(M_ForeFootPress.instance.UseSkill(m_Core.PlayerTr.position));            //앞발찍기 사용
                }

                //브레스 사용 불가 앞발찍기 사용 불가
                else
                {
                    //스킬을 선택할 때 사용할 랜덤 값 설정
                    Random.seed = (int)System.DateTime.Now.Ticks;           //랜덤값의 시드를 무작위로 만든다
                    int randomChance = Random.Range(0, 1000);
                     
                    if (randomChance < 500)
                    {
                        alleySkillState = M_AlleySkillState.Magic_1;
                        StartCoroutine(M_Magic_1.instance.UseSkill(alleyInCornerPos));                      //마법 1 사용
                    }
                    else
                    {
                        alleySkillState = M_AlleySkillState.Magic_3;
                        StartCoroutine(M_Magic_3.instance.UseSkill(alleyInCornerPos));                      //마법 3 사용
                    }
                }
            }
            
            //추적 검토중 시야 안에 들어왔으면 추적 검토 중단
            if (isCheckTracing)                                            
                isCheckTracing = false;
        }

        //플레이어가 직선거리 상에 위치하지 않다면 (시야에서 벗어났다면)
        else
        {
            //브레스 2 사용 검토를 하지 않고 있었으면
            if (!isCheckingBreath_2)
            {    
                isCheckingBreath_2 = true;                                  //브레스 2 사용 검토중 체크
                isTrunOfCountInSight = true;                                //다음 차례는 InSight
                countTurn = 1;                                              //이 자체로 이미 1회로 친다

                StartCoroutine(CheckTimeToBreath_2());                      //브레스 2 사용 검토 시작
            }

            //추적 검토중이 아니면 추적 검토 시작
            if (!isCheckTracing)                                             
                StartCoroutine(CheckTracing());
        }
    }
    
    //플레이어가 보이지 않아 추격
    void Tracing() 
    {
        m_Core.delayTime = 0.2f;                                            //업데이트 주기 설정

        //플레이어가 청각 영역 안에 있으면
        if (m_Core.CheckAuditoryField().isHearing)
        {
            //플레이어와 가까운 쪽의 Gate에 저장된 Pos를 목적지로 함
            NavMeshPath path = new NavMeshPath();

            float minTriggerDist = 10000;
            float tempDist = 0;
            Vector3 tempTargetPosition;

            for (int i = 0; i < alleyBigTrigger.gateTrigger.Length; i++)
            {
                tempDist = 0;
                tempTargetPosition = alleyBigTrigger.gateTrigger[i].GetComponent<Transform>().position;

                //Debug.Log("target"+ i + "  " + tempTargetPosition);

                //<<추가>> 여기에 Alley 레이어마스크 씌워야하나?? 모르겠네 나중에 문제생기려나
                NavMesh.CalculatePath(m_Core.PlayerTr.position, tempTargetPosition, NavMesh.AllAreas, path);

                tempDist += Vector3.Distance(m_Core.PlayerTr.position, path.corners[0]);
                //Debug.Log("startDist" + i + "  " + m_Core.PlayerTr.position + "-" + path.corners[0] + " = " + Vector3.Distance(m_Core.PlayerTr.position, path.corners[0]));

                for (int j = 0; j < path.corners.Length - 1; j++)
                {
                    tempDist += Vector3.Distance(path.corners[j], path.corners[j + 1]);
                    //Debug.Log("inDist" + i + "-" + j + "  " + path.corners[j] + "-" + path.corners[j + 1] + " = " + Vector3.Distance(path.corners[j], path.corners[j + 1]));
                }

                tempDist += Vector3.Distance(path.corners[path.corners.Length - 1], tempTargetPosition);
                //Debug.Log("LastDist" + i + "  " + path.corners[path.corners.Length - 1] + "-" + tempTargetPosition + " = " + Vector3.Distance(path.corners[path.corners.Length - 1], tempTargetPosition));


                //Debug.Log("mindist  " + minTriggerDist + " / dist" + i + "  " + tempDist);

                if (tempDist < minTriggerDist)
                {
                    //Debug.Log("Changed" + i);

                    minTriggerDist = tempDist;
                    targetGatePos = tempTargetPosition;
                    targetCornerPos = alleyBigTrigger.gateTrigger[i].GetComponent<O_AlleyGateTrigger>().inCornerPos.position;
                    targetGateForwardPos = alleyBigTrigger.gateTrigger[i].GetComponent<O_AlleyGateTrigger>().gateForwardPos.position;
                }
            }


            // Debug.Log("TargetDist  " + minTriggerDist);
            // Debug.Log("TargetGatePos  " + targetGatePos);
            // Debug.Log("TargetGateForwardPos  " + targetGateForwardPos);


            // NavMesh.CalculatePath(m_Core.PlayerTr.position, targetGatePos, NavMesh.AllAreas, path);
            //this.GetComponent<LineRenderer>().SetVertexCount(path.corners.Length);

            // for (int i = 0; i < path.corners.Length; i++)
            // {
            //     this.GetComponent<LineRenderer>().SetPosition(i, path.corners[i]);
            // }



            //목적지까지의 path 경로 구함
            if (m_Core.NvAgent.enabled)
                m_Core.NvAgent.CalculatePath(targetGateForwardPos, path);

            //Debug.Log("PathCorner  " + path.corners.Length);

            //경로의 코너가 3개 이하이며, 일정 거리 이내로 목적지에 들어왔을 때
            if ((path.corners.Length < 5) && (Vector3.Distance(m_Core.Tr.position, targetGateForwardPos) < finTraceDist))
            {
                //기습 대기중 체크
                isFinTrace = true;


                //Debug.Log("Tracing Waiting");


                m_Core.NvAgent.speed = originSpeed;

                m_Core.NvAgent.Stop();
                m_Core.SetDestinationRealtime(false, null);
                m_Core.Animator.SetBool("IsRunning", false);


                //대기 시간이 일정 시간이 넘어가면 
                finTraceWaitTimeCounter += Time.deltaTime + 0.1f;

                if (finTraceWaitTimeCounter > finTraceWaitTimeLimit)
                {

                    //Debug.Log("NoMoreWait");


                    isFinTrace = false;
                    finTraceWaitTimeCounter = 0;


                    alleyState = M_AlleyState.NoWait;                       //보스 골목 안쪽 바라보기 상태로 변경
                }
            }
            else
            {
                //기습 위해 이동
                isFinTrace = false;

                m_Core.NvAgent.speed = alleyTracingSpeed;

                m_Core.NvAgent.Resume();
                m_Core.NvAgent.destination = targetGateForwardPos;
                m_Core.SetDestinationRealtime(false, null);
                m_Core.Animator.SetBool("IsRunning", true);

                finTraceWaitTimeCounter = 0;
            }
        }


        //플레이어의 위치를 아예 잃어버렸다면
        else
        {
            Debug.Log("Alley Tracing GoTo Patrol");


            SelectStartPatrol();


            Debug.Log("Start Patrol " + nowPatrolingPointIndex);


            alleyState = M_AlleyState.Patrol;                               //골목 외길 순찰로 상태 변경
        }
    }

    //골목 안쪽으로 따라가 바라봄
    void NoWait()
    {
        //골목 바깥 포인트로 이동
        m_Core.NvAgent.Resume();
        m_Core.NvAgent.destination = targetGateForwardPos;
        m_Core.SetDestinationRealtime(false, null);
        m_Core.Animator.SetBool("IsRunning", true);

        //골목 바깥 포인트에 도착하면 안쪽을 확인
        if (Vector3.Distance(m_Core.Tr.position, targetGateForwardPos) < 1.0f)
        {
            m_Core.NvAgent.Stop();
            m_Core.SetDestinationRealtime(false, null);
            m_Core.Animator.SetBool("IsRunning", false);

            StartCoroutine(CheckInAlley());
        }
    }

    //골목 순찰
    void Patrol() 
    {
        //플레이어를 발견했다면 다시 Tracing으로 상태 변경
        if (m_Core.CheckAuditoryField().isHearing)                  //플레이어가 청각 영역 안에 있으면
        {
            alleyState = M_AlleyState.Tracing;                      //골목 외길 순찰로 상태 변경
        }


        //현재 패트롤 지점으로 이동
        m_Core.NvAgent.speed = originSpeed;

        m_Core.NvAgent.Resume();
        m_Core.NvAgent.destination = nowPatrolingPos;
        m_Core.SetDestinationRealtime(false, null);
        m_Core.Animator.SetBool("IsRunning", true);


        if (Vector3.Distance(nowPatrolingPos, m_Core.Tr.position) < 1.0f)   //패트롤 지점에 충분히 가까워졌다면
        {
            if ((isStartPatroling) && (nowPatrolingPointIndex.Equals(patrolingEndIndex)))  //첫 지점 통과 후, 도착 지점에 도달한 것이면
            {
                Debug.Log("Alley Patrol Fin");

                m_Core.ChangeState(M_Patrol.instance);                      //순찰상태로 변경 
            }


            //다음으로 이동할 waypoint Index 지정
            if (isPatrolingClockwise)
            {
                nowPatrolingPointIndex++;                                   //현재 패트롤할 인덱스 증가

                if (nowPatrolingPointIndex > alleyBigTrigger.patrolWayPoints.Length - 1)  //패트롤 인덱스가 범위를 벗어나게 되면 
                    nowPatrolingPointIndex = 0;                             //0번부터 다시
            }
            else
            {
                nowPatrolingPointIndex--;                                   //현재 패트롤할 인덱스 감소

                if (nowPatrolingPointIndex < 0)                             //패트롤 인덱스가 범위를 벗어나게 되면 
                    nowPatrolingPointIndex = alleyBigTrigger.patrolWayPoints.Length - 1;  //마지막부터 다시
            }


            Debug.Log("Next Patrol " + nowPatrolingPointIndex);



            isStartPatroling = true;                                        //첫 지점 통과했습니다!


            //다음으로 이동할 Index에 맞는 waypoint 위치 지정 
            nowPatrolingPos = alleyBigTrigger.patrolWayPoints[nowPatrolingPointIndex].position;
        }
    }

    #endregion



    //브레스 2 사용체크
    IEnumerator CheckTimeToBreath_2()
    {
        float timeCounter = 0.0f;

        while (true)
        {
            timeCounter += Time.deltaTime;                          //시간 계산


            //브레스 2 사용 검토중이며 그 차례가 InSight일때
            if (isTrunOfCountInSight && m_Core.CheckSight().isPlayerInStraightLine)
            {
                isTrunOfCountInSight = false;
                countTurn++;
            }

            //브레스 2 사용 검토중이며 그 차례가 InSight가 아닐 때
            else if (!isTrunOfCountInSight && !m_Core.CheckSight().isPlayerInStraightLine)
            {
                isTrunOfCountInSight = true;
                countTurn++;
            }

            //시야 안밖을 왔다갔다 한 횟수가 일정 횟수를 넘어가면 
            if (countTurn >= maxTrun * 2)
            {
                Debug.Log("Alley Breath2 Use");

                StartCoroutine(M_AlleyBreath_2.instance.UseSkill(alleyInCornerPos));  //브레스 2 사용

                isCheckingBreath_2 = false;                         //브레스 2 검토 중단        
                yield break;
            }

            //일정 시간이 지나면 브레스 2 검토 중단
            if (timeCounter > checkTimeToBreath_2)
            {
                isCheckingBreath_2 = false;                         //브레스 2 검토 중단
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }


    //추적 체크
    IEnumerator CheckTracing()
    {
        Debug.Log("Alley Tracing Check Start");

        float timeCounter = 0.0f;

        isCheckTracing = true;

        while (true)
        {
            timeCounter += Time.deltaTime;                          //시간 계산

            if (!isCheckTracing)                                     //추적 검토가 외부에서 중단되었다면
                yield break;

            if ((timeCounter > checkTimeToBreath_2) &&              //브레스 2 사용을 위한 체크 시간이 다 지나갔고
                !m_Core.CheckSight().isPlayerInStraightLine)        //캐릭터가 시야에 없다면
            {
                Debug.Log("Alley GoTo Tracing");

                isCheckTracing = false;                             //추적 검토 중단
                isCheckingBreath_2 = false;                         //브레스 2 검토 중단
                alleyState = M_AlleyState.Tracing;                  //추적으로 상태 변경
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }



    //골목 안쪽을 확인
    IEnumerator CheckInAlley()
    {
        yield return StartCoroutine(RotateToPoint(m_Core.transform, targetGatePos, lookRotationTime));

        if (m_Core.CheckSight().isPlayerInStraightLine)                     //플레이어가 직선거리 상에 위치하고 있으면
        {
            //정보 갱신
            alleyGateTriggerPos = targetGatePos;
            alleyGateForwardPos = targetGateForwardPos;
            alleyInCornerPos = targetCornerPos;

            alleyState = M_AlleyState.Starting;                             //골목 시작 상태로 변경
        }
        else
        {

            Debug.Log("Alley CheckInAlley GoTo Patrol");

            SelectStartPatrol();

            Debug.Log("Start Patrol " + nowPatrolingPointIndex);

            alleyState = M_AlleyState.Patrol;                               //골목 순찰 상태로 변경
        }
    }

    //패트롤 할 시작지점을 선택
    void SelectStartPatrol()
    {
        isStartPatroling = false;                                           //패트롤 첫 지점 통과 false

        if (alleyBigTrigger.isPatrolLooping)                                //순찰 지점이 순환 가능하다면
        {
            //현재 자신과 가장 가까운 Nav거리 상의 패트롤 지점 선택
            float minTriggerDist = 10000.0f;

            for (int i = 0; i < alleyBigTrigger.patrolWayPoints.Length; i++)
            {
                float tempDist = m_Core.CheckNevDist(alleyBigTrigger.patrolWayPoints[i].position);

                if (tempDist < minTriggerDist)                              //가장 가까운 지점에서 순찰 시작
                {
                    minTriggerDist = tempDist;

                    nowPatrolingPointIndex = i;
                    patrolingStartIndex = i;
                    patrolingEndIndex = i;
                    nowPatrolingPos = alleyBigTrigger.patrolWayPoints[i].position;
                }
            }

            //<<나중추가>>  기획쪽에서 이 순찰 방향을 결정할 로직이 세워지면 변경
            isPatrolingClockwise = Random.Range(0, 1).Equals(0) ? true : false;  //순찰 방향 결정
        }

        else                                                                //순찰 지점이 순환하지 않는다면
        {
            //패트롤의 시작지점과 끝지점 중에서 Nav거리 상 가까운 쪽 선택 
            float tempStartDist = m_Core.CheckNevDist(alleyBigTrigger.patrolWayPoints[0].position);
            float tempEndDist = m_Core.CheckNevDist(alleyBigTrigger.patrolWayPoints[alleyBigTrigger.patrolWayPoints.Length - 1].position);

            if (tempStartDist < tempEndDist)                                //시작 지점이 더 가까우면 시작 지점에서 순찰 시작
            {
                nowPatrolingPointIndex = 0;
                patrolingStartIndex = 0;
                patrolingEndIndex = alleyBigTrigger.patrolWayPoints.Length - 1;
                nowPatrolingPos = alleyBigTrigger.patrolWayPoints[0].position;

                isPatrolingClockwise = true;
            }
            else                                                            //끝 지점이 더 가까우면 끝 지점에서 순찰 시작
            {
                nowPatrolingPointIndex = alleyBigTrigger.patrolWayPoints.Length - 1;
                patrolingStartIndex = alleyBigTrigger.patrolWayPoints.Length - 1;
                patrolingEndIndex = 0;
                nowPatrolingPos = alleyBigTrigger.patrolWayPoints[alleyBigTrigger.patrolWayPoints.Length - 1].position;

                isPatrolingClockwise = false;
            }

        }
    }



    #region 골목 체크 

    //플레이어 골목 진입
    public void EnterAlley(O_AlleyTrigger alley)
    {
        //해당 골목 정보 가져오기
        nowInAlleyIndex = alley.alleyIndex;
        alleyBigTrigger = alley;
    }

    //플레이어 골목 이탈
    public void ExitAlley()
    {
        nowInAlleyIndex = -1;

        if ((m_Core.CheckSight().isPlayerInSight)                               //시야에 플레이어가 있거나
            || (m_Core.CheckAuditoryField().isHearing))                         //청역범위에 플레이어가 있으면
            m_Core.ChangeState(M_Attack.instance);                              //공격상태로 변경

        else                                                                    //플레이어 감지 불가능 상태라면 
            m_Core.ChangeState(M_Patrol.instance);                              //순찰 상태로 변경
    }


    //대기 시 플레이어가 GateTrigger를 밟으면 플레이어 덮침
    public void CheckAlleying(GameObject obj, Vector3 gatePoint, Vector3 forwardPoint, Vector3 inCornerPoint)
    {
        if (!isAlleyEnterCheck                                                  //골목 진입 체크를 했는가
           || isFinTrace)                                                       //혹은 추격 대기를 완료했는가
        {
            //정보 갱신
            alleyGateTrigger = obj;                                             //으 브레스땜시 어쩔 수 없긴 한데 넘 더럽다

            alleyGateTriggerPos = gatePoint;
            alleyGateForwardPos = forwardPoint;
            alleyInCornerPos = inCornerPoint;

            Debug.Log("Alley GateTrigger STEP  " + alleyGateTrigger.name);

            isAlleyEnterCheck = true;                                           //진입 체크 완료

        }

        if (isFinTrace)                                                         //현재 추적을 끝내고 대기상태인지
        {
            MonCheckHoldDown();                                                 //플레이어 덮침
        }
    }


    //플레이어 덮침
    void MonCheckHoldDown()
    {
        m_Core.delayTime = 0.5f;                                                //딜레이 설정

        isFinTrace = false;                                                     //상태 변환

        //확률에 따라 브레스 1과 점프 어택 중 택 1
        Random.seed = (int)System.DateTime.Now.Ticks;                           //랜덤값의 시드를 무작위로 만든다
        int randomChance = Random.Range(0, 1000);



        if (randomChance < 500)
        {
            alleySkillState = M_AlleySkillState.JumpAttack;
            StartCoroutine(M_JumpAttack.instance.UseSkill(alleyGateForwardPos));    //점프 어택으로 덮침
        }
        else
        {
            alleySkillState = M_AlleySkillState.Breath;
            StartCoroutine(M_Breath.instance.UseSkill(alleyGateForwardPos));        //브레스로 덮침
        }

        alleyState = M_AlleyState.Starting;                                         //골목 스타트 상태로 변경
    }

    #endregion



    //몬스터 경직
    public override void MonRigid()
    {
        Debug.Log(alleySkillState);

        switch (alleySkillState)
        {
            case M_AlleySkillState.None:
                {
                    base.MonRigid();
                }
                break;

            case M_AlleySkillState.ForeFootPress:
                {
                    //경직 불가
                }
                break;

            case M_AlleySkillState.Breath:
                {
                    StopAllCoroutines();
                    M_Breath.instance.CancelSkill();

                    base.MonRigid();
                }
                break;

            case M_AlleySkillState.Magic_1:
                {
                    StopAllCoroutines();
                    M_Magic_1.instance.CancelSkill();

                    base.MonRigid();
                }
                break;

            case M_AlleySkillState.Magic_3:
                {
                    StopAllCoroutines();
                    M_Magic_3.instance.CancelSkill();

                    base.MonRigid();
                }
                break;

            case M_AlleySkillState.AlleyBreath_1:
                {
                    //경직 불가
                }
                break;

            case M_AlleySkillState.JumpAttack:
                {
                    //일단은 경직 불가
                    //<<나중추가>>  점프 준비때에는 가능하나, 점프 도중에는 경직이 먹지 않는다  점프 도중을 체크하고 그때만 경직 먹게 하자
                }
                break;
        }
    }


    //상태 진입
    public override void Enter()
    {
        //진입점이 대기 또는 순찰상태였다면 플레이어와 실 거리가 가장 가까운 입구쪽을 목표 지점으로 시작한다
        if (isStartToIdleOrPatrol)
        {
            isStartToIdleOrPatrol = false;

            //목적지까지의 경로를 구함
            NavMeshPath path = new NavMeshPath();

            //플레이어와 가까운 쪽의 Gate에 저장된 Pos를 목적지로 함
            float minTriggerDist = 10000;
            float tempDist = 0;
            Vector3 tempTargetPosition;

            for (int i = 0; i < alleyBigTrigger.gateTrigger.Length; i++)
            {
                tempDist = 0;
                tempTargetPosition = alleyBigTrigger.gateTrigger[i].GetComponent<Transform>().position;

                NavMesh.CalculatePath(m_Core.PlayerTr.position, tempTargetPosition, NavMesh.AllAreas, path);

                tempDist += Vector3.Distance(m_Core.PlayerTr.position, path.corners[0]);    //현재 플레이어 위치에서 경로 첫 지점까지의 거리부터 더하기 시작

                for (int j = 0; j < path.corners.Length - 1; j++)                           //경로 지점의 선분 갯수만큼 반복하며
                {
                    //저장된 경로를 따라 경로 지점 사이의 거리를 구해 총 거리를 계산한다
                    tempDist += Vector3.Distance(path.corners[j], path.corners[j + 1]);
                }

                tempDist += Vector3.Distance(path.corners[path.corners.Length - 1], tempTargetPosition);


                if (tempDist < minTriggerDist)
                {
                    minTriggerDist = tempDist;


                    //정보 갱신
                    alleyGateTrigger = alleyBigTrigger.gateTrigger[i];                      //으 브레스땜시 어쩔 수 없긴 한데 넘 더럽다 이거 분명 다른데에서 더 깔끔하게 가져올 수 있을텐데 일단은


                    Debug.Log("Alley GateTrigger After THINK  " + alleyGateTrigger.name);


                    alleyGateTriggerPos = alleyBigTrigger.gateTrigger[i].GetComponent<Transform>().position;
                    alleyGateForwardPos = alleyBigTrigger.gateTrigger[i].GetComponent<O_AlleyGateTrigger>().gateForwardPos.position;
                    alleyInCornerPos = alleyBigTrigger.gateTrigger[i].GetComponent<O_AlleyGateTrigger>().inCornerPos.position;
                }
            }
        }


        originSpeed = m_Core.NvAgent.speed;


        alleyState = M_AlleyState.Starting;                                 //Starting상태로 시작

        //////Debug.Log("Enter Alley");
    }

    //상태 이탈                   
    public override void Exit()
    {
        m_Core.NvAgent.speed = originSpeed;                                 //원래 속도로 변경
        isAlleyEnterCheck = false;                                          //진입 체크 미완료로 변경

        //////Debug.Log("Exit Alley");
    }
}

