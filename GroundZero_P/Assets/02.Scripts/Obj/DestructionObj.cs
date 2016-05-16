using UnityEngine;
using System.Collections;

public class DestructionObj : MonoBehaviour {


    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag(Tags.Player))
        {
            Vector3 _vPoint, _vNormal;
            _vPoint = coll.contacts[0].point;
            _vNormal = coll.contacts[0].normal;

            GetComponent<Rigidbody>().AddForceAtPosition(_vNormal * 500f, _vPoint);
        }
    }

}
