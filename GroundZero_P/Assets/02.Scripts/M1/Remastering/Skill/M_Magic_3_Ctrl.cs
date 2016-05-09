using UnityEngine;
using System.Collections;

public class M_Magic_3_Ctrl : MonoBehaviour {
    
    private float delayTime = 1.0f;                     //발사 대기 시간
    private float maxDist = 100.0f;                     //최대 사거리

    private bool isShooted = false;                     //이미 발사되었는가
    public bool IsShooted { get { return isShooted; } }

    private float gravity = -9.8f;
    private float horizontalSpeed = 10.0f;
    private float verticalSpeed = 10.0f;
    private float forcastTime = 0.0f;

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
            horizontalSpeed = M_Magic_3.instance.horizontalSpeed;
        

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

        gravity = M_Magic_3.instance.gravity;
        horizontalSpeed = M_Magic_3.instance.horizontalSpeed;

        destination = playerTr.position;                                                            //발사를 시작하려는 타이밍의 플레이어 위치를 목표 지점으로 삼음

        toDestination = Vector3.Normalize(new Vector3(destination.x, 0, destination.z) 
            - new Vector3(transform.position.x, 0, transform.position.z));                          //목표 지점을 향하는 수평 성분만의 방향 벡터 산출

        forcastTime = Vector2.Distance(new Vector2(destination.x, destination.z),
           new Vector2(transform.position.x, transform.position.z)) / horizontalSpeed;              //타겟까지의 거리를 수평성분 속도로 나눠서 이동에 걸릴 예상 시간을 산출

        verticalSpeed = (-(transform.position.y - destination.y) / forcastTime) - (0.5f * gravity * forcastTime);  //4번째 운동방정식의 변형으로 수직성분 속도를 구한다
        

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

                gameObject.SetActive(false);                                                //마법 3 비활성화
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
