using UnityEngine;
using System.Collections;

namespace T2.Pref
{
    public class DimensionBallPref : T2.Pref.PrefLifeTimer
    {
        private float lifeTime = 3.0f;

        private float fSpeed, fPlayerSpeed = 100.0f;
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
        private float fReachOfBall;

        void Awake()
        {
            mgr = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.Manager>();
            trPlayer = GameObject.FindGameObjectWithTag(Tags.Player).transform;
            controller = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<CharacterController>();

            
            fSpeed = T2.Skill.DimensionBall.GetInstance().fBallSpeed;
            iDamage = T2.Skill.DimensionBall.GetInstance().iDamage;            
            vStartPos = transform.position;
            //fOrizinRadius = spherCollider.radius;            
           
            oExplosion = (GameObject)Instantiate(oExplosion);
            oExplosion.SetActive(false);

            T2.Skill.DimensionBall.GetInstance().bCoolTime = false;
        }

        void OnEnable()
        {            
            meshRenderer = GetComponent<MeshRenderer>();
            spherCollider = GetComponent<SphereCollider>();
            spherCollider.radius = 0.5f;
            fOrizinRadius = spherCollider.radius;
            meshRenderer.enabled = true;
            fReach = T2.Skill.DimensionBall.GetInstance().fReach;
            vStartPos = new Vector3(trPlayer.position.x, 0.0f, trPlayer.position.z);
            bMove = true;
            bPlayerMove = false;
            spherCollider.radius = fOrizinRadius;

            oExplosion.SetActive(false);

            //lifeTime에 -0.2f는 폭발하는 시간이다.
            lifeTime = T2.Skill.DimensionBall.GetInstance().coolTime - 0.2f;
            //디멘션볼 스킬의 쿨타임을 재설정한다.
            T2.Skill.DimensionBall.GetInstance().StopCoroutine(T2.Skill.DimensionBall.GetInstance().CoolTimer(lifeTime));
            //T2.Skill.DimensionBall.GetInstance().bCoolTime = true;
            StartCoroutine(LifeTimer(this.gameObject, lifeTime));
        }

