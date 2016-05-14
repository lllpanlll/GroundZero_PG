using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// AlleyBreath 2 Control
/// 골목 브레스 2 발사체 컨트롤. 골목 안쪽을 향해 발사되며, 분기를 만났을 때 갈라진다.
/// 
/// *코멘트
/// </summary>



public class M_AlleyBreath_2_Ctrl : MonoBehaviour {

    private float speed;                                        //발사 속도  

    private GameObject alleyBreath_2_Obj;                       //골목 브레스_2 오브젝트

    private Rigidbody rigidBody;                                //골목 브레스_2 리지드 바디

    private GameObject starting;                                //시작 지점 GameObject
    public GameObject Starting { set{ starting = value; } }
    private GameObject destination;                             //목표 지점 GameObject
    public GameObject Destination { set { destination = value; } }


    private GameObject emberObj;                                //잔불 오브젝트

    private int emberDamage = 10;                               //잔불 데미지

    private float createEmberDistance = 3.0f;                   //잔불 생성 간격                        
    private Vector3 beforeEmberPos;                             //잔불 생성 이전 위치

    private bool isEndSoon = false;                             //발사 금방 종료
    private bool isEnd = false;                                 //발사 종료

    private bool isStart = false;                               //초기화 여부

    

    //OnEnable
    void OnEnable()
    {
        if (!isStart)                                           //아직 초기화가 되어있지 않다면
        {
            //필요한 정보 수집
            speed = M_AlleyBreath_2.instance.alleyBreath_2_Speed;
            emberDamage = M_AlleyBreath_2.instance.emberDamage;

            rigidBody = GetComponent<Rigidbody>();

            isStart = true;
        }
        else
        {
            isEnd = false;
            isEndSoon = false;

            StartCoroutine(this.CheckAlleyBreath_2());                                      //발사 및 거리체크 시작
        }
    }

    //브레스 발사 및 거리 체크
    IEnumerator CheckAlleyBreath_2()
    {
        yield return new WaitForSeconds(0.03f);
        
        //Debug.Log(starting.transform.position + " " + destination.transform.position);
        
        rigidBody.AddForce(Vector3.Normalize(destination.transform.position
            - starting.transform.position) * speed);                                                //목표지점 방향으로 Speed만큼 힘을 가해 브레스 발사

        beforeEmberPos = transform.position;                                                        //잔불 생성 이전 위치 시작 위치로 초기화  
        createEmberDistance = M_AlleyBreath_2.instance.CreatrEmberDistance;                         //잔불 생성 간격

        while (true)
        {
            if (isEnd)                                                                              //발사가 종료되면 체크 종료
                yield break;
            
            //Debug.Log(Vector3.Distance(transform.position, destination.transform.position));
            
            //목표 지점에 어느정도 가까워지면 
            if (!isEndSoon &&
                (Vector3.Distance(transform.position, destination.transform.position) < 2.5f))
            {
                M_AlleyBreath_2_Points alleyBreath_2_Points = destination.GetComponent<M_AlleyBreath_2_Points>();

                bool canUseThis = true;

                GameObject curStarting = starting;
                GameObject curDestination = destination;


                yield return new WaitForSeconds(0.1f);                                              //5의 거리를 두고 검사했으니 그만큼 다시 가라고...


                //이전에 Starting지점을 제외한 다른 Point들이 있는지 체크 <- 연결 지점이 1개 이상인지 (1개라면 온 데밖에 없는것임)
                if (alleyBreath_2_Points.connectPoints.Length > 1)
                {
                    for (int i = 0; i < alleyBreath_2_Points.connectPoints.Length; ++i)
                    {
                        //이 목적지에 연결된 시작지점(온 방향)을 제외한 지점은
                        if (!alleyBreath_2_Points.connectPoints[i].Equals(curStarting))
                        {
                            if (canUseThis)          //현재 이 오브젝트를 사용할 수 있는 상황이라면 
                            {
                                starting = curDestination;                                          //시작지점 갱신
                                destination = alleyBreath_2_Points.connectPoints[i];                //목표지점 갱신

                                rigidBody.velocity = new Vector3(0.0f, 0.0f, 0.0f);                 //이전에 적용된 힘 제거
                                rigidBody.AddForce(Vector3.Normalize(
                                    destination.transform.position - transform.position) * speed);  //목표지점 방향으로 Speed만큼 힘을 가해 브레스 발사

                                canUseThis = false; //더는 못써
                            }

                            else                    //이 오브젝트를 더는 못쓰면
                            {
                                alleyBreath_2_Obj = M_AlleyBreath_2.instance.AlleyBreath_2_Pool.UseObject();

                                alleyBreath_2_Obj.transform.position = new Vector3(
                                    curDestination.transform.position.x, transform.position.y, curDestination.transform.position.z);
                                alleyBreath_2_Obj.transform.rotation = transform.rotation;

                                alleyBreath_2_Obj.GetComponent<M_AlleyBreath_2_Ctrl>().Starting = curDestination;
                                alleyBreath_2_Obj.GetComponent<M_AlleyBreath_2_Ctrl>().Destination = alleyBreath_2_Points.connectPoints[i];

                                //스킬 프로퍼티스 설정
                                alleyBreath_2_Obj.GetComponent<M_AttackCtrl>().SetAttackProperty(M_AlleyBreath_2.instance.skillStatus.damage, false);
                            }
                        }
                    }
                }

                else
                {
                    //없으면 거기가 마지막 지점, 종료 대기
                    StartCoroutine(WaitForDestroy());
                    isEndSoon = true;
                }
            }

            

            //이전 잔불 생성 위치에서 생성 간격만큼 거리가 벌어지면 잔불 생성
            if (Vector3.Distance(transform.position, beforeEmberPos) > createEmberDistance - 1.0f)
            {
                beforeEmberPos = transform.position;

                emberObj = M_AlleyBreath_2.instance.EmberPool.UseObject();

                emberObj.transform.position = new Vector3(transform.position.x, 0.25f, transform.position.z);
                emberObj.transform.rotation = transform.rotation;

                emberObj.GetComponent<M_EmberCtrl>().CurTime = M_AlleyBreath_2.instance.emberCurTime;

                //스킬 프로퍼티스 설정
                emberObj.GetComponent<M_AttackCtrl>().SetAttackProperty(emberDamage, false);
            }


            yield return new WaitForEndOfFrame();
        }
    }


    IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(0.1f);

        isEnd = true;

        rigidBody.velocity = new Vector3(0.0f, 0.0f, 0.0f);                         //이전에 적용된 힘 제거
        gameObject.SetActive(false);                                                //골목 브레스 1 비활성화
    }

}
