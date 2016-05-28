using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenUI_DmgPopup : MonoBehaviour {

    // 나는 damage panel 이다.

    public GameObject oTarget;
    //Text textUi;
    Vector3 WorldToScreen;

    ObjectPool damagePool = new ObjectPool();
    public GameObject oDamageText;
    GameObject oDamageUi;
    RawImage[] rawValue = new RawImage[3];
    public int iInterval = 30; // 간격
    int iCount = 1; // 현재 자릿수

    public static ScreenUI_DmgPopup instance = null;

    public int TESTCHECK() { return iCount; }

    void Awake()
    {
        instance = this;
    }

	// Use this for initialization
	void Start ()
    {
        damagePool.CreatePool(oDamageText, 20);
        damagePool.BeChilldren(gameObject); // 캔버스 안으로 옮김
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = Camera.main.WorldToScreenPoint(oTarget.transform.position);
    }

    public void HitUi(int _damage, Vector3 hitPos)
    {
        oDamageUi = damagePool.UseObject();
        
        rawValue = oDamageUi.GetComponentsInChildren<RawImage>();

        oDamageUi.transform.position = hitPos;
        oDamageUi.transform.position = Camera.main.WorldToScreenPoint(hitPos); // hitPos
        oDamageUi.transform.localRotation = transform.localRotation;
        oDamageUi.transform.localScale = transform.localScale; // 안전장치.

        int iLength = 0;
        int iChk = 1;
        while(_damage >= iChk)
        {
            iLength++;
            iChk *= 10;
        }
        iCount = iLength;

        float fmul = 1;
        for(int i = 0; i < iCount; i++)
        {
            rawValue[i].uvRect = new Rect(0.1f * ((int)(_damage * fmul) % 10), 0, 0.1f, 1);
            fmul *= 0.1f;
        }

        // 나열
        transform.localPosition = new Vector3(
            iInterval * (iCount) * 0.5f - iInterval * 0.5f,
            transform.localPosition.y,
            transform.localPosition.z);
        for (int i = iCount; i < rawValue.Length; i++)
        {
            if (rawValue[i].enabled.Equals(true))
                rawValue[i].enabled = false;
        }
        for (int i = 0; i < iCount; i++)
        {
            if (rawValue[i].enabled.Equals(false))
                rawValue[i].enabled = true;
            rawValue[i].rectTransform.localPosition = new Vector3(
                transform.localPosition.x - (iInterval * (i)),
                rawValue[i].rectTransform.localPosition.y,
                rawValue[i].rectTransform.localPosition.z);
        }
    }
}
