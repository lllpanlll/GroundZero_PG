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
    public GameObject safeZone;                                                 //안전지대 콜리더
    public GameObject safeZoneEff;                                              //안전지대 경고 이펙트
    
    public float pressDelayTime = 0.25f;    

    private bool isPlayerInSafeZone = true;
    public bool IsPlayerInSafeZone
    {
        set { isPlayerInSafeZone = value; }
        get { return isPlayerInSafeZone; }
    }

    private bool canRigid = false;
    public bool CanRigid { get{ return canRigid; } }



    //최초 스킬 초기화
    public override void InitSkill()
    {
        skillStatus.SkillCode = M_SkillCoad.BodyPress;

        //스킬 프로퍼티스 설정
        safeZone.GetComponent<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);

        safeZone.SetActive(false);
        safeZoneEff.SetActive(false);
    }

    //스킬 사용                   
    public override IEnumerator UseSkill(Vector3 target)
    {
        if (m_Core.IsRigid)                                                     //경직이면 아무것도 하지 않는다
            yield break;

        m_Core.IsDoingOther = true;                                             //행동 시작


        canRigid =  true;                                                       //준비동작 동안엔 경직 가능

        yield return new WaitForSeconds(skillStatus.beforeDelayTime);           //스킬 선딜
        
        for (int i = 0; i < 5; i++)                                             //안전지대 표시 이펙트
        {
            safeZoneEff.SetActive(true);

            yield return new WaitForSeconds(skillStatus.beforeDelayTime * 0.1f);     

            safeZoneEff.SetActive(false);

            yield return new WaitForSeconds(skillStatus.beforeDelayTime * 0.1f);         
        }

        canRigid = false;                                                       //공격에 들어가면 경직 불가

        m_Core.Animator.SetTrigger("BodyPress");                                //애니메이션 실행

        yield return new WaitForSeconds(pressDelayTime);                        //바닥을 찍는 애니메이션이 될 때까지 대기


        safeZone.SetActive(true);                                               //안전지대 활성화

        yield return new WaitForSeconds(skillStatus.curTime);

        safeZone.SetActive(false);                                              //안전지대 비활성화

        isPlayerInSafeZone = true;                                              //스킬 종료되면 플레이어 체크 초기화


        yield return new WaitForSeconds(skillStatus.AfterDelayTime);
        
        
        m_Core.IsDoingOther = false;                                            //행동 종료

        ResetUseSkillNone();                                                    //스킬 상태 None으로 복귀
    }

    //스킬 캔슬 시 처리           
    public override void CancelSkill()
    {
        safeZone.SetActive(false);                                             //안전지대 비활성화
        safeZoneEff.SetActive(false);
        isPlayerInSafeZone = true;
    }
}

