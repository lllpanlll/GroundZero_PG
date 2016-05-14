using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Monster Magic_2 Control
/// 마법 2 발사체 컨트롤. 앞을 향해 발사된다.
/// 
/// *코멘트
///     <<추가완료>>  오브젝트 풀 사용
/// </summary>



public class M_Magic_2_Ctrl : MonoBehaviour
{
    private float traceSpeed;                               //추적 속도
    private float speed;                                    //발사 속도
    public float delayTime = 0.5f;                          //발사 대기 시간
    private float maxDist = 100.0f;                         //최대 사거리

    private bool isShooted = false;                         //이미 발사되었는가
    public bool IsShooted { get { return isShooted; } }

    private Transform shootPointTr;                         //발사하려는 Transform 위치
    public Transform ShootPointTr { set { shootPointTr = value; } }
    private Vector3 shootPos;                               //발사 위치
    

    private bool isStart = false;                           //Start 함수 호출 후인지



    //OnEnable
    void OnEnable()
    {
        isShooted = false;

        if (!isStart)                                       //아직 초기화가 되어있지 않다면
        {
            //필요한 정보 수집
            traceSpeed = M_Magic_2.instance.magic_2_TraceSpeed;
            speed = M_Magic_2.instance.magic_2_Speed;
            delayTime = M_Magic_2.instance.magic_2_ShootDelayTIme;
            maxDist = M_Magic_2.instance.magic_2_MaxDist;

            isStart = true;
        }
        else
        {
            StartCoroutine(this.ShootMagic_2());            //마법 2 발사
        }
    }



    //마법 2 발사
    IEnumerator ShootMagic_2()
    {
        float timeCounter = 0.0f;

        yield return new WaitForSeconds(0.03f);                                         //위치 설정이 되길 대기

        //딜레이 타임 동안 발사 시작 위치를 따라다닌다
        while (timeCounter < delayTime)
        {
            transform.position = Vector3.Lerp(transform.position, shootPointTr.position, Time.deltaTime * traceSpeed);
            transform.rotation = shootPointTr.rotation;

            timeCounter += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        shootPos = shootPointTr.position;                                               //발사가 시작되는 위치 저장


        GetComponent<Rigidbody>().AddForce(transform.forward * speed);                  //바라보는 방향으로 Speed만큼 힘을 가해 발사

        StartCoroutine(this.CheckDist());                                               //거리 체크 시작

        isShooted = true;                                                               //발사 완료
    }


    //거리 체크
    IEnumerator CheckDist()
    {
        while (true)
        {
            //발사 시작 지점에서 일정 거리 이상 멀어지면 마법 2 비활성화
            if (Vector3.Distance(gameObject.transform.position, shootPos) > maxDist)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);     //이전에 적용된 힘 제거
                gameObject.SetActive(false);
                isShooted = false;

                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }


    //OnTriggerEnter
    void OnTriggerEnter(Collider coll)
    {
        //if (coll.gameObject.layer == LayerMask.NameToLayer(Layers.Bullet))              //플레이어의 공격(총알)하면 파괴
        //{
        //    GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);         //이전에 적용된 힘 제거
        //    gameObject.SetActive(false);
        //}

        //else 
        if (coll.gameObject.layer == LayerMask.NameToLayer(Layers.Floor))          //큰 배경(바닥 벽)에 부딛히면 파괴
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);         //이전에 적용된 힘 제거
            gameObject.SetActive(false);
        }
    }
}
