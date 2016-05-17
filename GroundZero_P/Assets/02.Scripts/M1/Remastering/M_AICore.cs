using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Monster AI Core
/// 몬스터 AI의 중심점. FSM 실행 및 판단 정의.
/// 
/// *코멘트
///     <<수정>> 청각판단 수정 정확도 상승
/// </summary>



#region 몬스터 상태 정의

public enum M_TopState          //최상위 상태
{
    None = 0,
    Idle,
    Patrol,
    Attack,
    Alley,
    Getaway,
    Dead
}

public enum M_AttackState       //공격 상태
{
    None = 0,
    UnderCycle,
    InCycle,
    OverCycle
}

public enum M_AlleyState        //골목 상태
{
    None = 0,
    Starting,
    InSight,
    Tracing,
    NoWait,
    Patrol
}

public enum M_PatrolState       //순찰 상태
{
    None = 0,
    RandomPatrol,
    GoToPlayerPatrol
}

#endregion


#region 몬스터 수치 구조체

public struct SightValue        //시야 체크 수치
{
    public bool isPlayerInSight;                    //플레이어가 시야에 있는가
    public bool isPlayerInStraightLine;             //플레이어가 몬스터와 직선상에 위치하는가 (다른 구조물에 가려지지않고!)
    public float sightAngle;                        //현재 시야각
}

public struct AuditoryValue     //청역 체크 수치
{
    public AuditoryValue(bool _isHearing, float _langth)
    {
        isHearing = _isHearing;
        trToPlayerPathLangth = _langth;
        trToPlayerPathCornerLangth = (int)_langth;
        sideValue = PlayerSideValue.None;
    }

    public bool isHearing;                          //플레이어가 청각 범위 내에 있는가
    public float trToPlayerPathLangth;              //플레이어까지의 실제 거리
    public int trToPlayerPathCornerLangth;          //플레이어까지 경로의 코너 갯수

    public PlayerSideValue sideValue;
}

public struct CycleDistValue    //플레이어와의 거리 수치
{
    public float trToPlayerDist;                    //플레이어까지의 직선거리  
    public M_AttackState sightInCycleState;         //플레이어의 시야 Cycle 상태
}

public enum PlayerSideValue           //플레이어 위치 수치
{
    None,
    Front,
    Side,
    Back
}
#endregion



public class M_AICore : MonoBehaviour {


    #region 몬스터 스테이터스 및 상태

    public int HP = 500;                                                    //HP

    private bool isDie = false;                                             //사망 여부

    public M_TopState monState;                                             //몬스터 현재 상위 상태 Key
    private M_FSMState m_FsmState;                                          //몬스터 현재 상위 상태 객체

    private bool isDoingOther = false;                                      //다른 행동중 여부
    public bool IsDoingOther { get { return isDoingOther; } set { isDoingOther = value; } }

    private bool isDelay = false;                                           //판단 딜레이 여부
    public float delayTime = 1.0f;                                          //딜레이 시간
    private float delayTimeCounter = 0.0f;                                  //딜레이 카운터

    private bool isStop = false;                                            //행동 강제종료 명령
    
    private Transform tr;                                                   //몬스터 Transform
    public Transform Tr { get { return tr; } }
    private NavMeshAgent nvAgent;                                           //몬스터 NavMeshAgent
    public NavMeshAgent NvAgent { get { return nvAgent; } }
    private Animator animator;                                              //몬스터 Animator
    public Animator Animator { get { return animator; } }

    private bool isRigid = false;                                           //몬스터 경직 여부
    public bool IsRigid { get { return isRigid; } set { isRigid = value; } }

    #endregion


    #region 플레이어 정보

    private Transform playerTr;                                             //플레이어 위치
    public Transform PlayerTr { get { return playerTr; } }
    private bool isPlayerDie = false;                                       //플레이어 사망여부

    #endregion


    #region 몬스터 기본 판단

    //거리 판단
    private CycleDistValue distValue;                                       //몬스터에서 플레이어까지의 거리
    public CycleDistValue DistValue { get { return distValue; } }
    private Vector3 trToPlayerVector;                                       //몬스터에서 플레이어를 가리키는 Vector

    //시야 판단
    public float sightDistRange = 50.0f;                                    //시야 거리 범위
    public float sightAngleRange = 40.0f;                                   //시야각 범위
    public float sightSideAngleRange = 120.0f;                              //측면 각 범위
    private Ray sightRay;                                                   //시야 Ray
    private RaycastHit hit;                                                 //시야 Ray에 맞은 물체
    private int inSightLayerMask;                                           //시야 Ray 레이어 마스크 

    private SightValue sightValue = new SightValue();                       //시야 정보     

    //청각 판단
    public float hearingMaxRadiusDist = 80.0f;                              //청각 범위 반지름 값
    public float hearingMaxNavDist = 180.0f;                                //청각 범위 실 거리값

    private AuditoryValue auditoryValue = new AuditoryValue(false, -1);     //청각정보

    //공격 사이클 범위
    public float minCycleRange = 4.0f;                                      //공격 사이클 최소 범위
    public float maxCycleRange = 14.0f;                                     //공격 사이클 최대 범위
    
