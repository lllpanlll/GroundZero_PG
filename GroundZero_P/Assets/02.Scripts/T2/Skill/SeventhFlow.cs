using UnityEngine;
using System.Collections;
using System;

namespace T2.Skill
{
    class SeventhFlow : T2.Skill.Skill, T2.Skill.ISkillTimer
    {
        #region<싱글톤>
        private static T2.Skill.SeventhFlow instance;
        public static T2.Skill.SeventhFlow GetInstance()
        {
            if (!instance)
            {
                instance = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.Skill.SeventhFlow>();
                if (!instance)
                    print("Evasion 인스턴스 생성에 실패하였습니다.");
            }
            return instance;
        }
        #endregion
        //스킬의 필수 기본 변수들, 나중에 public으로 변환.
        public Manager.SkillType PointType = Manager.SkillType.PP;
        public int iDecPoint = 10;
        public float beforeDelayTime = 0.0f;
        public float afterDelayTime = 0.0f;
        public float coolTime = 0.0f;
        //blinkTime이 이 스킬의 actionTime.
        public float blinkTime = 0.15f;
        //============================================
        //각 스킬의 고유 변수들        
        public float blinkDist = 12.0f;
        public float fPuseTime = 0.07f;
        private bool bNextFlow = false;
        private bool bMove = false;
        private float fTargetRot;
        
        //사격거리
        private float fReach = 100.0f;
        private float blinkSpeed;   
        Vector3 moveDir = Vector3.zero;

        private int iFlow = 0;
        private int iFlowMax;
        private float[] fFlowRot;
       

        //카메라 줌 인,아웃
        private float fTargetFOV = 100.0f;
        private float fOrizinFOV;
        private float fZoomSpeed = 25.0f;

        public GameObject oAfterModelPref;
        private GameObject oAfterModel;
        private ObjectPool afterModelPool = new ObjectPool();
        //한번 이동 당 잔상 출력 갯수
        public int afterImageMax = 5;
        private int afterImageCount = 0;

        private Vector3 vFireTargetPos = Vector3.zero;
        private Vector3 vPivotTargetPos = Vector3.zero;

        //궁버프 이후 바뀔 수치
        private float beforeDelayTime_Buff = 0.0f;
        private float afterDelayTime_Buff = 0.0f;
        private float coolTime_Buff = 0.0f;
        private int iFlowMax_Buff = 4;
        private float blinkTime_Buff = 0.1f;
        private float blinkDist_Buff = 3.0f;
        //궁버프 이전 수치
        private float beforeDelayTime_Orizin;
        private float afterDelayTime_Orizin;
        private float coolTime_Orizin;
        private int iFlowMax_Orizin = 3;
        private float blinkTime_Orizin;
        private float blinkDist_Orizin;

        void Awake()
        {
            blinkSpeed = blinkDist / blinkTime;
            //blinkSpeed = 10.0f;

            afterModelPool.CreatePool(oAfterModelPref, 20);

            beforeDelayTime_Orizin = beforeDelayTime;
            afterDelayTime_Orizin = afterDelayTime;
            coolTime_Orizin = coolTime;
            blinkTime_Orizin = blinkTime;
            blinkDist_Orizin = blinkDist;
            iFlowMax = iFlowMax_Orizin;

            fFlowRot = new float[4];
            fFlowRot[0] = 160.0f;
            fFlowRot[1] = 200.0f;
            fFlowRot[2] = 160.0f;
            fFlowRot[3] = 200.0f;

            bNextFlow = false;
            bSkillCancel = true;
        }

