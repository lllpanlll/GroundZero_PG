using UnityEngine;
using System.Collections;
//using RootMotion.FinalIK;

public class FollowCam : MonoBehaviour {
    private Transform trTarget;
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

    float fCamDist;
    Vector3 vCamPos;
    float fMouseClamp;

    //초기화 변수
    public float fMouseRotSpeed = 200.0f;

	public GameObject	oPlayerLight;

    private Vector3 vTarget = Vector3.zero;
    private float fTargetRotSpeed = 50.0f;
    private bool bForwardTarget = true;

    //Dist, FOV 변경
    private Camera cam;
    private bool bFOV, bDist;
    private float fTargetFOV, fOrizinFOV;
    private float fDampTraceFOV;
    private float fTargetDist, fOrizinDist;
    private float fDampTraceDist;


    void Start () {
        fDist = DIST;
        zoomOutDist = DIST;
        fRight = RIGHT;
        fDampTrace = DAMP_TRACE;
        trTarget = GameObject.FindGameObjectWithTag(Tags.CameraTarget).GetComponent<Transform>();
        trPlayerModel = GameObject.FindGameObjectWithTag(Tags.PlayerModel).transform;

        moveCtrl = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.MoveCtrl>();
        cam = Camera.main;
        fOrizinFOV = cam.fieldOfView;
        fOrizinDist = DIST;
        vTarget = trTarget.position + (trTarget.forward * 30.0f);

        fCamDist = 0.1f;
    }

    void LateUpdate () {
        #region<바닥충돌처리>
        // 카메라 바닥 안뚫,
        RaycastHit hit = new RaycastHit();
        Ray rTargetToTargetBackward = new Ray(trTarget.position, -trTarget.forward);
        //vCamPos = trTarget.position - (trTarget.forward * DIST) + (trTarget.right * RIGHT);
        vCamPos = trTarget.position - (trTarget.forward * DIST) + (trTarget.right * RIGHT) + (trTarget.up * fUp);
        fMouseClamp = vCamPos.y;
        if (Physics.Linecast(trTarget.position, vCamPos, out hit, 1 << 14))
        {
            //Debug.DrawRay(hit.point, hit.normal, Color.magenta);
            //norm = hit.point + hit.normal;
            fMouseClamp -= Input.GetAxis("Mouse Y") * fCamDist;
            fMouseClamp = Mathf.Clamp(fMouseClamp, -0.5f, 3.4f);

            float _fDist = Vector3.Distance(trTarget.position, vCamPos);

            // 벡터기반 - 벽에 사용
            //fCamDist = Vector3.Distance(transform.position, hit.point);
            //fCamDist = Mathf.Clamp(fCamDist, 0.1f, 5f);
            //transform.position = Vector3.Lerp(transform.position, (hit.point + (hit.normal * fCamDist)), Time.deltaTime * 20f);
            //print(hit.distance + " || " + fCamDist);
            //if(_fDist +)
            vCamPos = new Vector3(hit.point.x + hit.normal.x * fCamDist,
                fMouseClamp,
                hit.point.z + hit.normal.z * fCamDist);


            // 거리기반 - 땅에 사용 : 자주 충돌되는 부분이라 레이어로 처리하는게 좋아보임.
            //if (hit.transform.CompareTag("FLOOR"))
            //{
            //    zoomOutDist = hit.distance;
            //    zoomOutDist = Mathf.Clamp(zoomOutDist, ZOOM_DIST, hit.distance * 0.8f);
            //}
        }
        else
        {
            //fCamDist = 0;
            zoomOutDist = fDist;
            //transform.position = Vector3.Lerp(transform.position, trTarget.position - (trTarget.forward * fDist) + (transform.right * RIGHT), Time.deltaTime * 20);
        }
        //DIST = Mathf.Lerp(DIST, zoomOutDist, Time.deltaTime * fAimOutLerpSpeed); //check
        #endregion

        oPlayerLight.transform.LookAt (trTarget);

        //transform.position = Vector3.Lerp(transform.position, trTarget.position - (trTarget.forward * fDist) + (trTarget.right * RIGHT),
        //    Time.deltaTime * DAMP_TRACE);

        //transform.position = trTarget.position - (trTarget.forward * DIST) + (trTarget.right * RIGHT) + (trTarget.up * fUp); // 기존, 계산은 바닥충돌처리에서 한꺼번에 함으로 주석처리

        //transform.position = Vector3.Lerp(transform.position, vCamPos, Time.deltaTime * 30); // 러프 버전 O
        transform.rotation = Quaternion.Slerp(transform.rotation, trTarget.rotation, Time.deltaTime * 30);

        transform.position = vCamPos; // 러프 버전 X
        transform.LookAt((trTarget.position + (trTarget.right * RIGHT)));

        if (bFOV)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fTargetFOV, Time.deltaTime * fDampTraceFOV);

            if(cam.fieldOfView <= fTargetFOV + 0.01f && cam.fieldOfView >= fTargetFOV - 0.01f)
            {
                bFOV = false;
            }
        }

        if(bDist)
        {
            DIST = Mathf.Lerp(DIST, fTargetDist, Time.deltaTime * fDampTraceDist);
            if (DIST <= fTargetDist + 0.01f && DIST >= fTargetDist - 0.01f)
            {
                bDist = false;
            }
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

    public void ChangeFOV(float targetFOV, float Damp)
    {
        bFOV = true;
        this.fTargetFOV = targetFOV;
        this.fDampTraceFOV = Damp;
    }

    public void ChangeDist(float targetDist, float Damp)
    {
        bDist = true;
        this.fTargetDist = targetDist;
        this.fDampTraceDist = Damp;
    }

}
