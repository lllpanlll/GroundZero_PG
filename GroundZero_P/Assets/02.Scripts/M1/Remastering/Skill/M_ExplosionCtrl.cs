using UnityEngine;
using System.Collections;

public class M_ExplosionCtrl : MonoBehaviour {
    
	void OnEnable ()
    {
        StartCoroutine(BOOM());
    }

    //폭발
    IEnumerator BOOM()
    {
        yield return new WaitForSeconds(0.8f);

        gameObject.SetActive(false);
    }
}