        public override void Enter(T2.Skill.SkillCtrl skillCtrl)
        {
            //기본 변수 초기화.
            base.Enter(skillCtrl);

            if (T2.Skill.SilverStream.GetInstance().bSilverStream == true)
            {
                beforeDelayTime = beforeDelayTime_Buff;
                afterDelayTime = afterDelayTime_Buff;
                coolTime = coolTime_Buff;
                iFlowMax = iFlowMax_Buff;
                blinkTime = blinkTime_Buff;
                blinkDist = blinkDist_Buff;
            }
            else
            {
                beforeDelayTime_Orizin = beforeDelayTime;
                afterDelayTime_Orizin = afterDelayTime;
                coolTime_Orizin = coolTime;
                iFlowMax_Orizin = iFlowMax;
                blinkTime_Orizin = blinkTime;
                blinkDist_Orizin = blinkDist;
            }
            blinkSpeed = blinkDist_Orizin / blinkTime_Orizin;

            base.skillCtrl.mgr.DecreaseSkillPoint(PointType, iDecPoint);
            base.CoolTimeCoroutine = CoolTimer(coolTime_Orizin);
            skillCtrl.mgr.ChangeState(T2.Manager.State.Skill);

            fOrizinFOV = base.skillCtrl.cam.fieldOfView;                       

            //스킬이 끝난 후, 이동속도를 '처음'부터 가속하기 위해 moveState를 Stop으로 해 놓는다.
            base.skillCtrl.moveCtrl.SetMoveState(T2.MoveCtrl.MoveState.Stop);

            //화면의 중앙에서 카메라의 정면방향으로 레이를 쏜다.
            Ray aimRay = new Ray(base.skillCtrl.cam.transform.position, base.skillCtrl.cam.transform.forward);
            //카메라의 기준이 될 피벗의 방향을 정하는 레이를 만든다.
            Ray pivotRay = new Ray(base.skillCtrl.trCamPivot.position, base.skillCtrl.trCamPivot.forward);

            //카메라에서 쏘는 레이가 부딪힌 위치를 바라보도록 한다.
            RaycastHit rayHit;
            //레이어 마스크 ignore처리 (-1)에서 빼 주어야 함
            int mask = (1 << LayerMask.NameToLayer(Layers.T_HitCollider)) | (1 << LayerMask.NameToLayer(Layers.Bullet));
            mask = ~mask;

            //총알이 날아갈 위치를 얻는다.
            if (Physics.Raycast(aimRay, out rayHit, fReach, mask))
            {
                vFireTargetPos = rayHit.point;
                //피벗레이는 화면상보다 좀더 앞에 있기 때문에 FollowCam스크립트의 Dist만큼 빼준다.
                vPivotTargetPos = pivotRay.GetPoint(rayHit.distance - base.skillCtrl.cam.GetComponent<FollowCam>().GetDist());
            }
            else
            {
                vFireTargetPos = aimRay.GetPoint(fReach);
                vPivotTargetPos = pivotRay.GetPoint(fReach);
            }

            //선딜 타이머 시작.                
            StartCoroutine(BeforeDelayTimer(beforeDelayTime_Orizin));            
        }
        public override void Execute(T2.Skill.SkillCtrl skillCtrl)
        {
            //피격시 비정상 종료.
            if (skillCtrl.mgr.GetState() == T2.Manager.State.be_Shot)
                Exit(skillCtrl);

            if (Input.GetKeyDown(KeyCode.Alpha2) && bMove)
            {
                if (!bNextFlow)
                {
                    bNextFlow = true;
                }
                print(bNextFlow);
            }
        }
        public override void Exit(T2.Skill.SkillCtrl skillCtrl)
        {
            //bUsing이 아직 true라면, 스킬이 도중에 캔슬 되는 경우이다.
            //스킬 캔슬로 인해 제대로 정리되지 않은 변수들을 초기화 시켜준다.
            if (bUsing == true)
            {
                this.StopAllCoroutines();
                //잔상을 모두 지운다.
                afterModelPool.AllDeActiveObject();
                afterImageCount = 0;
                base.skillCtrl.animator.speed = 1.0f;
                base.skillCtrl.mgr.SetCtrlPossible(T2.Manager.CtrlPossibleIndex.MouseRot, true);

                //카메라 줌인
                base.skillCtrl.followCam.ChangeFOV(fOrizinFOV, fZoomSpeed * 0.3f);
            }

            iFlow = 0;
            bNextFlow = false;
            base.Exit(skillCtrl);
        }

        public IEnumerator BeforeDelayTimer(float time)
        {
            //base.skillCtrl.mgr.SetCtrlPossible(T2.Manager.CtrlPossibleIndex.MouseRot, false);
            //카메라 줌아웃
            base.skillCtrl.followCam.ChangeFOV(fTargetFOV, fZoomSpeed);

            
            yield return new WaitForSeconds(time);
            StartAction();
        }

        public IEnumerator ActionTimer(float time)
        {
            yield return new WaitForSeconds(time);            
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
            print("SeventhFlow 쿨타임 시작");
            base.bCoolTime = true;
            yield return new WaitForSeconds(time);
            base.bCoolTime = false;
            print("SeventhFlow 쿨타임 종료");
        }


        void StartAction()
        {           
            //플레이어와 캐릭터(모델)를 회전시킬 값을 구한다.
            fTargetRot = fFlowRot[iFlow] + base.skillCtrl.cam.transform.eulerAngles.y;
            //캐릭터(모델)를 회전시킨다.
            base.skillCtrl.trPlayerModel.rotation = Quaternion.Euler(0.0f, fTargetRot, 0.0f);

            moveDir = base.skillCtrl.trPlayerModel.forward;

            //이동시 애니메이션 플레이.
            int iSprintHash = 0;
            iSprintHash = Animator.StringToHash("T2_Sprint");
            base.skillCtrl.animator.speed = 2.0f;
            base.skillCtrl.animator.Play(iSprintHash);

            bMove = true;
            //이동 코루틴.
            this.StartCoroutine(StartMove(blinkTime_Orizin));
        }

