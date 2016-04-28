using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Monster Alley State
/// 
/// *코멘트
///     <<추가완료>>  경직 적용
///     <<추가완료>>  순찰 시 청역 안(?)에 플레이어 감지하면 Tracing으로 전환
///     <<추가완료>>  시야 안 공격 시 확률에 따라 마법 1과 마법 3 중 1 택
///     <<추가완료>>  대기2 시 확률에 따라 브레스 1과 점프 어택 중 택 1
///     <<추가완료>>  플레이어가 일정 시간동안 보였다(InSight) 안보였다(Tracing) 반복하면 브레스 2 발동
///     인식에서 벗어난 상태에서 도로 밖으로 나갔을 때 어떤 상태가 됩니까 기획자님 호엑  -> 임시로 Patrol로 변경
///     Attack와 겹치는 스킬이 넘 많다 그냥 스킬매니저 만들까 거기서 다 가져와 쓰게
///     코루틴 함수를 가져와 쓰는거랑 IEmurator 변수 가져와 쓰는거랑 왜 다른걸까???
/// </summary>



public class M_Alley : M_FSMState
{
    #region SingleTon

    public static M_Alley instance = null;

    void Awake()
    { instance = this; }

    #endregion


    private M_AlleyState alleyState = M_AlleyState.None;                    //골목 상태  
    
    private O_AlleyTrigger alleyBigTrigger;                                 //현재 위치 골목 Trigger 스크립트

    private int nowInAlleyIndex = -1;                                       //현재 위치한 골목 Index

    private Vector3 alleyGateTriggerPos;                                    //연락 온 골목 Trigger위치
    private Vector3 alleyGateForwardPos;                                    //연락 온 골목 Trigger에 연결된 도로측 위치
    private Vector3 alleyInCornerPos;                                       //연락 온 골목 Trigger에 연결된 골목 안 코너 위치


    public float alleyStartingTimeLimit = 0.5f;                             //골목 추격 시간 제한
    private float alleyStartingTimeCounter = 0f;                            //골목 추격 시간 카운터


    private bool isCheckingBreath_2 = false;                                //브레스 2 사용을 검토중인가
    public float checkTimeToBreath_2 = 3.0f;                                //브레스 2 사용을 위한 타임 카운트
    private bool isTrunOfCountInSight = false;                              //시야 안 카운트를 할 시점인가
    public int maxTrun = 3;                                                 //브레스 2 사용을 위해 시야 안밖을 왔다갔다 해야 하는 횟수
    private int countTurn = 0;                                              //시야 안밖을 왔다갔다한 횟수 (2번을 1회로 친다)

    
    private Vector3 targetGatePos;                                          //추격할 골목 문 위치
    public float finTraceDist = 30.0f;                                      //추격 제한 거리
    private bool isFinTrace = false;                                        //골목 내 추격 완료 및 대기중 여부


    private bool isStartPatroling = false;                                  //Patrol 첫 지점을 통과했는가
    private int nowPatrolingPointIndex = -1;                                //현재 Patrol 중인 waypoint Index
    private Vector3 nowPatrolingPos;                                        //현재 Patrol 목표 waypoint
    private bool isPatrolingClockwise;                                      //시계방향 (+ 방향)으로 Ptarol 하는가
    private int patrolingStartIndex = -1;                                   //Patrol 시작 Index
    private int patrolingEndIndex = -1;                                     //Patrol 끝 Index 

    public float lookRotationTime = 0.5f;                                   //플레이어를 바라볼 회전 시간


    #region  스킬

    //공격용 콜리더
    public GameObject[] monAttkArms;                                //몬스터 공격용 팔 Collider
    public GameObject monAttkBody;                                  //몬스터 공격용 바디 Collider 

    //골목 앞발찍기
    public int alleyForeFootPressDamage = 20;                       //골목 앞발찍기 데미지
    public float alleyForeFootPressTime = 3.0f;                     //골목 앞발찍기 시간

    //골목 브레스 1
    public GameObject monAlleyBreath;                               //골목 브레스 오브젝트
    private Vector3 alleyBreathEndScale = new Vector3(3.0f, 20.0f, 3.0f); //골목 브레스 콜리더 최종 크기
    public int alleyBreathDamage = 20;                              //골목 브레스 데미지
    public float alleyBreathTime = 2.0f;                            //골목 브레스 시간

