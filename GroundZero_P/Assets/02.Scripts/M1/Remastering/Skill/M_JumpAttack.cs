using UnityEngine;
using System.Collections;

public class M_JumpAttack : M_Skill
{
    #region SingleTon

    public static M_JumpAttack instance = null;

    void Awake()
    { instance = this; }

    #endregion


    //JumpAttack 
    public GameObject jumpAttack;                                               //바디 프레스 콜리더


    //최초 스킬 초기화
    public override void InitSkill()
    {
        skillStatus.SkillCode = M_SkillCoad.JumpAttack;

        jumpAttack.SetActive(false);
    }

    //스킬 사용                   
    public override IEnumerator UseSkill(Vector3 target)
    {
        //플레이어를 향해 회전
        yield return StartCoroutine(this.RotateToPoint(m_Core.transform, target, lookRotationTime));

        m_Core.IsDoingOther = true;                                             //행동 시작


        jumpAttack.SetActive(true);                                             //몸통의 콜리더 활성화

        //스킬 프로퍼티스 설정
        jumpAttack.GetComponent<M_AttackCtrl>().SetAttackProperty(skillStatus.damage, false);

        Vector3 startPosition = m_Core.transform.position;                      //출발 위치
        Vector3 endPosition = target;                                           //도착 위치
        float jumpDistance = 0;

        m_Core.Animator.SetTrigger("JumpAttack");                               //애니메이션 실행

        yield return new WaitForSeconds(skillStatus.beforeDelayTime);           //점프공격 대기

        //플레이어의 위치를 받아 점프.
        while (jumpDistance <= 1)
        {
            m_Core.transform.position = Vector3.Lerp(startPosition, endPosition, jumpDistance);
            jumpDistance += 0.01f / skillStatus.curTime;

            yield return new WaitForSeconds(0.01f);
        }

        jumpAttack.SetActive(false);                                            //몸통의 콜리더 비활성화

        yield return new WaitForSeconds(skillStatus.AfterDelayTime);            //점프공격 대기


        m_Core.IsDoingOther = false;                                //행동 종료 
    }

    //스킬 캔슬 시 처리           
    public override void CancelSkill()
    {
        jumpAttack.SetActive(false);
    }
}
