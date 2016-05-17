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
        private bool bPuseTime = false;
        private float fTargetRot;
        //사격거리
        private float fReach = 100.0f;
        private float blinkSpeed;   
        Vector3 moveDir = Vector3.zero;

        private int iFlow = 0;
        private int iFlowMax;
       

        //카메라 줌 인,아웃
        private float fTargetFOV = 90.0f;
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
        private int iFlowMax_Buff = 20;
        private float blinkTime_Buff = 0.1f;
        private float blinkDist_Buff = 3.0f;
        //궁버프 이전 수치
        private float beforeDelayTime_Orizin;
        private float afterDelayTime_Orizin;
        private float coolTime_Orizin;
        private int iFlowMax_Orizin = 20;
        private float blinkTime_Orizin;
        private float blinkDist_Orizin;

        public struct MoveFlag
        {
            public bool forward ;
            public bool backward ;
            public bool right ;
            public bool left ;
        }
        MoveFlag moveFlag;

        void Awake()
        {
            blinkSpeed = blinkDist / blinkTime;
            
            afterModelPool.CreatePool(oAfterModelPref, 20);

            beforeDelayTime_Orizin = beforeDelayTime;
            afterDelayTime_Orizin = afterDelayTime;
            coolTime_Orizin = coolTime;
            blinkTime_Orizin = blinkTime;
            blinkDist_Orizin = blinkDist;
            iFlowMax = iFlowMax_Orizin;

            moveFlag.forward = false;
            moveFlag.backward = false;
            moveFlag.right = false;
            moveFlag.left = false;
        }
        void FixedUpdate()
        {

            if (Input.GetKeyUp(KeyCode.W)) { moveFlag.forward = false; }
            if (Input.GetKeyUp(KeyCode.S)) { moveFlag.backward = false; }
            if (Input.GetKeyUp(KeyCode.D)) { moveFlag.right = false; }
            if (Input.GetKeyUp(KeyCode.A)) { moveFlag.left = false; }

            if (Input.GetKey(KeyCode.W)) moveFlag.forward = true;
            if (Input.GetKey(KeyCode.S)) moveFlag.backward = true;
            if (Input.GetKey(KeyCode.D)) moveFlag.right = true;
            if (Input.GetKey(KeyCode.A)) moveFlag.left = true;

            //float test = CalcTargetRot();
            //print("그냥 " + test.ToString());
        }
        float CalcTargetRot()
        {
            //캐릭터 기준 rotation값에 카메라의 rotation값을 더해준다.
            float CamRot = Camera.main.transform.eulerAngles.y;

            if (moveFlag.forward)
            {
                if (moveFlag.right)          //전방 우측 대각선
                    fTargetRot = 45.0f + CamRot;
                else if (moveFlag.left)    //전방 좌측 대각선
                    fTargetRot = 315.0f + CamRot;
                else                    //전방
                    fTargetRot = 0.0f + CamRot;
            }
            else if (moveFlag.backward)
            {
                if (moveFlag.right)          //후방 우측 대각선
                    fTargetRot = 135.0f + CamRot;
                else if (moveFlag.left)    //후방 좌측 대각선
                    fTargetRot = 225.0f + CamRot;
                else                    //후방
                    fTargetRot = 180.0f + CamRot;
            }
            else if (moveFlag.right)
            {
                //if (moveFlag.forward)
                //    fTargetRot = 45.0f + CamRot;
                //else if (moveFlag.backward)
                //    fTargetRot = 135.0f + CamRot;
                //else                     //우측
                    fTargetRot = 90.0f + CamRot;
            }
            else if (moveFlag.left)
            {
                //if (moveFlag.forward)
                //    fTargetRot = 315.0f + CamRot;
                //else if (moveFlag.backward)
                //    fTargetRot = 225.0f + CamRot;
                //else                     //좌측
                
                fTargetRot = 270.0f + CamRot;
            }
            print("왼쪽" + moveFlag.left);
            print("오른쪽" + moveFlag.right);
            print("전방" + moveFlag.forward);
            print("후방" + moveFlag.backward);
            moveFlag.forward = false;
            moveFlag.backward = false;
            moveFlag.right = false;
            moveFlag.left = false;

            return fTargetRot;
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
                beforeDelayTime = beforeDelayTime_Orizin;
                afterDelayTime = afterDelayTime_Orizin;
                coolTime = coolTime_Orizin;
                iFlowMax = iFlowMax_Orizin;
                blinkTime = blinkTime_Orizin;
                blinkDist = blinkDist_Orizin;
            }
            blinkSpeed = blinkDist / blinkTime;

            base.skillCtrl.mgr.DecreaseSkillPoint(PointType, iDecPoint);
            base.CoolTimeCoroutine = CoolTimer(coolTime);
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
            StartCoroutine(BeforeDelayTimer(beforeDelayTime));            
        }
        public override void Execute(T2.Skill.SkillCtrl skillCtrl)
        {
            //피격시 비정상 종료.
            if (skillCtrl.mgr.GetState() == T2.Manager.State.be_Shot)
                Exit(skillCtrl);

            if (Input.GetMouseButtonDown(1))
            {
                if (!bPuseTime)
                {
                    print(iFlow);
                    //CalcTargetRot();
                    fTargetRot = CalcTargetRot();
                    // print("스킬 중 "+fTargetRot.ToString());
                    bPuseTime = true;
                }
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
                iFlow = 0;
                base.skillCtrl.animator.speed = 1.0f;
                base.skillCtrl.mgr.SetCtrlPossible(T2.Manager.CtrlPossibleIndex.MouseRot, true);

                //카메라 줌인
                base.skillCtrl.followCam.ChangeFOV(fOrizinFOV, fZoomSpeed * 0.3f);
            }

            base.Exit(skillCtrl);
        }

        public IEnumerator BeforeDelayTimer(float time)
        {
            base.skillCtrl.mgr.SetCtrlPossible(T2.Manager.CtrlPossibleIndex.MouseRot, false);
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
            if (iFlow == 0)
                fTargetRot = base.skillCtrl.cam.transform.rotation.y + 180.0f;

            
            //캐릭터(모델)를 회전시킨다.
            base.skillCtrl.trPlayerModel.rotation = Quaternion.Euler(0.0f, fTargetRot, 0.0f);

            moveFlag.forward = false;
            moveFlag.backward = false;
            moveFlag.right = false;
            moveFlag.left = false;

            moveDir = base.skillCtrl.trPlayerModel.forward;

            //이동시 애니메이션 플레이.
            int iSprintHash = 0;
            iSprintHash = Animator.StringToHash("T2_Sprint");
            base.skillCtrl.animator.speed = 2.0f;
            base.skillCtrl.animator.Play(iSprintHash);

            //이동 코루틴.
            this.StartCoroutine(StartMove(blinkTime));
        }

        IEnumerator StartMove(float time)
        {
            float timeConunt = 0.0f;
            float fCamDist = base.skillCtrl.cam.GetComponent<FollowCam>().GetDist();
            float fCamUp = base.skillCtrl.cam.GetComponent<FollowCam>().GetUp();

            bool bAfterImageOn = false;
            while (time > timeConunt)
            {
                base.skillCtrl.controller.Move(moveDir * Time.deltaTime * blinkSpeed);

                //base.skillCtrl.trCamPivot.LookAt(vPivotTargetPos);

                if (timeConunt >= (time * 0.6) && !bAfterImageOn)
                {
                    bAfterImageOn = true;
                    this.StartCoroutine(AfterImagesDraw());
                }
                
                yield return new WaitForEndOfFrame();

                timeConunt += Time.fixedDeltaTime;
            }
            

            this.StartCoroutine(puseTime(fPuseTime));
        }

        IEnumerator puseTime(float time)
        {
            //모델과 카메라 방향을 타겟 위치로 회전시킨다.
            //base.skillCtrl.trPlayerModel.LookAt(vFireTargetPos);            
            //base.skillCtrl.trPlayerModel.rotation = Quaternion.Euler(0.0f, base.skillCtrl.trPlayerModel.eulerAngles.y, 0.0f);
            base.skillCtrl.trPlayerModel.rotation = Quaternion.Euler(0.0f, skillCtrl.cam.transform.rotation.y, 0.0f);

            //정지시 애니메이션 플레이.
            int iSprintHash = 0;
            iSprintHash = Animator.StringToHash("T2_Idle");
            base.skillCtrl.animator.speed = 1.0f;
            base.skillCtrl.animator.Play(iSprintHash);

            //if(Input.GetMouseButtonDown(1))
            //{
            //    //총알을 발사하고, 다음 이동방향 각도를 위해 iFlow를 증가시킨다.
            //    base.skillCtrl.basicAttack.TargetFire(vFireTargetPos);
            //    iFlow++;
            //}
            //else
            //{
            //    iFlow = iFlowMax;
            //}    
            
            float timeConunt = 0.0f;
            while (time > timeConunt)
            {
                if (bPuseTime)
                {
                    iFlow++;
                    bPuseTime = false;
                    StopAllCoroutines();

                    if (iFlow < iFlowMax)
                    {
                        //총알을 발사하고, 다음 이동방향 각도를 위해 iFlow를 증가시킨다.
                        base.skillCtrl.basicAttack.TargetFire(vFireTargetPos);
                        StartAction();
                    }
                    else
                    {
                        FinishFlow();
                    }
                }
                yield return new WaitForEndOfFrame();

                timeConunt += Time.fixedDeltaTime;
            }


            print("puseTime exit");
            if (!bPuseTime)
            {
                FinishFlow();
            }
        }

        void FinishFlow()
        {
            bPuseTime = false;
            iFlow = 0;
            base.skillCtrl.mgr.SetCtrlPossible(T2.Manager.CtrlPossibleIndex.MouseRot, true);
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

            yield return new WaitForSeconds(0.0225f);
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

