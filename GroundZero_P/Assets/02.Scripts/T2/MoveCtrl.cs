using UnityEngine;
using System.Collections;

namespace T2
{
    public class MoveCtrl : MonoBehaviour
    {
        public enum MoveState { Run, Sprint, Stop }
        private MoveState moveState;

        private float h = 0.0f;
        private float v = 0.0f;

        private T2.Manager mgr;
        private Transform trPlayerModel;
        private CharacterController controller;
        private Animator animator;

        //실제 사용 변수
        private float fMoveSpeed;
        private float fMaxMoveSpeed;
        private float fMoveSpeedDamp = 1.0f;

        public struct MoveFlag
        {
            public bool forward;
            public bool backward;
            public bool right;
            public bool left;
        }
        MoveFlag moveFlag;

        public GameObject oSprintEffect;

        #region<방향전환>
        //회피시 타겟 각도로도 사용됨.
        float fTargetRot = 0.0f;
        private float fSprintRot = 0.0f;  //캐릭터의 y축 각도 값.(전력질주)
        private float fRunRot = 0.0f;   //캐릭터의 y축 각도 값.(기본달리기)
                                        //캐릭터 회전 속도 변수
        public float fCharRotSpeed = 15.0f;
        //private bool bOppositRotation = false;  //반대방향 회전 여부.
        #endregion

        private float fOrizinFOV;
        public float fSprintFOV = 80.0f;
        public float fFOV_ZoomSpeed = 3.0f;
        private Camera mainCamera;

        //그로기 상태 변수
        bool bGroggy;
        float groggyTimer = 0.0f, groggyTime = 2.0f;

        void Awake()
        {
            fMoveSpeed = T2.Stat.MAX_RUN_MOVE;
            fMaxMoveSpeed = T2.Stat.MAX_RUN_MOVE;

            mgr = GetComponent<T2.Manager>();
            trPlayerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel).GetComponent<Transform>();
            controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            mainCamera = Camera.main;
            fOrizinFOV = mainCamera.fieldOfView;

            moveState = MoveState.Stop;

            oSprintEffect.SetActive(true);
        }

        void OnEnable()
        {
            fMoveSpeed = T2.Stat.MAX_RUN_MOVE;
            fMaxMoveSpeed = T2.Stat.MAX_RUN_MOVE;
        }

        void OnGUI()
        {
            GUI.Box(new Rect(0, 0, 220, 200), "STAT");
            GUI.Label(new Rect(20, 70, 200, 25), "MoveState : " + moveState.ToString());
            GUI.Label(new Rect(20, 170, 200, 25), "MoveSpeed : " + fMoveSpeed.ToString());
        }

        void Update()
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

            //전력질주의 캐릭터 방향을 정하는 'targetRot'변수를 설정해주는 함수,
            CalcTargetRot();

            #region<switch(moveState)>
            switch (moveState)
            {
                case MoveState.Stop:
                    moveDir = Vector3.zero;
                    //h = v = 0.0f;
                    fMoveSpeed = 5.0f;
                    oSprintEffect.SetActive(false);
                    //oSprintEffect.GetComponentInChildren<ParticleSystem>().playOnAwake = false;
                    break;
                case MoveState.Run:
                    //fMoveSpeed가 Walk상태에서의 MaxSpeed보다 높을 떄, 즉 Run상태의 이동속도 구간이였다면.
                    //SpeedDamp 수치를 높여 이동속도를 빠르게 낮춘다(run->walk).
                    //반대 상황(walk->run)에서는 속도를 천천히 높이기 위해 SpeedDamp 수치를 낮춘다.
                    if (fMoveSpeed > T2.Stat.MAX_RUN_MOVE)
                        fMoveSpeedDamp = 10.0f;
                    else
                        fMoveSpeedDamp = 1.0f;
                    fMaxMoveSpeed = T2.Stat.MAX_RUN_MOVE;

                    oSprintEffect.SetActive(false);
                    break;
                case MoveState.Sprint:
                    fMoveSpeedDamp = 0.8f;
                    fMaxMoveSpeed = T2.Stat.MAX_SPRINT_MOVE;

                    if (fMoveSpeed > 12.0f)
                        oSprintEffect.SetActive(true);
                    break;
            }
            #endregion

