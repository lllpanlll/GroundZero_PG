using UnityEngine;
using System.Collections;
using System;

namespace T2.Skill
{
    public class Snipe : T2.Skill.Skill
    {

        //public GameObject oBulletPref;
        //private GameObject oBullet;
        //private ObjectPool bulletPool = new ObjectPool();

        //private T2.Manager mgr;
        //private RotationPivotOfCam rotationPivotOfCam;
        //private FollowCam followCam;
        //private SnipeModeRotation snipeModeRotation;

        //private Transform trPivotOfCam;
        //private Transform trCam;
        //private GameObject oPlayerModel;

        //private int iDecAP = 10;

        //private bool bZoom = false;
        //private float fZoomInFOV = 30.0f;
        //private float fOriginFOV = 60.0f;

        //private float fReach = 1000.0f;

        //private float beforeDelayTime = 0.0f;
        //private float actionTime = 0.0f;
        //private float afterDelayTime = 1.0f;
        //private float coolTime = 0.0f;

        //private Ray fireRay;

        //void Start()
        //{
        //    mgr = GetComponent<T2.Manager>();
        //    rotationPivotOfCam = GetComponentInChildren<RotationPivotOfCam>();
        //    oPlayerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel);
        //    followCam = Camera.main.GetComponent<FollowCam>();
        //    snipeModeRotation = Camera.main.GetComponent<SnipeModeRotation>();

        //    snipeModeRotation.enabled = false;

        //    trCam = Camera.main.transform;
        //    trPivotOfCam = GameObject.FindGameObjectWithTag(Tags.CameraTarget).transform;

        //    bulletPool.CreatePool(oBulletPref, 5);

        //    base.SetCoolTime(coolTime);
        //}

        //void Update()
        //{
        //    //화면의 중앙 벡터
        //    Vector3 centerPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));
        //    //화면의 중앙에서 카메라의 정면방향으로 레이를 쏜다.
        //    fireRay = new Ray(centerPos, trCam.forward);

        //    if (mgr.GetState() == T2.Manager.State.be_Shot)
        //        base.SkillCancel();

        //    if (!base.IsCoolTime())
        //    {
        //        if (Input.GetMouseButtonDown(1) && !base.IsRunning())
        //            InputCommand(T2.Manager.SkillType.AP, iDecAP);
        //        if (base.IsBeforeDelay())
        //            BeforeActionDelay(beforeDelayTime);
        //        if (base.IsExecute())
        //            Execute(actionTime);
        //        if (base.IsAfterDelay())
        //            AfterActionDelay(afterDelayTime);
        //    }
        //    else
        //    {
        //        if (base.IsRunning())
        //        {
        //            base.CoolTimeDelay();
        //        }
        //    }

        //    if (bZoom)
        //    {
        //        if (Input.GetMouseButtonDown(0))
        //        {
        //            bZoom = false;

        //            Camera.main.fieldOfView = fOriginFOV;

        //            //먼저 저격 중 카메라 회전 스크립트 종료.
        //            snipeModeRotation.enabled = false;
        //            //다음으로 카메라 Pivot을 저격으로 조준했던 방향으로 조정.              
        //            //rotationPivotOfCam.TargetLookat(snipeModeRotation.GetLastShotPos());
        //            rotationPivotOfCam.transform.LookAt(snipeModeRotation.GetLastShotPos());
        //            //Pivot스크립트 재활성화.(카메라 회전 역할)
        //            rotationPivotOfCam.enabled = true;
        //            //followCam스크립트 재활성화.
        //            followCam.enabled = true;
        //            //발사 방향으로 캐릭터(모델) 방향 조정.
        //            oPlayerModel.SetActive(true);
        //            oPlayerModel.transform.rotation = Quaternion.Euler(0.0f, trCam.eulerAngles.y, 0.0f);

        //            //카메라에서 쏘는 레이가 부딪힌 위치에 플레이어의 총알이 발사되는 각도를 조정한다.
        //            RaycastHit aimRayHit;
        //            if (Physics.Raycast(fireRay, out aimRayHit, fReach))
        //            {
        //                //데미지 계산 코드 작성 위치
        //                if (aimRayHit.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
        //                {
        //                    aimRayHit.collider.gameObject.GetComponent<MonsterHitCtrl>().OnHitMonster(10);
        //                    GameObject.FindWithTag(Tags.Monster).GetComponent<M_FSMTest>().stiffValue += 70; ;
        //                }
        //            }
        //            base.FinishExecute();

        //            #region<투사체>
        //            oBullet = bulletPool.UseObject();
        //            oBullet.transform.position = trPivotOfCam.position;
        //            oBullet.transform.rotation = trCam.rotation;
        //            oBullet.SetActive(true);
        //            #endregion
        //        }
        //    }

        //}

        //protected override void InputCommand(T2.Manager.SkillType type, int decPoint)
        //{
        //    base.InputCommand(type, decPoint);
        //}
        //protected override void BeforeActionDelay(float time)
        //{
        //    print("선딜");
        //    mgr.SetCtrlPossible(T2.Manager.CtrlPossibleIndex.MouseRot, false);
        //    base.BeforeActionDelay(time);
        //}
        //protected override void Execute(float time)
        //{
        //    print("액션");


        //    followCam.enabled = false;
        //    rotationPivotOfCam.enabled = false;

        //    //카메라 이동 및 회전.
        //    trCam.position = trPivotOfCam.position + (trPivotOfCam.right * followCam.GetRight());
        //    trCam.LookAt(fireRay.GetPoint(100.0f));

        //    bZoom = true;

        //    Camera.main.fieldOfView = fZoomInFOV;
        //    oPlayerModel.SetActive(false);

        //    snipeModeRotation.enabled = true;

        //    //base.Execute(time);
        //}
        //protected override void AfterActionDelay(float time)
        //{
        //    print("후딜");

        //    base.AfterActionDelay(time);
        //}
        //protected override void CoolTimeDelay()
        //{
        //    mgr.SetCtrlPossible(T2.Manager.CtrlPossibleIndex.MouseRot, true);
        //    base.CoolTimeDelay();
        //}
    }
}