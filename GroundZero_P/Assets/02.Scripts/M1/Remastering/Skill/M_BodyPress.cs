using UnityEngine;
using System.Collections;

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
    
    public Vector3 floorScale = new Vector3(25.0f, 1.0f, 25.0f);       //바디 프레스 장판 콜리더 크기
    

    //최초 스킬 초기화
    public override void InitSkill()
    {
        skillStatus.SkillCode = M_SkillCoad.BodyPress;

        bodyPress.SetActive(false);
        bodyPressFloor.SetActive(false);
    }

    //스킬 사용                   
    public override IEnumerator UseSkill(Vector3 target)
    {
        Vector3 startScale = bodyPressFloor.GetComponent<Transform>().localScale;  //공용 콜리더이기 때문에 원본 사이즈 미리 저장
        bodyPressFloor.GetComponent<Transform>().localScale = floorScale;       //원하는 사이즈로 확대

        m_Core.IsDoingOther = true;                                             //행동 시작

        
        m_Core.Animator.SetTrigger("BodyPress");                                //애니메이션 실행

        yield return new WaitForSeconds(skillStatus.beforeDelayTime);           //바닥을 찍는 애니메이션이 될 때까지 대기

        bodyPress.SetActive(true);                                              //바디 프레스용 콜리더 활성화
        bodyPressFloor.SetActive(true);                                         //바디 프레스 장판용 콜리더 활성화

        //스킬 프로퍼티스 설정
        bodyPress.GetComponent<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);
        bodyPressFloor.GetComponentInChildren<M_AttackCtrl>().SetAttackProperty(floorDamage, false);

        yield return new WaitForSeconds(skillStatus.curTime);

        bodyPress.SetActive(false);                                             //바디 프레스용  콜리더 비활성화
        bodyPressFloor.SetActive(false);                                        //바디 프레스 장판용 콜리더 비활성화

        bodyPressFloor.GetComponent<Transform>().localScale = startScale;       //원본 사이즈로 축소

        yield return new WaitForSeconds(skillStatus.AfterDelayTime);
        
        m_Core.IsDoingOther = false;                                            //행동 종료
    }

    //스킬 캔슬 시 처리           
    public override void CancelSkill()
    {
        bodyPress.SetActive(false);
    }
}
