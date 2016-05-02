using UnityEngine;
using System.Collections;

namespace T2
{
    public class RotationPivotOfCam : MonoBehaviour
    {
        private float fAngleX, fAngleY;
        private float fRotSpeed;
        private MoveCtrl moveCtrl;
        private BasicAttack basicAttack;
        private Animator animator;
        private Manager mgr;

        private float fStartY;
        private float fClamp = 30.0f;

        private Quaternion rotation = Quaternion.identity;

        void Start()
        {
            moveCtrl = GetComponentInParent<MoveCtrl>();
            mgr = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<Manager>();
            fRotSpeed = Camera.main.GetComponent<FollowCam>().fMouseRotSpeed;
            basicAttack = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent< BasicAttack>();
            animator = GameObject.FindGameObjectWithTag(Tags.Player).GetComponentInChildren< Animator>();
            fAngleY = transform.eulerAngles.x;
            fAngleX = transform.eulerAngles.y;

            fStartY = fAngleY;
        }

        void OnEnable()
        {
            fAngleY = transform.eulerAngles.x;
            fAngleX = transform.eulerAngles.y;

            fStartY = fAngleY;
        }

        void Update()
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