using TaleWorlds.Core;

namespace GiveOrdersAfterDeath
{
    public interface IPatch
    { 
        void Apply();
        
        void Reset();
        
        bool Applied { get; }
    }
}