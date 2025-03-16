using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/Skill")]
    public class SkillData : ScriptableObject
    {
        public string skillName;
        public Sprite skillIcon;
        public bool isActive = false;
    }
}