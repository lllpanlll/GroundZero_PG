using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Magic 3 Control
/// 마법 3 발사체 컨트롤. 목표 지점까지 포물선을 그리며 이동, 목표 지점에 가까워지면 폭파.
/// 
/// *코멘트
///     <<추가완료>>  거리가 벌어지면 시간이나 높이가 기획자의 의도와는 안맞게 작동. 추후 포물선 공식 수정 필요.
/// </summary>



public class M_Magic_3_Ctrl : MonoBehaviour {
    
    private float delayTime = 1.0f;                             //발사 대기 시간
    private float maxDist = 100.0f;                             //최대 사거리

    private bool isShooted = false;                             //이미 발사되었는가
    public bool IsShooted { get { return isShooted; } }

    private float gravity = -9.8f;
    private float horizontalSpeed = 10.0f;
    private float verticalSpeed = 10.0f;
    private float flightTime = 0.0f;

    private Vector3 destination;                                //목표 위치
    private Transform playerTr;                                 //플레이어 위치

    private Vector3 toDestination;                              //목표 위치 까지의 벡터
    
    private GameObject explosion_Obj;                           //폭발 이펙트 오브젝트

    private bool isStart = false;                               //Start 함수 호출 후인지



    //OnEnable
    void OnEnable()
    {
        isShooted = false;

        if (!isStart)                                           //아직 초기화가 되어있지 않다면
        {
            //필요한 정보 수집
            delayTime = M_Magic_3.instance.magic_3_ShootDelayTime;
            gravity = M_Magic_3.instance.gravity;
            flightTime = M_Magic_3.instance.flightTime;

            playerTr = GameObject.FindWithTag(Tags.Player).GetComponent<Transform>();

            isStart = true;
        }
        else
        {
            StartCoroutine(this.ShootMagic_3());                //마법 3 발사
        }
    }

    //마법 3 발사
    IEnumerator ShootMagic_3()
    {
        yield return new WaitForSeconds(delayTime);                                                 //발사 대기

        destination = playerTr.position;                                                            //발사를 시작하려는 타이밍의 플레이어 위치를 목표 지점으로 삼음

        toDestination = Vector3.Normalize(new Vector3(destination.x, 0, destination.z)
            - new Vector3(transform.position.x, 0, transform.position.z));                          //목표 지점을 향하는 수평 성분만의 방향 벡터 산출

        horizontalSpeed = Vector2.Distance(new Vector2(destination.x, destination.z),
           new Vector2(transform.position.x, transform.position.z)) / flightTime;                   //타겟까지의 거리를 이동 시간으로 나눠서 이동에 걸릴 수평선분 속도를 산출

        verticalSpeed = (-(transform.position.y - destination.y) / flightTime) - (0.5f * gravity * flightTime);  //4번째 운동방정식의 변형으로 수직성분 속도를 구한다


        StartCoroutine(this.ParabolaMagic());                                                       //발사 포물선 운동 및 거리 체크 시작

        isShooted = true;                                                                           //발사 완료
    }


    //포물선
    IEnumerator ParabolaMagic()
    {
        while (true)
        {
            transform.Translate(toDestination * horizontalSpeed * Time.deltaTime, Space.World);
            transform.Translate(transform.up * verticalSpeed * Time.deltaTime, Space.World);

            verticalSpeed += (gravity * Time.deltaTime);

            //목표 지점에 근접하게 되면
            if (Vector3.Distance(gameObject.transform.position, destination) < 3.0f)
            {
                explosion_Obj = M_Magic_3.instance.Explosion_Pool.UseObject();
                explosion_Obj.transform.position = transform.position;

                gameObject.SetActive(false);                                                        //마법 3 비활성화
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }


    //OnTriggerEnter
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer(Layers.Bullet))                          //플레이어의 공격(총알)하면 파괴
        {
            explosion_Obj = M_Magic_3.instance.Explosion_Pool.UseObject();
            explosion_Obj.transform.position = transform.position;

            gameObject.SetActive(false);
        }
    }
}