    #endregion

    
       
    //Start
    void Start()
    {
        Initialize();                                               //AI 초기화

        StartCoroutine(UpdateMon());                                //몬스터 행동 시작
    }

    

    //몬스터가 아직 죽지 않았거나 판단 딜레이중이 아니면 현재 FSM상태를 받아와서 그 상태의 Update문 실행
    IEnumerator UpdateMon()
    {
        while(!isDie)                                               //죽지 않았고
        {
            if(!isDoingOther)                                       //다른 특별한 행동을 하지 않고
            {
                if (!isDelay)                                       //판단 딜레이중이 아니면
                {
                    //상태 Update 실행
                    m_FsmState.FSMUpdate();
                    
                    if (delayTime > 0.0f)                           //Update실행 중 딜레이 타임이 설정되었으면 딜레이 시작
                        isDelay = true;
                }
                else
                {
                    //딜레이 타이머
                    delayTimeCounter += Time.deltaTime;

                    if (delayTimeCounter > delayTime)
                    {
                        isDelay = false;
                        delayTimeCounter = 0.0f;
                    }
                }
            }

           
            //IsDoingOther과는 별개로 매 프레임 해야 하는 Update문 실행
            m_FsmState.FSMMustUpdate();
            
            //if (isNeedToChaseTr)                                    //실시간 목표지점 갱신이 필요하다면
            //    nvAgent.destination = destinationTr.position;       //목표지점 갱신

            //사망 체크
            if (HP < 0)
                DieMon();

            yield return new WaitForEndOfFrame();
        }
    }

    

    //Initialize  초기화
    void Initialize()
    {
        //몬스터 컴포넌트 가져오기
        tr = GetComponent<Transform>();
        nvAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        

        //시야 마스크 설정
        inSightLayerMask = (1 << LayerMask.NameToLayer(Layers.MonsterAttkCollider))
                         | (1 << LayerMask.NameToLayer(Layers.MonsterHitCollider))
                         | (1 << LayerMask.NameToLayer(Layers.AlleyTrigger));
        inSightLayerMask = ~inSightLayerMask;


        //플레이어 정보
        playerTr = GameObject.FindWithTag(Tags.Player).GetComponent<Transform>();


        //idle상태로 시작
        monState = M_TopState.Idle;
        m_FsmState = M_Idle.instance;
    }



    #region 판단

    //시야 체크
    public SightValue CheckSight()
    {
        //몬스터에서 플레이어를 가리키는 벡터 구하기
        trToPlayerVector = Vector3.Normalize((playerTr.position + playerTr.up) - (tr.position + (tr.up * 5.0f)));
        sightRay = new Ray(tr.position + (tr.up * 5.0f), trToPlayerVector);

        //시야 판단
        if (Physics.Raycast(sightRay, out hit, sightDistRange, inSightLayerMask))
        {
            if (hit.collider.tag == Tags.Player)                                            //시야에 플레이어가 들어온다면
            {
                sightValue.isPlayerInStraightLine = true;                                   //직선거리 안에서 플레이어 확인 성공


                sightValue.sightAngle = Vector3.Angle(tr.forward, trToPlayerVector);        //현재 시야각 저장
                
                if (sightValue.sightAngle < sightAngleRange)                                //시야 벡터의 각도가 시야각 이내이면
                {
                    sightValue.isPlayerInSight = true;                                      //시야 안에 플레이어 확인 성공
                }
                else
                {
                    sightValue.isPlayerInSight = false;                                     //시야각 외면 플레이어 확인은 실패
                }
            }
            else                                                                            //시야 안에 플레이어 확인 실패
            {
                sightValue.isPlayerInSight = false;
                sightValue.isPlayerInStraightLine = false;
                sightValue.sightAngle = -1.0f;
            }
        }
        else                                                                                //시야 안에 플레이어 확인 실패
        {
            sightValue.isPlayerInSight = false;
            sightValue.isPlayerInStraightLine = false;
            sightValue.sightAngle = -1.0f;
        }
        

        return sightValue;
    }

