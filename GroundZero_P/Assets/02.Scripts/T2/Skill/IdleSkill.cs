using UnityEngine;
using System.Collections;

namespace T2.Skill
{
    /// <summary>
    /// 2016-05-04
    /// 스킬 FSM의 idle상태를 위한 스킬이다.
    /// 타 스킬 사용 후 idleSkill상태로 변환한다.
    /// </summary>
    public class IdleSkill : Skill
    {
        private static T2.Skill.IdleSkill instance;
        public static T2.Skill.IdleSkill GetInstance()
        {
            if (!instance)
            {
                instance = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.Skill.IdleSkill>();
                if (!instance)
                    print("IdleSkill 인스턴스 생성에 실패하였습니다.");
            }
            return instance;
        }
    }
}
