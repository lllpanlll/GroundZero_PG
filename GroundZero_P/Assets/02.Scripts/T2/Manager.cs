using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace T2
{
    public class Manager : MonoBehaviour
    {
        bool bAimForMonster; // tj

        private int hp;
        private int dp;
        private int pp;
        private int ap;
        private float ep;
        
        public Scrollbar fillGaugeBar;
        
        public enum LayerState { invincibility, normal }
        private LayerState curLayerState;

        public enum State { idle, attack, Skill, be_Shot }
        private State skillCtrl;

        public enum SkillType { AP, EP, PP }

        //현재 T_SkillMgr에서 사용중이다.
        private static T2.Manager instance;
        public static T2.Manager GetInstance()
        {
            if (!instance)
            {
                instance = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.Manager>();
                if (!instance)
                    print("T2.Manager스크립트가 있는 게임오브젝트가 없습니다.");
            }
            return instance;
        }

        public struct CtrlPossible
        {
            public bool Run;
            public bool Sprint;
            public bool Attack;
            public bool MouseRot;
            public bool Skill;
        }
        public enum CtrlPossibleIndex { Run, Sprint, Attack, MouseRot, Skill };
        CtrlPossible ctrlPossible;

        private T2.MoveCtrl moveCtrl;
        private CharacterController controller;

        #region <달리기 상태에 따른 ep증감>
        private float fDecEP = 0.4f, fIncEP = 0.6f;
        private float fIncAccelPoint = 0.2f;
        private float decreaseTime = 0.01f, decreaseTimer = 0.0f;
        private float increaseTime = 0.005f, increaseTimer = 0.0f;
        private float incAccelTime = 1.0f, incAccelTimer = 0.0f;
        #endregion

        private LineRenderer line;
        public Transform trFire;
        void Start()
        {
            hp = T2.Stat.MAX_HP;
            dp = T2.Stat.INIT_DP;
            pp = T2.Stat.MAX_PP;
            ap = T2.Stat.MAX_AP;
            ep = T2.Stat.MAX_EP;

            curLayerState = LayerState.normal;

            moveCtrl = GetComponent<T2.MoveCtrl>();
            controller = GetComponent<CharacterController>();
            line = GetComponent<LineRenderer>();

            //마우스 커서 숨기기
            //Cursor.visible = false;

            ctrlPossible.Run = true;
            ctrlPossible.Sprint = true;
            ctrlPossible.Attack = true;
            ctrlPossible.MouseRot = true;
            ctrlPossible.Skill = true;

            ChangeState(State.idle);
        }
        void OnGUI()
        {
            GUI.Label(new Rect(20, 20, 200, 25), "State : " + skillCtrl);
            GUI.Label(new Rect(20, 90, 200, 25), "ep : " + ep + " / " + T2.Stat.MAX_EP);
            GUI.Label(new Rect(20, 110, 200, 25), "hp : " + hp + " / " + T2.Stat.MAX_HP);
            GUI.Label(new Rect(20, 130, 200, 25), "dp : " + dp + " / " + T2.Stat.MAX_DP);
            GUI.Label(new Rect(20, 150, 200, 25), "pp : " + pp + " / " + T2.Stat.MAX_PP);
        }
        void Update()
        {
            //임시 지구력 UI.
            fillGaugeBar.size = ep * 0.01f;

            #region<Sprint로 인한 EP증감>
            if (skillCtrl == State.idle || skillCtrl == State.attack)
            {
                if (ep < 0.0f)
                    ctrlPossible.Sprint = false;

                if (moveCtrl.GetMoveState() == T2.MoveCtrl.MoveState.Run ||
                    moveCtrl.GetMoveState() == T2.MoveCtrl.MoveState.Stop)
                {
                    EpIncrease();
                }
                else if (moveCtrl.GetMoveState() == T2.MoveCtrl.MoveState.Sprint)
                {
                    EpDecrease();
                }
            }
            #endregion

            controller.Move(-transform.up * Time.deltaTime * 20.0f);

            Transform camTr = Camera.main.transform;
            //화면의 중앙 벡터
            Vector3 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));

            //화면의 중앙에서 카메라의 정면방향으로 레이를 쏜다.
            Ray aimRay = new Ray(centerPos, camTr.forward);
            //카메라의 기준이 되는 피벗의 방향을 정하는 레이를 만든다.
            Ray pivotRay = new Ray(GameObject.FindGameObjectWithTag(Tags.CameraTarget).transform.position,
                GameObject.FindGameObjectWithTag(Tags.CameraTarget).transform.forward);

            /*=======================================================
             * 보스에 룩 온 하면 에임이 변하도록 만들겠다 
             * 최적화는 신경 쓰지 않을테니 잘 수정하시오 -tj-
             ========================================================*/
            RaycastHit aimRayhit;
             if(Physics.Raycast(aimRay,out aimRayhit))
            {
                if (aimRayhit.transform.CompareTag(Tags.Monster))
                {
                    bAimForMonster = true;
                }
                else
                    bAimForMonster = false;
            }



            Debug.DrawLine(aimRay.origin, aimRay.GetPoint(100.0f), Color.red);
            Debug.DrawLine(pivotRay.origin, pivotRay.GetPoint(100.0f), Color.blue);

            Ray fireRay = new Ray(trFire.position, trFire.forward);
            Debug.DrawLine(fireRay.origin, fireRay.GetPoint(100.0f), Color.yellow);
        }

        void OnTriggerEnter(Collider coll)
        {
            //몬스터 공격이면 데미지만큼 hp 차감
            if (coll.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterAttkCollider))
            {
                print("hit");
                int iDamage = coll.gameObject.GetComponent<M_AttackCtrl>().GetDamage();

                if (iDamage != 0 && dp > 0)
                {
                    if (dp > iDamage)
                    {
                        dp -= iDamage;
                        //피격 지속시간은 나중에 피격상태에 따라 달라지도록 구현헤야 할 듯,
                        StartCoroutine(BeShotTimer(0.2f));
                    }
                    else
                    {
                        dp = 0;
                    }
                }
                else
                {
                    //쥬금
                    print("쥬금");
                }
            }
            if (coll.gameObject.tag == Tags.Floor)
            {
                print("coll");
            }
        }

        void OnCollisionStay(Collision coll)
        {
            print("coll");
        }

        public float GetEP() { return ep; }
        public void SetEP(float f) { ep = f; }
        public int GetAP() { return ap; }
        public void SetAP(int i) { ap = i; }
        public int GetDP() { return dp; }
        public void SetDP(int i) { dp = i; }
        public int GetPP() { return pp; }
        public void SetPP(int i) { pp = i; }
        public bool GetCheckAimForMonster() { return bAimForMonster; }

        public LayerState GetLayerState() { return curLayerState; }
        public void SetLayerState(LayerState s)
        {
            curLayerState = s;
            //캐릭터의 State를 이용하여 무적상태와 일반 상태를 변경시켜주는 부분,
            if (curLayerState == LayerState.normal)
            {
                gameObject.layer = LayerMask.NameToLayer(Layers.T_HitCollider);
            }
            else if (curLayerState == LayerState.invincibility)
            {
                gameObject.layer = LayerMask.NameToLayer(Layers.T_Invincibility);
            }
        }

        //possibleState함수들.
        public void SetCtrlPossible(CtrlPossibleIndex index, bool b)
        {
            if (index == CtrlPossibleIndex.Run) ctrlPossible.Run = b;
            else if (index == CtrlPossibleIndex.Sprint) ctrlPossible.Sprint = b;
            else if (index == CtrlPossibleIndex.Attack) ctrlPossible.Attack = b;
            else if (index == CtrlPossibleIndex.MouseRot) ctrlPossible.MouseRot = b;
            else if (index == CtrlPossibleIndex.Skill) ctrlPossible.Skill = b;
        }
        public CtrlPossible GetCtrlPossible() { return ctrlPossible; }

        public void ChangeState(State s)
        {
            skillCtrl = s;
            if (skillCtrl == State.idle)
            {
                ctrlPossible.Run = true;
                ctrlPossible.Sprint = true;
                ctrlPossible.Attack = true;
                ctrlPossible.MouseRot = true;
                ctrlPossible.Skill = true;
            }
            else if (skillCtrl == State.attack)
            {
                ctrlPossible.Run = true;
                ctrlPossible.Sprint = false;
                ctrlPossible.Attack = true;
                ctrlPossible.MouseRot = true;
                ctrlPossible.Skill = true;
            }
            else if (skillCtrl == State.Skill)
            {
                ctrlPossible.Run = false;
                ctrlPossible.Sprint = false;
                ctrlPossible.Attack = false;
                ctrlPossible.MouseRot = true;
                ctrlPossible.Skill = true;
            }
            else if (skillCtrl == State.be_Shot)
            {
                ctrlPossible.Run = false;
                ctrlPossible.Sprint = false;
                ctrlPossible.Attack = false;
                ctrlPossible.MouseRot = true;
                ctrlPossible.Skill = false;
            }
        }
        public State GetState() { return skillCtrl; }
        IEnumerator BeShotTimer(float time)
        {
            print("beShot");
            ChangeState(State.be_Shot);
            //피격 애니메이션 시작.

            yield return new WaitForSeconds(time);
            ChangeState(State.idle);
        }

        //전력질주시에 사용하는 ep감소, 충전, 충전가속 함수.
        void EpDecrease()
        {
            //가속으로 인해 바뀐 수치들 초기화
            fIncEP = 1;
            incAccelTimer = 0.0f;

            if (ep > 0.0f)
            {
                if (decreaseTimer > decreaseTime)
                {
                    ep -= fDecEP;
                    decreaseTimer = 0.0f;
                }
                else
                    decreaseTimer += Time.deltaTime;
            }
            else
                return;
        }
        void EpIncrease()
        {
            if (ep < T2.Stat.MAX_EP)
            {
                EpIncAccel();
                if (increaseTimer > increaseTime)
                {
                    ep += fIncEP;
                    increaseTimer = 0.0f;
                }
                else
                    increaseTimer += Time.deltaTime;
            }
            else
            {
                if (ep > 100)
                    ep = 100;
            }
        }
        void EpIncAccel()
        {
            if (incAccelTimer > incAccelTime)
            {
                fIncEP += fIncAccelPoint;
                incAccelTimer = 0.0f;
            }
            else
                incAccelTimer += Time.deltaTime;
        }

        //스킬 타입별 포인트 감소
        //public bool DecreaseSkillPoint(T2.Manager.SkillType type, int decPoint)
        //{
        //    if (type == T2.Manager.SkillType.AP)
        //    {
        //        if (ap <= 0 || ap < decPoint)
        //        {
        //            print("ap가 부족합니다.");
        //            return false;
        //        }
        //        else
        //        {
        //            ap -= decPoint;
        //            return true;
        //        }
        //    }
        //    else if (type == T2.Manager.SkillType.EP)
        //    {
        //        if (ep <= 0 || ep < decPoint)
        //        {
        //            print("ep가 부족합니다.");
        //            return false;
        //        }
        //        else
        //        {
        //            //가속으로 인해 바뀐 수치들 초기화
        //            fIncEP = 0;
        //            incAccelTimer = 0.0f;

        //            ep -= decPoint;
        //            return true;
        //        }
        //    }
        //    else if (type == T2.Manager.SkillType.PP)
        //    {
        //        if (pp <= 0 || pp < decPoint)
        //        {
        //            print("pp가 부족합니다.");
        //            return false;
        //        }
        //        else
        //        {
        //            pp -= decPoint;
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public bool PointCheck(T2.Manager.SkillType type, int decPoint)
        {
            if (type == T2.Manager.SkillType.AP)
            {
                if (ap <= 0 || ap < decPoint)
                {
                    print("ap가 부족합니다.");
                    return false;
                }
            }
            else if (type == T2.Manager.SkillType.EP)
            {
                if (ep <= 0 || ep < decPoint)
                {
                    print("ep가 부족합니다.");
                    return false;
                }

            }
            else if (type == T2.Manager.SkillType.PP)
            {
                if (pp <= 0 || pp < decPoint)
                {
                    print("pp가 부족합니다.");
                    return false;
                }

            }
            return true;
        }
        public void DecreaseSkillPoint(T2.Manager.SkillType type, int decPoint)
        {
            if (type == T2.Manager.SkillType.AP)
            {
                if (ap > decPoint)
                {
                    ap -= decPoint;
                    return;
                }
            }
            else if (type == T2.Manager.SkillType.EP)
            {
                if (ep > decPoint)
                {
                    //가속으로 인해 바뀐 수치들 초기화
                    fIncEP = 0;
                    incAccelTimer = 0.0f;

                    ep -= decPoint;
                    return;
                }
            }
            else if (type == T2.Manager.SkillType.PP)
            {
                if (pp > decPoint)
                {
                    pp -= decPoint;
                    return;
                }
            }
        }
    }
}