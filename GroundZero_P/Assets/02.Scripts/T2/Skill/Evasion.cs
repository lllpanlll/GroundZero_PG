using UnityEngine;
using System.Collections;
using System;

namespace T2.Skill
{
    /// <summary>
    /// 이 스킬은 플레이어(에임방향)의 후방으로
    /// 일정시간동안 일정 거리를 이동하는 스킬이다.
    ///
    /// 무적 여부 : x
    /// 스킬 캔슬 : o
    /// </summary>

    public class Evasion : T2.Skill.Skill, T2.Skill.ISkillTimer
    {
        #region<싱글톤>
        private static T2.Skill.Evasion instance;
        public static T2.Skill.Evasion GetInstance()
        {
            if (!instance)
            {
                instance = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.Skill.Evasion>();
                if (!instance)
                    print("Evasion 인스턴스 생성에 실패하였습니다.");
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
        //blinkTime이 이 스킬의 actionTime.
        public float blinkTime = 0.8f;

        //각 스킬의 고유 변수들
        public float blinkDist = 8.0f;
        //public GameObject oBlinkEffect;        
        private float blinkSpeed;
        Vector3 moveDir = Vector3.zero;

        void Awake()
        {
            blinkSpeed = blinkDist / blinkTime;
            instance = GetInstance();
        }

        public override void Enter(T2.Skill.SkillCtrl skillCtrl)
        {
            print("일반회피");
            //기본 변수 초기화.
            base.Enter(skillCtrl);
            base.skillCtrl.mgr.DecreaseSkillPoint(PointType, iDecPoint);
            base.CoolTimeCoroutine = CoolTimer(coolTime);

            skillCtrl.mgr.ChangeState(T2.Manager.State.Skill);
            //선딜 타이머 시작.                
            StartCoroutine(BeforeDelayTimer(beforeDelayTime));    
        }
        public override void Execute(T2.Skill.SkillCtrl skillCtrl)
        {
            //피격시 비정상 종료.
            if (skillCtrl.mgr.GetState() == T2.Manager.State.be_Shot)
            {
                Exit(skillCtrl);
            }

            base.Execute(skillCtrl);          
        }
        public override void Exit(T2.Skill.SkillCtrl skillCtrl)
        {
            //사용 중 캔슬 되어 버릴 수 있으니 바뀐 값을 되돌려야 한다.
            //ec)만약 무적판정이 있다면, 여기서 다시 한번 꺼주어야 한다.
            //oBlinkEffect.SetActive(false);

            base.Exit(skillCtrl);
        }


        public IEnumerator BeforeDelayTimer(float time)
        {
            yield return new WaitForSeconds(time);            
            StartCoroutine(ActionTimer(blinkTime));
        }
        public IEnumerator ActionTimer(float time)
        {            
            //oBlinkEffect.SetActive(true);

            //캐릭터(모델)를 회전시킬 값을 구해서 모델링의 forward방향으로 moveDir을 설정한다.
            float targetRot = base.skillCtrl.moveCtrl.GetTargetRot();
            base.skillCtrl.trPlayerModel.rotation = Quaternion.Euler(0.0f, targetRot, 0.0f);
            moveDir = base.skillCtrl.trPlayerModel.forward;
            base.skillCtrl.moveCtrl.SetMoveState(T2.MoveCtrl.MoveState.Stop);

            base.skillCtrl.animator.SetTrigger("tEvasion");

            //blinkTime동안 매 프레임마다 반복.
            float timeConut = 0;            
            while (time > timeConut)
            {
                base.skillCtrl.controller.Move(moveDir.normalized * Time.deltaTime * blinkSpeed);
                yield return new WaitForEndOfFrame();
                timeConut += Time.deltaTime;

               
            }

            //oBlinkEffect.SetActive(false);
            

         

            //base.skillCtrl.trPlayerModel.rotation = Quaternion.Euler(0.0f, base.skillCtrl.trCamPivot.eulerAngles.y, 0.0f);
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
