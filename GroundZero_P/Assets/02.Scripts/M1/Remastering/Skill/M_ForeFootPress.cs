using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// ForeFootPress
/// 앞발 찍기.
/// 
/// *코멘트
/// </summary>



public class M_ForeFootPress : M_Skill
{
    #region SingleTon

    public static M_ForeFootPress instance = null;

    void Awake()
    { instance = this; }

    #endregion


    //ForeFootAttack
    public GameObject attkArm;                                                  //오른팔 콜리더



    //최초 스킬 초기화
    public override void InitSkill()
    {
        skillStatus.SkillCode = M_SkillCoad.AlleyForeFootPress;

        attkArm.SetActive(false);
    }

    //스킬 사용                   
    public override IEnumerator UseSkill(Vector3 target)
    {
        if (m_Core.IsRigid)                                                     //경직이면 아무것도 하지 않는다
            yield break;

        //플레이어를 향해 회전
        yield return StartCoroutine(this.RotateToPoint(m_Core.transform, target, lookRotationTime));

        m_Core.IsDoingOther = true;                                             //행동 시작


        m_Core.Animator.SetTrigger("ForeFootPress");                            //애니메이션 실행

        attkArm.SetActive(true);                                                //왼팔의 콜리더 활성화

        attkArm.GetComponent<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);  //스킬 프로퍼티스 설정

        yield return new WaitForSeconds(skillStatus.curTime);                   //스킬 사용 시간동안 대기

        attkArm.SetActive(false);                                               //왼팔의 콜리더 비활성화


        m_Core.IsDoingOther = false;                                            //행동 종료

        ResetUseSkillNone();                                                //스킬 상태 None으로 복귀
    }

}
