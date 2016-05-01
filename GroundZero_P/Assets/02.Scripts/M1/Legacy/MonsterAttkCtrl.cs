using UnityEngine;
using System.Collections;

public class MonsterAttkCtrl : MonoBehaviour {
    MonTestState test;
    MonCycleState cycle;

    private int damage;                 //데미지
    //private bool isRigid;               //경직
    //private bool isTotter;              //비틀거림
    //private bool isKnockback;           //넉벡
    //private bool isKnockdown;           //넉다운
    //private bool isSlam;                //날아감

    void OnEnable()
    {
        //공격이 시작될 때 활성화되면서 현재 공격 상태 가져옴
        //test = GameObject.FindWithTag(Tags.Monster).GetComponent<M_FSMTest>().monState;
        //cycle = GameObject.FindWithTag(Tags.Monster).GetComponent<M_FSMTest>().monCycleState;
        //SetAttkProperty();
    }

    #region <공격에 따른 수치 설정>
    public void SetAttkProperty()
    {
        if (test == MonTestState.Press)
        {
            damage = GameObject.FindWithTag(Tags.Monster).GetComponent<M_FSMTest>().bodyPressDamage;
        }

        else if (test == MonTestState.Cycle)
        {
            switch (cycle)
            {
                case MonCycleState.Breath:
                    damage = GameObject.FindWithTag(Tags.Monster).GetComponent<M_FSMTest>().breathDamage;
                    break;

                case MonCycleState.Magic_1:
                    damage = GameObject.FindWithTag(Tags.Monster).GetComponent<M_FSMTest>().magic_1_Damage;
                    break;

                case MonCycleState.Magic_2:
                    damage = GameObject.FindWithTag(Tags.Monster).GetComponent<M_FSMTest>().magic_2_Damage;
                    break;

                case MonCycleState.JumpAttack:
                    damage = GameObject.FindWithTag(Tags.Monster).GetComponent<M_FSMTest>().jumpAttkDamage;
                    break;
            }
        }
    }
    #endregion

    public int GetDamage() { return damage; }
    //public bool IsRigid() { return isRigid; }
    //public bool IsTotter() { return isTotter; }
    //public bool IsKnockback() { return isKnockback; }
    //public bool IsKnockdown() { return isKnockdown; }
    //public bool IsSlam() { return isSlam; }
    //몬스터 공격 상태
    
}