    //골목 브레스 2
    //public GameObject monAlleyBreath_2;                              //골목 브레스 데미지
    //public int alleyBreath_2_Damage = 20;                            //골목 브레스 데미지
    //public float alleyBreate_2_AfterDelayTime = 2.0f;                //골목 브레스 후 딜레이
    //public Transform alleyBreath_2_Pivot;                            //골목 브레스가 발동될 Pivot

    //점프 공격
    public int jumpAttkDamage = 20;                                 //점프공격 데미지
    public float jumpAttackBeforeDelayTime = 0.3f;                  //점프 대기 시간
    public float jumpAttkJumpTime = 0.43f;                          //점프하는 시간
    public float jumpAttackAfterDelayTime = 1.0f;                   //점프 후 딜레이

    #endregion



    //상태 초기화
    public override void FSMInitialize()
    {
        topState = M_TopState.Alley;                                        //이 상태는 Alley입니다

        //공격용 콜리더 비활성화
        foreach (GameObject armObj in monAttkArms)
        { armObj.SetActive(false); }
        monAttkBody.SetActive(false);
        monAlleyBreath.SetActive(false);
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
        alleyStartingTimeCounter += (Time.deltaTime + 0.2f);                //쫒는 데 걸린 시간을 카운트한다

        if (Vector3.Distance(m_Core.Tr.position, alleyGateForwardPos) < 1.0f)  //도착지점과의 거리가 충분히 가까워지면
        {
            if (alleyStartingTimeCounter < alleyStartingTimeLimit)          //쫒는 데 걸린 시간이 일정 시간 안일 때
            {
                m_Core.NvAgent.Stop();
                m_Core.Animator.SetBool("IsRunning", false);

                StartCoroutine(this.MonAlleyForeFootPress(m_Core.PlayerTr.position));  //앞발찍기 사용
            }
            
            alleyStartingTimeCounter = 0;
            alleyState = M_AlleyState.InSight;                              //시야 안으로 상태 변경
        }
        else                                                                //도착지점까지의 거리가 충분히 가깝지 않다면
        {
            //골목으로 플레이어를 추격
            m_Core.NvAgent.Resume();
            m_Core.NvAgent.destination = alleyGateForwardPos;
            m_Core.Animator.SetBool("IsRunning", true);
        }
    }

    //골목 안쪽으로 플레이어가 보일 때
    void InSight() 
    {
        if (m_Core.CheckSight().isPlayerInStraightLine)                     //플레이어가 직선거리 상에 위치하고 있으면 <-직선거리 상? 아니면 시야각 적용?
        {
            if (alleyInCornerPos != Vector3.zero)                           //골목 안 코너가 존재하며    
            {
                if (Vector3.Distance(alleyInCornerPos, m_Core.PlayerTr.position) < 10.0f)  //그 코너에 플레이어가 충분히 가깝다면 브레스 사용 가능 거리
                {
                    //Debug.Log("Alley Trace Dist " + Vector3.Distance(alleyInCornerPos, m_Core.Tr.position));


                    alleyBreathEndScale = new Vector3(1.0f, Vector3.Distance(alleyInCornerPos, m_Core.Tr.position), 1.0f);
                    StartCoroutine(this.MonAlleyBreath01());                //골목 안쪽을 향하여 브레스 1 사용
                }
                else                                                        //브레스 사용 불가 거리라면
                {
                    //확률에 따라 마법 1과 마법 3 중 1 택
                    Random.seed = (int)System.DateTime.Now.Ticks;           //랜덤값의 시드를 무작위로 만든다
                    int randomChance = Random.Range(0, 1000);
                    

                    if(randomChance < 500)
                    {
                        //StartCoroutine(M_Attack.instance.magic_1_Corutine);     //마법 1 사용 <-??? 2회째부턴 작동 안됨
                        //StartCoroutine(M_Attack.instance.MonMagic_1());         //마법 1 사용
                        StartCoroutine(M_Magic_1.instance.UseSkill(m_Core.PlayerTr.position));  //마법 1 사용
                    }
                    else
                    {
                        //<<추가>> 마법 3 사용
                    }
                }
            }
            else                                                            //골목에 코너가 없고 일직선이라면
            {
                alleyBreathEndScale = new Vector3(1.0f, Vector3.Distance(alleyInCornerPos, m_Core.Tr.position), 1.0f);
                StartCoroutine(this.MonAlleyBreath01());                    //브레스 1 사용
            }



            //브레스 2 사용 검토중이며 그 차례가 InSight일때
            if (isCheckingBreath_2 && isTrunOfCountInSight)
            {
                isTrunOfCountInSight = false;
                countTurn++;
            }
        }
        else                                                                //플레이어가 직선거리 상에 위치하지 않았으면
        {
            //브레스 2 사용 검토를 하지 않고 있었다면
            if (!isCheckingBreath_2)
            {
                isCheckingBreath_2 = true;                                  //브레스 2 사용 검토중 체크
                isTrunOfCountInSight = true;                                //다음 차례는 InSight
                countTurn = 1;                                              //이 자체로 이미 1회로 친다
                StartCoroutine(CheckTimeToBreath_2());
            }


            //브레스 2 사용 검토중이며 그 차례가 InSight가 아닐 때
            else if (isCheckingBreath_2 && !isTrunOfCountInSight)
            {
                isTrunOfCountInSight = true;
                countTurn++;
            }
        }
    }
    
