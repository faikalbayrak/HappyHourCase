using Scriptables;

namespace Interfaces
{
    public interface ISkillObserver
    {
        void OnSkillActivated(SkillData skill);
        void OnSkillDeactivated(SkillData skill);
    }
}