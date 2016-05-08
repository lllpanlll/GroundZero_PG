using UnityEngine;
using System.Collections;

namespace T2.Pref
{
    public class DimensionBallPref : T2.Pref.PrefLifeTimer
    {
        public float lifeTime = 3.0f;

        private float fSpeed;
        private int iDamage;
        private float fReach;
        private Vector3 vStartPos;
        private float fOrizinRadius;
        private float fExplosionRange = 3.0f;

        public GameObject oExplosion;

        private SphereCollider spherCollider;
        private MeshRenderer meshRenderer;

        void Start()
        {
            fSpeed = T2.Skill.DimensionBall.GetInstance().fBallSpeed;
            iDamage = T2.Skill.DimensionBall.GetInstance().iDamage;
            fReach = T2.Skill.DimensionBall.GetInstance().fReach;
            vStartPos = transform.position;
            fOrizinRadius = spherCollider.radius;

            oExplosion = (GameObject)Instantiate(oExplosion);
            oExplosion.SetActive(false);
        }

        void OnEnable()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            spherCollider = GetComponent<SphereCollider>();
            meshRenderer.enabled = true;
            vStartPos = GameObject.FindGameObjectWithTag(Tags.Player).transform.position;
            print(vStartPos);

            oExplosion.SetActive(false);

            StartCoroutine(LifeTimer(this.gameObject, lifeTime));
        }

        void OnCollisionEnter(Collision col)
        {
            if (col.collider.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
            {
                //데미지와 경직을 준다.
                col.collider.gameObject.GetComponent<M_HitCtrl>().OnHitMonster(iDamage, true);
            }
            gameObject.SetActive(false);
        }

        void FixedUpdate()
        {
            //사정거리 이내에서만 이동, 사정거리에 도착시 정지.
            float curDist = Vector3.Distance(vStartPos, transform.position);
            if (curDist < fReach)
            {
                transform.Translate(Vector3.forward * fSpeed * Time.fixedDeltaTime, Space.Self);
            }
            
        }

        protected override IEnumerator LifeTimer(GameObject obj, float time)
        {
            yield return new WaitForSeconds(time);
            meshRenderer.enabled = false;
            spherCollider.radius = fExplosionRange;           
            StartCoroutine(ExplosionTimer());
        }

        IEnumerator ExplosionTimer()
        {
            oExplosion.transform.position = transform.position;
            oExplosion.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            spherCollider.radius = fOrizinRadius;
            //StartCoroutine(ExplosionDisableTimer());

            gameObject.SetActive(false);
        }

        //IEnumerator ExplosionDisableTimer()
        //{
        //    yield return new WaitForSeconds(1.5f);
        //    oExplosion.SetActive(false);
        //    gameObject.SetActive(false);
            
        //}
        
    }
}
