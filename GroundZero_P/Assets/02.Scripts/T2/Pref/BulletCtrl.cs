using UnityEngine;
using System.Collections;

namespace T2.Pref
{
    public class BulletCtrl : T2.Pref.PrefLifeTimer
    {

        public float fSpeed = 100.0f;
        public float lifeTime = 3.0f;

        private float fReach = 0.0f;
        private Vector3 vStartPos;
        
        private T2.BasicAttack basicAttack;
        private T2.Manager mgr;
        private CapsuleCollider capsuleCollider;

        private bool bMove = true;

        void Awake()
        {
            basicAttack = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.BasicAttack>();
            mgr = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.Manager>();
            capsuleCollider = GetComponent<CapsuleCollider>();
        }

        void OnEnable()
        {
            bMove = true;
            fReach = basicAttack.GetReach();
            vStartPos = transform.position;
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


        void Update()
        {
            #region<이동처리 Ray>
            //투사체 이동시 장애물 체크
            Ray[] ray = new Ray[2];
            ray[0] = new Ray(transform.position - transform.right * capsuleCollider.radius, transform.forward);
            ray[1] = new Ray(transform.position + transform.right * capsuleCollider.radius, transform.forward);
            RaycastHit hit;
            int mask = (
                        (1 << LayerMask.NameToLayer(Layers.T_HitCollider)) | (1 << LayerMask.NameToLayer(Layers.Bullet)) |
                        (1 << LayerMask.NameToLayer(Layers.T_Invincibility)) | (1 << LayerMask.NameToLayer(Layers.Except_Monster)) |
                        (1 << LayerMask.NameToLayer(Layers.MonsterHitCollider)) | (1 << LayerMask.NameToLayer(Layers.AlleyTrigger))
                        );
            mask = ~mask;
            for (int i = 0; i < 2; i++)
            {
                float fDist = fSpeed * Time.deltaTime;
                Debug.DrawLine(ray[i].origin, ray[i].GetPoint(fDist), Color.cyan);
                if (Physics.Raycast(ray[i], out hit, fDist, mask))
                {
                    if (!bMove)
                    {
                        Vector3 vHitPos = Vector3.zero;
                        if (i == 0)
                            vHitPos = hit.point + transform.right * capsuleCollider.radius;
                        else
                            vHitPos = hit.point - transform.right * capsuleCollider.radius;

                        StartCoroutine(AccuracyPosition(vHitPos));
                    }
                    bMove = false;
                    break;
                }
            }
            #endregion

            float fCurDist = Vector3.Distance(vStartPos, transform.position);
            if (fCurDist > fReach)
            {
                gameObject.SetActive(false);
            }
            if(bMove)
                transform.Translate(Vector3.forward * fSpeed * Time.fixedDeltaTime, Space.Self);
        }
        IEnumerator AccuracyPosition(Vector3 pos)
        {
            yield return new WaitForEndOfFrame();
            transform.position = pos - (transform.forward * capsuleCollider.radius * 1.5f);
        }
    }
}