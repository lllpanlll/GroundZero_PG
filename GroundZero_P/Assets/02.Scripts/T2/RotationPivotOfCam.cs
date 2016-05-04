using UnityEngine;
using System.Collections;

namespace T2
{
    /// <summary>
    /// 2016-05-04
    /// 플레이어의 머리부분의 오브젝트에 붙는 스크립트
    /// 이 오브젝트를 회전시키고, 카메라가 이 오브젝트를 LookAt하도록 해서 카메라가 회전하도록 한다.
    /// 마우스의 Axis값을 받아와 회전시킨다.
    /// fClamp값 만큼만 위,아래 회전이 가능하다.
    /// 마우스 회전 컨트롤이 불가능한 경우를 구현함.
    /// </summary>
    public class RotationPivotOfCam : MonoBehaviour
    {
        private float fAngleX, fAngleY;
        private float fRotSpeed;
        private MoveCtrl moveCtrl;
        private BasicAttack basicAttack;
        private Animator animator;
        private Manager mgr;

        private float fStartY;
        private float fClamp = 40.0f;

        private Quaternion rotation = Quaternion.identity;

        void Start()
        {
            moveCtrl = GetComponentInParent<MoveCtrl>();
            mgr = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<Manager>();
            fRotSpeed = Camera.main.GetComponent<FollowCam>().fMouseRotSpeed;
            basicAttack = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent< BasicAttack>();
            animator = GameObject.FindGameObjectWithTag(Tags.Player).GetComponentInChildren< Animator>();

            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            fAngleY = transform.eulerAngles.x;
            fAngleX = transform.eulerAngles.y;

            fStartY = fAngleY;
        }

        void OnEnable()
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

            fAngleY = transform.eulerAngles.x;
            fAngleX = transform.eulerAngles.y;

            fStartY = fAngleY;
        }

        void LateUpdate()
        {            

            if (mgr.GetCtrlPossible().MouseRot == true)
            {
                fAngleY += -Input.GetAxis("Mouse Y") * fRotSpeed * Time.deltaTime;
                fAngleX += Input.GetAxis("Mouse X") * fRotSpeed * Time.deltaTime;
                //Clamp
                if ((fAngleY - fStartY) > fClamp) fAngleY = fStartY + fClamp;
                else if ((fAngleY - fStartY) < -fClamp) fAngleY = fStartY + (-fClamp);

                rotation = Quaternion.Euler(fAngleY, fAngleX, 0.0f);
                transform.rotation = rotation;
            }
            else if (mgr.GetCtrlPossible().MouseRot == false)
            {
                //마우스 회전이 정지되어있는 동안 회전이 되었다면, 그 회전값을 유지하기 위해 앵글값을 설정해 준다.  
                if (transform.rotation.eulerAngles.x > fClamp)
                    fAngleY = transform.rotation.eulerAngles.x - 360.0f;
                else
                    fAngleY = transform.rotation.eulerAngles.x;

                fAngleX = transform.rotation.eulerAngles.y;

            }

            animator.SetFloat("fAimAngle", fAngleY);
            Debug.DrawRay(transform.position, transform.forward);
        }
    }
}