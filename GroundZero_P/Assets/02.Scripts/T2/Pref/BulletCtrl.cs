using UnityEngine;
using System.Collections;

namespace T2.Pref
{
    public class BulletCtrl : T2.Pref.PrefLifeTimer
    {

        public float fSpeed = 100.0f;

        public float lifeTime = 3.0f;
        
        private T2.BasicAttack basicAttack;

        void Awake()
        {
            basicAttack = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.BasicAttack>();
        }

        void OnEnable()
        {
            StartCoroutine(base.LifeTimer(this.gameObject, lifeTime));
        }

        void OnCollisionEnter(Collision col)
        {
            if (col.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
            {
                if (col.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
                {
                    col.collider.gameObject.GetComponent<M_HitCtrl> ().OnHitMonster(10, false);
                }

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