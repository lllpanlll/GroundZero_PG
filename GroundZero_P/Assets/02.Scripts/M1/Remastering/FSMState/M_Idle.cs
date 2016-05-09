using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Monster Idle State
/// 
/// *코멘트
///     <<추가완료>  인식했을 때 플레이어가 골목인지 아닌지 판단 후 골목/공격 상태 변환 결정 
///     <<추가완료>>  대기상태일 때 피격 시 상태 변경 -> HitCtrl에서 컨트롤
///     <<추가완료>>  경직 적용  
///     160429일자 기획서 추가 없으면 구현 완료로 판단
/// </summary>



public class M_Idle : M_FSMState
{
    #region SingleTon

    public static M_Idle instance = null;

    //컴포넌트로 연결되어있으면 이렇게 가능(MonoBehavior 상속 시) 
    void Awake()
    {
        instance = this; //<-지금은 상위 클래스인 M_FSMState가 MonoBehavior을 사용하기 때문에 이거 사용
    }
    
    //어떻게 해줄까 싱글톤
    //이렇게 하면 오브젝트의 컴포넌트로 연결하지 않고 사용 가능(MonoBehavior상속 안해도 됨) <- 위에서 private로 해줘야겠징
    //public static M_Idle Instance
    //{
    //    get
    //    {
    //        if(instance == null)
    //            instance = new M_Idle();

    //        return instance;
    //    }
    //}

    //이런 방법도 있는 모양
    //M_Idle컴포넌트를 가진 오브젝트의 M_Idle과 연결이란 느낌 <- 이걸 못찾으면 아예 새로 생성해주는 코드도 있더라 (MonoBehavior일때 말이지)
    //public static M_Idle GetInstance()
    //{
    //    if (instance != null)
    //    {
    //        instance = FindObjectOfType(typeof(M_Idle)) as M_Idle;
    //    }
    //    return instance;
    //}

    #endregion


    private bool isPlayerInAlley = false;                       //현재 플레이어가 골목에 있는지
    public bool IsPlayerInAlley { set { isPlayerInAlley = value; } }


    public float lookRotationTime = 0.5f;                       //플레이어를 바라볼 회전 시간



    //상태 초기화
    public override void FSMInitialize()
    {
        topState = M_TopState.Idle;                             //이 상태는 Idle입니다
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


        #region 상태 행동 

        //제자리에서 Idle 출력
        m_Core.NvAgent.Stop();
        m_Core.SetDestinationRealtime(false, null);
        m_Core.Animator.SetBool("IsRunning", false);

        #endregion
    }



    //인식 상태로 체인지
    public void ChangeStateToRecognition()
    {
        if (isPlayerInAlley)                                    //플레이어가 골목 상태에 있다면
        {
            M_Alley.instance.IsStartToIdleOrPatrol = true;      //현재 대기상태에서 골목상태로 전환한다고 알려주고
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
        //////Debug.Log("Enter Idle");
    }

    //상태 탈출                   
    public override void Exit()
    {
        //플레이어를 바라보기
        StartCoroutine(RotateToPoint(m_Core.transform, m_Core.PlayerTr.position, lookRotationTime));

        //////Debug.Log("Exit Idle");
    }
}