        void OnDisable()
        {
            
            T2.Skill.DimensionBall.GetInstance().fReach = T2.Skill.DimensionBall.GetInstance().fMaxReach;
            spherCollider.radius = fOrizinRadius;            

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
                    //쿨타임 시작
                    T2.Skill.DimensionBall.GetInstance().StartCoroutine(T2.Skill.DimensionBall.GetInstance().CoolTimer(lifeTime));
                    gameObject.SetActive(false);
                }
            }
        }

        void Update()
        {
            #region<이동처리 Ray>
            //투사체 이동시 장애물 체크
            Ray[] ray = new Ray[3];
            ray[0] = new Ray(transform.position, transform.forward);
            ray[1] = new Ray(transform.position - transform.right * spherCollider.radius, transform.forward);
            ray[2] = new Ray(transform.position + transform.right * spherCollider.radius, transform.forward);
            RaycastHit hit;
            int mask = (
                        (1 << LayerMask.NameToLayer(Layers.T_HitCollider)) | (1 << LayerMask.NameToLayer(Layers.Bullet)) |
                        (1 << LayerMask.NameToLayer(Layers.T_Invincibility)) | (1 << LayerMask.NameToLayer(Layers.Except_Monster)) |
                        (1 << LayerMask.NameToLayer(Layers.MonsterAttkCollider)) | (1 << LayerMask.NameToLayer(Layers.MonsterHitCollider))
                        );
            mask = ~mask;
            for (int i = 0; i < 3; i++)
            {
                float fDist = fSpeed * Time.deltaTime;
                Debug.DrawLine(ray[i].origin, ray[i].GetPoint(fDist), Color.cyan);
                if (Physics.Raycast(ray[i], out hit, fDist, mask))
                {
                    print(hit.collider.name);
                    //최상위 오브젝트의 태그가 몬스터인지 체크한다.
                    if (hit.collider.transform.root.tag != Tags.Monster)
                    {
                        if (!bMove && (spherCollider.radius).Equals(fOrizinRadius))
                        {
                            Vector3 vHitPos = Vector3.zero;
                            if (i == 1)
                                vHitPos = hit.point + transform.right * spherCollider.radius;
                            else if (i == 2)
                                vHitPos = hit.point - transform.right * spherCollider.radius;
                            else
                                vHitPos = hit.point;
                            StartCoroutine(AccuracyPosition(vHitPos));
                        }
                        bMove = false;
                    }
                    break;
                }
            }
            #endregion

            //사정거리 이내에서만 이동, 사정거리에 도착시 정지.
            float curDist = Vector3.Distance(vStartPos, new Vector3(transform.position.x, 0.0f, transform.position.z));
            if (curDist < fReach && bMove)
            {
                transform.Translate(Vector3.forward * fSpeed * Time.deltaTime, Space.Self);
            }

            
            //스킬키와 동일한 키 입력시 해당 오브젝트로 플레이어를 이동시킨다.
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (mgr.GetCtrlPossible().Skill)
                {                    
                    //if (T2.Skill.SilverStream.GetInstance().bSilverStream && !bPlayerMove)
                    if(!bPlayerMove)
                    {
                        
                        bMove = false;
                        bPlayerMove = true;
                        moveDir = transform.position - new Vector3(trPlayer.position.x, transform.position.y, trPlayer.position.z);
                        moveDir = moveDir.normalized;
                        fReachOfBall = Vector3.Distance(new Vector3(trPlayer.position.x, transform.position.y, trPlayer.position.z), transform.position);
                        mgr.ChangeState(T2.Manager.State.Skill);
                        mgr.SetLayerState(Manager.LayerState.invincibility);                        
                        StopAllCoroutines();
                    }
                }
            }
            if(bPlayerMove)
            {
                Vector3 vPlayerPos = new Vector3(trPlayer.position.x, transform.position.y, trPlayer.position.z);
                Ray[] playerRay = new Ray[3];
                playerRay[0] = new Ray(vPlayerPos, moveDir);
                playerRay[1] = new Ray(vPlayerPos - trPlayer.right * controller.radius, moveDir);
                playerRay[2] = new Ray(vPlayerPos + trPlayer.right * controller.radius, moveDir);
                for (int i = 0; i < 3; i++)
                {
                    float fDist = fPlayerSpeed * Time.deltaTime;
                    Debug.DrawLine(playerRay[i].origin, playerRay[i].GetPoint(fDist), Color.cyan);
                    if (Physics.Raycast(playerRay[i], out hit, fDist, mask))
                    {
                        //최상위 오브젝트의 태그가 몬스터인지 체크한다.
                        if (hit.collider.transform.root.tag != Tags.Floor)
                        {
                            print(hit.collider.tag);
                            bPlayerMove = false;
                            mgr.ChangeState(T2.Manager.State.idle);
                            mgr.SetLayerState(Manager.LayerState.normal);
                            //쿨타임 시작
                            T2.Skill.DimensionBall.GetInstance().StartCoroutine(T2.Skill.DimensionBall.GetInstance().CoolTimer(lifeTime));
                            gameObject.SetActive(false);
                        }
                        break;
                    }
                }

                float PlayerDist = Vector3.Distance(vStartPos, new Vector3(trPlayer.position.x, transform.position.y, trPlayer.position.z));
                if (PlayerDist >= fReachOfBall)
                {
                    bPlayerMove = false;
                    mgr.ChangeState(T2.Manager.State.idle);
                    mgr.SetLayerState(Manager.LayerState.normal);
                    //쿨타임 시작
                    T2.Skill.DimensionBall.GetInstance().StartCoroutine(T2.Skill.DimensionBall.GetInstance().CoolTimer(lifeTime));
                    gameObject.SetActive(false);
                }
                else
                {
                    controller.Move(moveDir * fPlayerSpeed * Time.deltaTime);
                }
                
            }

            //우클릭시 폭파.
            if(Input.GetMouseButtonDown(1))
            {
                if (!(mgr.GetState()).Equals(Manager.State.Skill) && !(mgr.GetState()).Equals(Manager.State.be_Shot))
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
            //쿨타임 시작
            T2.Skill.DimensionBall.GetInstance().StartCoroutine(T2.Skill.DimensionBall.GetInstance().CoolTimer(lifeTime));
            gameObject.SetActive(false);
        }

        IEnumerator AccuracyPosition(Vector3 pos)
        {
            yield return new WaitForEndOfFrame();
            transform.position = pos - (transform.forward * spherCollider.radius * 1.5f);
        }

        public void ExplosionDimensionBall()
        {
            StopAllCoroutines();
            meshRenderer.enabled = false;
            spherCollider.radius = fExplosionRange;
            StartCoroutine(ExplosionTimer());
        }
    }
}
