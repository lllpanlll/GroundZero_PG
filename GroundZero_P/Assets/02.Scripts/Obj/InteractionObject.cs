using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InteractionObject : MonoBehaviour {

    T2.Manager mgr;
    public enum KindOfObj { Ap, Dp}
    public KindOfObj kindObj;
    float fSpeed = 20f;

    public int iMaxApCapacity = 1000;
    public int iMaxDpCapacity = 100;
    int iApCapacity;
    int iDpCapacity;
    public int iIncreaseDp = 1;
    bool bOutDp = false;

    // ui
    //public GameObject oUi;
    //Vector3 WorldToScreen;

    //Transform trPlayer;
    //Text textUi;

    //GameObject oInstant;
    //public Transform trScreenCanvas;

 //   void Awake()
 //   {
 //       //ui
 //       trPlayer = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<Transform>();
 //   }

	//// Use this for initialization
	//void Start () {
 //       // ui
 //       //trScreenCanvas = transform.Find("Screen_Canvas").GetComponent<Transform>();
 //       oInstant = Instantiate(oUi) as GameObject;
 //       textUi = oInstant.GetComponentInChildren<Text>();
 //       oInstant.transform.SetParent(trScreenCanvas);
 //   }

 //   void Update()
 //   {
 //       // ui
 //       WorldToScreen = Camera.main.WorldToScreenPoint(transform.position);
 //       if (WorldToScreen.z > 0)
 //       {
 //           oInstant.SetActive(true);
 //       }
 //       else
 //           oInstant.SetActive(false);
 //       oInstant.transform.position = new Vector3(WorldToScreen.x, WorldToScreen.y, oInstant.transform.position.z);

 //       float fDist = Vector3.Distance(trPlayer.position, transform.position);

 //       textUi.text = ((int)fDist).ToString() + "m";
 //   }


    void OnEnable()
    {
        StartCoroutine(FallObject());
    }

    void OnDisable()
    {
        iApCapacity = iMaxApCapacity;
        iDpCapacity = iMaxDpCapacity;
    }

    IEnumerator FallObject()
    {
        bool _bChk = true;
        while(_bChk)
        {
            Ray _ray = new Ray(transform.position, -Vector3.up);
            RaycastHit _hit = new RaycastHit();
            float _fDist = fSpeed * Time.deltaTime;

            transform.Translate(-Vector3.up * fSpeed * Time.deltaTime);

            if(Physics.Raycast(_ray, out _hit, _fDist, 1 << 14))
            {
                transform.position = _hit.point + Vector3.up;
                _bChk = false;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag(Tags.Player))
        {
            mgr = coll.GetComponent<T2.Manager>();
            switch (kindObj)
            {
                case KindOfObj.Dp:

                    bOutDp = true;
                    StartCoroutine(ChagingDp(mgr));

                    break;
            }
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.CompareTag(Tags.Player))
        {
            switch (kindObj)
            {
                case KindOfObj.Ap:

                    int iNeedAp = Mathf.Clamp((T2.Stat.MAX_AP - mgr.GetAP()), 0, iApCapacity);
                    mgr.SetAP(mgr.GetAP() + iNeedAp);

                    iApCapacity -= iNeedAp;

                    if (iApCapacity.Equals(0))
                        gameObject.SetActive(false);
                    break;
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag(Tags.Player))
        {
            switch (kindObj)
            {
                case KindOfObj.Dp:

                    bOutDp = false;

                    break;
            }
        }
    }

    IEnumerator ChagingDp(T2.Manager _mgr)
    {
        while (bOutDp)
        {

            if (T2.Stat.MAX_DP > _mgr.GetDP() && iDpCapacity > 0)
            {
                _mgr.SetDP(_mgr.GetDP() + iIncreaseDp);
                iDpCapacity -= iIncreaseDp;
            }

            if (iDpCapacity.Equals(0))
                gameObject.SetActive(false);

            yield return new WaitForSeconds(0.1f);
        }
    }
}
