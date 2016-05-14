using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// AlleyBreath 1 
/// 골목 브레스 1. 골목 안쪽을 향해 발사체를 연사하고, 잔불을 남긴다.
/// 
/// *코멘트
/// </summary>



public class M_AlleyBreath_1 : M_Skill
{
    #region SingleTon

    public static M_AlleyBreath_1 instance = null;

    void Awake()
    { instance = this; }

    #endregion


    //골목 브레스 1
    public GameObject alleyBreath_1_Pref;                                       //골목 브레스_1 프리팹
    private GameObject alleyBreath_1_Obj;                                       //골목 브레스_1 오브젝트
    private ObjectPool alleyBreath_1_Pool = new ObjectPool();                   //골목 브레스_1 오브젝트풀

    public Transform alleyBreath_1_Pivot;                                       //골목 브레스_1이 발동될 Pivot 

    public int alleyBreath_1_FireNum = 3;                                       //골목 브레스_1의 발사체 갯수
    public float alleyBreath_1_CreateDelayTime = 0.5f;                          //골목 브레스_1 생성 시간 차

    public float alleyBreath_1_Speed = 2000.0f;                                 //골목 브레스_1 발사 속도

    private Vector3 targetPos;                                                  //목표 위치
    public Vector3 TargetPos { get { return targetPos; } }


    //잔불
    public GameObject emberPref;                                                //잔불 프리팹
    private ObjectPool emberPool = new ObjectPool();                            //잔불 오브젝트풀
    public ObjectPool EmberPool { get{ return emberPool; } }

    public int emberDamage = 10;                                                //잔불 데미지
    public float emberCurTime = 2.0f;                                           //잔불 유지시간
    public float createEmberDistance = 4.0f;                                    //잔불 생성 간격
    public float CreatrEmberDistance { get { return createEmberDistance; } }
    


    //최초 스킬 초기화
    public override void InitSkill()
    {
        skillStatus.SkillCode = M_SkillCoad.AlleyBreath_1;

        alleyBreath_1_Pool.CreatePool(alleyBreath_1_Pref, alleyBreath_1_FireNum * 2);
        emberPool.CreatePool(emberPref, 30);
    }

    //스킬 사용                   
    public override IEnumerator UseSkill(Vector3 target)
    {
        if (m_Core.IsRigid)                                                     //경직이면 아무것도 하지 않는다
            yield break;

        targetPos = target;

        //안쪽 코너를 향해 회전
        yield return StartCoroutine(this.RotateToPoint(m_Core.transform, target, lookRotationTime));

        m_Core.IsDoingOther = true;                                             //행동 시작
        

        m_Core.Animator.SetTrigger("Breath");                                   //애니메이션 실행

        for (int i = 0; i < alleyBreath_1_FireNum; ++i)                         //일정 시간 간격을 두고 정해진 갯수의 발사체 발사
        {
            //골목 브레스 1 오브젝트 풀 사용
            alleyBreath_1_Obj = alleyBreath_1_Pool.UseObject();

            alleyBreath_1_Obj.transform.position = alleyBreath_1_Pivot.position;
            alleyBreath_1_Obj.transform.rotation = alleyBreath_1_Pivot.rotation;

            if (i.Equals(0))
                alleyBreath_1_Obj.GetComponent<M_AlleyBreath_1_Ctrl>().IsRoleOfEmber = true;    //잔불 생성 역할 위임
               
            else
                alleyBreath_1_Obj.GetComponent<M_AlleyBreath_1_Ctrl>().IsRoleOfEmber = false;   
               
            //스킬 프로퍼티스 설정
            alleyBreath_1_Obj.GetComponent<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);

            yield return new WaitForSeconds(alleyBreath_1_CreateDelayTime);     //다음 발사체 생성까지 딜레이
        }

        yield return new WaitForSeconds(skillStatus.AfterDelayTime);


        m_Core.IsDoingOther = false;                                            //행동 종료 

        ResetUseSkillNone();                                                    //스킬 상태 None으로 복귀
    }

    //스킬 캔슬 시 처리           
    public override void CancelSkill()
    {
        //얜 경직 불가
    }
}
