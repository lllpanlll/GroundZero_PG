using UnityEngine;
using System.Collections;

public class ToDrive : MonoBehaviour {

    // 테스트 노트. 발판에 가면 슝~
    public Transform tDest;
    public float fMovingTime = 5f;

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == (Tags.Player))
        {
            coll.transform.position = transform.position;
            coll.transform.parent = gameObject.transform;
            StartCoroutine(FlyBoard(coll));
        }
    }

    IEnumerator FlyBoard(Collision coll)
    {
        float fTimer = fMovingTime;
        float fDist = Vector3.Distance(transform.position, tDest.position);
        while (true)
        {
            fTimer -= Time.deltaTime;
            if(fTimer <= 0f)
            {
                coll.transform.parent = null;
                break;
            }
            transform.Translate(Vector3.forward * (fDist/ fMovingTime) * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }
        yield return null;
    }
}
