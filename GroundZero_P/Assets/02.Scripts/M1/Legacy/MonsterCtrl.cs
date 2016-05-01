using UnityEngine;
using System.Collections;
using System;

public enum MonsterState
{
    Idle,
    Trace,
    NearAttk,
    BreathAttk,
    JumpAttk
}


public class MonsterCtrl : MonoBehaviour {

   
    public int HP = 500;                            //HP
    public int EP = 0;                              //EP

    private bool isDie = false;                     //사망여부

    private Transform tr;                           //내 위치


    public float sightLengthRange = 15.0f;          //시야 거리
    public float sightAngleRange = 40.0f;           //시야각

    public Transform SightRayPivot;                 //시야 시작점
    public Transform SightRayReceiver;              //시야 도착점(플레이어 발각지점)
    private Vector3 IncomeVec;                      //시야 벡터
    private float IncomeAngle;
    private Ray sightRay;                           //시야 Ray

    private Vector3 IncomeVecTransform;             //움직임 Ray

    public float jumpAttkRange = 10.0f;             //점프공격 범위
    public int jumpAttkDamage = 20;                 //점프공격 데미지
    public float jumpAttkRotTime = 0.3f;            //회전하는 시간
    public float jumpAttkWaitTime = 0.15f;          //대기하는 시간
    public float jumpAttkJumpTime = 0.43f;          //점프하는 시간
    public float nearAttkRange = 5.0f;              //근접공격 범위
    public int nearAttkDamage = 10;                 //근접공격 데미지
    public float nearAttkAfterDelay = 1.25f;        //근접공격 후딜

    public float jumpAttkAfterDelay = 1.0f;         //점프공격 후딜

    public float breathAttkCooltime = 10.0f;        //브레스공격 쿨타임
    private bool canBreathAttk = true;              //브레스공격 사용가능여부
    public float breathAttkSpeed = 700.0f;          //브레스공격 속도
    public int breathAttkDamage = 30;               //브레스공격 데미지
    public float breathAttkRotTime = 0.6f;          //회전하는 시간
    public float breathAttkWaitTime = 0.5f;         //대기하는 시간
    public float breathAttkAfterDelay = 1.0f;       //브레스공격 후딜
    public Transform breathPivot;                   //브레스공격 시작 위치 
    public GameObject breathEff;                    //브레스 이펙트
        
    private float waitForStateCheckTime = defaultWaitTime;     //상태 변화 딜레이시간(0.2초 디폴트)
    const float defaultWaitTime = 0.2f;             //기본 딜레이시간

    public GameObject[] monsterAttkArms;            //몬스터 공격 팔 콜리더 
    public GameObject monsterAttkBody;              //몬스터 공격 바디 콜리더 

    private NavMeshAgent nvAgent;
    private Transform playerTr;
    private Animator animator;
    
    private int inSightLayerMask;                   //시야 Ray 레이어 마스크 

    
    private MonsterState monsterState = MonsterState.Idle;      //몬스터 상태
    
    public MonsterState GetMonsterState() { return monsterState; }      


    #region <Start>
    void Start () {
        tr = GetComponent<Transform>();
        nvAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        playerTr = GameObject.FindWithTag(Tags.Player).GetComponent<Transform>();


        //공격용 콜리더 찾아서 비활성화
        monsterAttkArms = GameObject.FindGameObjectsWithTag(Tags.MonsterArm);
        monsterAttkBody = GameObject.FindGameObjectWithTag(Tags.MonsterBody);

        foreach (GameObject armObj in monsterAttkArms)
        {
            armObj.SetActive(false);
        }
        monsterAttkBody.SetActive(false);


        //시야 레이 초기설정, 시야 레이 충돌 마스크 설정
        sightRay = new Ray(SightRayPivot.position, SightRayPivot.forward);

        inSightLayerMask = (1 << LayerMask.NameToLayer(Layers.MonsterAttkCollider)) | (1 << LayerMask.NameToLayer(Layers.MonsterHitCollider));
        inSightLayerMask = ~inSightLayerMask;


        //몬스터 상태 돌리기 시작
        StartCoroutine(this.CheckMonsterState());
        StartCoroutine(this.MonsterAction());
    }
    #endregion

