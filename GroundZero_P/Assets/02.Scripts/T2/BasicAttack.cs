using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        public GameObject[] muzzleFlash;

        public Transform[] trFire;

        private T2.MoveCtrl moveCtrl;  //이동상태 변경을 위함.
        private T2.Manager mgr;
        private Animator animator;
        private Transform trPlayerModel;
        private Transform trCamPivot;

        //타이머 변수
        private bool bFire = false;
        private float attackTimer = 0.0f;
        private float attackTime = 0.5f;

        private float fReach = 300.0f;

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


        float fRot = 0.0f;
        void Start()
        {
            iMaxMagazine = T2.Stat.MAX_MAGAZINE;

            moveCtrl = GetComponent<T2.MoveCtrl>();
            mgr = GetComponent<T2.Manager>();
            trPlayerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel).transform;
            trCamPivot = GameObject.FindGameObjectWithTag(Tags.CameraTarget).transform;
            animator = GetComponentInChildren<Animator>();

            cam = Camera.main;
            followCam = cam.GetComponent<FollowCam>();            
            fOrizinDist = followCam.GetDist();
            fOrizinFOV = cam.fieldOfView;
            fCamDist = followCam.GetDist();

            bulletPool.CreatePool(oBulletPref, iMaxMagazine);
            flarePool.CreatePool(oFlarePref, iMaxMagazine * 2);

            muzzleFlash[0].SetActive(false);
            muzzleFlash[1].SetActive(false);

        }

        void Update()
        {
            if (mgr.GetCtrlPossible().Attack == false)
            {
                bFire = false;
                animator.SetBool("bAttack", false);
                attackTimer = 0.0f;
                return;
            }

            if (Input.GetMouseButton(0) &&  mgr.GetAP() > 0)
            {
                //첫 공격시에는 바로 공격 가능하고, 그 다음부터 연사 속도 적용.
                if (!bFire)
                //if (bFirstShot || !bFire)
                    fRpmTime = 0.0f;
                else
                    fRpmTime = fRpmSpeed;

                //bFirstShot = false;

                //연사속도 조절.
                if (fRpmTimer > fRpmTime)
                {
                    StartCoroutine(Fire());
                    //ap소모
                    mgr.SetAP(mgr.GetAP() - 1);
                    bFire = true;
                    animator.SetBool("bAttack", true);
                    fRpmTimer = 0.0f;
                }
                else
                    fRpmTimer += Time.deltaTime;
            }

            //if (Input.GetMouseButtonUp(0))
            //{
            //    fRpmTime = 0.0f;
            //    bFirstShot = true;
            //    attackTimer = 0.0f;
            //}

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
                    
                    //float fCamY = Camera.main.transform.eulerAngles.y;
                    //fRot = Mathf.LerpAngle(fRot, 0.0f + fCamY, Time.deltaTime * 0.0000002f);
                    //trPlayerModel.rotation = Quaternion.Euler(0.0f, fRot, 0.0f);
                }

                //카메라 줌인
                if (followCam.GetDist() <= fOrizinDist + 1.0f)
                    followCam.ChangeDist(fTargetDist, fZoomSpeed);
                if (cam.fieldOfView <= fOrizinFOV + 1.0f)
                    followCam.ChangeFOV(fTargetFOV, fZoomSpeed);
            }
            else
            {
                //카메라 줌아웃
                if (followCam.GetDist() < fOrizinDist)
                    followCam.ChangeDist(fOrizinDist, fZoomSpeed * 0.2f);
                //FOV를 원래값으로 돌리는 것은 MoveCtrl에서 자동으로 처리된다.
            }
        }


        //void Fire()
        IEnumerator Fire()
        {
            yield return new WaitForSeconds(0.01f);
            //bFire = true;
            attackTimer = 0.0f;

            Transform camTr = Camera.main.transform;
            //화면의 중앙 벡터
            //Vector3 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));

            float fPlayerDist = Vector3.Distance(trCamPivot.position + trCamPivot.forward, camTr.position);
            //화면의 중앙에서 카메라의 정면방향으로 레이를 쏜다.
            Ray aimRay = new Ray(camTr.position + camTr.forward * fPlayerDist, camTr.forward);
            Debug.DrawLine(aimRay.origin, aimRay.direction, Color.blue);

            //카메라에서 쏘는 레이가 부딪힌 위치에 플레이어의 총알이 발사되는 각도를 조정한다.
            RaycastHit aimRayHit;
            //레이어 마스크 ignore처리 (-1)에서 빼 주어야 함
            int mask = (
                       (1 << LayerMask.NameToLayer(Layers.T_HitCollider)) | (1 << LayerMask.NameToLayer(Layers.Bullet)) |
                       (1 << LayerMask.NameToLayer(Layers.T_Invincibility))
                       );
            mask = ~mask;

            if (Physics.Raycast(aimRay, out aimRayHit, fReach, mask))
            {
                //aimRayHit.point와 플레이어 포지션 위치의 거리.(사정거리 체크)
                float fRangeCheck = Vector3.Distance(transform.position, aimRayHit.point);

                //거리에 따라 명중률 조정.
                fAccuracy = 0.1f + (fRangeCheck * 0.02f);
                Vector3 vTarget = aimRayHit.point;
                vTarget = new Vector3(Random.Range(vTarget.x - fAccuracy, vTarget.x + fAccuracy),
                                      Random.Range(vTarget.y - fAccuracy, vTarget.y + fAccuracy),
                                      Random.Range(vTarget.z - fAccuracy, vTarget.z + fAccuracy));

                //레이에 부딪힌 오브젝트가 있으면 부딪힌 위치를 바라보도록 방향 조정.
                trFire[0].LookAt(vTarget);
                trFire[1].LookAt(vTarget);

                //aimRayHit.point가 사정거리 이내에 위치할 경우.
                if (fRangeCheck < fReach)
                {
                    ////데미지 계산 코드 작성 위치

                }
            }
            else
            {
                //최대거리 명중률 조정.
                fAccuracy = 2.0f;
                Vector3 vTarget = aimRay.GetPoint(fReach);
                vTarget = new Vector3(Random.Range(vTarget.x - fAccuracy, vTarget.x + fAccuracy),
                                      Random.Range(vTarget.y - fAccuracy, vTarget.y + fAccuracy),
                                      Random.Range(vTarget.z - fAccuracy, vTarget.z + fAccuracy));
                trFire[0].LookAt(vTarget);
                trFire[1].LookAt(vTarget);
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
        public void TargetFire(Vector3 _vFirePos, Vector3 _vTarget)
        {
            Vector3 vOrizinFirePos = trFire[0].position;
            trFire[0].position = _vFirePos;
            trFire[0].LookAt(_vTarget);

            //투사체 오브젝트 풀 생성.    
            oBullet = bulletPool.UseObject();
            oBullet.transform.position = trFire[0].position;
            oBullet.transform.rotation = trFire[0].rotation;

            trFire[0].position = vOrizinFirePos;
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
            Transform target = trFire[1];
            yield return new WaitForSeconds(0.05f);

            //투사체 오브젝트 풀 생성.    
            oBullet = bulletPool.UseObject();
            oBullet.transform.position = target.position;
            oBullet.transform.rotation = target.rotation;

            //머즐플래시
            this.StartCoroutine(ShowMuzzleFlash(1));
        }

        IEnumerator ShowMuzzleFlash(int i)
        {
            //i == 0 이면 오른손,
            //i == 1 이면 왼손.

            muzzleFlash[i].SetActive(true);

            yield return new WaitForSeconds(0.25f);

            muzzleFlash[i].SetActive(false);
        }
        IEnumerator ShowMuzzleFlash(Vector3 pos)
        {
            //i == 0 이면 오른손,
            //i == 1 이면 왼손.

            Vector3 vOrizinPos = muzzleFlash[0].transform.position;
            muzzleFlash[0].transform.position = pos;
            muzzleFlash[0].SetActive(true);

            yield return new WaitForSeconds(0.25f);

            muzzleFlash[0].transform.position = vOrizinPos;
            muzzleFlash[0].SetActive(false);
            
        }
    }
}