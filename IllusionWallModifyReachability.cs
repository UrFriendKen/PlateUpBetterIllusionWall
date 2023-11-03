using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace KitchenBetterIllusionWall
{
    public class IllusionWallModifyReachability : RestaurantSystem, IModSystem
    {
        EntityQuery IllusionWallsWithModifier;
        EntityQuery IllusionWallsWithoutModifier;

        public struct CIllusionWallReachability : IComponentData, IModComponent
        {
            public Entity ReachabilityEntity;
        }

        protected override void Initialise()
        {
            base.Initialise();
            IllusionWallsWithModifier = GetEntityQuery(typeof(CApplianceIllusionWall), typeof(CIllusionWallReachability));
            IllusionWallsWithoutModifier = GetEntityQuery(new QueryHelper()
                .All(typeof(CApplianceIllusionWall), typeof(CPosition))
                .None(typeof(CIllusionWallReachability)));
        }

        protected override void OnUpdate()
        {
            if (!Has<SIsDayTime>())
            {
                if (!IllusionWallsWithModifier.IsEmpty)
                    EntityManager.RemoveComponent<CIllusionWallReachability>(IllusionWallsWithModifier);
                return;
            }

            using NativeArray<Entity> entities = IllusionWallsWithoutModifier.ToEntityArray(Allocator.Temp);
            using NativeArray<CPosition> positions = IllusionWallsWithoutModifier.ToComponentDataArray<CPosition>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                CPosition position = positions[i];

                Entity modifier = EntityManager.CreateEntity();
                Set(modifier, new CReachabilityModifier()
                {
                    Orientation = position.Rotation.ToOrientation(),
                    TwoWay = true
                });
                Set(modifier, position);

                Set(entity, new CIllusionWallReachability()
                {
                    ReachabilityEntity = modifier
                });
            }
        }
    }
}
