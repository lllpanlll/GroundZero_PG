using UnityEngine;
using System.Collections;

public class SupplyArea : MonoBehaviour {

    const int COUNT = 10;
    int iCount;

    // 리스폰 시간
    public float fRespawnTime = 1f;
    public GameObject oApCharger;
    GameObject oAp;
    public float x1, x2, z1, z2;
    ObjectPool apPool = new ObjectPool();
    Transform[] trSpawnpoint = new Transform[COUNT];

    void Awake()
    {
        trSpawnpoint[0] = GetComponent<Transform>();
        trSpawnpoint = trSpawnpoint[0].GetComponentsInChildren<Transform>();
    }

    // Use this for initialization
    void Start ()
    {
        x1 = transform.localPosition.x - x1;
        z1 = transform.localPosition.z - z1;
        x2 = transform.localPosition.x + x2;
        z2 = transform.localPosition.z + z2;
        apPool.CreatePool(oApCharger, COUNT);
        StartCoroutine(RespawnCycle());
    }

    IEnumerator RespawnCycle()
    {
        yield return new WaitForSeconds(fRespawnTime);
        // 생성 -> 이동 -> 활성

        //if (iCount < COUNT)
        {
            iCount++;
            oAp = apPool.UseObject();
            int i = Random.Range(1, trSpawnpoint.Length);
            oAp.transform.position = new Vector3(trSpawnpoint[i].position.x, 50, trSpawnpoint[i].position.z);
            oAp.transform.rotation = Quaternion.identity;
        }

        // random
        //oAp.transform.position = new Vector3(Random.Range(x1, x2), 50, Random.Range(z1, z2));
        //oAp.transform.rotation = Quaternion.identity;
        StartCoroutine(RespawnCycle());
    }
}
