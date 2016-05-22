using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace T2.Skill
{
    /// <summary>
    /// 에임 방향으로 무차별 사격하는 컨셉의 스킬.
    /// 이동이 불가능 하다.
    /// </summary>

    class Suppression : T2.Skill.Skill, T2.Skill.ISkillTimer
    {
        #region<싱글톤>
        private static T2.Skill.Suppression instance;
        public static T2.Skill.Suppression GetInstance()
        {
            if (!instance)
            {
                instance = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.Skill.Suppression>();
                if (!instance)
                    print("Suppression 인스턴스 생성에 실패하였습니다.");
            }
            return instance;
        }
        #endregion

        //스킬의 필수 기본 변수들, 나중에 public으로 변환.
        public Manager.SkillType PointType = Manager.SkillType.EP;
        public int iDecPoint = 10;
        public float beforeDelayTime = 0.0f;
        public float afterDelayTime = 0.0f;
        public float coolTime = 0.0f;
        public float bActionTime = 0.8f;

        private float fReach = 100.0f;
        private Vector3 vFireTargetPos = Vector3.zero;
        private Vector3 vFirePos = Vector3.zero;
        public int iMaxCount = 30;
        private int count = 0;
        public float fIntervalTime = 0.1f;
        private float fAccuracy = 0.0f;

        //궁버프 이후 바뀔 수치
        private float beforeDelayTime_Buff = 0.0f;
        private float afterDelayTime_Buff = 0.0f;
        public float coolTime_Buff = 1.0f;
        public int iMaxCount_Buff = 30;
        public float fIntervalTime_Buff = 0.1f;
        //궁버프 이전 수치
        private float beforeDelayTime_Orizin;
        private float afterDelayTime_Orizin;
        private float coolTime_Orizin;
        private int iMaxCount_Orizin;
        private float fIntervalTime_Orizin;

        void Awake()
        {
            beforeDelayTime_Orizin = beforeDelayTime;
            afterDelayTime_Orizin = afterDelayTime;
            coolTime_Orizin = coolTime;
            iMaxCount_Orizin = iMaxCount;
            fIntervalTime_Orizin = fIntervalTime;
        }

        public override void Enter(SkillCtrl skillCtrl)
        {
            //기본 변수 초기화.
            base.Enter(skillCtrl);
            base.skillCtrl.mgr.DecreaseSkillPoint(PointType, iDecPoint);
            base.CoolTimeCoroutine = CoolTimer(coolTime);
            base.bSkillCancel = true;

            if (T2.Skill.SilverStream.GetInstance().bSilverStream == true)
            {
                beforeDelayTime_Orizin = beforeDelayTime_Buff;
                afterDelayTime_Orizin = afterDelayTime_Buff;
                coolTime_Orizin = coolTime_Buff;
                iMaxCount_Orizin = iMaxCount_Buff;
                fIntervalTime_Orizin = fIntervalTime_Buff;
            }
            else
            {
                beforeDelayTime_Orizin = beforeDelayTime;
                afterDelayTime_Orizin = afterDelayTime;
                coolTime_Orizin = coolTime;
                iMaxCount_Orizin = iMaxCount;
                fIntervalTime_Orizin = fIntervalTime;
            }

            count = 0;

            skillCtrl.mgr.ChangeState(T2.Manager.State.Skill);
            //선딜 타이머 시작.                
            StartCoroutine(BeforeDelayTimer(beforeDelayTime));
        }

        public override void Execute(SkillCtrl skillCtrl)
        {
            //피격시 비정상 종료.
            if (skillCtrl.mgr.GetState() == T2.Manager.State.be_Shot)
            {
                print("beShot");
                Exit(skillCtrl);
            }

            base.skillCtrl.trPlayerModel.rotation = Quaternion.Euler(0.0f, skillCtrl.cam.transform.eulerAngles.y, 0.0f);
            transform.rotation = Quaternion.Euler(0.0f, skillCtrl.cam.transform.eulerAngles.y, 0.0f);
        }

        public override void Exit(SkillCtrl skillCtrl)
        {
            if (bUsing)
            {
                base.skillCtrl.animator.enabled = true;
                this.StopAllCoroutines();
            }

            base.Exit(skillCtrl);
        }


        public IEnumerator BeforeDelayTimer(float time)
        {
            base.skillCtrl.animator.enabled = false;
            yield return new WaitForSeconds(time);
            Fire();
        }

        public IEnumerator ActionTimer(float time)
        {
            yield return new WaitForSeconds(time);
        }

        void Fire()
        {
            //화면의 중앙에서 카메라의 정면방향으로 레이를 쏜다.
            Ray aimRay = new Ray(base.skillCtrl.cam.transform.position, base.skillCtrl.cam.transform.forward);

            //카메라에서 쏘는 레이가 부딪힌 위치를 바라보도록 한다.
            RaycastHit rayHit;
            //레이어 마스크 ignore처리 (-1)에서 빼 주어야 함
            int mask = (
                        (1 << LayerMask.NameToLayer(Layers.T_HitCollider)) | (1 << LayerMask.NameToLayer(Layers.Bullet)) |
                        (1 << LayerMask.NameToLayer(Layers.AlleyTrigger))
                        );
            mask = ~mask;

            vFirePos = transform.position + transform.forward * 1.0f + transform.up * 1.5f;
            for (int i = 0; i < 5; i++)
            {
                //총알이 날아갈 위치를 얻는다.
                if (Physics.Raycast(aimRay, out rayHit, fReach, mask))
                {
                    float fRangeCheck = Vector3.Distance(transform.position, rayHit.point);
                    fAccuracy = 0.1f + (fRangeCheck * 0.08f);
                    vFireTargetPos = rayHit.point;
                    vFireTargetPos = new Vector3(Random.Range(vFireTargetPos.x - fAccuracy, vFireTargetPos.x + fAccuracy),
                                                 Random.Range(vFireTargetPos.y - fAccuracy, vFireTargetPos.y + fAccuracy),
                                                 Random.Range(vFireTargetPos.z - fAccuracy, vFireTargetPos.z + fAccuracy));
                }
                else
                {
                    //최대거리 명중률 조정.
                    fAccuracy = 10.0f;
                    vFireTargetPos = aimRay.GetPoint(fReach);
                    vFireTargetPos = new Vector3(Random.Range(vFireTargetPos.x - fAccuracy, vFireTargetPos.x + fAccuracy),
                                                 Random.Range(vFireTargetPos.y - fAccuracy, vFireTargetPos.y + fAccuracy),
                                                 Random.Range(vFireTargetPos.z - fAccuracy, vFireTargetPos.z + fAccuracy));
                } 
                base.skillCtrl.basicAttack.TargetFire(vFirePos, vFireTargetPos);
            }


            StartCoroutine(ShotIntervalTime(fIntervalTime));
        }

        IEnumerator ShotIntervalTime(float time)
        {
            yield return new WaitForSeconds(time);
            count++;
            if(count < iMaxCount)
            {
                Fire();
            }
            else
            {
                StartCoroutine(AfterDelayTimer(afterDelayTime));
            }
        }

        public IEnumerator AfterDelayTimer(float time)
        {
            yield return new WaitForSeconds(time);
            base.skillCtrl.animator.enabled = true;
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
