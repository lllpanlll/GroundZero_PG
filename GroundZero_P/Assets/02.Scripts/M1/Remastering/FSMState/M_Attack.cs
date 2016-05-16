using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Monster Attack State
/// 도로에서 플레이어를 추격하며 전투하는 상태.
/// 
/// *코멘트
///     <<추가완료>>  경직 적용 
///     <<추가완료>>  Cycle의 스킬들이 스킬세트1과 스킬세트2로 나눠 사용
///     <<추가완료>>  스킬세트 1 - 마법 1   500 / 마법 2   500 
///     <<추가완료>>  스킬세트 2 - 에너지 방출   1000
///     <<추가완료>>  스킬세트 1 사용 시 이동 가능
/// </summary>


public enum M_AttackSkillSetState       //스킬 세트 1(약공격)  스킬 세트 2(강공격)  사용여부
{
    None = 0,
    SkillSet_1,
    SkillSet_2
}


public enum M_AttackSkillState          //공격 상태 사용 스킬
{
    None = 0,
    BodyPress,
    Magic_1,
    Magic_2,
    Magic_4,
    EnergyEmission
}



public class M_Attack : M_FSMState
{
    #region SingleTon

    public static M_Attack instance = null;
    
    void Awake()
    { instance = this; }

    #endregion
    
       
    private M_AttackState attackState = M_AttackState.None;                         //공격 상태

    private M_AttackSkillSetState skillSetState = M_AttackSkillSetState.None;       //사용 중인 SkillSet 
    private M_AttackSkillState attackSkillState = M_AttackSkillState.None;          //사용 중인 스킬 상태
    public M_AttackSkillState AttackSkillState { set { attackSkillState = value; } }

    private bool isCycling = false;                                                 //사이클 실행중 여부


    private Transform realtimeDestination;                                          //쫓아야 할 목표 Transform   
    //<<추가>> 뭔가 NavMeshAgent.Velocity와 Animator Parametor Float(Speed) 로 어떻게 해결 가능할 거 같은데 이런 거 없이도 이걸로 애니메이션 속도를 조절해도 좋고
    private bool isMustCheckDistToPlayer = false;                                   //플레이어와의 거리를 체크할 필요가 있는가


    public float roadStoppingDistance = 10.0f;                                      //도로 전투 시 제동 거리



    //상태 초기화
    public override void FSMInitialize()
    {
        topState = M_TopState.Attack;                                               //이 상태는 Attack입니다

        isCycling = false;                                                          //사이클 실행중
    }


    //상태 Update   
    public override void FSMUpdate()
    {
        m_Core.delayTime = 0.0f;                                                    //업데이트 주기 설정 -> 스킬 사용 판단은 딜레이 없이!


        #region 하위 FSM

        attackState = m_Core.CheckDist().sightInCycleState;                         //거리 판단

        switch (attackState)                       
        {
            case M_AttackState.None:                                                //거리 판단이 안 되어있을 때는 에러 출력
                Debug.LogError("NoneCycleState!!!");
                break;

            case M_AttackState.UnderCycle:                                          //사이클 범위 미만
                UnderCycle();
                break;

            case M_AttackState.InCycle:                                             //사이클 범위 내
                InCycle();
                break;

            case M_AttackState.OverCycle:                                           //사이클 범위 초과
                OverCycle();
                break;
        }

        #endregion
        
        //<<나중추가>>  일정 (짧은)시간동안 큰 피해를 입으면 도주 상태로 변경한다
    }


    //상태 매 프레임 Update
    public override void FSMMustUpdate()
    {
        //매 프레임 목표하는 Trnasform의 position을 쫓아가야 한다면 위치 갱신
        if (realtimeDestination)
            m_Core.NvAgent.destination = realtimeDestination.position;
        
        //플레이어와의 거리를 체크해야 한다면 
        if(isMustCheckDistToPlayer)
        {
            //플레이어와 일정 거리 이상 가까워지면 Idle
            if(m_Core.NvAgent.remainingDistance > m_Core.NvAgent.stoppingDistance + 2.0f)
                m_Core.Animator.SetBool("IsRunning", true);
            else
                m_Core.Animator.SetBool("IsRunning", false);
        }
    }



    #region 하위 상태 

    //사이클 범위 미만
    void UnderCycle() 
    {
        m_Core.NvAgent.Stop();
        realtimeDestination = null;
        isMustCheckDistToPlayer = false;
        m_Core.Animator.SetBool("IsRunning", false);

        isCycling = false;                                                          //사이클 실행중이 아님


        //바디프레스 실행
        attackSkillState = M_AttackSkillState.BodyPress;
        StartCoroutine(M_BodyPress.instance.UseSkill(m_Core.PlayerTr.position));
    }