            #region<Groggy>
            if (mgr.GetEP() < 0.0f)
                bGroggy = true;

            if (bGroggy)
            {
                if (groggyTimer < groggyTime)
                {
                    if (mgr.GetCtrlPossible().Sprint == true)
                        mgr.SetCtrlPossible(T2.Manager.CtrlPossibleIndex.Sprint, false);
                    groggyTimer += Time.deltaTime;
                }
                else
                {
                    mgr.SetCtrlPossible(T2.Manager.CtrlPossibleIndex.Sprint, true);
                    bGroggy = false;
                    groggyTimer = 0.0f;
                }
            }
            #endregion

            #region<Input WASD>
            if (Input.GetKeyUp(KeyCode.W)) { moveFlag.forward = false; h = 0.0f; }
            if (Input.GetKeyUp(KeyCode.S)) { moveFlag.backward = false; h = 0.0f; }
            if (Input.GetKeyUp(KeyCode.D)) { moveFlag.right = false; v = 0.0f; }
            if (Input.GetKeyUp(KeyCode.A)) { moveFlag.left = false; v = 0.0f; }

            if (mgr.GetCtrlPossible().Run == true)
            {
                if (Input.GetKey(KeyCode.W)) moveFlag.forward = true;
                if (Input.GetKey(KeyCode.S)) moveFlag.backward = true;
                if (Input.GetKey(KeyCode.D)) moveFlag.right = true;
                if (Input.GetKey(KeyCode.A)) moveFlag.left = true;
            }
            //else
            //{
            //    moveFlag.forward = false;
            //    moveFlag.backward = false;
            //    moveFlag.right = false;
            //    moveFlag.left = false;
            //    //h = v = 0.0f;
            //}
            #endregion

            #region<MoveState Change>
            //걷기상태가 가능상태여야 전력질주도 가능하도록 한다.
            if (mgr.GetCtrlPossible().Run == true)
            {
                if (moveFlag.forward || moveFlag.backward || moveFlag.right || moveFlag.left)
                {
                    if (Input.GetKey(KeyCode.LeftShift) && mgr.GetCtrlPossible().Sprint == true)
                    {
                        moveState = MoveState.Sprint;
                    }
                    else
                        moveState = MoveState.Run;
                }
                else
                {
                    moveState = MoveState.Stop;
                }
            }


            #endregion