    #region <Update>
    void Update()
    {
        //위치 검사용 벡터 계산
        IncomeVecTransform = Vector3.Normalize(playerTr.position - tr.position);

        Debug.DrawRay(sightRay.origin, sightRay.direction * sightLengthRange, Color.yellow);

        //HP가 0 이하 시 사망처리
        if(HP <=0)
        {
            MonsterDie();
        }
    }
    #endregion

    #region <몬스터 상태 체크>
    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            //Debug.Log("A" + waitForStateCheckTime);
            yield return new WaitForSeconds(waitForStateCheckTime);

            //////시야의 캐릭터 판단

            //시야 레이 발사
            IncomeVec = Vector3.Normalize(SightRayReceiver.position - SightRayPivot.position);
            sightRay = new Ray(SightRayPivot.position, IncomeVec);
            
            //각도 계산
            IncomeAngle = Vector2.Angle(new Vector2(SightRayPivot.forward.z, SightRayPivot.forward.x), new Vector2(IncomeVec.z, IncomeVec.x));
            
            //캐릭터가 시야 안에 있으면
            if (IncomeAngle < sightAngleRange)
            {
                RaycastHit hit;
                
                if (Physics.Raycast(sightRay, out hit, sightLengthRange, inSightLayerMask))
                {
                    //플레이어 발견
                    if (hit.collider.tag == Tags.Player)
                    {
                        //아무래도 나중엔 제대로 된 상태 FSM을 만들어서 상태 Enter과 Exit를 관리해야겠다 싶음
                        //Enter시 해당 공격 시에 필요한 콜리더들을 활성화시켜주고
                        //Exit시 다시 비활성화 하면 타격간의 혼동을 줄일 수 있을 거 같은데

                        #region <근접공격>
                        if (hit.distance < nearAttkRange)
                        {
                            monsterState = MonsterState.NearAttk;

                            waitForStateCheckTime = nearAttkAfterDelay;

                            //근접공격이 가능한 팔의 콜리더 활성화
                            foreach (GameObject armObj in monsterAttkArms)
                            {
                                armObj.SetActive(true);
                            }

                            ////Debug.Log("near");
                        }
                        #endregion

                        else
                        {
                            //근접공격이 아닐 경우 팔의 콜리더 비활성화
                            foreach (GameObject armObj in monsterAttkArms)
                            {
                                armObj.SetActive(false);
                            }

                            #region <브레스공격>
                            if (canBreathAttk)
                            {
                                //브레스 사용 불가
                                canBreathAttk = false;
                                animator.SetTrigger("BreathAttk");

                                monsterState = MonsterState.BreathAttk;

                                //브레스를 내뿜는 코루틴 실행
                                StartCoroutine(this.MonsterBreathAttk());

                                waitForStateCheckTime = breathAttkAfterDelay;

                                ////Debug.Log("breath");
                            }
                            #endregion

                            #region <점프공격>
                            else if (hit.distance > jumpAttkRange)
                            {
                                animator.SetTrigger("JumpAttk");

                                monsterState = MonsterState.JumpAttk;

                                StartCoroutine(this.MonsterJumpAttk());

                                waitForStateCheckTime = jumpAttkAfterDelay;

                                ////Debug.Log("jump");
                            }
                            #endregion

                            #region <캐릭터 추적>
                            else
                            {
                                monsterState = MonsterState.Trace;
                                waitForStateCheckTime = defaultWaitTime;

                                ////Debug.Log("trace");
                            }
                            #endregion
                        }
                    }

                    //////시야에 캐릭터가 없으면 대기
                    else
                    {
                        //아님 말고
                        monsterState = MonsterState.Idle;
                        waitForStateCheckTime = defaultWaitTime;

                        ////Debug.Log("idle01");
                    }
                }
                else
                {
                    //아님 말고
                    monsterState = MonsterState.Idle;
                    waitForStateCheckTime = defaultWaitTime;

                    ////Debug.Log("idle02");
                }
            }
            else
            {
                //아님 말고
                monsterState = MonsterState.Idle;
                waitForStateCheckTime = defaultWaitTime;

                ////Debug.Log("idle03");
            }
        }  
    }
    #endregion

    #region <몬스터 상태에 따른 행동>
    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (monsterState)
            {
                //추적 중지
                case MonsterState.Idle:
                    nvAgent.Stop();
                    animator.SetBool("IsTrace", false);
                    animator.SetBool("IsAttack", false);
                    break;

                //추적
                case MonsterState.Trace:
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                    { 
                        nvAgent.destination = playerTr.position;
                        nvAgent.Resume();
                    }
                    animator.SetBool("IsTrace", true);
                    animator.SetBool("IsAttack", false);
                    break;

                //근접공격
                case MonsterState.NearAttk:
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    {
                        nvAgent.Stop();
                    }
                    
                    tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(IncomeVecTransform), nvAgent.angularSpeed/2 * Time.deltaTime);
                    animator.SetBool("IsAttack", true);
                    break;

                //브레스공격
                case MonsterState.BreathAttk:
                    nvAgent.Stop();
                    break;

                //점프공격
                case MonsterState.JumpAttk:
                    nvAgent.Stop();
                    break;
            }
            yield return null;
        }
    }
    #endregion

    #region <몬스터 브레스 공격>
    IEnumerator MonsterBreathAttk()
    {
        Quaternion startRotation = tr.rotation;
        float breathAngle = 0;
        
        //->플레이어를 바라보도록 회전
        while (breathAngle <= 1)
        {
            tr.rotation = Quaternion.Slerp(startRotation, Quaternion.LookRotation(IncomeVecTransform), breathAngle);

            breathAngle += 0.01f / breathAttkRotTime;

            yield return new WaitForSeconds(0.01f);
        }

        //생성 대기 시간
        yield return new WaitForSeconds(breathAttkWaitTime);

        Instantiate(breathEff, breathPivot.position, breathPivot.rotation);

        //쿨타임까지 기다렸다 브레스 사용 허가 후 종료
        yield return new WaitForSeconds(breathAttkCooltime);

        canBreathAttk = true;
    }
    #endregion

    #region <몬스터 점프 공격>
    IEnumerator MonsterJumpAttk()
    {
        Vector3 startPosition = tr.position;
        Quaternion startRotation = tr.rotation;
        float jumpDistance = 0;
        float jumpAngle = 0;

        //몸통의 콜리더 활성화
        monsterAttkBody.SetActive(true);

        //플레이어를 바라보도록 회전
        while (jumpAngle <= 1)
        {
            tr.rotation = Quaternion.Slerp(startRotation, Quaternion.LookRotation(IncomeVecTransform), jumpAngle);

            jumpAngle += 0.01f / jumpAttkRotTime;

            yield return new WaitForSeconds(0.01f);
        }

        Vector3 endPosition = playerTr.position;

        //잠깐대기
        yield return new WaitForSeconds(jumpAttkWaitTime);


        //플레이어의 위치를 받아 점프.
        while (jumpDistance <= 1)
        {
            tr.position = Vector3.Lerp(startPosition, endPosition, jumpDistance);

            jumpDistance += 0.01f / jumpAttkJumpTime;

            yield return new WaitForSeconds(0.01f);
        }

        //몸통의 콜리더 비활성화
        monsterAttkBody.SetActive(false);
    }
    #endregion

    #region <몬스터 사망>
    void MonsterDie()
    {
        isDie = true;

        Destroy(gameObject);
    }
    #endregion
}
