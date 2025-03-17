using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/Skill")]
    public class SkillData : ScriptableObject
    {
        public SkillType skillType;
        public string skillName;
        public Sprite skillIcon;
        public bool isActive = false;

        public enum SkillType
        {
            DoubleArrow,
            BounceDamage,
            BurnDamage,
            AttackSpeed,
            Rage
        }
    }
}