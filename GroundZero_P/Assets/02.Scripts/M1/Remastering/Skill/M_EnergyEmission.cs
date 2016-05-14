using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// EnergyEmission
/// 에너지 방출. 콜리더를 키워가면서 공격을 날린다.
/// 
/// *코멘트
/// </summary>



public class M_EnergyEmission : M_Skill
{
    #region SingleTon

    public static M_EnergyEmission instance = null;

    void Awake()
    { instance = this; }

    #endregion

    public GameObject energyEmission;                                               //바디 프레스 장판 콜리더

    public Vector3 energyEmissionMinScale = new Vector3(12.0f, 1.0f, 12.0f);        //바디 프레스 장판 콜리더 크기
    public Vector3 energyEmissionMaxScale = new Vector3(40.0f, 7.0f, 40.0f);        //바디 프레스 장판 콜리더 크기



    //최초 스킬 초기화
    public override void InitSkill()
    {
        skillStatus.SkillCode = M_SkillCoad.EnergyEmission;
        
        energyEmission.SetActive(false);
    }

    //스킬 사용                   
    public override IEnumerator UseSkill(Vector3 target)
    {
        if (m_Core.IsRigid)                                                     //경직이면 아무것도 하지 않는다
            yield break;

        energyEmission.GetComponent<Transform>().localScale = energyEmissionMinScale;    //원래 사이즈로 축소
        float energySize = 0;

        //플레이어를 향해 회전
        yield return StartCoroutine(this.RotateToPoint(m_Core.transform, target, lookRotationTime));
        
        m_Core.IsDoingOther = true;                                             //행동 시작

        //Debug.Log("EnergyEmission Start");

        m_Core.Animator.SetTrigger("BodyPress");                                //애니메이션 실행

        yield return new WaitForSeconds(skillStatus.beforeDelayTime);

        energyEmission.SetActive(true);                                      //에너지 활성화
        
        //Debug.Log("EnergyEmission Active Start");

        //스킬 프로퍼티스 설정
        energyEmission.GetComponentInChildren<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);

        while (energySize <= 1)                                                 //에너지 사이즈 점점 키우기
        {
            energyEmission.GetComponent<Transform>().localScale = Vector3.Lerp(energyEmissionMinScale, energyEmissionMaxScale, energySize);
            energySize += 0.01f / skillStatus.curTime;

            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.08f);

        //Debug.Log("EnergyEmission Active End");

        energyEmission.SetActive(false);                                     //에너지 비활성화

        yield return new WaitForSeconds(skillStatus.AfterDelayTime);

        //Debug.Log("EnergyEmission Delay End");

        m_Core.IsDoingOther = false;                                            //행동 종료 

        ResetUseSkillNone();                                                    //스킬 상태 None으로 복귀
    }

    //스킬 캔슬 시 처리           
    public override void CancelSkill()
    {
        energyEmission.SetActive(false);
    }
}