    //청역 체크
    public AuditoryValue CheckAuditoryField()
    {
        //플레이어까지의 직선거리 판단
        distValue.trToPlayerDist = Vector3.Distance(tr.position, playerTr.position);

        //직선거리 반경 한계 이하일 때 청역판단 활성화
        if (distValue.trToPlayerDist < hearingMaxRadiusDist)
        {
            NavMeshPath path = new NavMeshPath();                               //NavMesh경로를 저장할 Path


            //NavAgent가 활성화중이면 NavAgent를 이용하여 플레이어 지점까지 경로 계산
            if (nvAgent.enabled)
                nvAgent.CalculatePath(playerTr.position, path);


            //계산된 경로를 따라 실 거리를 구한다
            float pathLength = 0;                                               //경로 총 거리
            int pathCornersLength = path.corners.Length;                        //경로 내 코너 갯수
            
            if (pathCornersLength > 0)                                          //경로 내 코너 갯수가 0 이상
            {
                pathLength += Vector3.Distance(tr.position, path.corners[0]);   //현재 몬스터 위치에서 경로 첫 지점까지의 거리부터 더하기 시작

                for (int i = 0; i < pathCornersLength - 1; i++)                 //경로 지점의 선분 갯수만큼 반복하며
                {
                    //저장된 경로를 따라 경로 지점 사이의 거리를 구해 총 거리를 계산한다
                    pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                }

                pathLength += Vector3.Distance(path.corners[path.corners.Length - 1], playerTr.position);


                //실 거리가 청역 범위 이내일 때 청역 내 플레이어 확인 성공 
                if (pathLength < hearingMaxNavDist)
                {
                    auditoryValue.isHearing = true;
                    auditoryValue.trToPlayerPathLangth = pathLength;
                    auditoryValue.trToPlayerPathCornerLangth = pathCornersLength;
                    auditoryValue.sideValue = CheckToPlayerPosition();
                }
                else                                                            //실 거리 일정 이상 청역 내 플레이어 확인 실패
                {
                    auditoryValue.isHearing = false;
                    auditoryValue.trToPlayerPathLangth = pathLength;
                    auditoryValue.trToPlayerPathCornerLangth = pathCornersLength;
                    auditoryValue.sideValue = PlayerSideValue.None;
                }
            }
            else                                                                //Path가 0 일 때는 그냥 직선거리만으로 거리 체크 OK시킨다 
            {                                                                   //NavMash가 없는 지역에 있는 플레이어의 체크
                auditoryValue.isHearing = true;
                auditoryValue.trToPlayerPathLangth = 0;
                auditoryValue.trToPlayerPathCornerLangth = 0;
                auditoryValue.sideValue = CheckToPlayerPosition();
            }
        }
        else                                                                    //반경 일정 이상 청역 내 플레이어 확인 실패
        {
            auditoryValue.isHearing = false;
            auditoryValue.trToPlayerPathLangth = -1.0f;
            auditoryValue.trToPlayerPathCornerLangth = -1;
            auditoryValue.sideValue = PlayerSideValue.None;
        }
        
        return auditoryValue;
    }

    //거리 체크 사이클 거리와 비교
    public CycleDistValue CheckDist()
    {
        //플레이어와의 직선거리 판단
        distValue.trToPlayerDist = Vector3.Distance(tr.position, playerTr.position);

        //거리 사이클 체크
        if (distValue.trToPlayerDist < minCycleRange)
        { distValue.sightInCycleState = M_AttackState.UnderCycle; }

        else if (distValue.trToPlayerDist < maxCycleRange)
        { distValue.sightInCycleState = M_AttackState.InCycle; }

        else
        { distValue.sightInCycleState = M_AttackState.OverCycle; }

        return distValue;
    }

    //실제 거리 체크
    public float CheckNevDist(Vector3 targetPos)
    {
        NavMeshPath path = new NavMeshPath();                                   //NavMesh경로를 저장할 Path


        //NavAgent가 활성화중이면 NavAgent를 이용하여 타겟 지점까지 경로 계산
        if (nvAgent.enabled)
            nvAgent.CalculatePath(targetPos, path);

        //계산된 경로를 따라 실 거리를 구한다
        float pathLength = 0;                                                   //경로 총 거리
        int pathCornersLength = path.corners.Length;

        if (pathCornersLength > 0)                                               //경로의 총 길이가 0 이상일 때만
        {
            pathLength += Vector3.Distance(tr.position, path.corners[0]);   //현재 몬스터 위치에서 경로 첫 지점까지의 거리부터 더하기 시작

            for (int i = 0; i < pathCornersLength - 1; i++)                 //경로 지점의 선분 갯수만큼 반복하며
            {
                //저장된 경로를 따라 경로 지점 사이의 거리를 구해 총 거리를 계산한다
                pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            pathLength += Vector3.Distance(path.corners[path.corners.Length - 1], playerTr.position);
        }

        return pathLength;
    }

    //플레이어 위치 각 체크 
    public PlayerSideValue CheckToPlayerPosition()
    {
        //몬스터에서 플레이어를 가리키는 벡터 구하기
        trToPlayerVector = Vector3.Normalize(playerTr.position - tr.position);

        float toPlayerAngle = Vector3.Angle(tr.forward, trToPlayerVector);        //현재 시야각
        
        if (toPlayerAngle < sightAngleRange)
            return PlayerSideValue.Front;
        else if (toPlayerAngle < sightSideAngleRange)
            return PlayerSideValue.Side;
        else
            return PlayerSideValue.Back;
    }
    
    #endregion


    #region 공통 판단

    //경직
    public void RigidMon()
    {
        m_FsmState.MonRigid();
    }

    //몬스터 사망 
    public void DieMon()
    {
        Debug.Log("Die Monster");

        isDie = true;
        //Destroy(gameObject);
    }

    #endregion



    //스테이트 변경
    public void ChangeState(M_FSMState m_ChangState)
    {
        m_FsmState.Exit();                              //이전 State 이탈
        m_FsmState = m_ChangState;                      //현재 State 변경
        monState = m_ChangState.TopState;               //변경된 State의 enum Index 설정
        m_FsmState.Enter();                             //변경된 State 진입
    }
}