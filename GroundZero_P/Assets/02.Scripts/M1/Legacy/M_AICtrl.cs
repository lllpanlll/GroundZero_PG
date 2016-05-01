using UnityEngine;
using System.Collections;

public enum MonState
{
    Patrol,
    Detection,
    Tracking,
    Attack
}

public enum MonDetectionState
{
    Idle,
    Lookaround,
    Movement
}

public enum MonTrackingState
{
    Look,
    Trace
}

public enum MonAttackState
{
    ForefootPress,
    BodyPress,
    TailSwing,
    JumpAttack,
    Breath,
    Magic
}

public class M_AICtrl : MonoBehaviour
{

    //////Monster Status 스테이터스
    public int HP = 500;                            //HP
    public int EP = 0;                              //EP

    private bool isDie = false;                     //사망 여부

    private bool isInDelay = false;                 //딜레이 여부
    private float delayTime = 1.0f;                 //딜레이 시간
    private float delayTimeCounter = 0.0f;          //딜레이 카운터


    private Transform tr;
    private NavMeshAgent nvAgent;
    private Animator animator;

    public GameObject[] monAttkArms;                //몬스터 공격 팔 콜리더 
    public GameObject monAttkBody;                  //몬스터 공격 바디 콜리더 

    //Player Information 
    private Transform playerTr;



    //////상태
    public MonState monState = MonState.Detection;
    public MonDetectionState monDetectionState = MonDetectionState.Idle;
    public MonTrackingState monTrackingState = MonTrackingState.Look;
    public MonAttackState monAttackState = MonAttackState.ForefootPress;



    //////추적 시야
    private bool isInSight = false;                 //플레이어가 시야 안에 있는지

    public float sightLengthRange = 15.0f;          //시야 거리 범위
    public float sightAngleRange = 40.0f;           //시야각 범위

    public Transform sightRayStartPos;              //시야 시작점
    public Transform SightRayEndPos;                //시야 도착점(플레이어 발각지점)

    private Vector3 sightVector;                    //시야 벡터
    private float sightAngle;                       //시야각

    private Ray sightRay;                           //시야 Ray
    private RaycastHit hit;
    private int inSightLayerMask;                   //시야 Ray 레이어 마스크 

    private Vector3 trToPlayerTrVector;             //tr에서 playerTr을 가리키는 Vector


    //////탐색
    private Vector3 lastPlayerPos = Vector3.zero;   //마지막으로 시야에서 확인한 플레이어의 위치
    public float guessAttackRange = 3.0f;           //예상 위치에서 바로 공격에 들어갈 범위
    public float guessFindedRange = 1.0f;           //예상 위치에 도착했을 때의 범위


    //////전투 
    public float maxAttackRange = 10.0f;            //공격 최대 사거리;

    //앞발 찍기
    public float forefootPressRange = 3.0f;         //앞발 찍기 범위
    public int forefootPressDamage = 10;            //앞발 찍기 데미지

    public float forefootPressAfterDelay = 1.25f;   //앞발 찍기 후딜


    //바디 프레스
    public float bodyPressRange = 5.0f;              //바디 프레스 범위
    public int bodyPressDamage = 10;                 //바디 프레스 데미지

    public float bodyPressAfterDelay = 1.25f;        //바디 프레스 후딜


    //점프 어택
    public float jumpAttkMinRange = 4.0f;          //점프 어택 최소 범위
    public float jumpAttkMaxRange = 7.0f;          //점프 어택 최대 범위
    public int jumpAttkDamage = 20;                 //점프 어택 데미지

    public float jumpAttkRotTime = 0.3f;            //회전하는 시간
    public float jumpAttkWaitTime = 0.15f;          //대기하는 시간
    public float jumpAttkJumpTime = 0.43f;          //점프하는 시간

    public float jumpAttkAfterDelay = 1.0f;         //점프공격 후딜



