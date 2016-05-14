using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// AlleyBreath 1 Control
/// 골목 브레스 1의 발사체 컨트롤.
/// 
/// *코멘트
/// </summary>



public class M_AlleyBreath_1_Ctrl : MonoBehaviour
{
    private float speed;                                                        //발사 속도  
    private float maxDist = 100.0f;                                             //최대 사거리

    private Vector3 shootPos;                                                   //발사 위치
    private Vector3 targetPos;                                                  //목표 위치


    private GameObject emberObj;                                                //잔불 오브젝트

    private bool isRoleOfEmber = false;                                         //잔불 생성 역할 여부 
    public bool IsRoleOfEmber { set { isRoleOfEmber = value; } }                

    private int emberDamage = 10;                                               //잔불 데미지

    private float createEmberDistance = 3.0f;                                   //잔불 생성 간격                        
    private Vector3 beforeEmberPos;                                             //잔불 생성 이전 위치

    private bool isEndSoon = false;                                             //발사 금방 종료
    private bool isEnd = false;                                                 //발사 종료

    private bool isStart = false;                                               //초기화 여부



    //OnEnable
    void OnEnable()
    {
        if (!isStart)                                                           //아직 초기화가 되어있지 않다면
        {
            //필요한 정보 수집
            speed = M_AlleyBreath_1.instance.alleyBreath_1_Speed;
            emberDamage = M_AlleyBreath_1.instance.emberDamage;

            isStart = true;
        }
        else
        {
            isEnd = false;
            isEndSoon = false;

            targetPos = M_AlleyBreath_1.instance.TargetPos;                     //목표 위치 찾음
            
            StartCoroutine(this.CheckAlleyBreath_1());                          //발사 및 거리체크 시작
        }
    }
    
    //브레스 발사 및 거리 체크
    IEnumerator CheckAlleyBreath_1()
    {
        yield return new WaitForSeconds(0.03f);
        
        GetComponent<Rigidbody>().AddForce(Vector3.Normalize(targetPos - transform.position) * speed); //플레이어 방향으로 Speed만큼 힘을 가해 브레스 발사

        beforeEmberPos = transform.position;                                                //잔불 생성 이전 위치 시작 위치로 초기화  
        createEmberDistance = M_AlleyBreath_1.instance.CreatrEmberDistance;                 //잔불 생성 간격

        while (true)
        {
            if (isEnd)                                                                      //발사가 종료되면 체크 종료
                yield break;


            //목표 지점에 어느정도 가까워지면 
            if (!isEndSoon &&
                (Vector3.Distance(transform.position, targetPos) < 5.0f))
            {
                StartCoroutine(WaitForDestroy());
                isEndSoon = true;
            }
            

            //잔불 생성 역할을 가지고 있으며   이전 잔불 생성 위치에서 생성 간격만큼 거리가 벌어지면
            if (isRoleOfEmber &&
                (Vector3.Distance(transform.position, beforeEmberPos) > createEmberDistance - 1.0f))
            {
                beforeEmberPos = transform.position;

                emberObj = M_AlleyBreath_1.instance.EmberPool.UseObject();

                emberObj.transform.position = new Vector3(transform.position.x, 0.25f, transform.position.z);
                emberObj.transform.rotation = transform.rotation;

                emberObj.GetComponent<M_EmberCtrl>().CurTime = M_AlleyBreath_1.instance.emberCurTime;

                //스킬 프로퍼티스 설정
                emberObj.GetComponent<M_AttackCtrl>().SetAttackProperty(emberDamage, false);
            }


            yield return new WaitForEndOfFrame();
        }
    }


    IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(0.2f);

        isEnd = true;

        GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);         //이전에 적용된 힘 제거
        gameObject.SetActive(false);                                                //골목 브레스 1 비활성화
    }
}
