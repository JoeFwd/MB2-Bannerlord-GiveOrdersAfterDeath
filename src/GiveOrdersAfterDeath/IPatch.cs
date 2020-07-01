using TaleWorlds.Core;

namespace GiveOrdersAfterDeath
{
    public interface IPatch
    { 
        void Apply(Game game);
        
        bool Applied { get; }
    }
}