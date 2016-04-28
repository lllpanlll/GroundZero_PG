using UnityEngine;
using System.Collections;

public class Magic_2_Ctrl : MonoBehaviour {

    private float speed;

    void Start()
    {
        speed = GameObject.FindWithTag(Tags.Monster).GetComponent<M_FSMTest>().magic_2_Speed;

        StartCoroutine(this.ShootMagic_2());
    }
    
    IEnumerator ShootMagic_2()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);

        Destroy(gameObject, 2.0f);
    }

    //플레이어가 때리면... 박살나야 하는데..?!
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.gameObject.layer == LayerMask.NameToLayer(Layers.Bullet))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer(Layers.Bullet))
        {
            Destroy(gameObject);
        }
    }
}
