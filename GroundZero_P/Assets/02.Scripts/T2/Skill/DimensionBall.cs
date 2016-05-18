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
        public float fMaxReach = 10.0f;
        [HideInInspector]
        public float fReach;
        public GameObject oDimensionBall;

        //궁버프 이후 바뀔 수치
        private float beforeDelayTime_Buff = 0.0f;
        private float afterDelayTime_Buff = 0.0f;
        private float coolTime_Buff = 1.0f;
        //궁버프 이전 수치
        private float beforeDelayTime_Orizin;
        private float afterDelayTime_Orizin;
        private float coolTime_Orizin;


        void Awake()
        {
            oDimensionBall = (GameObject)Instantiate(oDimensionBall);
            oDimensionBall.SetActive(false);

            fReach = fMaxReach;
            beforeDelayTime_Orizin = beforeDelayTime;
            afterDelayTime_Orizin = afterDelayTime;
            coolTime_Orizin = coolTime;
        }

        public override void Enter(SkillCtrl skillCtrl)
        {
            base.Enter(skillCtrl);

            if(T2.Skill.SilverStream.GetInstance().bSilverStream == true)
            {
                beforeDelayTime = beforeDelayTime_Buff;
                afterDelayTime = afterDelayTime_Buff;
                coolTime = coolTime_Buff;
            }
            else
            {
                beforeDelayTime_Orizin = beforeDelayTime;
                afterDelayTime_Orizin = afterDelayTime;
                coolTime_Orizin = coolTime;
            }

            //기본 변수 초기화.
            base.skillCtrl.mgr.DecreaseSkillPoint(PointType, iDecPoint);
            base.CoolTimeCoroutine = CoolTimer(coolTime_Orizin);

            skillCtrl.mgr.ChangeState(T2.Manager.State.Skill);
            //선딜 타이머 시작.                
            StartCoroutine(BeforeDelayTimer(beforeDelayTime_Orizin));
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
            transform.rotation = Quaternion.Euler(0.0f, Camera.main.transform.eulerAngles.y, 0.0f);
            yield return new WaitForSeconds(time);
            StartCoroutine(ActionTimer(actionTime));
        }

        public IEnumerator ActionTimer(float time)
        {
            
            float fPlayerDist = Vector3.Distance(skillCtrl.trCamPivot.position + skillCtrl.trCamPivot.forward * 1.0f, skillCtrl.cam.transform.position);
            //캐릭터의 y축 1.5미터 지점, z축 1.0미터 지점에서부터 카메라 방향으로 레이를 만든다.
            Ray aimRay = new Ray(skillCtrl.cam.transform.position + skillCtrl.cam.transform.forward * fPlayerDist, skillCtrl.cam.transform.forward);
            oDimensionBall.transform.position = transform.position + (transform.up * 1.5f + transform.forward * 1.0f);
            //카메라에서 쏘는 레이가 부딪힌 위치에 플레이어의 총알이 발사되는 각도를 조정한다.
            RaycastHit aimRayHit;
            int mask = (
                       (1 << LayerMask.NameToLayer(Layers.T_HitCollider)) | (1 << LayerMask.NameToLayer(Layers.Bullet)) |
                       (1 << LayerMask.NameToLayer(Layers.T_Invincibility)) | (1 << LayerMask.NameToLayer(Layers.Except_Monster)) |
                       (1 << LayerMask.NameToLayer(Layers.MonsterAttkCollider)) | (1 << LayerMask.NameToLayer(Layers.MonsterHitCollider))
                       );
            mask = ~mask;
            if (Physics.Raycast(aimRay, out aimRayHit, fMaxReach, mask))
            {
                if (aimRayHit.collider.transform.root.tag != Tags.Monster)
                {
                    oDimensionBall.transform.LookAt(aimRayHit.point);
                    fReach = Vector3.Distance(new Vector3(aimRay.origin.x, 0.0f, aimRay.origin.z), new Vector3(aimRayHit.point.x, 0.0f, aimRayHit.point.z));
                }
            }
            else
            {
                oDimensionBall.transform.LookAt(aimRay.GetPoint(fMaxReach));
            }
            oDimensionBall.transform.rotation = Quaternion.Euler(0.0f, oDimensionBall.transform.eulerAngles.y,  oDimensionBall.transform.eulerAngles.z);
            //투사체를 활성화시킨뒤, 현재 위치에서 에임방향으로 발사한다.
            oDimensionBall.SetActive(true);
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
            print("디멘션 볼 쿨타임 시작");
            base.bCoolTime = true;
            yield return new WaitForSeconds(time);
            base.bCoolTime = false;
            print("디멘션 볼 쿨타임 끝");
        }
    }
}
