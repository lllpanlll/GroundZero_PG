using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Monster Magic_1 Control
/// 
/// *코멘트
///     <<추가완료>>  오브젝트 풀 사용
///     <<추가완료>> 아직 출발하지 않은 애들은 캔슬 시 삭제
/// </summary>

    

public class M_Magic_1_Ctrl : MonoBehaviour
{
    private float traceSpeed;                           //추적 속도
    private float speed;                                //발사 속도    
    private float delayTime = 1.0f;                     //발사 대기 시간
    private float maxDist = 100.0f;                     //최대 사거리

    private bool isShooted = false;                     //이미 발사되었는가
    public bool IsShooted { get { return isShooted; } }


    private Transform shootPointPivotTr;                //발사 위치들의 중심점

    private Transform shootPointTr;                     //발사하려는 Transform 위치
    public Transform ShootPointTr { set{ shootPointTr = value; } }
    private Vector3 shootPos;                           //발사 위치

    private Transform playerTr;                         //플레이어 위치
    private Vector3 trToPlayerVector;                   //플레이어를 향하는 벡터


    private bool isStart = false;                       //Start 함수 호출 후인지



    //OnEnable
    void OnEnable()
    {
        isShooted = false;
        
        if (!isStart)                                           //아직 초기화가 되어있지 않다면
        {
            //필요한 정보 수집
            traceSpeed = M_Magic_1.instance.magic_1_TraceSpeed;
            speed = M_Magic_1.instance.magic_1_Speed;
            delayTime = M_Magic_1.instance.magic_1_ShootDelayTime;
            maxDist = M_Magic_1.instance.magic_1_MaxDist;
            shootPointPivotTr = M_Magic_1.instance.magic_1_Pivots[0];

            playerTr = GameObject.FindWithTag(Tags.Player).GetComponent<Transform>();

            isStart = true;
        }
        else
        {
            StartCoroutine(this.ShootMagic_1());            //마법 1 발사
        }
    }



    //마법 1 발사
    IEnumerator ShootMagic_1()
    {
        float timeCounter = 0.0f;
        
        yield return new WaitForSeconds(0.03f);                                             //위치 설정이 되길 대기
        
        //딜레이 타임 동안 발사 시작 위치를 따라다닌다
        while (timeCounter < delayTime)
        {
            transform.position = Vector3.Lerp(transform.position, shootPointTr.position, Time.deltaTime * traceSpeed);

            timeCounter += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        shootPos = shootPointTr.position;                                                   //발사가 시작되는 위치 저장


        trToPlayerVector = Vector3.Normalize(transform.position - shootPointPivotTr.position);  //약간 곡선을 그리며 발사되기 위해 바깥 방향 벡터 구함 
        GetComponent<Rigidbody>().AddForce(trToPlayerVector * speed * 0.05f);               //바깥 방향으로 소량의 Speed만큼 힘을 가해 발사

        StartCoroutine(this.GuidedMagic());                                                 //유도 마법 및 거리 체크 시작

        isShooted = true;                                                                   //발사 완료
    }

        
    //유도미사일화
    IEnumerator GuidedMagic()
    {
        while (true)
        {
            trToPlayerVector = Vector3.Normalize(playerTr.position - transform.position);   //플레이어를 향하는 방향 벡터를 구함
            GetComponent<Rigidbody>().AddForce(trToPlayerVector * speed * Time.deltaTime * 3.2f);  //플레이어 방향으로 소량의 Speed만큼 힘을 가해 발사
            
            //발사 시작 지점에서 일정 거리 이상 멀어지면 마법 1 비활성화
            if (Vector3.Distance(gameObject.transform.position, shootPos) > maxDist)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);         //이전에 적용된 힘 제거
                gameObject.SetActive(false);                                                //마법 1 비활성화
                isShooted = false;

                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    
    //OnTriggerEnter
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer.Equals(LayerMask.NameToLayer(Layers.Floor)))
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);             //이전에 적용된 힘 제거
            gameObject.SetActive(false);                                                    //마법 1 비활성화
        }
    }
}
