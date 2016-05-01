using UnityEngine;
using System.Collections;

namespace T2.Pref
{
    public class SetLifeTimer : T2.Pref.PrefLifeTimer
    {
        public float lifeTime = 3.0f;
                
        void OnEnable()
        {
            StartCoroutine(base.LifeTimer(this.gameObject, lifeTime));
        }
    }
}