using UnityEngine;
using System.Collections;

namespace T2.Skill
{
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
