using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// BodyPress
/// 바디 프레스. 바닥을 찍어누르면서 충격파가 생긴다.
/// 
/// *코멘트
/// </summary>



public class M_BodyPress : M_Skill
{
    #region SingleTon

    public static M_BodyPress instance = null;

    void Awake()
    { instance = this; }

    #endregion
    
    //바디 프레스
    public GameObject bodyPress;                                                //바디 프레스 콜리더
    public GameObject bodyPressFloor;                                           //바디 프레스 장판 콜리더

    public int floorDamage = 20;                                                //장판 데미지



    //최초 스킬 초기화
    public override void InitSkill()
    {
        skillStatus.SkillCode = M_SkillCoad.BodyPress;

        //스킬 프로퍼티스 설정
        bodyPress.GetComponent<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);
        bodyPressFloor.GetComponentInChildren<M_AttackCtrl>().SetAttackProperty(floorDamage, false);

        bodyPress.SetActive(false);
        bodyPressFloor.SetActive(false);
    }

    //스킬 사용                   
    public override IEnumerator UseSkill(Vector3 target)
    {
        if (m_Core.IsRigid)                                                     //경직이면 아무것도 하지 않는다
            yield break;

        m_Core.IsDoingOther = true;                                             //행동 시작


        m_Core.Animator.SetTrigger("BodyPress");                                //애니메이션 실행

        yield return new WaitForSeconds(skillStatus.beforeDelayTime);           //바닥을 찍는 애니메이션이 될 때까지 대기

        bodyPress.SetActive(true);                                              //바디 프레스용 콜리더 활성화
        bodyPressFloor.SetActive(true);                                         //바디 프레스 장판용 콜리더 활성화

        yield return new WaitForSeconds(skillStatus.curTime);

        bodyPress.SetActive(false);                                             //바디 프레스용 콜리더 비활성화
        bodyPressFloor.SetActive(false);                                        //바디 프레스 장판용 콜리더 비활성화

        yield return new WaitForSeconds(skillStatus.AfterDelayTime);

        m_Core.IsDoingOther = false;                                            //행동 종료

        ResetUseSkillNone();                                                    //스킬 상태 None으로 복귀
    }

    //스킬 캔슬 시 처리           
    public override void CancelSkill()
    {
        bodyPress.SetActive(false);                                             //바디 프레스용 콜리더 비활성화
        bodyPressFloor.SetActive(false);                                        //바디 프레스 장판용 콜리더 비활성화
    }
}
