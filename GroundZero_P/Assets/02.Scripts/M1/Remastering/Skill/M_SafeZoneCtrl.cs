using UnityEngine;
using System.Collections;

public class M_SafeZoneCtrl : MonoBehaviour {

    //플레이어 진입
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag.Equals(Tags.Player))                //트리거에 플레이어 진입 시
        {
            M_SafeZone.instance.IsPlayerInSafeZone = false;
        }
    }

    //플레이어 이탈
    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag.Equals(Tags.Player))                //트리거에 플레이어 이탈 시
        {
            M_SafeZone.instance.IsPlayerInSafeZone = true;
        }
    }
}