    void Start()
    {
        //컴포넌트 가져오기
        tr = GetComponent<Transform>();
        nvAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        playerTr = GameObject.FindWithTag(Tags.Player).GetComponent<Transform>();


        //공격용 콜리더 찾아서 비활성화
        monAttkArms = GameObject.FindGameObjectsWithTag(Tags.MonsterArm);
        monAttkBody = GameObject.FindGameObjectWithTag(Tags.MonsterBody);

        foreach (GameObject armObj in monAttkArms)
        { armObj.SetActive(false); }


        //시야 레이 초기설정, 시야 레이 충돌 마스크 설정
        sightRay = new Ray(sightRayStartPos.position, sightRayStartPos.forward);

        inSightLayerMask = (1 << LayerMask.NameToLayer(Layers.MonsterAttkCollider)) | (1 << LayerMask.NameToLayer(Layers.MonsterHitCollider));
        inSightLayerMask = ~inSightLayerMask;


        //행동 시작
        StartCoroutine(this.ActiveMonster());
    }

    //Update에서 판단
    void Update()
    {
        //위치 검사용 벡터 계산
        trToPlayerTrVector = Vector3.Normalize(playerTr.position - tr.position);
        //Debug.DrawRay(sightRay.origin, sightRay.direction * sightLengthRange, Color.yellow);    //레이 디버깅

        //죽으면 판단 중지
        if (!isDie)
        {
            //딜레이면 판단 중지
            if (!isInDelay)
            {
                //////시야 판단

                //시야 레이 발사
                sightVector = Vector3.Normalize(SightRayEndPos.position - sightRayStartPos.position);
                sightRay = new Ray(sightRayStartPos.position, sightVector);

                //각도 계산
                //sightAngle = Vector2.Angle(new Vector2(sightRayStartPos.forward.z, sightRayStartPos.forward.x), new Vector2(sightVector.z, sightVector.x));
                sightAngle = Vector3.Angle(sightRayStartPos.position, SightRayEndPos.position);

                //시야 안에 플레이어가 있는가 판단
                if (sightAngle < sightAngleRange)
                {
                    if (Physics.Raycast(sightRay, out hit, sightLengthRange, inSightLayerMask))
                    {
                        if (hit.distance < sightLengthRange)
                        {
                            if (hit.collider.tag == Tags.Player)
                            {
                                isInSight = true;   //발견
                                Debug.DrawRay(sightRay.origin, sightRay.direction * sightLengthRange, Color.yellow);    //레이 디버깅
                                print("시야 안");
                            }
                            else
                            {
                                isInSight = false;
                                Debug.DrawRay(sightRay.origin, sightRay.direction * sightLengthRange, Color.red);    //레이 디버깅
                                print("시야 밖");
                            }
                        }
                        else
                        {
                            isInSight = false;
                            Debug.DrawRay(sightRay.origin, sightRay.direction * sightLengthRange, Color.red);    //레이 디버깅
                            print("시야 밖");
                        }
                    }
                    else
                    {
                        isInSight = false;
                        Debug.DrawRay(sightRay.origin, sightRay.direction * sightLengthRange, Color.red);    //레이 디버깅 
                        print("시야 밖");
                    }
                }
                else
                {
                    isInSight = false;
                    Debug.DrawRay(sightRay.origin, sightRay.direction * sightLengthRange, Color.red);    //레이 디버깅
                    print("시야 밖");
                } 
                print("hit " + hit.distance);


                //////행동 판단
                if (isInSight)   //시야 안에 있다
                {
                    float distance = Vector3.Distance(tr.position, playerTr.position);  //플레이어와의 거리
                    print(distance);

                    //앞발찍기 범위 내 앞발찍기
                    if (distance < forefootPressRange)
                    {
                        monState = MonState.Attack;                                  
                        monAttackState = MonAttackState.ForefootPress;
                        print("앞발");
                    }

                    //점프공격 범위 내 점프공격
                    else if ((distance > jumpAttkMinRange) && (distance < jumpAttkMaxRange))
                    {
                        monState = MonState.Attack;
                        monAttackState = MonAttackState.JumpAttack;
                        print("점프");
                    }

                    //공격 범위 외 플레이어 주시
                    else if (distance > maxAttackRange)
                    {
                        monState = MonState.Tracking;
                        monTrackingState = MonTrackingState.Look;
                        //주시해오
                        print("주시");
                    }

                    lastPlayerPos = playerTr.position;                                  //플레이어 마지막 위치 변경
                }


                else   //시야 안에 없다
                {
                    float distance = Vector3.Distance(tr.position, lastPlayerPos);      //예상 위치와의 거리
                    print(distance);

                    //예상위치에 도달했다
                    if (distance < guessFindedRange)
                    {
                        monState = MonState.Detection;
                        monDetectionState = MonDetectionState.Idle;
                        print("대기");
                    }

                    //예상 위치 공격 범위 안
                    else if (distance < guessAttackRange)
                    {
                        monState = MonState.Attack;
                        monAttackState = MonAttackState.BodyPress;
                        print("바디");
                    }
                    else
                    {
                        monState = MonState.Detection;
                        monDetectionState = MonDetectionState.Lookaround;
                        print("두리번");
                    }
                }

                //다음 판단까지 딜레이
                isInDelay = true;
            }
            else
            {
                //////딜레이 타이머
                delayTimeCounter += Time.deltaTime;

                if (delayTimeCounter > delayTime)
                {
                    isInDelay = false;
                    delayTimeCounter = 0.0f;
                }
            }
        }

        //////HP가 0 이하 시 사망처리
        if (HP <= 0)
        {
            DieMonster();
        }
    }


