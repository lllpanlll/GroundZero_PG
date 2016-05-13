using UnityEngine;
using System.Collections;

public class SupplyArea : MonoBehaviour {

    public float fRespawnTime = 1f;
    public GameObject oApCharger;
    GameObject oAp;
    public float x1, x2, z1, z2;
    ObjectPool apPool = new ObjectPool();

    // Use this for initialization
    void Start ()
    {
        x1 = transform.localPosition.x - x1;
        z1 = transform.localPosition.z - z1;
        x2 = transform.localPosition.x + x2;
        z2 = transform.localPosition.z + z2;
        apPool.CreatePool(oApCharger, 10);
        StartCoroutine(RespawnCycle());
    }
    
    IEnumerator RespawnCycle()
    {
        yield return new WaitForSeconds(fRespawnTime);
        // 생성 -> 이동 -> 활성
        // vPoint[0] = fixedCam.WorldToScreenPoint(new Vector3(
        // rend.bounds.center.x + rend.bounds.extents.x, 
        // rend.bounds.center.y + rend.bounds.extents.y, 
        // rend.bounds.center.z + rend.bounds.extents.z));

        oAp = apPool.UseObject();
        oAp.transform.position = new Vector3(Random.Range(x1, x2), 50, Random.Range(z1, z2));
        oAp.transform.rotation = Quaternion.identity;
        
        StartCoroutine(RespawnCycle());
    }
}