    //사이클 범위 내
    void InCycle() 
    {
        //플레이어가 직선거리상에 위치하고 있다면 공격 실행
        if (m_Core.CheckSight().isPlayerInStraightLine)                             
        {
            //사이클 실행중이 아니었으면 사이클 실행
            if (!isCycling)                                                         
            {
                isCycling = true;                                         
                skillSetState = M_AttackSkillSetState.SkillSet_1;                   //시작은 스킬 세트 1부터

                m_Core.delayTime = 0.8f;                                            //진입 시점에서 살짝 딜레이
            }

            //스킬을 선택할 때 사용할 랜덤 값 설정
            Random.seed = (int)System.DateTime.Now.Ticks;                           //랜덤값의 시드를 무작위로 만든다
            int randomChance = Random.Range(0, 1000);

            //<<추가>>  스킬을 추가 삭제가 자유롭고, 확률을 설정하면 자동으로 계산할 수 있도록 해야 할 듯. 확률 자동화 필요 <- 룰렛 알고리즘?
            //          그러기 위해선 스킬 세트에 소속된 스킬과 그 확률 정보를 설정하여 저장할 수 있게 해야한다

            switch (skillSetState)
            {
                //스킬세트 1 (약 공격)
                case M_AttackSkillSetState.SkillSet_1:
                    {
                        //스킬 세트 1은 이동하면서 사용한다
                        m_Core.NvAgent.Resume();
                        realtimeDestination = m_Core.PlayerTr;
                        isMustCheckDistToPlayer = true;

                        if (randomChance < 300)
                        {
                            attackSkillState = M_AttackSkillState.Magic_1;
                            StartCoroutine(M_Magic_1.instance.UseSkill(m_Core.PlayerTr.position));  //마법 1 사용
                        }
                        else if (randomChance < 630)
                        {
                            attackSkillState = M_AttackSkillState.Magic_2;
                            StartCoroutine(M_Magic_2.instance.UseSkill(m_Core.PlayerTr.position));  //마법 2 사용
                        }
                        else
                        {
                            attackSkillState = M_AttackSkillState.Magic_4;
                            StartCoroutine(M_Magic_4.instance.UseSkill(m_Core.PlayerTr.position));  //마법 4 사용
                        }
                        
                        skillSetState = M_AttackSkillSetState.SkillSet_2;           //그 다음엔 스킬 세트 2를 진행해야 함
                    }
                    break;

                //스킬세트 2 (강 공격)
                case M_AttackSkillSetState.SkillSet_2:
                    {
                        m_Core.NvAgent.Stop();
                        realtimeDestination = null;
                        isMustCheckDistToPlayer = false;
                        m_Core.Animator.SetBool("IsRunning", false);

                        //아직은 스킬 세트 2에 소속된 스킬은 에너지 방출밖에 없다
                        attackSkillState = M_AttackSkillState.EnergyEmission;
                        StartCoroutine(M_EnergyEmission.instance.UseSkill(m_Core.PlayerTr.position));  //에너지 방출 사용   

                        skillSetState = M_AttackSkillSetState.SkillSet_1;           //스킬 세트 2를 사용한 후엔 다시 스킬 세트 1을 진행해야 함
                    }
                    break;
            }
        }


        //플레이어가 직선거리상에 위치하고 있지 않다면 (장애물에 가려진다던가 해서) 플레이어 따라서 이동
        else
        {
            m_Core.delayTime = 0.1f;                                                //업데이트 주기 설정
            
            m_Core.NvAgent.Resume();
            m_Core.NvAgent.destination = m_Core.PlayerTr.position;
            realtimeDestination = null;
            isMustCheckDistToPlayer = false;
            m_Core.Animator.SetBool("IsRunning", true);
        }
    }

    //사이클 범위 초과
    void OverCycle()
    {
        m_Core.delayTime = 0.1f;                                                    //업데이트 주기 설정

        isCycling = false;                                                          //사이클 실행중이 아님                                  
        
        m_Core.NvAgent.Resume();
        m_Core.NvAgent.destination = m_Core.PlayerTr.position;
        realtimeDestination = null;
        isMustCheckDistToPlayer = false;
        m_Core.Animator.SetBool("IsRunning", true);
    }

    #endregion



    //골목 Trigger에서 사용할 상태 변경
    public void ChangeStateAttackToAlley()
    {
        m_Core.ChangeState(M_Alley.instance);                                       //골목상태로 변경
    }



    //몬스터 경직
    public override void MonRigid()
    {
        //<<추가>>  여기도 자동화시키자  경직가능여부를 스킬에게 들고있게 할까?   이걸 이렇게 처리하면 굳이 인터페이스를 통일시킨 이유가 없지않는가
        //          사용 스킬 인덱스를 기억한다던가 M_Skill 슈퍼클래스 변수 하나 가지고 있다던가...
        //          여기 처리 때문에 매 스킬 사용때마다 SkillState 설정해주고 있잖은가!

        Debug.Log(attackSkillState);

        switch (attackSkillState)
        {
            case M_AttackSkillState.None:                                           //아무런 상태가 아니면 일반 경직
                {
                    base.MonRigid();
                }
                break;

            case M_AttackSkillState.BodyPress:                                      //바디 프레스 캔슬 가능
                {
                    StopAllCoroutines();
                    M_BodyPress.instance.CancelSkill();

                    base.MonRigid();
                }
                break;
                
            case M_AttackSkillState.Magic_1:                                        //마법_1 캔슬 가능
                {
                    StopAllCoroutines();
                    M_Magic_1.instance.CancelSkill(); 

                    base.MonRigid();
                }
                break;

            case M_AttackSkillState.Magic_2:                                        //마법_2 캔슬 가능
                {
                    StopAllCoroutines();
                    M_Magic_2.instance.CancelSkill();

                    base.MonRigid();
                }
                break;

            case M_AttackSkillState.EnergyEmission:                                 //에너지 방출 캔슬 가능
                {
                    StopAllCoroutines();
                    M_EnergyEmission.instance.CancelSkill();

                    base.MonRigid();
                }
                break;
        }
    }



    //상태 진입
    public override void Enter()
    {
        isCycling = false;                                                  //사이클 실행중이 아님
        skillSetState = M_AttackSkillSetState.SkillSet_1;                   //스킬 세트 1에서부터 사용

        m_Core.NvAgent.stoppingDistance = roadStoppingDistance;             //플레이어와 너무 겹치지 않기 위한 거리 설정
        //////Debug.Log("Enter Attack");
    }

    //상태 이탈                 
    public override void Exit()
    {
        m_Core.NvAgent.stoppingDistance = 0.0f;                             //제동 거리 초기화
        //////Debug.Log("Exit Attack");
    }
}

