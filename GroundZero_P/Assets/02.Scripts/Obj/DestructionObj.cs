using UnityEngine;
using System.Collections;

public class DestructionObj : MonoBehaviour
{

    Light light;

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("DestructionObj"))
        {
            Vector3 _vPoint, _vNormal;
            _vPoint = coll.contacts[0].point;
            _vNormal = coll.contacts[0].normal;

            coll.transform.GetComponent<Rigidbody>().AddForceAtPosition(_vNormal * 500f, _vPoint);
            coll.transform.root.gameObject.SetActive(false);
            //light = coll.transform.GetComponentInChildren<Light>();
            //light.enabled = false;

            StartCoroutine(destruct(coll.gameObject));
        }
    }

    IEnumerator destruct(GameObject coll)
    {
        yield return new WaitForSeconds(2f);
        coll.SetActive(false);
    }
}
