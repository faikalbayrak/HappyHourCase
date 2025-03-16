using Scriptables;

namespace Interfaces
{
    public interface ISkillManagerService
    {
        public void ToggleSkill(int skill);
        public void AddObserver(ISkillObserver observer);
        public void RemoveObserver(ISkillObserver observer);
    }
}
