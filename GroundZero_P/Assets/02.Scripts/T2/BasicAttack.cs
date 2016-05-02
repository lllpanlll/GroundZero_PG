using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;

namespace T2
{
    public class BasicAttack : MonoBehaviour
    {
        public GameObject oBulletPref;
        private GameObject oBullet;
        private ObjectPool bulletPool = new ObjectPool();

        public GameObject oFlarePref;
        private GameObject oFlare;
        private ObjectPool flarePool = new ObjectPool();

        public MeshRenderer[] muzzleFlash;

        public Transform[] trFire;

        private T2.MoveCtrl moveCtrl;  //이동상태 변경을 위함.
        private T2.Manager mgr;
        private Animator animator;
        private Transform trPlayerModel;

        //타이머 변수
        private bool bFire = false;
        private float attackTimer = 0.0f;
        private float attackTime = 0.3f;

        private float fReach = 1000.0f;

        //연사속도 타이머
        private float fRpmSpeed = 0.1f;
        private float fRpmTime;
        private float fRpmTimer = 0.0f;

        //카메라 줌인
        private Camera cam;
        private FollowCam followCam;
        private float fCamDist = 0.0f;
        private float fCamFOV;
        private float fOrizinDist;
        public float fTargetDist = 1.0f;
        private float fOrizinFOV;
        public float fTargetFOV = 70.0f;
        public float fZoomSpeed = 20.0f;

        private int iMaxMagazine;
        //집탄율..?
        private float fAccuracy = 0.1f;
        private bool bFirstShot = true;

        void Start()
        {
            iMaxMagazine = T2.Stat.MAX_MAGAZINE;

            moveCtrl = GetComponent<T2.MoveCtrl>();
            mgr = GetComponent<T2.Manager>();
            trPlayerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel).transform;            
            animator = GetComponentInChildren<Animator>();

            cam = Camera.main;
            followCam = cam.GetComponent<FollowCam>();            
            fOrizinDist = followCam.GetDist();
            fOrizinFOV = cam.fieldOfView;
            fCamDist = followCam.GetDist();

            bulletPool.CreatePool(oBulletPref, iMaxMagazine);
            flarePool.CreatePool(oFlarePref, iMaxMagazine * 2);

            muzzleFlash[0].enabled = false;
            muzzleFlash[1].enabled = false;

        }

        void Update()
        {
            if (mgr.GetCtrlPossible().Attack == false)
            {
                bFire = false;
                attackTimer = 0.0f;
                return;
            }

            if (Input.GetMouseButton(0))
            {


                //첫 공격시에는 바로 공격 가능하고, 그 다음부터 연사 속도 적용.
                //if (!bFire)
                if (bFirstShot || !bFire)
                    fRpmTime = 0.0f;
                else
                    fRpmTime = fRpmSpeed;

                bFirstShot = false;

                //연사속도 조절.
                if (fRpmTimer > fRpmTime)
                {
                    Fire();
                    bFire = true;
                    animator.SetBool("bAttack", true);
                    fRpmTimer = 0.0f;
                }
                else
                    fRpmTimer += Time.deltaTime;
            }

            if (Input.GetMouseButtonUp(0))
            {
                fRpmTime = 0.0f;
                bFirstShot = true;
                attackTimer = 0.0f;
            }

            //공격하면 플레이어 이동상태를 일정 시간 동안 변경.
            if (bFire)
            {
                mgr.ChangeState(T2.Manager.State.attack);

                if (attackTimer > attackTime)
                {
                    //어택타임이 지난 공격종료 시점.
                    bFire = false;
                    animator.SetBool("bAttack", false);
                    attackTimer = 0.0f;
                    
                    fRpmTimer = 0.0f;
                    fRpmTime = fRpmSpeed;

                    mgr.ChangeState(T2.Manager.State.idle);
                }
                else
                {
                    attackTimer += Time.deltaTime;
                }

                //플레이어가 정지상태일 때,
                if (moveCtrl.GetMoveState() == MoveCtrl.MoveState.Stop)
                {
                    //플레이어 모델을 정면을 바라보게 한다.
                    trPlayerModel.rotation = Quaternion.Euler(0.0f, Camera.main.transform.eulerAngles.y, 0.0f);
                }

                //카메라 줌인
                fCamDist = Mathf.Lerp(fCamDist, fTargetDist, Time.deltaTime * fZoomSpeed);
                followCam.SetDist(fCamDist);
                fCamFOV = Mathf.Lerp(fCamFOV, fTargetFOV, Time.deltaTime * fZoomSpeed);
                cam.fieldOfView = fCamFOV;          
            }
            else
            {
                //카메라 줌아웃
                fCamDist = Mathf.Lerp(fCamDist, fOrizinDist, Time.deltaTime * fZoomSpeed * 0.2f);
                followCam.SetDist(fCamDist);
                fCamFOV = Mathf.Lerp(fCamFOV, fOrizinFOV, Time.deltaTime * fZoomSpeed * 0.2f);
                cam.fieldOfView = fCamFOV;
            }
        }


