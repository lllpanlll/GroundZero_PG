using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenUI_DamagePopUp : MonoBehaviour {

	ObjectPool damagePool = new ObjectPool();
	public GameObject oDamageText;
	public GameObject oDamagePanel;
    RectTransform trDamagePanel;
	GameObject oDamageUi;
	public GameObject oTarget;
	Text textValue = null;
    Vector3 WorldToScreen = Vector3.zero;

    void Awake()
    {
        trDamagePanel = oDamagePanel.GetComponent<RectTransform>();
    }

	// Use this for initialization
	void Start ()
	{
		damagePool.CreatePool(oDamageText, 10);

    }

	void Upadate()
	{
        WorldToScreen = Camera.main.WorldToScreenPoint (oTarget.transform.position);
		if (WorldToScreen.z > 0) {
			oDamagePanel.SetActive (true);
		} else
			oDamagePanel.SetActive (false);
		trDamagePanel.transform.position = new Vector3 (WorldToScreen.x, WorldToScreen.y, oDamagePanel.transform.position.z);
	}

	void OnCollisionEnter(Collision coll)
	{
		if (coll.gameObject.CompareTag("Bullet"))
		{
			HitUi(Random.Range(1, 999));
		}
	}

	void HitUi(int _damage)
	{
		oDamageUi = damagePool.UseObject ();
        //oDamageUi.transform.SetParent(oDamagePanel.transform);// 캔버스 안에 들어가야 함.
        textValue = oDamageUi.GetComponent<Text> ();
		oDamageUi.transform.localPosition = oDamagePanel.transform.localPosition;
		oDamageUi.transform.localRotation = oDamagePanel.transform.localRotation;
		oDamageUi.transform.localScale = oDamagePanel.transform.localScale; // 안전장치.

		textValue.text = _damage.ToString ();
	}
}
