using UnityEngine;
using System.Collections;

namespace T2.Pref
{
    /// <summary>
    /// 2016-05-04
    /// 게임 진행중 생성되는 오브젝트의 생존시간 함수를 상속시켜줄 클래스
    /// 생존시간이 필요한 오브젝트는 이 클래스를 상속받는다.
    /// 
    /// 코루틴을 가상함수로 만드는것은 원하는 방식으로 되지 않기 때문에 인터페이스로 수정 할 예정
    /// </summary>
    public class PrefLifeTimer : MonoBehaviour
    {
        protected virtual IEnumerator LifeTimer(GameObject obj, float time)
        {
            yield return new WaitForSeconds(time);
            obj.SetActive(false);
        }
    }
}