        void Fire()
        {
            //bFire = true;
            attackTimer = 0.0f;

            Transform camTr = Camera.main.transform;
            //화면의 중앙 벡터
            Vector3 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));

            //화면의 중앙에서 카메라의 정면방향으로 레이를 쏜다.
            Ray aimRay = new Ray(centerPos, camTr.forward);
            Debug.DrawLine(aimRay.origin, aimRay.direction, Color.blue);

            //카메라에서 쏘는 레이가 부딪힌 위치에 플레이어의 총알이 발사되는 각도를 조정한다.
            RaycastHit aimRayHit;
            //레이어 마스크 ignore처리 (-1)에서 빼 주어야 함
            int mask = (1 << LayerMask.NameToLayer(Layers.T_HitCollider)) | (1 << LayerMask.NameToLayer(Layers.Bullet));
            mask = ~mask;

            if (Physics.Raycast(aimRay, out aimRayHit, fReach, mask))
            {
                //aimRayHit.point와 플레이어 포지션 위치의 거리.(사정거리 체크)
                float fRangeCheck = Vector3.Distance(transform.position, aimRayHit.point);

                //거리에 따라 명중률 조정.
                fAccuracy = 0.1f + (fRangeCheck * 0.02f);
                Vector3 fTarget = aimRayHit.point;
                fTarget = new Vector3(fTarget.x,
                                      Random.Range(fTarget.y - fAccuracy, fTarget.y + fAccuracy),
                                      Random.Range(fTarget.z - fAccuracy, fTarget.z + fAccuracy));

                //레이에 부딪힌 오브젝트가 있으면 부딪힌 위치를 바라보도록 방향 조정.
                trFire[0].LookAt(fTarget);
                trFire[1].LookAt(fTarget);


                //aimRayHit.point가 사정거리 이내에 위치할 경우.
                if (fRangeCheck < fReach)
                {
                    ////데미지 계산 코드 작성 위치

                }
            }
            else
            {
                //최대거리 명중률 조정.
                fAccuracy = 20.0f;
                Vector3 fTarget = aimRay.GetPoint(fReach);
                fTarget = new Vector3(fTarget.x,
                                      Random.Range(fTarget.y - fAccuracy, fTarget.y + fAccuracy),
                                      Random.Range(fTarget.z - fAccuracy, fTarget.z + fAccuracy));
                trFire[0].LookAt(fTarget);
                trFire[1].LookAt(fTarget);
            }

            //투사체 오브젝트 풀 생성.    
            oBullet = bulletPool.UseObject();
            oBullet.transform.position = trFire[0].position;
            oBullet.transform.rotation = trFire[0].rotation;

            //머즐플래시
            this.StartCoroutine(ShowMuzzleFlash(0));

            //왼손 사격
            StartCoroutine(LeftGunShot());

        }

        public void TargetFire(Vector3 vTarget)
        {
            trFire[0].LookAt(vTarget);

            //투사체 오브젝트 풀 생성.    
            oBullet = bulletPool.UseObject();
            oBullet.transform.position = trFire[0].position;
            oBullet.transform.rotation = trFire[0].rotation;

            //머즐플래시
            this.StartCoroutine(ShowMuzzleFlash(0));
        }

        public bool isFire() { return bFire; }
        public float GetReach() { return fReach; }
        public void SetFlareEffect(Vector3 pos)
        {
            oFlare = flarePool.UseObject();
            oFlare.transform.position = pos;
        }

        IEnumerator LeftGunShot()
        {
            yield return new WaitForSeconds(0.05f);

            //투사체 오브젝트 풀 생성.    
            oBullet = bulletPool.UseObject();
            oBullet.transform.position = trFire[1].position;
            oBullet.transform.rotation = trFire[1].rotation;

            //머즐플래시
            this.StartCoroutine(ShowMuzzleFlash(1));
        }

        IEnumerator ShowMuzzleFlash(int i)
        {
            //i == 0 이면 오른손,
            //i == 1 이면 왼손.
            float scale = Random.Range(1.0f, 1.5f);
            muzzleFlash[i].transform.localScale = Vector3.one * scale;

            Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
            muzzleFlash[i].transform.localRotation = rot;

            muzzleFlash[i].enabled = true;

            yield return new WaitForSeconds(Random.Range(0.05f, 0.008f));

            muzzleFlash[i].enabled = false;
        }
    }
}