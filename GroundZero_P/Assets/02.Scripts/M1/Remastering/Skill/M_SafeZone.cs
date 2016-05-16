using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// SafeZone
/// 안전지대. 이펙트 출력 후 안전지대 외 부분 공격
/// 
/// *코멘트
/// </summary>



public class M_SafeZone : M_Skill
{
    #region SingleTon

    public static M_SafeZone instance = null;

    void Awake()
    { instance = this; }

    #endregion

    //바디 프레스
    public GameObject safeZone;                                                 //바디 프레스 콜리더
    public GameObject safeZoneEff;                                                 //바디 프레스 콜리더

    public int floorDamage = 20;                                                //장판 데미지

    public float pressDelayTime = 0.25f;    

    private bool isPlayerInSafeZone = true;
    public bool IsPlayerInSafeZone
    {
        set { isPlayerInSafeZone = value; }
        get { return isPlayerInSafeZone; }
    }


    //최초 스킬 초기화
    public override void InitSkill()
    {
        skillStatus.SkillCode = M_SkillCoad.BodyPress;

        safeZone.SetActive(false);
        safeZoneEff.SetActive(false);
    }

    //스킬 사용                   
    public override IEnumerator UseSkill(Vector3 target)
    {
        if (m_Core.IsRigid)                                                     //경직이면 아무것도 하지 않는다
            yield break;

        m_Core.IsDoingOther = true;                                             //행동 시작


        yield return new WaitForSeconds(skillStatus.beforeDelayTime);           //바닥을 찍는 애니메이션이 될 때까지 대기

        //블링크
        for (int i = 0; i < 3; i++)                                            //랜덤한 순서의 위치로 마법 발사
        {
            safeZoneEff.SetActive(true);

            yield return new WaitForSeconds(skillStatus.beforeDelayTime * 0.333f);           //다음 마법 생성까지 딜레이

            safeZoneEff.SetActive(false);
        }


        m_Core.Animator.SetTrigger("BodyPress");                                //애니메이션 실행

        yield return new WaitForSeconds(pressDelayTime);           //바닥을 찍는 애니메이션이 될 때까지 대기

        safeZone.SetActive(true);                                              //바디 프레스용 콜리더 활성화

        //스킬 프로퍼티스 설정
        safeZone.GetComponent<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);

        yield return new WaitForSeconds(skillStatus.curTime);

        safeZone.SetActive(false);                                             //바디 프레스용 콜리더 비활성화

        isPlayerInSafeZone = true;


        yield return new WaitForSeconds(skillStatus.AfterDelayTime);
        


        m_Core.IsDoingOther = false;                                            //행동 종료

        ResetUseSkillNone();                                                    //스킬 상태 None으로 복귀
    }

    //스킬 캔슬 시 처리           
    public override void CancelSkill()
    {
        safeZone.SetActive(false);                                             //바디 프레스용 콜리더 비활성화
        safeZoneEff.SetActive(false);
        isPlayerInSafeZone = true;
    }
}

