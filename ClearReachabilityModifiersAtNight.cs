using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenBetterIllusionWall
{
    public class ClearReachabilityModifiersAtNight : NightSystem, IModSystem
    {
        EntityQuery Modifiers;

        protected override void Initialise()
        {
            base.Initialise();
            Modifiers = GetEntityQuery(typeof(CReachabilityModifier));
            RequireForUpdate(Modifiers);
        }

        protected override void OnUpdate()
        {
            EntityManager.DestroyEntity(Modifiers);
        }
    }
}
