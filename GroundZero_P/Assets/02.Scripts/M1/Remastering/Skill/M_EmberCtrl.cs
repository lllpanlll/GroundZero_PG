using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Ember Control
/// 잔불 유지 컨트롤. 잔불이 생성될 때 지정된 유지시간만큼만 살아있다.
/// 
/// *코멘트
/// </summary>



public class M_EmberCtrl : MonoBehaviour
{
    private float curTime = 2.0f;        //잔불 유지시간
    public float CurTime { set{ curTime = value; } }

    private bool isStart = false;       //초기화 여부


    
    //OnEnable
    void OnEnable()
    {
        StartCoroutine(WaitForDestroy());
    }

    //유지시간만큼 사망 대기
    IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(0.03f);     //curTime이 외부에서 무사히 설정되길 대기

        yield return new WaitForSeconds(curTime);

        gameObject.SetActive(false);
    }
}
