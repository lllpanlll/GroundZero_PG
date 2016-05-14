using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Breath
/// 브레스. 입에서 브레스를 키워가면서 공격.
/// 
/// *코멘트
/// </summary>



public class M_Breath : M_Skill
{
    #region SingleTon

    public static M_Breath instance = null;

    void Awake()
    { instance = this; }

    #endregion

    //브레스
    public GameObject breath;                                                   //브레스 오브젝트
    public Vector3 breathEndScale = new Vector3(3.0f, 20.0f, 3.0f);             //브레스 콜리더 최종 크기



    //최초 스킬 초기화
    public override void InitSkill()
    {
        skillStatus.SkillCode = M_SkillCoad.Breath;

        breath.SetActive(false);
    }

    //스킬 사용                   
    public override IEnumerator UseSkill(Vector3 target)
    {
        if (m_Core.IsRigid)                                                     //경직이면 아무것도 하지 않는다
            yield break;

        Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);                     //원본 사이즈
        breath.GetComponent<Transform>().localScale = startScale;            //원래 사이즈로 축소
        float breathSize = 0;

        //플레이어를 향해 회전
        yield return StartCoroutine(this.RotateToPoint(m_Core.transform, target, lookRotationTime));


        m_Core.IsDoingOther = true;                                             //행동 시작


        m_Core.Animator.SetTrigger("Breath");                                   //애니메이션 실행

        breath.SetActive(true);                                              //브레스 활성화

        //스킬 프로퍼티스 설정
        breath.GetComponentInChildren<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);

        while (breathSize <= 1)                                                 //브레스 사이즈 점점 키우기
        {
            breath.GetComponent<Transform>().localScale = Vector3.Lerp(startScale, breathEndScale, breathSize);
            breathSize += 0.01f / skillStatus.curTime;

            yield return new WaitForSeconds(0.01f);
        }

        breath.SetActive(false);                                             //브레스 비활성화

        yield return new WaitForSeconds(skillStatus.AfterDelayTime);


        m_Core.IsDoingOther = false;                                            //행동 종료 

        ResetUseSkillNone();                                                    //스킬 상태 None으로 복귀
    }

    //스킬 캔슬 시 처리           
    public override void CancelSkill()
    {
        breath.SetActive(false);
    }
}
