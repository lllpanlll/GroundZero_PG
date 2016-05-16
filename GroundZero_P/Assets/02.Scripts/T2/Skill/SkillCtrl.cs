using UnityEngine;
using System.Collections;

namespace T2.Skill
{
    public class SkillCtrl : MonoBehaviour
    {
        //스킬 상태 변경
        //[HideInInspector]
        public Skill curSkill;

        //스킬에서 사용할 각종 스크립트들        
        private GameObject oPlayer;
        [HideInInspector]
        public Transform trPlayerModel;
        [HideInInspector]
        public Transform trCamPivot;
        [HideInInspector]
        public T2.Manager mgr;
        [HideInInspector]
        public T2.MoveCtrl moveCtrl;
        [HideInInspector]
        public CharacterController controller;
        [HideInInspector]
        public T2.BasicAttack basicAttack;
        [HideInInspector]
        public Animator animator;
        [HideInInspector]
        public Camera cam;
        [HideInInspector]
        public FollowCam followCam;

        private T2.MoveCtrl.MoveFlag moveFlag;
        void Awake()
        {
            curSkill = T2.Skill.IdleSkill.GetInstance();
            
            oPlayer = GameObject.FindGameObjectWithTag(Tags.Player);
            trPlayerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel).transform;

            mgr = GetComponent<T2.Manager>();
            moveCtrl = GetComponent<T2.MoveCtrl>();
            controller = GetComponent<CharacterController>();
            basicAttack = GetComponent<T2.BasicAttack>();
            animator = GetComponentInChildren<Animator>();
            cam = Camera.main;
            followCam = cam.GetComponent<FollowCam>();
            trCamPivot = GameObject.FindGameObjectWithTag(Tags.CameraTarget).transform;
        }

        void Update()
        {
            moveFlag = moveCtrl.GetMoveFlag();
            //스킬 커맨드 입력 부분.
            if (mgr.GetCtrlPossible().Skill == true)
            {

                //회피기
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //방향키 입력 후 Space입력은 일반회피.
                    if (moveFlag.forward || moveFlag.backward || moveFlag.right || moveFlag.left)
                    {
                        //일반회피
                        if (mgr.PointCheck(Evasion.GetInstance().PointType, T2.Skill.Evasion.GetInstance().iDecPoint))
                            ChangeSkill(T2.Skill.Evasion.GetInstance());
                    }
                    else
                    {
                        //긴급회피
                        //방향키 입력x
                        //캔슬이 가능한 스킬은 현재 사용중인 스킬이 본인의 스킬인지 체크해야 한다.
                        if (mgr.PointCheck(Evasion_E.GetInstance().PointType, T2.Skill.Evasion_E.GetInstance().iDecPoint))
                        {
                            if(curSkill != T2.Skill.Evasion_E.GetInstance())
                                ChangeSkill(T2.Skill.Evasion_E.GetInstance());
                        }
                    }
                }



                //SeventhFlow
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if(mgr.PointCheck(SeventhFlow.GetInstance().PointType, T2.Skill.SeventhFlow.GetInstance().iDecPoint))
                        ChangeSkill(T2.Skill.SeventhFlow.GetInstance());
                }

                //DimensionBall
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    if (DimensionBall.GetInstance().oDimensionBall.activeSelf == false)
                    {
                        if (mgr.PointCheck(DimensionBall.GetInstance().PointType, T2.Skill.DimensionBall.GetInstance().iDecPoint))
                            ChangeSkill(T2.Skill.DimensionBall.GetInstance());
                    }
                }

                //SilverStream
                if (Input.GetKeyDown(KeyCode.R))
                {
                    if (SilverStream.GetInstance().bSilverStream == false)
                    {
                        if (mgr.PointCheck(SilverStream.GetInstance().PointType, T2.Skill.SilverStream.GetInstance().iDecPoint))
                            ChangeSkill(T2.Skill.SilverStream.GetInstance());
                    }
                }
            }


            //스킬 실행 부분.
            //Execute에는 기본적으로 피격으로 인한 스킬 정지 코드가 있다.
            if(curSkill != T2.Skill.IdleSkill.GetInstance())
                curSkill.Execute(this);
        }

        void ChangeSkill(T2.Skill.Skill newSkill)
        {
            //스킬이 현재 사용중이라면, 새로 쓰려고 하는 스킬이 캔슬이 가능한 스킬이고 쿨타임이 false일 때만 스킬 체인지.
            if(curSkill.bUsing == true)
            {
                if (newSkill.bSkillCancel == true && newSkill.bCoolTime == false)
                {
                    print("캔슬 체인지");
                    ChangeSkillState(newSkill);
                }
            }
            else
            {
                //스킬 사용중이 아니라면, 새로운 스킬의 쿨타임이 false인 경우에만 스킬 체인지.
                if (newSkill.bCoolTime == false)
                {                   
                    //현재 스킬 상태가 IdleSkill이면 바로 스킬 체인지.
                    //(정상 종료됬었다면 무조건 IdleSkill상태일 테니)
                    if (curSkill == T2.Skill.IdleSkill.GetInstance())
                    {
                        print("노멀 체인지");
                        ChangeSkillState(newSkill);
                    }
                }
            }
        }
        void ChangeSkillState(Skill newSkill)
        {
            curSkill.Exit(this);
            curSkill = newSkill;
            curSkill.Enter(this);
        }
    }
}
