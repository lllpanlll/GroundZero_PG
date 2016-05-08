using UnityEngine;
using System.Collections;
using System;

namespace T2.Skill
{
    public class DimensionBall : T2.Skill.Skill, T2.Skill.ISkillTimer
    {
        #region<싱글톤>
        private static T2.Skill.DimensionBall instance;
        public static T2.Skill.DimensionBall GetInstance()
        {
            if (!instance)
            {
                instance = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.Skill.DimensionBall>();
                if (!instance)
                    print("DimensionBall 인스턴스 생성에 실패하였습니다.");
            }
            return instance;
        }
        #endregion

        //스킬의 필수 기본 변수들, 나중에 public으로 변환.
        public Manager.SkillType PointType = Manager.SkillType.PP;
        public int iDecPoint = 10;
        public float beforeDelayTime = 0.0f;
        public float actionTime = 0.0f;
        public float afterDelayTime = 0.0f;
        public float coolTime = 0.0f;

        //각 스킬의 고유 변수들
        public int iDamage = 10;
        public float fBallSpeed = 30.0f;
        public float fReach = 10.0f;
        public GameObject oDimensionBallPref;
        private GameObject oDimensionBall;
        private ObjectPool ballPool = new ObjectPool();

        void Start()
        {
            ballPool.CreatePool(oDimensionBallPref, 3);
        }

        public override void Enter(SkillCtrl skillCtrl)
        {
            base.Enter(skillCtrl);
            //기본 변수 초기화.
            base.skillCtrl.mgr.DecreaseSkillPoint(Manager.SkillType.EP, iDecPoint);
            base.CoolTimeCoroutine = CoolTimer(coolTime);

            skillCtrl.mgr.ChangeState(T2.Manager.State.Skill);
            //선딜 타이머 시작.                
            StartCoroutine(BeforeDelayTimer(beforeDelayTime));
        }
        public override void Execute(SkillCtrl skillCtrl)
        {
            //피격시 비정상 종료.
            if (skillCtrl.mgr.GetState() == T2.Manager.State.be_Shot)
            {
                Exit(skillCtrl);
            }
            base.Execute(skillCtrl);
        }
        public override void Exit(SkillCtrl skillCtrl)
        {
            base.Exit(skillCtrl);
        }


        public IEnumerator BeforeDelayTimer(float time)
        {
            yield return new WaitForSeconds(time);
            StartCoroutine(ActionTimer(actionTime));
        }

        public IEnumerator ActionTimer(float time)
        {
            //특정 스킬로 쿨타임이 없어지면 오브젝트가 늘어날 가능성이 존재하여 임시로 오브젝트풀의 모든 오브젝트가 활성화되어있지 않은
            //상태인 경우에만 스킬이 발동되도록 한다.
            if (!ballPool.FullActiveCheck())
            {
                //투사체를 활성화시킨뒤, 현재 위치에서 에임방향으로 발사한다.
                //oDimensionBall.SetActive(true);
                oDimensionBall = ballPool.UseObject();
                //캐릭터의 y축 1.5미터 지점, z축 1.0미터 지점에서부터 카메라 방향으로 레이를 만든다.
                Ray aimRay = new Ray(transform.position + (transform.up * 1.5f + transform.forward * 1.0f), skillCtrl.cam.transform.forward);
                oDimensionBall.transform.position = aimRay.origin;

                //카메라에서 쏘는 레이가 부딪힌 위치에 플레이어의 총알이 발사되는 각도를 조정한다.
                RaycastHit aimRayHit;
                if (Physics.Raycast(aimRay, out aimRayHit, fReach, 1))
                    oDimensionBall.transform.LookAt(aimRayHit.point);
                else
                    oDimensionBall.transform.LookAt(aimRay.GetPoint(fReach));
            }
            yield return new WaitForSeconds(time);
            StartCoroutine(AfterDelayTimer(afterDelayTime));
        }

        public IEnumerator AfterDelayTimer(float time)
        {
            yield return new WaitForSeconds(time);
            //후딜레이가 끝나면 State를 idle상태로 체인지한다.
            base.skillCtrl.mgr.ChangeState(T2.Manager.State.idle);

            //후딜이 끝나면 bUsing을 정상적으로 false시키고 Exit()한다.
            base.bUsing = false;
            Exit(base.skillCtrl);
        }


        public IEnumerator CoolTimer(float time)
        {
            base.bCoolTime = true;
            yield return new WaitForSeconds(time);
            base.bCoolTime = false;
        }
    }
}
