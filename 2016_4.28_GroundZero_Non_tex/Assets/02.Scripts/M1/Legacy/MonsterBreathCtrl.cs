using UnityEngine;
using System.Collections;

public class MonsterBreathCtrl : MonoBehaviour {

    private float speed;

    void Start()
    {
        speed = GameObject.FindWithTag(Tags.Monster).GetComponent<MonsterCtrl>().breathAttkSpeed;

        StartCoroutine(this.ShootBreathAttk());
    }

    #region <발사 후 삭제>
    IEnumerator ShootBreathAttk()
    {
        yield return new WaitForSeconds(0.15f);

        GetComponent<Rigidbody>().AddForce(transform.forward * speed);

        Destroy(gameObject, 2.0f);
    }
    #endregion
}
