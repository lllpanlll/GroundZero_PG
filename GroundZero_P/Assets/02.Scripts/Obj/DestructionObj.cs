using UnityEngine;
using System.Collections;

public class DestructionObj : MonoBehaviour
{

    Light light;

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.name.Equals("Hit_me"))
        {
            Vector3 _vPoint, _vNormal;
            _vPoint = coll.contacts[0].point;
            _vNormal = coll.contacts[0].normal;

            coll.transform.GetComponent<Rigidbody>().AddForceAtPosition(_vNormal * 1000f, _vPoint);
            //coll.transform.root.gameObject.SetActive(false);
            light = coll.transform.parent.GetComponentInChildren<Light>();
            light.enabled = false;

            StartCoroutine(destruct(coll.gameObject));
        }
    }

    IEnumerator destruct(GameObject coll)
    {
        yield return new WaitForSeconds(5f);
        coll.transform.parent.gameObject.SetActive(false);
    }
}
