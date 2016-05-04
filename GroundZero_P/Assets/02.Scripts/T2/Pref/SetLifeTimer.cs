using UnityEngine;
using System.Collections;

namespace T2.Pref
{
    /// <summary>
    ///  2016-05-04
    ///  다른 구조가 필요하지 않은 생존시간만 구현된 클래스
    /// </summary>
    public class SetLifeTimer : T2.Pref.PrefLifeTimer
    {
        public float lifeTime = 3.0f;
                
        void OnEnable()
        {
            StartCoroutine(base.LifeTimer(this.gameObject, lifeTime));
        }
    }
}