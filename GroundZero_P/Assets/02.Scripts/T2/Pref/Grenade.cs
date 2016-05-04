using UnityEngine;
using System.Collections;

namespace T2.Pref
{
    /// <summary>
    /// 2016-05-04
    /// 캐릭터의 EvastionCounter스킬로 인해 생성되는 오브젝트
    /// 무언가 부딪히면 폭발한다.
    /// 보스가 부딪히면 보스에게 대미지를 주며 폭발한다.
    /// </summary>
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