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
///     데이터 구조체로 묶어 정리
///     경직 재정의
///     마법 1 마법 2 오브젝트 풀 -> 오브젝트 풀 왜이러세요 나한테
///     Alley와 겹치는 스킬이 넘 많다 그냥 스킬매니저 만들까 거기서 다 가져와 쓰게
/// </summary>

public enum M_AttackSkillSetState
{
    SkillSet_1,
    SkillSet_2
}

public class M_Attack : M_FSMState
{
    #region SingleTon

    public static M_Attack instance = null;
    
    void Awake()
    { instance = this; }

    #endregion
    
       
    private M_AttackState attackState = M_AttackState.None;         //공격 상태
    public M_AttackState AttackState { get { return attackState; }}

    private DistValue _distValue;                                   //거리 값    

    private bool isCycling = false;                                 //사이클 실행중 여부

    private M_AttackSkillSetState skillSetState = M_AttackSkillSetState.SkillSet_1;   //SkillSet1사용 여부

    public float lookRotationTime = 0.5f;                           //플레이어를 바라볼 회전 시간

    
    //상태 초기화
    public override void FSMInitialize()
    {
        topState = M_TopState.Attack;                               //이 상태는 Attack입니다

        isCycling = false;                                          //사이클 실행중
    }


    //상태 Update   
    public override void FSMUpdate()
    {
        m_Core.delayTime = 0.0f;                                    //업데이트 주기 설정


        #region 하위 FSM

        _distValue = m_Core.CheckDist();                            //거리 판단

        switch (_distValue.sightInCycleState)                       
        {
            case DistValue.SigntInCycleState.None:                  //거리 판단이 안 되어있을 때는 에러 출력
                Debug.LogError("NoneCycleState!!!");
                break;

            case DistValue.SigntInCycleState.UnderCycle:            //사이클 범위 미만
                UnderCycle();
                break;

            case DistValue.SigntInCycleState.InCycle:               //사이클 범위 내
                InCycle();
                break;

            case DistValue.SigntInCycleState.OverCycle:             //사이클 범위 초과
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
        m_Core.Animator.SetBool("IsRunning", false);

        isCycling = false;                                      //사이클 실행중이 아님


        //바디프레스 실행
        attackState = M_AttackState.BodyPress;
        StartCoroutine(M_BodyPress.instance.UseSkill(m_Core.PlayerTr.position));
    }

    //사이클 범위 내
    void InCycle() 
    {
        m_Core.NvAgent.Stop();
        m_Core.Animator.SetBool("IsRunning", false);

        if (m_Core.CheckSight().isPlayerInStraightLine)                     //플레이어가 직선거리상에 위치하고 있으면
        {

            m_Core.NvAgent.Stop();
            m_Core.Animator.SetBool("IsRunning", false);


            if (!isCycling)                                                 //사이클 실행중이 아니었으면
            {
                isCycling = true;                                           //사이클 실행
                skillSetState = M_AttackSkillSetState.SkillSet_1;           //시작은 스킬 세트 1부터

                m_Core.delayTime = 0.8f;                                    //업데이트 주기 설정
            }


            Random.seed = (int)System.DateTime.Now.Ticks;                   //랜덤값의 시드를 무작위로 만든다
            int randomChance = Random.Range(0, 1000);

            //<<추가>>  이 부분 자동화 요망  
            //<<추가>>  해당 스킬 세트에 이런 스킬을 추가하고 그 확률을 적으면 자동으로 계산할 수 있게 해야할듯

            switch (skillSetState)
            {
                case M_AttackSkillSetState.SkillSet_1:
                    {
                        if (randomChance < 500)                             //1000에 500은 마법 1                         
                        {
                            attackState = M_AttackState.Magic_1;
                            StartCoroutine(M_Magic_1.instance.UseSkill(m_Core.PlayerTr.position));  //마법 1 사용
                        }
                        else                                                //아님 마법 2
                        {
                            attackState = M_AttackState.Magic_2;
                            StartCoroutine(M_Magic_2.instance.UseSkill(m_Core.PlayerTr.position));  //마법 2 사용
                        }

                        skillSetState = M_AttackSkillSetState.SkillSet_2;  //그 다음엔 스킬 세트 2를 진행해야 함
                    }
                    break;

                case M_AttackSkillSetState.SkillSet_2:
                    {
                        //if(randomChance < 1000)   //조건문이 의미가 없어서 말이징
                        attackState = M_AttackState.EnergyEmission;
                        StartCoroutine(M_EnergyEmission.instance.UseSkill(m_Core.PlayerTr.position));  //에너지 방출 사용   

                        skillSetState = M_AttackSkillSetState.SkillSet_1;  //스킬 세트 2를 사용한 수엔 다시 스킬 세트 1을 진행해야 함
                    }
                    break;
            }
        }

        else                                                                //플레이어가 직선거리상에 위치하고 있지 않다면
        {
            m_Core.delayTime = 0.1f;                                        //업데이트 주기 설정

            //플레이어 추격
            attackState = M_AttackState.Trace;
            m_Core.NvAgent.Resume();
            m_Core.NvAgent.destination = m_Core.PlayerTr.position;
            m_Core.Animator.SetBool("IsRunning", true);
        }
    }

    //사이클 범위 초과
    void OverCycle() 
    {
        m_Core.delayTime = 0.1f;                                            //업데이트 주기 설정

        isCycling = false;                                                  //사이클 실행중이 아님                                     

        //플레이어 추격
        attackState = M_AttackState.Trace;
        m_Core.NvAgent.Resume();
        m_Core.NvAgent.destination = m_Core.PlayerTr.position;
        m_Core.Animator.SetBool("IsRunning", true);
    }

    #endregion



    //골목 Trigger에서 사용할 상태 변경
    public void ChangeStateAttackToAlley()
    {
        m_Core.ChangeState(M_Alley.instance);                               //골목상태로 변경
    }



    //몬스터 경직
    public override void MonRigid()
    {
        //<<추가>> 어떤 스킬은 캔슬되지 말아야 한다

        Debug.Log("Rigid Rigid");

        switch(attackState)
        {
            case M_AttackState.BodyPress:
                {
                    StopAllCoroutines();
                    M_BodyPress.instance.CancelSkill();

                    base.MonRigid();
                }
                break;

            case M_AttackState.Breath:
                {
                    StopAllCoroutines();
                    M_Breath.instance.CancelSkill();

                    base.MonRigid();
                }
                break;

            case M_AttackState.Magic_1:
                {
                    StopAllCoroutines();
                    M_Magic_1.instance.CancelSkill();

                    base.MonRigid();
                }
                break;

            case M_AttackState.Magic_2:
                {
                    StopAllCoroutines();
                    M_Magic_2.instance.CancelSkill();

                    base.MonRigid();
                }
                break;

            case M_AttackState.EnergyEmission:
                {
                    StopAllCoroutines();
                    M_EnergyEmission.instance.CancelSkill();

                    base.MonRigid();
                }
                break;

            case M_AttackState.JumpAttack:
                {
                    //<<나중추가>>  점프 도중에는 경직이 먹지 않는다  점프 도중을 체크하고 그때만 경직 먹게 하자
                    //StopAllCoroutines();
                    //base.MonRigid();
                }
                break;
        }

        StopAllCoroutines();
        
        base.MonRigid();

        m_Core.delayTime = 0.5f;                                            //경직 후 딜레이
    }



    //상태 진입
    public override void Enter()
    {
        isCycling = false;                                                  //사이클 실행중이 아님
        
        //////Debug.Log("Enter Attack");
    }

    //상태 이탈                 
    public override void Exit()
    {
        //////Debug.Log("Exit Attack");
    }
}

