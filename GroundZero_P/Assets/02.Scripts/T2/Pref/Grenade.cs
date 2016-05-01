using UnityEngine;
using System.Collections;

namespace T2.Pref
{
    public class Grenade : T2.Pref.PrefLifeTimer
    {
        private CapsuleCollider capsuleCollider;
        public GameObject oExplosionEffect;

        private float lifeTime = 3.0f;
        void Awake()
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
            oExplosionEffect = (GameObject)Instantiate(oExplosionEffect);
            oExplosionEffect.SetActive(false);
        }

        void OnEnable()
        {
            StartCoroutine(LifeTimer(this.gameObject, lifeTime));
                
            oExplosionEffect = (GameObject)Instantiate(oExplosionEffect);
            oExplosionEffect.SetActive(false);
            oExplosionEffect.transform.position = transform.position;

            capsuleCollider.enabled = true;
        }

        protected override IEnumerator LifeTimer(GameObject obj, float time)
        {
            yield return new WaitForSeconds(time);
            oExplosionEffect.SetActive(true);
            obj.SetActive(false);
        }

        void OnDisable()
        {
            capsuleCollider.enabled = false;
        }

        void OnCollisionEnter(Collision col)
        {
            //폭파 이펙트 실행
            //T_attack.SetFlareEffect(col.contacts[0].point);


            if (col.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
            {
                if (col.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
                {
                    col.collider.gameObject.GetComponent<MonsterHitCtrl>().OnHitMonster(50);
                    oExplosionEffect.SetActive(true);
                    gameObject.SetActive(false);
                }

            }
            oExplosionEffect.SetActive(true);
            gameObject.SetActive(false);

        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
            {
                if (col.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
                {
                    col.gameObject.GetComponent<MonsterHitCtrl>().OnHitMonster(50);
                    oExplosionEffect.SetActive(true);
                    gameObject.SetActive(false);
                }

            }
            oExplosionEffect.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}