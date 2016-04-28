using UnityEngine;
using System.Collections;

public class Magic_1_Ctrl : MonoBehaviour {

    private float speed;
    private Vector3 trToPlayerVector;
    private Transform playerTr;

    void Start()
    {
        speed = GameObject.FindWithTag(Tags.Monster).GetComponent<M_FSMTest>().magic_1_Speed;
        playerTr = GameObject.FindWithTag(Tags.Player).GetComponent<Transform>();

        StartCoroutine(this.ShootMagic_1());
    }
    
    IEnumerator ShootMagic_1()
    {
        yield return new WaitForSeconds(1.0f);

        trToPlayerVector = Vector3.Normalize(playerTr.position - transform.position);

        GetComponent<Rigidbody>().AddForce(trToPlayerVector * speed);

        Destroy(gameObject, 2.0f);
    }
}
