using UnityEngine;
using System.Collections;

public class FollowMapCam : MonoBehaviour {

    public Transform trPlayerModel;
    //public Transform trMainCam;
    //public Transform trMmCam;
    public Transform trIcon;
    public float fDist = 20f;

    T2.MoveCtrl moveCtrl;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        moveCtrl = trPlayerModel.root.transform.GetComponent<T2.MoveCtrl>();
    }

    void Update () {
        transform.position = new Vector3(trPlayerModel.position.x, fDist, trPlayerModel.position.z);

        //transform.position = trPlayer.position - (trPlayer.forward * 0) + (transform.right * 0) + trPlayer.up * fDist;

        //trMmCam.LookAt((trPlayer.position + (trPlayer.right * 0)));

        //transform.rotation = Quaternion.Euler(0, trMainCam.eulerAngles.y, 0);
        trIcon.rotation = Quaternion.Euler(90, trPlayerModel.eulerAngles.y, 0);
        trIcon.position = new Vector3(transform.position.x, -5, transform.position.z);

        if (moveCtrl.GetMoveState().Equals(T2.MoveCtrl.MoveState.Sprint))
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 40, Time.deltaTime * 20);
        }
        else
        {
           cam.orthographicSize = 30;
        }
    }
}
