using UnityEngine;
using System.Collections;

namespace T2.Pref
{
    public class PrefLifeTimer : MonoBehaviour
    {
        protected virtual IEnumerator LifeTimer(GameObject obj, float time)
        {
            yield return new WaitForSeconds(time);
            obj.SetActive(false);
        }
    }
}