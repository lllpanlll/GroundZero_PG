using UnityEngine;
using System.Collections;

namespace T2
{
    public class Animation : MonoBehaviour
    {
        #region<레거시 애니메이션>
        //[System.Serializable]
        //public class Anim
        //{
        //    public AnimationClip idle;
        //    public AnimationClip aimForward;
        //    public AnimationClip aimBackward;
        //    public AnimationClip aimRight;
        //    public AnimationClip aimLeft;

        //    public AnimationClip runForward;

        //    public AnimationClip sprintForward;

        //    public AnimationClip normalAttack;
        //}
        //public Anim anim;
        //public Animation _animation;
        //public Transform _spine;
        #endregion
            
        private Animator animator;

        private T2.MoveCtrl moveCtrfl;
        private T2.BasicAttack basicAttack;
        private T2.MoveCtrl moveCtrl;
        private Transform trPlayerModel;
        private T2.Manager mgr;

        private T2.MoveCtrl.MoveFlag moveFlag;
        private T2.MoveCtrl.MoveState moveState;
        private float h, v;
        private float fSpeed;


        GameObject model;


        void Start()
        {
            moveCtrfl = GetComponent<T2.MoveCtrl>();
            basicAttack = GetComponent<T2.BasicAttack>();
            moveCtrfl = GetComponent<T2.MoveCtrl>();
            animator = GetComponentInChildren<Animator>();
            mgr = GetComponent<T2.Manager>();
            trPlayerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel).transform;


            fSpeed = moveCtrfl.GetMoveSpeed();
        }

        bool freeze = false;
        float freezeTime = 0.0f;

        void Update()
        {
            moveFlag = moveCtrfl.GetMoveFlag();
            moveState = moveCtrfl.GetMoveState();
            fSpeed = moveCtrfl.GetMoveSpeed();

            if (mgr.GetState() != T2.Manager.State.Skill)
            {
                if (moveState == T2.MoveCtrl.MoveState.Stop)
                {
                    animator.SetBool("bIdle", true);
                }
                else if (moveFlag.forward || moveFlag.backward || moveFlag.right || moveFlag.left)
                {
                    animator.SetBool("bIdle", false);
                    animator.SetFloat("fSpeed", fSpeed);
                }
            }
            else
            {
                animator.SetBool("bIdle", false);
                animator.SetFloat("fSpeed", fSpeed);
            }


            #region<레거시 애니메이션>
            ////run상태 이동,
            //if(moveState == T2.MoveCtrl.MoveState.Run)
            //{

            //    if (moveFlag.forward) _animation.CrossFade(anim.aimForward.name, 0.15f);
            //    else if (moveFlag.backward) _animation.CrossFade(anim.aimBackward.name, 0.15f);
            //    else if (moveFlag.right) _animation.CrossFade(anim.aimRight.name, 0.15f);
            //    else if (moveFlag.left) _animation.CrossFade(anim.aimLeft.name, 0.15f);
            //    else _animation.CrossFade(anim.idle.name, 0.15f);

            //    if (basicAttack.isFire())
            //    {
            //        _animation.Play(anim.normalAttack.name);

            //    }
            //}
            ////sprint상태 이동,
            //else if (moveState == T2.MoveCtrl.MoveState.Sprint)
            //{

            //    if (moveFlag.forward) _animation.CrossFade(anim.runForward.name, 0.15f);
            //    else if (moveFlag.backward) _animation.CrossFade(anim.runForward.name, 0.15f);
            //    else if (moveFlag.right) _animation.CrossFade(anim.runForward.name, 0.15f);
            //    else if (moveFlag.left) _animation.CrossFade(anim.runForward.name, 0.15f);
            //    else _animation.CrossFade(anim.idle.name, 0.15f);
            //}
            ////이동상태 이외인 경우 ex(스킬 등의 애니메이션들)
            //else
            //{
            //    if (basicAttack.isFire())
            //    {
            //        _animation.CrossFade(anim.normalAttack.name, 0.15f);
            //    }
            //    //idle상태
            //    else
            //        _animation.CrossFade(anim.idle.name, 0.15f);
            //}
            #endregion
        }

        //void Freeze()
        //{
        //    freezeTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        //}
        //void UnFreeze()
        //{
        //    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //    animator.Play(stateInfo.nameHash, 0, freezeTime);
        //}

    }
}