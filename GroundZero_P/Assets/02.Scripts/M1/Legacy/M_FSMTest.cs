using UnityEngine;
using System.Collections;

public enum MonTestState
{
    Idle,
    Press,
    Cycle,
    Run,
    Stiff
}

public enum MonCycleState
{
    Breath,
    Magic_1,
    Magic_2,
    JumpAttack
};

public class M_FSMTest : MonoBehaviour
{
    //////Monster Status 스테이터스
    public int HP = 500;                            //HP

    public int stiffValue = 0;                     //경직치
    public int maxStiffValue = 50;                  //경직이 일어나는 최대 경직치

    private bool isDie = false;                     //사망 여부

    private bool isInDelay = false;                 //판단 딜레이 여부
    public float delayTime = 1.0f;                 //딜레이 시간
    private float delayTimeCounter = 0.0f;          //딜레이 카운터

    private bool isCycling = false;                 //사이클 실행중

    private bool isInActive = false;                //행동 여부

    private bool isStop = false;                    //사망 여부

    private Transform tr;
    private NavMeshAgent nvAgent;
    private Animator animator;

    public GameObject[] monAttkArms;                //몬스터 공격 팔 콜리더 
    public GameObject monAttkBody;                  //몬스터 공격 바디 콜리더 

    public MonTestState monState = MonTestState.Idle;
    public MonCycleState monCycleState = MonCycleState.Breath;



    //Player Information 
    private Transform playerTr;
    private bool isPlayerDie = false;

    //////거리 판단
    private float trToPlayerDist;                   //tr에서 playerTr의 거리
    private Vector3 trToPlayerTrVector;             //tr에서 playerTr을 가리키는 Vector


    //////공격 거리
    public float minCycleRange = 4.0f;
    public float maxCycleRange = 14.0f;

    //회전
    public float rotateTime = 0.5f;                 //회전하는 시간

    //바디 프레스
    public int bodyPressDamage = 20;                //바디 프레스 데미지
    public GameObject monAttkBodyPress;             //바디 프레스 콜리더
    public float bodyPressTime = 1.0f;              //바디 프레스 시간

    //브레스
    IEnumerator breathCorutine;
    public int breathDamage = 20;                   //브레스 데미지
    public GameObject monAttkBreath;                //브레스 콜리더
    public float breathTime = 2.0f;                 //브레스 시간

    //마법 1
    public int magic_1_Damage = 20;                 //Magic_1 데미지
    public float magic_1_Speed = 15.0f;                  //Magic_1 발사 스피드
    public Transform[] monAttkMagic_1_Pivots;       //Magic_1가 발동될 Pivot
    public GameObject monAttkMagic_1;               //Magic_1의 구체   

    //마법 2
    public int magic_2_Damage = 20;                 //Magic_2 데미지
    public float magic_2_Speed = 15.0f;                  //Magic_1 발사 스피드
    public Transform[] monAttkMagic_2_Pivots;       //Magic_2가 발동될 Pivot
    public GameObject monAttkMagic_2;               //Magic_2의 구체   

    //점프 공격
    public int jumpAttkDamage = 20;                 //점프공격 데미지
    public float jumpAttkJumpTime = 0.43f;          //점프하는 시간


    void Start()
    {
        breathCorutine = MonBreath();

        //컴포넌트 가져오기
        tr = GetComponent<Transform>();
        nvAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        playerTr = GameObject.FindWithTag(Tags.Player).GetComponent<Transform>();


        //공격용 콜리더 찾음
        //monAttkArms = GameObject.FindGameObjectsWithTag(Tags.MonsterArm);
        //monAttkBody = GameObject.FindGameObjectWithTag(Tags.MonsterBody);

        //공격용 콜리더 비활성화
        foreach (GameObject armObj in monAttkArms)
        { armObj.SetActive(false); }
        monAttkBody.SetActive(false);
        monAttkBodyPress.SetActive(false);

        //브레스 비활성화
        monAttkBreath.SetActive(false);

        //Magic_1 Pivot 찾음
        monAttkMagic_1_Pivots = GameObject.Find("Magic_1_Pivots").GetComponentsInChildren<Transform>();


        //행동 시작
        //StartCoroutine(this.ActiveMonster());
    }

