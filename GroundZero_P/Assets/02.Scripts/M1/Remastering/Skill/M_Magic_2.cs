using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Magic 2
/// 마법 2. 3개의 지점에서 발사체 발사.
/// 
/// *코멘트
/// </summary>



public class M_Magic_2 : M_Skill
{
    #region SingleTon

    public static M_Magic_2 instance = null;

    void Awake()
    { instance = this; }

    #endregion


    public GameObject magic_2_Pref;                                 //Magic_2의 오브젝트   
    private GameObject magic_2_Obj;                                 //Magic_2 오브젝트
    private ObjectPool magic_2_Pool = new ObjectPool();             //Magic_2 오브젝트풀 

    public Transform[] magic_2_Pivots;                              //Magic_2가 발동될 Pivot

    public float magic_2_ShootDelayTIme = 1.0f;                     //Magic_2 발사 대기 시간
    public float magic_2_TraceSpeed = 7.0f;                         //Magic_2 원래 Pivot 추적 Speed
    public float magic_2_Speed = 3500.0f;                           //Magic_2 발사 스피드
    public float magic_2_MaxDist = 100.0f;                          //Magic_2_최대 사거리

    public float magic_2_CastingMonSpeed = 5.0f;                    //Magic_2 캐스팅 동안 몬스터 속력
    private float magic_2_OriginMonSpeed = 20.0f;                   //Magic_2 원래의 몬스터 속력



    //최초 스킬 초기화
    public override void InitSkill()
    {
        skillStatus.SkillCode = M_SkillCoad.Magic_2;

        //스킬 프로퍼티스 설정
        magic_2_Pref.GetComponent<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);

        magic_2_Pool.CreatePool(magic_2_Pref, magic_2_Pivots.Length * 2);
    }

    //스킬 사용                   
    public override IEnumerator UseSkill(Vector3 target)
    {
        if (m_Core.IsRigid)                                                     //경직이면 아무것도 하지 않는다
            yield break;

        m_Core.IsDoingOther = true;                                             //행동 시작


        //원래 속도 저장
        magic_2_OriginMonSpeed = m_Core.NvAgent.speed;

        //이 스킬은 특별히 본체의 속도를 제어합니다.
        m_Core.NvAgent.speed = magic_2_CastingMonSpeed;
        

        //m_Core.Animator.SetTrigger("Magic_2");                                  //애니메이션 실행

        yield return new WaitForSeconds(skillStatus.beforeDelayTime);

        foreach (Transform pivot in magic_2_Pivots)                             //지정된 Pivot에 마법 생성
        {
            //마법 2 오브젝트 풀 사용
            magic_2_Obj = magic_2_Pool.UseObject();

            magic_2_Obj.GetComponent<M_Magic_2_Ctrl>().ShootPointTr = pivot;
            magic_2_Obj.transform.position = pivot.position;
            magic_2_Obj.transform.rotation = pivot.rotation;
        }

        //이 스킬은 특별히 본체의 속도를 제어합니다.
        m_Core.NvAgent.speed = magic_2_OriginMonSpeed;

        yield return new WaitForSeconds(skillStatus.AfterDelayTime);    


        m_Core.IsDoingOther = false;                                            //행동 종료 

        ResetUseSkillNone();                                                    //스킬 상태 None으로 복귀
    }

    //스킬 캔슬 시 처리           
    public override void CancelSkill()
    {
        int num = magic_2_Pool.ObjectNum;
        GameObject tempObj = null;

        for (int i = 0; i < num; ++i)                                           //오브젝트 풀 수 만큼 반복
        {
            tempObj = magic_2_Pool.DetectiveAllObject(i);                       //해당 인덱스의 오브젝트를 빼옴

            if (tempObj &&                                                      //가져온 오브젝트가 활성화되어 있으며
                !tempObj.GetComponent<M_Magic_2_Ctrl>().IsShooted)              //오브젝트가 아직 발사되지 않았다면
            {
                tempObj.SetActive(false);                                       //오브젝트 비활성화
            }
        }
    }
}