        IEnumerator StartMove(float time)
        {
            float timeConunt = 0.0f;
            float fCamDist = base.skillCtrl.cam.GetComponent<FollowCam>().GetDist();
            float fCamUp = base.skillCtrl.cam.GetComponent<FollowCam>().GetUp();

            bool bAfterImageOn = false;
            while (time > timeConunt)
            {
                //blinkSpeed = Mathf.Lerp(blinkSpeed, 30.0f, Time.deltaTime * 30.0f);
                base.skillCtrl.controller.Move(moveDir * Time.deltaTime * blinkSpeed);

                //base.skillCtrl.trCamPivot.LookAt(vPivotTargetPos);

                if (timeConunt >= (time * 0.0) && !bAfterImageOn)
                {
                    bAfterImageOn = true;
                    this.StartCoroutine(AfterImagesDraw());
                }                
                yield return new WaitForEndOfFrame();

                timeConunt += Time.fixedDeltaTime;
            }
            //blinkSpeed = 10.0f;

            
            this.StartCoroutine(puseTime(fPuseTime));
        }

        IEnumerator puseTime(float time)
        {
            //모델과 카메라 방향을 타겟 위치로 회전시킨다.
            base.skillCtrl.trPlayerModel.LookAt(vFireTargetPos);            
            base.skillCtrl.trPlayerModel.rotation = Quaternion.Euler(0.0f, base.skillCtrl.trPlayerModel.eulerAngles.y, 0.0f);
            //base.skillCtrl.trPlayerModel.rotation = Quaternion.Euler(0.0f, skillCtrl.trCamPivot.transform.rotation.y, 0.0f);

            //정지시 애니메이션 플레이.
            int iSprintHash = 0;
            iSprintHash = Animator.StringToHash("T2_Idle");
            base.skillCtrl.animator.speed = 1.0f;
            base.skillCtrl.animator.Play(iSprintHash);


            float timeConunt = 0.0f;
            while (time > timeConunt)
            {
                if (timeConunt >= (time * 0.3))
                {
                    if (bNextFlow)
                    {
                        iFlow++;
                        bNextFlow = false;
                        if (iFlow < iFlowMax_Orizin)
                        {
                            base.skillCtrl.basicAttack.TargetFire(vFireTargetPos);
                            StartAction();
                        }
                        else
                        {
                            FinishFlow();
                        }
                        break;
                    }
                }
                yield return new WaitForEndOfFrame();              
                timeConunt += Time.fixedDeltaTime;
            }
            
            if (time < timeConunt)
            {
                if (!bNextFlow)
                {
                    print("puseTime exit");
                    FinishFlow();
                }
            }
        }

        void FinishFlow()
        {
            bNextFlow = false;
            bMove = false;
            iFlow = 0;
            //base.skillCtrl.mgr.SetCtrlPossible(T2.Manager.CtrlPossibleIndex.MouseRot, true);
            //후딜레이 시작.
            StartCoroutine(AfterDelayTimer(afterDelayTime));
            //카메라 줌인
            base.skillCtrl.followCam.ChangeFOV(fOrizinFOV, fZoomSpeed * 0.3f);
        }
        
        IEnumerator AfterImagesDraw()
        {
            //현재 애니메이션 스테이트와 타임값을 저장한다.
            float freezeTime = base.skillCtrl.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            AnimatorStateInfo stateInfo = base.skillCtrl.animator.GetCurrentAnimatorStateInfo(0);

            //잔상용 모델 하나를 키고, 위치와 회전값을 초기화 한다.
            oAfterModel = afterModelPool.UseObject();
            oAfterModel.transform.position = base.skillCtrl.trPlayerModel.position;
            oAfterModel.transform.rotation = base.skillCtrl.trPlayerModel.rotation;
            //잔상용 모델의 애니메이션을 현재 플레이어의 애니메이션의 스테이트와 타임으로 플레이시킨다.
            oAfterModel.GetComponent<Animator>().Play(stateInfo.fullPathHash, 0, freezeTime);
            //잔상용 모델의 애니메이션을 1프레임뒤에 정지 시킨다.
            this.StartCoroutine(AfterImageStopDelay(oAfterModel));

            yield return new WaitForSeconds(0.02f);
            //잔상용 모델을 afterImageMax 갯수만큼 만들지 못했으면 한번더 코루틴을 반복시킨다.
            if (afterImageCount < afterImageMax)
            {
                afterImageCount++;
                this.StartCoroutine(AfterImagesDraw());
            }
            else
                afterImageCount = 0;
        }
        IEnumerator AfterImageStopDelay(GameObject obj)
        {
            yield return new WaitForEndOfFrame();
            obj.GetComponent<Animator>().Stop();
        }
    }
}

