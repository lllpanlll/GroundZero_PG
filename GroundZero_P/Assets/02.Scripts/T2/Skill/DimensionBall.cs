using UnityEngine;
using System.Collections;
using System;

namespace T2.Skill
{
    class DimensionBall : T2.Skill.Skill, T2.Skill.ISkillTimer
    {
        #region<싱글톤>
        private static T2.Skill.DimensionBall instance;
        public static T2.Skill.DimensionBall GetInstance()
        {
            if (!instance)
            {
                instance = GameObject.FindGameObjectWithTag(Tags.Player).GetComponent<T2.Skill.DimensionBall>();
                if (!instance)
                    print("DimensionBall 인스턴스 생성에 실패하였습니다.");
            }
            return instance;
        }
        #endregion
        //스킬의 필수 기본 변수들, 나중에 public으로 변환.
        public int iDecPoint = 10;
        public float beforeDelayTime = 0.0f;
        public float afterDelayTime = 0.0f;
        public float coolTime = 0.0f;


        public IEnumerator ActionTimer(float time)
        {
            throw new NotImplementedException();
        }

        public IEnumerator AfterDelayTimer(float time)
        {
            throw new NotImplementedException();
        }

        public IEnumerator BeforeDelayTimer(float time)
        {
            throw new NotImplementedException();
        }

        public IEnumerator CoolTimer(float time)
        {
            throw new NotImplementedException();
        }

    }
}
