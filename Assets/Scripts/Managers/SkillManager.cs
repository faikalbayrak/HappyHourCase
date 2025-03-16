using System.Collections.Generic;
using Interfaces;
using Scriptables;
using UnityEngine;

namespace Managers
{
    public class SkillManager : MonoBehaviour, ISkillManagerService
    {
        public List<SkillData> skills;
        private bool isRageModeActive = false;
        
        public void ToggleSkill(int skill)
        {
            ToggleSkill(skills[skill]);
        }
        
        private void ToggleSkill(SkillData skill)
        {
            skill.isActive = !skill.isActive;
            ApplySkillEffects();
        }
        
        private void ApplySkillEffects()
        {
            
        }
    }
}