    //행동 코루틴
    IEnumerator ActiveMonster()
    {
        //죽으면 행동 중지
        if (!isDie)
        {
            //딜레이면 행동 중지
            if (!isInDelay)
            {
                //////행동 
                switch (monState)
                {
                    //배회
                    case MonState.Patrol:

                        break;


                    //탐색
                    case MonState.Detection:

                        switch (monDetectionState)
                        {
                            case MonDetectionState.Idle:
                                {
                                    nvAgent.Stop();
                                    animator.SetBool("IsRunning", false);
                                    animator.SetBool("IsForeFootAttack", false);

                                    Debug.Log("IdleAction");
                                }
                                break;

                            case MonDetectionState.Lookaround:
                                {
                                    //임시 둘러보기
                                    nvAgent.Stop();
                                    animator.SetBool("IsRunning", false);
                                    animator.SetBool("IsForeFootAttack", false);

                                    Debug.Log("LookaroundAction");
                                }
                                break;

                            case MonDetectionState.Movement:
                                {
                                    nvAgent.Resume();
                                    nvAgent.destination = lastPlayerPos;

                                    animator.SetBool("IsRunning", true);
                                    animator.SetBool("IsForeFootAttack", false);

                                    Debug.Log("MovementAction");
                                }
                                break;
                        }
                        break;


                    //추격
                    case MonState.Tracking:

                        switch (monTrackingState)
                        {
                            case MonTrackingState.Look:
                                {
                                    //임시 주시
                                    nvAgent.Stop();
                                    animator.SetBool("IsRunning", false);
                                    animator.SetBool("IsForeFootAttack", false);

                                    tr.rotation = Quaternion.LookRotation(sightVector);

                                    Debug.Log("LookAction");
                                }
                                break;

                            case MonTrackingState.Trace:
                                {
                                    nvAgent.Resume();
                                    nvAgent.destination = lastPlayerPos;
                                    animator.SetBool("IsRunning", true);
                                    animator.SetBool("IsForeFootAttack", false);

                                    Debug.Log("TraceAction");
                                }
                                break;
                        }
                        break;


                    //전투
                    case MonState.Attack:

                        switch (monAttackState)
                        {
                            case MonAttackState.ForefootPress:
                                break;

                            case MonAttackState.BodyPress:
                                break;

                            case MonAttackState.TailSwing:
                                break;

                            case MonAttackState.JumpAttack:
                                break;

                            case MonAttackState.Breath:
                                break;

                            case MonAttackState.Magic:
                                break;
                        }
                        break;
                }
            }
        }

        yield return new WaitForSeconds(delayTime);
    }


    //////몬스터 사망 
    void DieMonster()
    {
        isDie = true;

        Destroy(gameObject);
    }
}
