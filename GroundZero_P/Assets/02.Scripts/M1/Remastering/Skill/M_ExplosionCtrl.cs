using UnityEngine;
using System.Collections;

/// <summary>
/// 작성자                 JSH
/// Explosion Control
/// 폭발 피격처리 컨트롤. 유지시간 대기 후 사망.
/// 
/// *코멘트
/// </summary>



public class M_ExplosionCtrl : MonoBehaviour {
    
	void OnEnable ()
    {
        StartCoroutine(BOOM());
    }

    //폭발
    IEnumerator BOOM()
    {
        yield return new WaitForSeconds(0.8f);

        gameObject.SetActive(false);
    }
}
