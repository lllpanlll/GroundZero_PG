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
    Text textValue = null;

    public static ScreenUI_DmgPopup instance = null;

    void Awake()
    {
        instance = this;
    }

	// Use this for initialization
	void Start ()
    {
        damagePool.CreatePool(oDamageText, 10);
        damagePool.BeChilldren(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        WorldToScreen = Camera.main.WorldToScreenPoint(oTarget.transform.position);
        transform.position = new Vector3(WorldToScreen.x, WorldToScreen.y, transform.position.z);
    }

    public void HitUi(int _damage)
    {
        oDamageUi = damagePool.UseObject();
        //oDamageUi.transform.SetParent( transform ); // 캔버스 안에 들어가야 함.
        textValue = oDamageUi.GetComponent<Text>();
        oDamageUi.transform.localPosition = Vector3.zero;
        oDamageUi.transform.localRotation = transform.localRotation;
        oDamageUi.transform.localScale = transform.localScale; // 안전장치.

        textValue.text = _damage.ToString();
    }
}
