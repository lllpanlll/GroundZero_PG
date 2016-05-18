using UnityEngine;
using System.Collections;
using System;

namespace T2.Skill
{
    public class SilverStream : T2.Skill.Skill, T2.Skill.ISkillTimer
    {
        #region<싱글톤>
        private static T2.Skill.SilverStream instance;
        public static T2.Skill.SilverStream GetInstance()
        {
            if (!instance)
            {
                instance = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.Skill.SilverStream>();
                if (!instance)
                    print("SilverStream 인스턴스 생성에 실패하였습니다.");
            }
            return instance;
        }
        #endregion

        //스킬의 필수 기본 변수들, 나중에 public으로 변환.
        public Manager.SkillType PointType = Manager.SkillType.PP;
        public int iDecPoint = 10;
        public float beforeDelayTime = 0.5f;
        public float afterDelayTime = 0.0f;
        public float coolTime = 0.0f;
        //버프가 지속되는 시간.
        public float actionTime = 30.0f;
        //============================================
        //고유 스킬 변수
        public bool bSilverStream;

        void Awake()
        {
            bSilverStream = false;
        }

        public override void Enter(SkillCtrl skillCtrl)
        {
            base.Enter(skillCtrl);
            base.skillCtrl.mgr.DecreaseSkillPoint(PointType, iDecPoint);
            base.CoolTimeCoroutine = CoolTimer(coolTime);
            skillCtrl.mgr.ChangeState(T2.Manager.State.Skill);

            //스킬이 끝난 후, 이동속도를 '처음'부터 가속하기 위해 moveState를 Stop으로 해 놓는다.
            base.skillCtrl.moveCtrl.SetMoveState(T2.MoveCtrl.MoveState.Stop);

            //선딜 타이머 시작.                
            StartCoroutine(BeforeDelayTimer(beforeDelayTime));
        }

        public override void Execute(SkillCtrl skillCtrl)
        {
            //피격시 비정상 종료.
            if (skillCtrl.mgr.GetState() == T2.Manager.State.be_Shot)
                Exit(skillCtrl);
        }

        public override void Exit(SkillCtrl skillCtrl)
        {
            base.Exit(skillCtrl);
        }

        public IEnumerator BeforeDelayTimer(float time)
        {
            //스킬 시전 애니메이션 시작

            //마우스회전 막아야 하지 않을까..?
            //base.skillCtrl.mgr.SetCtrlPossible(T2.Manager.CtrlPossibleIndex.MouseRot, false);
            yield return new WaitForSeconds(time);
            StartCoroutine(ActionTimer(actionTime));
        }

        public IEnumerator ActionTimer(float time)
        {
            bSilverStream = true;
            StartCoroutine(AfterDelayTimer(afterDelayTime));


            //디멘션볼 오브젝트가 있으면 폭파시킨다.
            if(DimensionBall.GetInstance().oDimensionBall.activeSelf)
                DimensionBall.GetInstance().oDimensionBall.GetComponent<T2.Pref.DimensionBallPref>().ExplosionDimensionBall();
            yield return new WaitForSeconds(time);
            
            bSilverStream = false;
        }

        public IEnumerator AfterDelayTimer(float time)
        {
            yield return new WaitForSeconds(time);
            //후딜레이가 끝나면 State를 idle상태로 체인지한다.
            base.skillCtrl.mgr.ChangeState(T2.Manager.State.idle);

            base.bUsing = false;
            Exit(base.skillCtrl);
        }
                
        public IEnumerator CoolTimer(float time)
        {
            print("실버스트림 쿨 시작");
            base.bCoolTime = true;
            yield return new WaitForSeconds(time);
            base.bCoolTime = false;
            print("실버스트림 쿨 끝");
        }
    }
}