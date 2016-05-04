using UnityEngine;
using System.Collections;

namespace T2.Pref
{
    /// <summary>
    /// 2016-05-04
    /// 보스의 경직을 테스트하기 위한 오브젝트
    /// 보스와 부딪히면 일정 대미지와 경직을 체크해 준다.
    /// </summary>
    public class TestBall : T2.Pref.PrefLifeTimer
    {

        public float fSpeed = 30.0f;

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
                if (col.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
                {
                    col.collider.gameObject.GetComponent<M_HitCtrl> ().OnHitMonster(10, true);
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