using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Alley Gate Trigger
/// 
/// *코멘트
/// </summary>



public class O_AlleyGateTrigger : MonoBehaviour {

    public Transform gateForwardPos;                                            //골목입구 앞 포인트 위치
    public Transform inCornerPos;                                               //골목 안 코너 위치
    

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag.Equals(Tags.Player))                            //플레이어 트리거 진입 시
        {
            if (inCornerPos)    //코너가 존재하면
            {
                //게이트(자신) 위치, 게이트 앞 포인트 위치, 골목 안쪽 코너 위치  몬스터에게 전달
                M_Alley.instance.CheckAlleying(gameObject, transform.position, gateForwardPos.position, inCornerPos.position);  
            }
            else
            {
                //게이트(자신) 위치, 게이트 앞 포인트 위치  몬스터에게 전달
                M_Alley.instance.CheckAlleying(gameObject, transform.position, gateForwardPos.position, Vector3.zero);
            }
        }
    }
}
