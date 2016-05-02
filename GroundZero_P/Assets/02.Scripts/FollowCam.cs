using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class FollowCam : MonoBehaviour {
    private Transform trTarget;
    private LookAtIK lookAt;
    private Transform trPlayerModel;
    private T2.MoveCtrl moveCtrl;

    public float DIST = 3.5f;
    public float ZOOM_DIST = 1.0f;    
    public float RIGHT = 1.0f;
    private float zoomOutDist;

    public float DAMP_TRACE = 20.0f;
    private float fDampTrace;

    private float fDist;
    private float fRight;
    private float fUp = 0.0f;

    private float fAimOutLerpSpeed = 2.0f;

    //초기화 변수
    public float fMouseRotSpeed = 200.0f;

	public GameObject	oPlayerLight;

    private Vector3 vTarget = Vector3.zero;
    private float fTargetRotSpeed = 10.0f;
    private bool bForwardTarget = true;


    void Start () {
        fDist = DIST;
        zoomOutDist = DIST;
        fRight = RIGHT;
        fDampTrace = DAMP_TRACE;
        trTarget = GameObject.FindGameObjectWithTag(Tags.CameraTarget).GetComponent<Transform>();
        trPlayerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel).transform;
        lookAt = GameObject.FindGameObjectWithTag(Tags.PlayerModel).GetComponent<LookAtIK>();
        moveCtrl = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.MoveCtrl>();

        vTarget = trTarget.position + (trTarget.forward * 30.0f);

    }

    void LateUpdate () {
        #region<바닥충돌처리>
        // 카메라 바닥 안뚫,
        Vector3 vTargetToCamDir = (transform.position - trTarget.transform.position);
        Ray rTargetToTargetBackward = new Ray(trTarget.position, -trTarget.forward * fDist);
        //Debug.DrawRay(rTargetToTargetBackward.origin, (vTargetToCamDir.normalized) * fDist, Color.blue);
        RaycastHit hit;
        if (Physics.Raycast(rTargetToTargetBackward, out hit) && hit.collider.CompareTag(Tags.Floor))
        {
                if (hit.distance < fDist)
                {
                    if (zoomOutDist > ZOOM_DIST)
                        zoomOutDist = Mathf.Lerp(zoomOutDist, zoomOutDist * 0.8f, Time.deltaTime * fAimOutLerpSpeed);
                    else
                        zoomOutDist = DIST;
                }
        }
        else
        {
            zoomOutDist = DIST;
        }
        #endregion

		oPlayerLight.transform.LookAt (trTarget);

        //transform.position = Vector3.Lerp(transform.position, trTarget.position - (trTarget.forward * fDist) + (trTarget.right * RIGHT),
        //    Time.deltaTime * DAMP_TRACE);


        transform.position = trTarget.position - (trTarget.forward * DIST) + (trTarget.right * RIGHT) + (trTarget.up * fUp);
        transform.LookAt((trTarget.position + (trTarget.right * RIGHT)));


        //LookIK
        //카메라가 캐릭터 후방에 있다면, 타겟 위치를 에임방향으로.
        //카메라가 캐릭터 전방에 있다면, 타겟 위치를 카메라위치로.        
        float fLookAngle = Vector3.Angle(trPlayerModel.forward, transform.forward);
        if (fLookAngle > 90.0f && moveCtrl.GetMoveState() == T2.MoveCtrl.MoveState.Stop)
            vTarget = transform.position;
        else
            vTarget = trTarget.position + (trTarget.forward * 30.0f);

        if (moveCtrl.GetMoveFlag().backward)
        {
            lookAt.enabled = false;
        }
        else
        {
            lookAt.enabled = true;
            lookAt.solver.IKPosition = Vector3.Lerp(lookAt.solver.IKPosition, vTarget, Time.deltaTime * fTargetRotSpeed);
        }
    }

    public void SetDampTrace(float f) { fDampTrace = f; }
    public float GetDampTrace() { return fDampTrace; }
    public void SetDist(float f) { DIST = f; }
    public float GetDist() { return DIST; }
    public void SetRight(float f) { RIGHT = f; }
    public float GetRight() { return RIGHT; }
    public void SetUp(float f) { fUp = f; }
    public float GetUp() { return fUp; }

}