    //Update에서 판단
    void Update()
    {
        //위치 검사용 벡터 계산
        trToPlayerTrVector = Vector3.Normalize(playerTr.position - tr.position);

        //플레이어 사망 판단
        //isPlayerDie = Manager.HP < 0 ? true : false;


        //죽으면 판단 중지
        if (!isDie)
        {
            if (isPlayerDie)
            {
                monState = MonTestState.Idle;
                delayTime = 1.0f;
                ///
                nvAgent.Stop();
                animator.SetBool("IsRunning", false);
                ///
            }
            ////다른 행동 중이면 판단 중지
            //else if(isInActive)
            //{
            //    //아무것도 안함
            //}

            else if (maxStiffValue < stiffValue)
            {

                stiffValue = 0;

                isInDelay = true;
                delayTime = 2.0f;
                delayTimeCounter = 0.0f;

                Stiff();
            }

            //딜레이면 판단 중지
            else if (!isInDelay)
            {
                //////거리 판단
                trToPlayerDist = Vector3.Distance(tr.position, playerTr.position);

                //////행동 판단
                if (trToPlayerDist < minCycleRange)
                {
                    //바디프레스
                    monState = MonTestState.Press;
                    isCycling = false;
                    delayTime = 10.0f;
                    ///
                    nvAgent.Stop();
                    isInActive = true;
                    StartCoroutine(this.MonBodyPress());
                    ///
                }
                else if (trToPlayerDist < maxCycleRange)
                {
                    //공격 사이클 돌아감
                    monState = MonTestState.Cycle;
                    ///
                    nvAgent.Stop();
                    animator.SetBool("IsRunning", false);
                    ///

                    if (isCycling) //사이클 실행중이었으면
                    {
                        //다음 사이클 스킬로 넘어감
                        switch (monCycleState)
                        {
                            case MonCycleState.Breath:
                                monCycleState = MonCycleState.Magic_1;
                                delayTime = 4.0f;
                                ///
                                isInActive = true;
                                StartCoroutine(this.MonMagic_1());
                                ///
                                break;

                            case MonCycleState.Magic_1:
                                monCycleState = MonCycleState.Magic_2;
                                delayTime = 4.0f;
                                ///
                                isInActive = true;
                                StartCoroutine(this.MonMagic_2());
                                ///
                                break;

                            case MonCycleState.Magic_2:
                                monCycleState = MonCycleState.JumpAttack;
                                delayTime = 4.0f;
                                ///
                                isInActive = true;
                                StartCoroutine(this.MonJumpAttk());
                                ///
                                break;

                            case MonCycleState.JumpAttack:
                                monCycleState = MonCycleState.Breath;
                                delayTime = 4.0f;
                                ///
                                isInActive = true;
                                StartCoroutine(this.MonBreath());
                                ///
                                break;
                        }
                    }
                    else //사이클 실행중이 아니라면
                    {
                        isCycling = true;                       //사이클 실행으로 처리
                        monCycleState = MonCycleState.Breath;   //브레스 스킬로 진입
                        delayTime = 4.0f;
                        ///
                        isInActive = true;
                        StartCoroutine(this.MonBreath());
                        ///
                    }
                }
                else
                {
                    //달리기
                    monState = MonTestState.Run;
                    isCycling = false;
                    delayTime = 0.2f;
                    ///
                    nvAgent.Resume();
                    nvAgent.destination = playerTr.position;
                    animator.SetBool("IsRunning", true);
                    ///    
                }

                //다음 판단까지 딜레이
                isInDelay = true;
            }
            else
            {
                if (!isInActive)
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
        }

        //////HP가 0 이하 시 사망처리
        if (HP <= 0)
        {
            DieMonster();
            Debug.Log("Die");
        }
    }


    ////행동 코루틴
    //IEnumerator ActiveMonster()
    //{
    //    while(true)
    //    {
    //        //죽으면 행동 중지
    //        if (!isDie)
    //        {
    //            //다른 행동 중이면 행동 중지
    //            if (!isInActive)
    //            {
    //                Debug.Log("DelayTime - " + delayTimeCounter);

    //                switch (monState)
    //                {
    //                    //대기
    //                    case MonTestState.Idle:
    //                        {
    //                            nvAgent.Stop();
    //                            animator.SetBool("IsRunning", false);
    //                            Debug.Log("Active - Idle");
    //                        }
    //                        break;

    //                    //바디프레스
    //                    case MonTestState.Press:
    //                        {
    //                            nvAgent.Stop();
    //                            isInActive = true;
    //                            StartCoroutine(this.MonBodyPress());
    //                            Debug.Log("Active - Press");
    //                        }
    //                        break;

    //                    //공격 사이클
    //                    case MonTestState.Cycle:
    //                        {
    //                            nvAgent.Stop();
    //                            Debug.Log("Active - Cycle");

    //                            switch (monCycleState)
    //                            {
    //                                case MonCycleState.Breath:
    //                                    isInActive = true;
    //                                    StartCoroutine(this.MonBreath());
    //                                    Debug.Log("Active - Breath");
    //                                    break;

    //                                case MonCycleState.Magic_1:
    //                                    isInActive = true;
    //                                    StartCoroutine(this.MonMagic_1());
    //                                    Debug.Log("Active - Magic_1");
    //                                    break;

    //                                case MonCycleState.Magic_2:
    //                                    isInActive = true;
    //                                    StartCoroutine(this.MonMagic_2());
    //                                    Debug.Log("Active - Magic_2");
    //                                    break;

    //                                case MonCycleState.JumpAttack:
    //                                    isInActive = true;
    //                                    StartCoroutine(this.MonJumpAttk());
    //                                    Debug.Log("Active - JumpAttack");
    //                                    break;
    //                            }
    //                        }
    //                        break;

    //                    //달리기
    //                    case MonTestState.Run:
    //                        {
    //                            nvAgent.Resume();
    //                            nvAgent.destination = playerTr.position;
    //                            animator.SetBool("IsRunning", true);
    //                            Debug.Log("Active - Run");
    //                        }
    //                        break;

    //                    //경직
    //                    case MonTestState.Stiff:
    //                        {
    //                            nvAgent.Stop();
    //                        }
    //                        break;
    //                }
    //            }
    //        }

    //        yield return new WaitForEndOfFrame();
    //        //yield return new WaitForSeconds(delayTime);
    //    }
    //}

    //플레이어의 방향으로 회전
    IEnumerator RotateToPlayer()
    {
        Quaternion startRotation = tr.rotation;
        float rotateGage = 0;


        if (isStop)
        {
            isStop = false;
            yield break;
        }

        Quaternion tempRot = Quaternion.LookRotation(trToPlayerTrVector);
        tempRot.eulerAngles = new Vector3(0, tempRot.eulerAngles.y, 0);


        //->플레이어를 바라보도록 회전
        while (rotateGage <= 1)
        {
            //tr.rotation = Quaternion.Slerp(startRotation, Quaternion.LookRotation(trToPlayerTrVector), rotateGage);
            tr.rotation = Quaternion.Slerp(startRotation, tempRot, rotateGage);

            rotateGage += 0.01f / rotateTime;

            if (isStop)
            {
                isStop = false;
                yield break;
            }

            yield return new WaitForSeconds(0.01f);
        }

        //이거 부하가 꽤 크단 말이지...????????
    }


    //바디 프레스
    IEnumerator MonBodyPress()
    {

        //회전 코루틴 실행
        yield return StartCoroutine(this.RotateToPlayer());

        //애니메이션 실행
        animator.SetTrigger("BodyPress");

        //몸통의 콜리더 활성화
        monAttkBodyPress.SetActive(true);

        if (isStop)
        {
            monAttkBodyPress.SetActive(false);
            isStop = false;
            yield break;
        }

        yield return new WaitForSeconds(bodyPressTime);

        if (isStop)
        {
            monAttkBodyPress.SetActive(false);
            isStop = false;
            yield break;
        }

        //몸통의 콜리더 비활성화
        monAttkBodyPress.SetActive(false);

        if (isStop)
        {
            isStop = false;
            yield break;
        }

        //행동 종료 딜레이 종료
        isInActive = false;
        delayTime = 0.0f;

    }


    //브레스
    IEnumerator MonBreath()
    {
        Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);
        Vector3 endScale = new Vector3(3.0f, 20.0f, 3.0f);
        float breathSize = 0;


        if (isStop)
        {
            isStop = false;
            yield break;
        }

        //회전 코루틴 실행
        //Debug.Log("Breath - Rotate");
        yield return StartCoroutine(this.RotateToPlayer());


        if (isStop)
        {
            isStop = false;
            yield break;
        }

        //애니메이션 실행
        animator.SetTrigger("Breath");

        if (isStop)
        {
            isStop = false;
            yield break;
        }

        //브레스 활성화
        //Debug.Log("Breath - BreathActive");
        monAttkBreath.SetActive(true);

        if (isStop)
        {
            monAttkBreath.SetActive(false);
            isStop = false;
            yield break;
        }


        while (breathSize <= 1)
        {
            monAttkBreath.GetComponent<Transform>().localScale = Vector3.Lerp(startScale, endScale, breathSize);
            breathSize += 0.01f / breathTime;

            yield return new WaitForSeconds(0.01f);
        }

        if (isStop)
        {
            monAttkBreath.SetActive(false);
            isStop = false;
            yield break;
        }

        monAttkBreath.GetComponent<Transform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);

