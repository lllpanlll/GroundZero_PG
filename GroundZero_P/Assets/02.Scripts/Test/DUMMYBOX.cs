using UnityEngine;
using System.Collections;

public class DUMMYBOX : MonoBehaviour {

	
    void OnCollisionEnter(Collision coll)
    {
        if (coll.transform.CompareTag("Bullet"))
        {
            ScreenUI_DmgPopup.instance.HitUi(Random.Range(1, 20), coll.contacts[0].point);
        }
    }
}
