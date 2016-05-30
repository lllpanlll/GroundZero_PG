using UnityEngine;
using System.Collections;

public class ScreenUI_DamageForText : MonoBehaviour {

	float fTimer = 0;
    public float fLifeTime = 1;
    public float fSpeed = 200;

    void Update () {

		transform.Translate (Vector2.up * Time.deltaTime * fSpeed);

        fTimer += Time.deltaTime;
		if (fTimer > fLifeTime) {
			gameObject.SetActive (false);
            fTimer = 0;
		}
	}
}
