using Kitchen;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace KitchenBetterIllusionWall
{
    public struct CReachabilityModifier : IComponentData, IModComponent
    {
        public Orientation Orientation;

        public bool TwoWay;
    }
}
