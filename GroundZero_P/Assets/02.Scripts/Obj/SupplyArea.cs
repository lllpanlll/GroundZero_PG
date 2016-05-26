using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SupplyArea : MonoBehaviour
{
    // 풀 개수.
    const int COUNT = 1;

    // 리스폰 시간.
    float fRespawnTime = 2f;

    public GameObject oCharger;
    GameObject oObj;
    public float x1, x2, z1, z2;
    ObjectPool objPool = new ObjectPool();
    Transform[] trSpawnpoint = new Transform[COUNT];
    [HideInInspector]
    public List<GameObject> listObjCount;

    public Image imageUi;
    GameObject oUi;
    public Transform trScreenUi;
    ScreenUI_DisanceObject distUi;

    void Awake()
    {
        trSpawnpoint[0] = GetComponent<Transform>();
        trSpawnpoint = trSpawnpoint[0].GetComponentsInChildren<Transform>();
    }

    // Use this for initialization
    void Start()
    {
        x1 = transform.localPosition.x - x1;
        z1 = transform.localPosition.z - z1;
        x2 = transform.localPosition.x + x2;
        z2 = transform.localPosition.z + z2;
        objPool.CreatePool(oCharger, COUNT);
        oUi = Instantiate(imageUi.gameObject) as GameObject;
        oUi.transform.SetParent(trScreenUi);
        distUi = oUi.GetComponent<ScreenUI_DisanceObject>();
        oUi.SetActive(false);
        StartCoroutine(RespawnCycle());
    }

    IEnumerator RespawnCycle()
    {
        yield return new WaitForSeconds(fRespawnTime);

        if(listObjCount.Count < 1)
        {
            oObj = objPool.UseObject();
            int i = Random.Range(1, trSpawnpoint.Length);
            oObj.transform.position = new Vector3(trSpawnpoint[i].position.x, 50, trSpawnpoint[i].position.z);
            oObj.transform.rotation = Quaternion.identity;

            oUi.SetActive(true);
            distUi.SetTaget(oObj);

            listObjCount.Add(oObj);

        }
        else
        {
            if(!oObj.activeSelf)
            listObjCount.Remove(oObj);
        }

        // random
        //oAp.transform.position = new Vector3(Random.Range(x1, x2), 50, Random.Range(z1, z2));
        //oAp.transform.rotation = Quaternion.identity;
        StartCoroutine(RespawnCycle());
    }
}
