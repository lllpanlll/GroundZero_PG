using UnityEngine;
using System.Collections;

namespace T2.Skill
{
    //모든 스킬들이 상속받게 되는 슈퍼클래스
    public class Skill : MonoBehaviour
    {
        protected bool bBeforeDelay;
        protected bool bAction;
        protected bool bAfterDelay;
        //bCoolTime은 후딜까지 종료된 시점부터 재사용 가능한 시간까지를 체크한다.
        [HideInInspector]
        public bool bCoolTime;
        //bUsing은 스킬이 시작된 순간부터 스킬이 종료되는 순간까지를 체크한다.
        //(선딜레이 시작 ~ 후딜레이 끝)
        [HideInInspector]
        public bool bUsing;
        //현재 사용중인 스킬을 캔슬시킬 수 있는 스킬인지 판별하는 변수.
        [HideInInspector]
        public bool bSkillCancel = false;

        protected T2.Skill.SkillCtrl skillCtrl;
        protected IEnumerator CoolTimeCoroutine;

        public virtual void Enter(T2.Skill.SkillCtrl skillCtrl) {
            bBeforeDelay = false;
            bAction = false;
            bAfterDelay = false;
            this.skillCtrl = skillCtrl;
            //this.skillCtrl.mgr.
        }
        public virtual void Execute(T2.Skill.SkillCtrl skillCtrl) { }
        public virtual void Exit(T2.Skill.SkillCtrl skillCtrl)
        {
            bBeforeDelay = false;
            bAction = false;
            bAfterDelay = false;


            //쿨타임 시작.
            if (CoolTimeCoroutine != null)
                StartCoroutine(CoolTimeCoroutine);

            skillCtrl.mgr.ChangeState(T2.Manager.State.idle);

            //정상 종료 체크
            if (bUsing == false)
            {
                /*
                정상종료 된다면 curSkill을 IdleSkill로 바로 대입한다.
                (changeSkillState()함수로 바꾸면 무한 뺑뺑이가 돌아버림...)
                IdleSkill에는 아무 기능도 없고 재설정해야하는 변수는 다른곳에서
                이미 했기때문에 바로 대입해도 상관없을 듯 하다.
                */
                if (skillCtrl.curSkill != T2.Skill.IdleSkill.GetInstance())
                    skillCtrl.curSkill = T2.Skill.IdleSkill.GetInstance();
            }
            else
            {
                bUsing = false;
            }
        }
    }

    //스킬 타이머 인터페이스
    public interface ISkillTimer
    {
        IEnumerator BeforeDelayTimer(float time);
        IEnumerator ActionTimer(float time);
        IEnumerator AfterDelayTimer(float time);
        IEnumerator CoolTimer(float time);
    }

}
