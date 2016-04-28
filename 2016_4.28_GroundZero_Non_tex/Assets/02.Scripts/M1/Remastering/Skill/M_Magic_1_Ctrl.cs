using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Monster Magic_1 Control
/// 
/// *코멘트
///     <<추가완료>>  오브젝트 풀 사용
/// </summary>


//<<추가>> 아직 출발하지 않은 애들은 캔슬 시 삭제된대. -> 출발했나 안했나 bool변수 쓰자

public class M_Magic_1_Ctrl : MonoBehaviour
{
    private float speed;                                //발사 속도    
    private float delayTime = 1.0f;                     //발사 대기 시간
    private float maxDist = 100.0f;                     //최대 사거리

    private bool isShooted = false;                     //이미 발사되었는가
    public bool IsShooted { get { return isShooted; } }

    private Transform monsterTr;                        //몬스터 위치
    private Vector3 shootPos;                           //발사 위치

    private Transform playerTr;                         //플레이어 위치
    private Vector3 trToPlayerVector;                   //플레이어를 향하는 벡터


    private bool isStart = false;                       //Start 함수 호출 후인지



    //Start
    void Start()
    {
        //필요한 정보 수집
        speed = M_Magic_1.instance.magic_1_Speed;
        delayTime = M_Magic_1.instance.magic_1_ShootDelayTIme;
        maxDist = M_Magic_1.instance.magic_1_MaxDist;

        monsterTr = GameObject.FindWithTag(Tags.Monster).GetComponent<Transform>();
        playerTr = GameObject.FindWithTag(Tags.Player).GetComponent<Transform>();


        isStart = true;
        
        shootPos = monsterTr.position;                  //발사 시점의 몬스터의 위치를 발사 시작 지점으로 설정
    }


    //OnEnable
    void OnEnable()
    {
        isShooted = false;

        if (isStart)                                    //Start함수가 호출된 후, 초기화가 완료된 시점에서
            shootPos = monsterTr.position;              //발사 시점의 몬스터의 위치를 발사 시작 지점으로 설정
        
        StartCoroutine(this.ShootMagic_1());            //마법 1 발사
    }



    //마법 1 발사
    IEnumerator ShootMagic_1()
    {
        yield return new WaitForSeconds(delayTime);                                         //발사 대기

        trToPlayerVector = Vector3.Normalize(playerTr.position - transform.position);       //플레이어를 향하는 방향 벡터를 구함

        GetComponent<Rigidbody>().AddForce(trToPlayerVector * speed);                       //플레이어 방향으로 Speed만큼 힘을 가해 발사

        StartCoroutine(this.CheckDist());                                                   //거리 체크 시작

        isShooted = true;                                                                   //발사 완료
    }


    //거리 체크
    IEnumerator CheckDist()
    {
        while (true)
        {
            //발사 시작 지점에서 일정 거리 이상 멀어지면 마법 1 비활성화
            if (Vector3.Distance(gameObject.transform.position, shootPos) > maxDist)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);         //이전에 적용된 힘 제거
                gameObject.SetActive(false);                                                //마법 1 비활성화


                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
