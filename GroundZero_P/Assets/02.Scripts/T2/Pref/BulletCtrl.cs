using UnityEngine;
using System.Collections;

namespace T2.Pref
{
    public class BulletCtrl : T2.Pref.PrefLifeTimer
    {

        public float fSpeed = 100.0f;

        public float lifeTime = 3.0f;
        
        private T2.BasicAttack basicAttack;
        private T2.Manager mgr;

        void Awake()
        {
            basicAttack = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.BasicAttack>();
            mgr = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.Manager>();
        }

        void OnEnable()
        {
            StartCoroutine(base.LifeTimer(this.gameObject, lifeTime));
        }

        void OnCollisionEnter(Collision col)
        {
            if (col.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
            {
                col.collider.gameObject.GetComponent<M_HitCtrl> ().OnHitMonster(10, false);
                if(mgr.GetPP() < Stat.MAX_PP)
                    mgr.SetPP(mgr.GetPP() + 1);
            }

            basicAttack.SetFlareEffect(col.contacts[0].point);
            gameObject.SetActive(false);
        }

        void OnTriggerEnter(Collider coll)
        {
            gameObject.SetActive(false);
        }


        void FixedUpdate()
        {
            transform.Translate(Vector3.forward * fSpeed * Time.fixedDeltaTime, Space.Self);
        }
    }
}