            //카메라 줌 인,아웃
            if (moveState == MoveState.Sprint)
            {
                if (fMoveSpeed > 12.0f)
                    mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, fSprintFOV, Time.deltaTime * fFOV_ZoomSpeed);
            }
            else
            {
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, fOrizinFOV, Time.deltaTime * fFOV_ZoomSpeed);
            }

            if (mgr.GetCtrlPossible().Run == true)
            {
                //이동 처리
                if (moveFlag.forward || moveFlag.backward || moveFlag.right || moveFlag.left)
                {
                    //플레이어를 카메라 정면방향으로 각도를 유지시킨다.
                    float CamRot = Camera.main.transform.eulerAngles.y;
                    transform.rotation = Quaternion.Euler(0.0f, CamRot, 0.0f);

                    //if (moveState == MoveState.Run)
                    //{
                    //    //전력질주일 때, 바뀌어버린 캐릭터(모델)의 각도를 정면으로 '보간'하며 바꾸어준다.
                    //    fRunRot = Mathf.LerpAngle(fRunRot, CamRot, Time.deltaTime * fCharRotSpeed + (fCharRotSpeed * 0.01f));
                    //    trPlayerModel.rotation = Quaternion.Euler(0.0f, fRunRot, 0.0f);

                    //    //transform.Rotate(Vector3.up * Time.deltaTime * fRotSpeed * Input.GetAxis("Mouse X"));
                    //}
                    //else if (moveState == MoveState.Sprint)
                    //{
                    #region<180도 회전>
                    //float fCurRot = transform.eulerAngles.y;
                    //fCurRot = Rotation360Clamp(fCurRot);
                    //fTargetRot = Rotation360Clamp(fTargetRot);

                    //float fRotGap = fCurRot - fTargetRot;
                    //if (fRotGap < 0.0f) fRotGap += 360.0f;

                    //if (fRotGap > 170.0f && fRotGap < 190.0f)
                    //{
                    //    bOppositRotation = true;
                    //}

                    ////좌우, 상하 180도 회전을 하는 경우.
                    //if (bOppositRotation)
                    //{
                    //    if (fRotGap < 10.0f || fRotGap > 350.0f)
                    //    {
                    //        fMoveSpeed = 5.0f;
                    //        bOppositRotation = false;
                    //    }
                    //    else
                    //    {
                    //        fMoveSpeed = 0.0f;
                    //    }

                    //    //targetRot각도로 fSprintRot을 설정.
                    //    //Lerp할 때마다 일정 수치를 상수값으로 항상 증감되도록 하여 끝부분에 아주 느려지는 부분을 보완한다.
                    //    fSprintRot = Mathf.LerpAngle(fSprintRot, fTargetRot, Time.deltaTime * fCharRotSpeed + (fCharRotSpeed * 0.001f));
                    //}
                    //else
                    //    fSprintRot = Mathf.LerpAngle(fSprintRot, fTargetRot, Time.deltaTime * fCharRotSpeed);
                    #endregion

                    //캐릭터를 기준으로 8방향 각도가 구해진 fTargetRot으로 보간하여 fSprintRot을 구한다.
                    fSprintRot = Mathf.LerpAngle(fSprintRot, fTargetRot, Time.deltaTime * fCharRotSpeed + (fCharRotSpeed * 0.01f));
                    trPlayerModel.rotation = Quaternion.Euler(0.0f, fSprintRot, 0.0f);
                    //}

                    float fAngle = Vector3.Angle(transform.forward, trPlayerModel.forward);
                    if (fAngle > 180.0f)
                        fAngle -= 180.0f;
                    animator.SetFloat("fAngle", fAngle);

                    fMoveSpeed = Mathf.Lerp(fMoveSpeed, fMaxMoveSpeed, Time.deltaTime * fMoveSpeedDamp);

                    moveDir = transform.TransformDirection(moveDir);
                    controller.Move(moveDir * fMoveSpeed * Time.deltaTime);
                }
                //정지 처리
                else
                {
                    animator.SetFloat("fAngle", 0.0f);
                    //GetAxis의 수치가 떨어지는 시간이 존재하기 때문에 캐릭터를 곧바로 정지시키기 위해 Vector3.zero로 초기화한다.                
                    moveState = MoveState.Stop;
                }
            }

        }


        void CalcTargetRot()
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
                if (moveFlag.forward)
                    fTargetRot = 45.0f + CamRot;
                else if (moveFlag.backward)
                    fTargetRot = 135.0f + CamRot;
                else                     //우측
                    fTargetRot = 90.0f + CamRot;
            }
            else if (moveFlag.left)
            {
                if (moveFlag.forward)
                    fTargetRot = 315.0f + CamRot;
                else if (moveFlag.backward)
                    fTargetRot = 225.0f + CamRot;
                else                     //좌측
                    fTargetRot = 270.0f + CamRot;
            }
        }
        public float GetTargetRot() { return fTargetRot; }
        public MoveState GetMoveState() { return moveState; }
        public void SetMoveState(MoveState val) { moveState = val; }
        public MoveFlag GetMoveFlag() { return moveFlag; }
        public float GetMoveSpeed() { return fMoveSpeed; }
    }
}