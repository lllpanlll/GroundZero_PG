using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Monster Attack State
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

    private bool isCycling = false;                                                 //사이클 실행중 여부

    

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



    #region 하위 상태 

    //사이클 범위 미만
    void UnderCycle() 
    {
        m_Core.NvAgent.Stop();
        m_Core.SetDestinationRealtime(false, null);
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
                        m_Core.SetDestinationRealtime(true, m_Core.PlayerTr);       //이 경우, 스킬 사용으로 판단이 멈추더라도 목적지 정보가 계속 갱신되어야 한다
                        m_Core.Animator.SetBool("IsRunning", true);

                        if (randomChance < 500)                                                  
                        {
                            attackSkillState = M_AttackSkillState.Magic_1;
                            StartCoroutine(M_Magic_1.instance.UseSkill(m_Core.PlayerTr.position));  //마법 1 사용
                        }
                        else                                                
                        {
                            attackSkillState = M_AttackSkillState.Magic_2;
                            StartCoroutine(M_Magic_2.instance.UseSkill(m_Core.PlayerTr.position));  //마법 2 사용
                        }

                        skillSetState = M_AttackSkillSetState.SkillSet_2;           //그 다음엔 스킬 세트 2를 진행해야 함
                    }
                    break;

                //스킬세트 2 (강 공격)
                case M_AttackSkillSetState.SkillSet_2:
                    {
                        m_Core.NvAgent.Stop();
                        m_Core.SetDestinationRealtime(false, null);
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
            m_Core.SetDestinationRealtime(false, null);                             //이 경우 업데이트가 0.1초 간격으로 이미 진행되고 있기 때문에 특별하게 따로 갱신할 필요는 없음
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
        m_Core.SetDestinationRealtime(false, null);
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

        switch(attackSkillState)
        {
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

        //////Debug.Log("Enter Attack");
    }

    //상태 이탈                 
    public override void Exit()
    {
        //////Debug.Log("Exit Attack");
    }
}