    //플레이어가 보이지 않아 추격
    void Tracing() 
    {
        m_Core.delayTime = 0.2f;                                            //업데이트 주기 설정


        if (m_Core.CheckAuditoryField().isHearing)                          //플레이어가 청각 영역 안에 있으면
        {
            //플레이어와 가까운 쪽의 Gate에 저장된 Pos를 목적지로 함
            float minTriggerDist = 10000;

            for (int i = 0; i < alleyBigTrigger.gateTrigger.Length; i++)    
            {
                float tempDist = Vector3.Distance(alleyBigTrigger.gateTrigger[i].GetComponent<Transform>().position,
                    m_Core.PlayerTr.position);

                if (tempDist < minTriggerDist)
                {
                    minTriggerDist = tempDist;
                    targetGatePos = alleyBigTrigger.gateTrigger[i].GetComponent<O_AlleyGateTrigger>().gateForwardPos.position;
                }
            }

            //목적지까지의 경로를 구함
            NavMeshPath path = new NavMeshPath();

            if (m_Core.NvAgent.enabled)
                m_Core.NvAgent.CalculatePath(targetGatePos, path);


            //경로의 코너가 3개 이하이며, 일정 거리 이내로 목적지에 들어왔을 때
            if ((path.corners.Length < 3) && (Vector3.Distance(m_Core.Tr.position, targetGatePos) < finTraceDist))
            {
                isFinTrace = true;
                m_Core.NvAgent.Stop();
                m_Core.Animator.SetBool("IsRunning", false);


                //<<추가>>  대기하는 시간이 일정 시간 이상 되면 골목 안쪽을 바라보게 한다 -> 골목 포인트쪽으로 이동해서 안쪽을 향함
            }
            else
            {
                isFinTrace = false;
                m_Core.NvAgent.Resume();
                m_Core.NvAgent.destination = targetGatePos;
                m_Core.Animator.SetBool("IsRunning", true);
            }
        }


        else                                                                //플레이어의 위치를 잃어버렸다면
        {
            isStartPatroling = false;                                       //패트롤 첫 지점 통과 false

            if (alleyBigTrigger.isPatrolLooping)                            //순찰 지점이 순환 가능하다면
            {
                //현재 자신과 가장 가까운 Nav거리 상의 패트롤 지점 선택
                float minTriggerDist = 10000.0f;

                for (int i = 0; i < alleyBigTrigger.patrolWayPoints.Length; i++)
                {
                    float tempDist = m_Core.CheckNevDist(alleyBigTrigger.patrolWayPoints[i].position);

                    if (tempDist < minTriggerDist)                          //가장 가까운 지점에서 순찰 시작
                    {
                        minTriggerDist = tempDist;

                        nowPatrolingPointIndex = i;
                        patrolingStartIndex = i;
                        patrolingEndIndex = i;
                        nowPatrolingPos = alleyBigTrigger.patrolWayPoints[i].position;
                    }
                }

                //<<추가>>  기획쪽에서 이 순찰 방향을 결정할 로직이 세워지면 변경
                isPatrolingClockwise = Random.Range(0, 1).Equals(0) ? true : false;  //순찰 방향 결정

                alleyState = M_AlleyState.Patrol;                           //골목 순환 순찰로 상태 변경
            }


            else                                                            //순찰 지점이 순환하지 않는다면
            {
                //패트롤의 시작지점과 끝지점 중에서 Nav거리 상 가까운 쪽 선택  <- 오우... 넘 비효율적인거같아... 이 무수한 GetComponent 오또케 할까나...
                float tempStartDist = m_Core.CheckNevDist(alleyBigTrigger.patrolWayPoints[0].position);
                float tempEndDist = m_Core.CheckNevDist(alleyBigTrigger.patrolWayPoints[alleyBigTrigger.patrolWayPoints.Length - 1].position);

                if (tempStartDist < tempEndDist)                            //시작 지점이 더 가까우면 시작 지점에서 순찰 시작
                {
                    nowPatrolingPointIndex = 0;
                    patrolingStartIndex = 0;
                    patrolingEndIndex = alleyBigTrigger.patrolWayPoints.Length - 1;
                    nowPatrolingPos = alleyBigTrigger.patrolWayPoints[0].position;

                    isPatrolingClockwise = true;
                }
                else                                                        //끝 지점이 더 가까우면 끝 지점에서 순찰 시작
                {
                    nowPatrolingPointIndex = alleyBigTrigger.patrolWayPoints.Length - 1;
                    patrolingStartIndex = alleyBigTrigger.patrolWayPoints.Length - 1;
                    patrolingEndIndex = 0;
                    nowPatrolingPos = alleyBigTrigger.patrolWayPoints[alleyBigTrigger.patrolWayPoints.Length - 1].position;

                    isPatrolingClockwise = false;
                }

                alleyState = M_AlleyState.Patrol;                   //골목 외길 순찰로 상태 변경
            }
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
        m_Core.NvAgent.Resume();
        m_Core.NvAgent.destination = nowPatrolingPos;
        m_Core.Animator.SetBool("IsRunning", true);

        
        if (Vector3.Distance(nowPatrolingPos, m_Core.Tr.position) < 1.0f)   //패트롤 지점에 충분히 가까워졌다면
        {
            //Debug.Log("Alley Change Patrol Waypoint " + nowPatrolingPointIndex + " " + patrolingEndIndex);


            if ((isStartPatroling) && (nowPatrolingPointIndex.Equals(patrolingEndIndex)))  //첫 지점 통과 후, 도착 지점에 도달한 것이면
            {
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

        while(true)
        {
            timeCounter += Time.deltaTime;                          //시간 계산

            if (countTurn >= maxTrun * 2)                           //시야 안밖을 왔다갔다 한 횟수가 일정 횟수를 넘어가면 
            {
                //<<추가>>  브레스 2 사용                          
                                                                    //브레스 2 사용

                isCheckingBreath_2 = false;                         //브레스 2 검토 중단        
                yield break;
            }


            //<<추가>>  오랫동안 계속 안보이면 추적

            else if (timeCounter > checkTimeToBreath_2)             //브레스 2 사용을 위한 체크 시간이 다 지나갔으면
            {
                isCheckingBreath_2 = false;                         //브레스 2 검토 중단
                alleyState = M_AlleyState.Tracing;                  //추적으로 상태 변경
                yield break;
            }

            yield return new WaitForEndOfFrame();
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

        if ((m_Core.CheckSight().isPlayerInSight)                           //시야에 플레이어가 있거나
            || (m_Core.CheckAuditoryField().isHearing))                     //청역범위에 플레이어가 있으면
            m_Core.ChangeState(M_Attack.instance);                          //공격상태로 변경

        else                                                                //플레이어 감지 불가능 상태라면 
            m_Core.ChangeState(M_Patrol.instance);                          //순찰 상태로 변경
    }


    //대기 시 플레이어가 GateTrigger를 밟으면 플레이어 덮침
    public void CheckAlleying(Vector3 gatePoint, Vector3 forwardPoint, Vector3 inCornerPoint)
    {
        //정보 갱신
        alleyGateTriggerPos = gatePoint;
        alleyGateForwardPos = forwardPoint;
        alleyInCornerPos = inCornerPoint;

        MonCheckHoldDown();                                                 //플레이어 덮침
    }

    //플레이어 덮침
    void MonCheckHoldDown()
    {
        m_Core.delayTime = 0.5f;                                            //딜레이 설정

        if (isFinTrace)                                                     //현재 추적을 끝내고 대기상태인지
        {
            isFinTrace = false;                                             //상태 변환

            //확률에 따라 브레스 1과 점프 어택 중 택 1
            Random.seed = (int)System.DateTime.Now.Ticks;                   //랜덤값의 시드를 무작위로 만든다
            int randomChance = Random.Range(0, 1000);

            StartCoroutine(M_JumpAttack.instance.UseSkill(alleyGateForwardPos));  //점프 어택으로 덮침

            //if (randomChance < 500)
            //{
            //    StartCoroutine(this.MonJumpAttack(alleyGateForwardPos));        //점프 어택으로 덮침
            //}
            //else
            //{
            //    //<<추가>>  브레스 1 여기에 못써요ㅠㅠ 브레스 1 공격 거리를 골목 안쪽을 체크해서 해요ㅠㅠ 여긴 골목이 없어...ㅠ...
            //}

            alleyState = M_AlleyState.Starting;                             //골목 스타트 상태로 변경
        }
    }

    #endregion



    #region 스킬

    //앞발찍기
    IEnumerator MonAlleyForeFootPress(Vector3 pos)
    {
        //플레이어를 향해 회전
        yield return StartCoroutine(this.RotateToPoint(m_Core.transform, m_Core.PlayerTr.position, lookRotationTime));

        m_Core.IsDoingOther = true;                                         //행동 시작


        m_Core.Animator.SetTrigger("ForeFootPress");                        //애니메이션 실행

        monAttkArms[0].SetActive(true);                                     //왼팔의 콜리더 활성화

        yield return new WaitForSeconds(alleyForeFootPressTime);            //스킬 사용 시간동안 대기

        monAttkArms[0].SetActive(false);                                    //왼팔의 콜리더 비활성화


        m_Core.IsDoingOther = false;                                        //행동 종료
    }
    
    //골목 브레스 1
    IEnumerator MonAlleyBreath01()
    {
        Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);                 //원본 사이즈
        float breathSize = 0;

        //플레이어를 향해 회전
        yield return StartCoroutine(this.RotateToPoint(m_Core.transform, m_Core.PlayerTr.position, lookRotationTime));


        m_Core.IsDoingOther = true;                                         //행동 시작


        m_Core.Animator.SetTrigger("Breath");                               //애니메이션 실행

        monAlleyBreath.SetActive(true);                                     //브레스 활성화

        while (breathSize <= 1)                                             //브레스 사이즈 점점 키우기
        {
            monAlleyBreath.GetComponent<Transform>().localScale = Vector3.Lerp(startScale, alleyBreathEndScale, breathSize);
            breathSize += 0.01f / alleyBreathTime;

            yield return new WaitForSeconds(0.01f);
        }

        monAlleyBreath.GetComponent<Transform>().localScale = startScale;   //다시 원래 사이즈로 축소

        monAlleyBreath.SetActive(false);                                    //브레스 비활성화


        m_Core.IsDoingOther = false;                                        //행동 종료 
    }

    //<<추가>>  골목 브레스 2 작성.  골목을 따라 투사체가 플레이어 위치로 이동하는 스킬
    


    //<<추가>>  마법 3 작성.  포물선으로 3개의 투사체를 날려 플레이어 위치 근처에서 폭발 -> 내용은 미확정인듯? 기획서에 없네





    //점프공격
    IEnumerator MonJumpAttack(Vector3 pos)
    {
        //플레이어를 향해 회전
        yield return StartCoroutine(this.RotateToPoint(m_Core.transform, pos, lookRotationTime));

        m_Core.IsDoingOther = true;                                         //행동 시작


        monAttkBody.SetActive(true);                                        //몸통의 콜리더 활성화

        Vector3 startPosition = m_Core.transform.position;                  //출발 위치
        float jumpDistance = 0;

        m_Core.Animator.SetTrigger("JumpAttack");                           //애니메이션 실행

        yield return new WaitForSeconds(jumpAttackBeforeDelayTime);         //점프공격 대기

        //플레이어의 위치를 받아 점프.
        while (jumpDistance <= 1)
        {
            m_Core.transform.position = Vector3.Lerp(startPosition, pos, jumpDistance);
            jumpDistance += 0.01f / jumpAttkJumpTime;

            yield return new WaitForSeconds(0.01f);
        }

        monAttkBody.SetActive(false);                                       //몸통의 콜리더 비활성화

        yield return new WaitForSeconds(jumpAttackAfterDelayTime);          //점프공격 대기


        m_Core.IsDoingOther = false;                                        //행동 종료 
    }

    #endregion



    //몬스터 경직
    public override void MonRigid()
    {
        //<<추가>> 어떤 스킬은 캔슬되지 말아야 한다

        StopAllCoroutines();

        //공격용 콜리더 비활성화
        foreach (GameObject armObj in monAttkArms)
        { armObj.SetActive(false); }
        monAttkBody.SetActive(false);


        base.MonRigid();
    }


    //상태 진입
    public override void Enter()
    {
        alleyState = M_AlleyState.Starting;                                 //Starting상태로 시작

        //////Debug.Log("Enter Alley");
    }

    //상태 이탈                   
    public override void Exit()
    {
        //////Debug.Log("Exit Alley");
    }
}

