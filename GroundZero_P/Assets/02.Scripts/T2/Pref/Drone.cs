using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace T2.Pref
{
    /// <summary>
    /// 2016-05-04
    /// 캐릭터의 DroneCall스킬로 소환되는 오브젝트
    /// 생존시간은 DroneCall스킬에서 관리한다.
    /// 캐릭터의 기본공격과 같은 커맨드 입력시 에임방향으로 총알을 발사한다.
    /// </summary>
    public class Drone : MonoBehaviour
    {
        private Vector3 vTargetPos;
        private float fTargetSpeed;

        private T2.Manager mgr;
        private Transform trPlayer;

        //연사속도 타이머
        private float rpmTime = 0.5f;
        private float rpmTimer = 0.0f;

        public GameObject oBulletPref;
        private GameObject oBullet;
        private ObjectPool bulletPool = new ObjectPool();

        void Start()
        {
            mgr = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.Manager>();
            trPlayer = GameObject.FindGameObjectWithTag(Tags.CameraTarget).transform;

            bulletPool.CreatePool(oBulletPref, 10);
        }

        void OnEnable()
        {
            rpmTime = Random.Range(0.5f, 1.0f);
        }

        void Update()
        {
            transform.position = Vector3.Lerp(transform.position, vTargetPos, Time.deltaTime * fTargetSpeed);

            if (Input.GetMouseButton(0) && mgr.GetCtrlPossible().Attack == true)
            {
                //연사속도 조절.
                if (rpmTimer > rpmTime)
                {
                    Fire();
                    rpmTimer = 0.0f;
                }
                else
                    rpmTimer += Time.deltaTime;
            }
            Debug.DrawRay(transform.position, transform.forward);
        }

        private void Fire()
        {
            float fCamRotX = Camera.main.transform.eulerAngles.x;
            transform.rotation = Quaternion.Euler(fCamRotX, trPlayer.eulerAngles.y, 0.0f);

            //투사체 오브젝트 풀 생성.
            oBullet = bulletPool.UseObject();
            oBullet.transform.position = transform.position;
            oBullet.transform.rotation = transform.rotation;
        }

        public void setTargetPos(Vector3 pos)
        {
            vTargetPos = pos;
        }
        public void setSpeed(float speed)
        {
            fTargetSpeed = speed;
        }
    }
}