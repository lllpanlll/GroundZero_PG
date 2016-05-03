using UnityEngine;
using System.Collections;

public class M_AlleyBreath_2 : M_Skill
{
    #region SingleTon

    public static M_AlleyBreath_2 instance = null;

    void Awake()
    { instance = this; }

    #endregion


    //골목 브레스 2
    public GameObject alleyBreath_2_Pref;                               //골목 브레스_2 프리팹
    private GameObject alleyBreath_2_Obj;                               //골목 브레스_2 오브젝트
    private ObjectPool alleyBreath_2_Pool = new ObjectPool();           //골목 브레스_2 오브젝트풀
    public ObjectPool AlleyBreath_2_Pool { get { return alleyBreath_2_Pool; } }

    public Transform alleyBreath_2_Pivot;                               //골목 브레스_2이 발동될 Pivot 

    public float alleyBreath_2_Speed = 2000.0f;                         //골목 브레스_2 발사 속도

    //잔불
    public GameObject emberPref;                                        //잔불 프리팹
    private ObjectPool emberPool = new ObjectPool();                    //잔불 오브젝트풀
    public ObjectPool EmberPool { get { return emberPool; } }

    public int emberDamage = 10;                                        //잔불 데미지
    public float emberCurTime = 2.0f;                                   //잔불 유지시간
    public float createEmberDistance = 4.0f;                            //잔불 생성 간격
    public float CreatrEmberDistance { get { return createEmberDistance; } }



    //최초 스킬 초기화
    public override void InitSkill()
    {
        skillStatus.SkillCode = M_SkillCoad.AlleyBreath_2;

        alleyBreath_2_Pool.CreatePool(alleyBreath_2_Pref, 20);
        emberPool.CreatePool(emberPref, 50);
    }

    //스킬 사용                   
    public override IEnumerator UseSkill(Vector3 target)
    {
        //안쪽 코너를 향해 회전
        yield return StartCoroutine(this.RotateToPoint(m_Core.transform, target, lookRotationTime));

        m_Core.IsDoingOther = true;                                             //행동 시작


        yield return new WaitForSeconds(skillStatus.beforeDelayTime);
        
        m_Core.Animator.SetTrigger("Breath");                                   //애니메이션 실행

        //골목 브레스 1 오브젝트 풀 사용
        alleyBreath_2_Obj = alleyBreath_2_Pool.UseObject();

        alleyBreath_2_Obj.transform.position = alleyBreath_2_Pivot.position;
        alleyBreath_2_Obj.transform.rotation = alleyBreath_2_Pivot.rotation;

        alleyBreath_2_Obj.GetComponent<M_AlleyBreath_2_Ctrl>().Starting = M_Alley.instance.AlleyGateTrigger;
        alleyBreath_2_Obj.GetComponent<M_AlleyBreath_2_Ctrl>().Destination = M_Alley.instance.
            AlleyGateTrigger.GetComponent<M_AlleyBreath_2_Points>().connectPoints[0];

        //스킬 프로퍼티스 설정
        alleyBreath_2_Obj.GetComponent<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);

        yield return new WaitForSeconds(skillStatus.AfterDelayTime);


        m_Core.IsDoingOther = false;                                            //행동 종료 
    }

    //스킬 캔슬 시 처리           
    public override void CancelSkill()
    {
        //얜 경직 불가
    }
}