        //브레스 비활성화
        //Debug.Log("Breath - BreathExit");
        monAttkBreath.SetActive(false);

        if (isStop)
        {
            monAttkBreath.SetActive(false);
            isStop = false;
            yield break;
        }

        //행동 종료 딜레이 종료
        isInActive = false;
        delayTime = 0.0f;
    }


    //마법 1
    IEnumerator MonMagic_1()
    {
        //회전 코루틴 실행
        yield return StartCoroutine(this.RotateToPlayer());

        if (isStop)
        {
            isStop = false;
            yield break;
        }

        //20개의 Pivot중 사용할 10개의 Pivot의 Num 골라냄
        int[] selectNums = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        bool isOverlaping = false;

        for (int i = 0; i < 10;)
        {
            selectNums[i] = Random.Range(0, 19);

            for (int j = 0; j < i; j++)
            {
                if (selectNums[j] == selectNums[i])
                {
                    isOverlaping = true;
                    break;
                }
            }

            if (isOverlaping)
            {
                isOverlaping = false;
                continue;
            }

            i++;
        }

        if (isStop)
        {
            isStop = false;
            yield break;
        }

        //애니메이션 실행
        animator.SetTrigger("Magic_1");

        //10개의 Pivot에 마법 생성
        for (int i = 0; i < 10; i++)
        {
            Instantiate(monAttkMagic_1, monAttkMagic_1_Pivots[selectNums[i]].position, monAttkMagic_1_Pivots[selectNums[i]].rotation);

            if (isStop)
            {
                isStop = false;
                yield break;
            }

            yield return new WaitForSeconds(0.05f);
        }

        if (isStop)
        {
            isStop = false;
            yield break;
        }

        yield return new WaitForSeconds(1.5f);

        if (isStop)
        {
            isStop = false;
            yield break;
        }

        //행동 종료 딜레이 종료
        isInActive = false;
        delayTime = 0.0f;
    }



    //마법 2
    IEnumerator MonMagic_2()
    {
        if (isStop)
        {
            isStop = false;
            yield break;
        }

        //회전 코루틴 실행
        yield return StartCoroutine(this.RotateToPlayer());

        if (isStop)
        {
            isStop = false;
            yield break;
        }

        //애니메이션 실행
        animator.SetTrigger("Magic_2");


        //지정된 Pivot에 마법 생성
        foreach (Transform pivot in monAttkMagic_2_Pivots)
        {
            if (isStop)
            {
                isStop = false;
                yield break;
            }

            Instantiate(monAttkMagic_2, pivot.position, pivot.rotation);
        }

        if (isStop)
        {
            isStop = false;
            yield break;
        }

        yield return new WaitForSeconds(1.5f);

        if (isStop)
        {
            isStop = false;
            yield break;
        }

        //행동 종료 딜레이 종료
        isInActive = false;
        delayTime = 0.0f;
    }


    //점프공격
    IEnumerator MonJumpAttk()
    {
        if (isStop)
        {
            isStop = false;
            yield break;
        }

        //회전 코루틴 실행
        yield return StartCoroutine(this.RotateToPlayer());

        if (isStop)
        {
            isStop = false;
            yield break;
        }

        Vector3 startPosition = tr.position;
        Vector3 endPosition = playerTr.position;
        float jumpDistance = 0;

        //몸통의 콜리더 활성화
        monAttkBodyPress.SetActive(true);

        //애니메이션 실행
        animator.SetTrigger("JumpAttack");

        //플레이어의 위치를 받아 점프.
        while (jumpDistance <= 1)
        {
            tr.position = Vector3.Lerp(startPosition, endPosition, jumpDistance);

            jumpDistance += 0.01f / jumpAttkJumpTime;

            yield return new WaitForSeconds(0.01f);
        }

        //몸통의 콜리더 비활성화
        monAttkBodyPress.SetActive(false);

        if (isStop)
        {
            isStop = false;
            yield break;
        }

        //행동 종료 딜레이 종료
        isInActive = false;
        delayTime = 0.0f;
    }

    //경직
    void Stiff()
    {
        nvAgent.Stop();


        isInActive = false;

        animator.SetTrigger("Stiff");

        switch (monState)
        {
            case MonTestState.Press:
                {
                    //StopCoroutine("RotateToPlayer");
                    //StopCoroutine("MonBodyPress");
                    isStop = true;
                }
                break;

            //공격 사이클
            case MonTestState.Cycle:
                {
                    //StopCoroutine("RotateToPlayer");

                    switch (monCycleState)
                    {
                        case MonCycleState.Breath:
                            //StopCoroutine(breathCorutine);
                            isStop = true;
                            break;

                        case MonCycleState.Magic_1:
                            //StopCoroutine("MonMagic_1");
                            isStop = true;
                            break;

                        case MonCycleState.Magic_2:
                            //StopCoroutine("MonMagic_2");
                            isStop = true;
                            break;

                        case MonCycleState.JumpAttack:
                            //StopCoroutine("MonJumpAttk");
                            isStop = true;
                            break;
                    }
                }
                break;
        }
    }

    //////몬스터 사망 
    void DieMonster()
    {
        isDie = true;

        Destroy(gameObject);
    }
}
