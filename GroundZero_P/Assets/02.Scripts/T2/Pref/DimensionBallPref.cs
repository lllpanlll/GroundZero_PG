using UnityEngine;
using System.Collections;

namespace T2.Pref
{
    public class DimensionBallPref : T2.Pref.PrefLifeTimer
    {
        private float lifeTime = 3.0f;

        private float fSpeed;
        private int iDamage;
        private float fReach;
        private Vector3 vStartPos;
        private float fOrizinRadius;
        private float fExplosionRange = 3.0f;
        private bool bMove = true;
        private bool bPlayerMove = false;

        public GameObject oExplosion;

        private SphereCollider spherCollider;
        private MeshRenderer meshRenderer;

        private T2.Manager mgr;
        private CharacterController controller;
        private Transform trPlayer;
        private Vector3 moveDir;

        void Awake()
        {
            mgr = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.Manager>();
            trPlayer = GameObject.FindGameObjectWithTag(Tags.Player).transform;
            controller = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<CharacterController>();

            lifeTime = T2.Skill.DimensionBall.GetInstance().coolTime - 0.2f;
            fSpeed = T2.Skill.DimensionBall.GetInstance().fBallSpeed;
            iDamage = T2.Skill.DimensionBall.GetInstance().iDamage;            
            vStartPos = transform.position;
            //fOrizinRadius = spherCollider.radius;            
           
            oExplosion = (GameObject)Instantiate(oExplosion);
            oExplosion.SetActive(false);
        }

        void OnEnable()
        {            
            meshRenderer = GetComponent<MeshRenderer>();
            spherCollider = GetComponent<SphereCollider>();
            spherCollider.radius = 0.5f;
            fOrizinRadius = spherCollider.radius;
            meshRenderer.enabled = true;
            fReach = T2.Skill.DimensionBall.GetInstance().fReach;
            vStartPos = GameObject.FindGameObjectWithTag(Tags.Player).transform.position;
            bMove = true;
            bPlayerMove = false;
            spherCollider.radius = fOrizinRadius;

            oExplosion.SetActive(false);

            //디멘션볼 스킬의 쿨타임을 재설정한다.
            //디멘션볼 스킬은 투사체가 없으면 쿨이 없는것?!        
            T2.Skill.DimensionBall.GetInstance().StopCoroutine(T2.Skill.DimensionBall.GetInstance().CoolTimer(lifeTime));
            T2.Skill.DimensionBall.GetInstance().bCoolTime = true;
            StartCoroutine(LifeTimer(this.gameObject, lifeTime));
        }

        void OnDisable()
        {
            T2.Skill.DimensionBall.GetInstance().bCoolTime = false;
            T2.Skill.DimensionBall.GetInstance().fReach = T2.Skill.DimensionBall.GetInstance().fMaxReach;
        }


        void OntriggerEnter(Collider col)
        {
            //콜리더의 지름이 폭발 지름이상인 경우에만 보스에게 데미지를 준다.
            if (spherCollider.radius >= fExplosionRange)
            {
                //데미지와 경직을 준다.          
                if (col.gameObject.layer == LayerMask.NameToLayer(Layers.MonsterHitCollider))
                {
                    col.gameObject.GetComponent<M_HitCtrl>().OnHitMonster(iDamage, true);
                    gameObject.SetActive(false);
                }
            }
        }

        void FixedUpdate()
        {
            //투사체 이동시 장애물 체크
            Ray[] ray = new Ray[3];
            ray[0] = new Ray(transform.position, transform.forward);
            ray[1] = new Ray(transform.position - transform.right * spherCollider.radius, transform.forward);
            ray[2] = new Ray(transform.position + transform.right * spherCollider.radius, transform.forward);
            RaycastHit hit;
            int mask = (1 << LayerMask.NameToLayer(Layers.T_HitCollider)) | (1 << LayerMask.NameToLayer(Layers.Bullet)) |
                (1 << LayerMask.NameToLayer(Layers.T_Invincibility) | (1 << LayerMask.NameToLayer(Layers.Except_Monster)));
            mask = ~mask;
            for (int i = 0; i < 3; i++)
            {
                float fDist = fSpeed * Time.fixedDeltaTime;
                Debug.DrawLine(ray[i].origin, ray[i].GetPoint(fDist), Color.cyan);
                if (Physics.Raycast(ray[i], out hit, fDist, mask))
                {
                    //최상위 오브젝트의 태그가 몬스터인지 체크한다.
                    if (hit.collider.transform.root.tag != Tags.Monster)
                    {
                        bMove = false;
                        Vector3 vHitPos = Vector3.zero;
                        if (i == 1)
                            vHitPos = hit.point + transform.right * spherCollider.radius;
                        else if (i == 2)
                            vHitPos = hit.point - transform.right * spherCollider.radius;
                        else
                            vHitPos = hit.point;
                        StartCoroutine(AccuracyPosition(vHitPos));
                    }
                    break;
                }
            }

            //사정거리 이내에서만 이동, 사정거리에 도착시 정지.
            float curDist = Vector3.Distance(vStartPos, new Vector3(transform.position.x, 0.0f, transform.position.z));
            if (curDist < fReach && bMove == true)
            {
                transform.Translate(Vector3.forward * fSpeed * Time.fixedDeltaTime, Space.Self);
            }


            //스킬키와 동일한 키 입력시 해당 오브젝트로 플레이어를 이동시킨다.
            if (Input.GetKeyDown(KeyCode.E))
            {
                if(mgr.GetState() != Manager.State.Skill && mgr.GetState() != Manager.State.be_Shot)
                {
                    if(T2.Skill.SilverStream.GetInstance().bSilverStream == true)
                    {
                        bMove = false;
                        bPlayerMove = true;
                        mgr.ChangeState(T2.Manager.State.Skill);
                        mgr.SetLayerState(Manager.LayerState.invincibility);                        
                        StopAllCoroutines();
                    }
                }
            }
            if(bPlayerMove == true)
            {
                moveDir = (transform.position + transform.up * 1.5f )- trPlayer.position;
                controller.Move(moveDir * 5.0f * Time.deltaTime);
                if(Vector3.Distance(trPlayer.position, transform.position) < 1.5f )
                {
                    bPlayerMove = false;
                    mgr.ChangeState(T2.Manager.State.idle);
                    mgr.SetLayerState(Manager.LayerState.normal);
                    //디멘션볼 스킬의 쿨타임을 끝낸다.
                    T2.Skill.DimensionBall.GetInstance().bCoolTime = false;
                    gameObject.SetActive(false);
                }
            }

            if(Input.GetMouseButtonDown(1))
            {
                if (mgr.GetState() != Manager.State.Skill && mgr.GetState() != Manager.State.be_Shot)
                {
                    StopAllCoroutines();
                    meshRenderer.enabled = false;
                    spherCollider.radius = fExplosionRange;
                    StartCoroutine(ExplosionTimer());
                }
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
            //디멘션볼 스킬의 쿨타임을 끝낸다.
            T2.Skill.DimensionBall.GetInstance().bCoolTime = false;
            gameObject.SetActive(false);
        }

        IEnumerator AccuracyPosition(Vector3 pos)
        {

            yield return new WaitForEndOfFrame();
            transform.position = pos - (transform.forward * spherCollider.radius);

        }
    }
}
