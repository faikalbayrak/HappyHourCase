using System;
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

        private List<ISkillObserver> observers = new List<ISkillObserver>();

        private void Awake()
        {
            ResetSkills();
        }

        public void ToggleSkill(int skill)
        {
            ToggleSkill(skills[skill]);
        }

        private void ToggleSkill(SkillData skill)
        {
            if (skill.isActive)
            {
                skill.isActive = false;
            }
            else
            {
                skill.isActive = true;
            }
            
            NotifyObservers(skill);
        }

        public void AddObserver(ISkillObserver observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
        }

        public void RemoveObserver(ISkillObserver observer)
        {
            if (observers.Contains(observer))
            {
                observers.Remove(observer);
            }
        }

        private void NotifyObservers(SkillData skill)
        {
            foreach (var observer in observers)
            {
                if (skill.isActive)
                {
                    observer.OnSkillActivated(skill);
                }
                else
                {
                    observer.OnSkillDeactivated(skill);
                }
            }
        }

        private void ResetSkills()
        {
            foreach (var skill in skills)
            {
                skill.isActive = false;
            }
        }
    }
}