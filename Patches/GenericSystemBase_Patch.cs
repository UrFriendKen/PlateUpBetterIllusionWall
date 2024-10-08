﻿using HarmonyLib;
using Kitchen;
using System;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenBetterIllusionWall.Patches
{
    [HarmonyPatch]
    static class GenericSystemBase_Patch
    {
        static EntityQuery? Query;

        static MethodInfo m_GetEntityQuery = typeof(ComponentSystemBase).GetMethod("GetEntityQuery", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(ComponentType[]) }, null);

        [HarmonyPatch(typeof(TileManager), "CanReach")]
        [HarmonyPostfix]
        static void CanReach_Postfix(ref TileManager __instance, IntVector3 from, IntVector3 to, ref bool __result)
        {
            if (__result)
                return;

            if (!Query.HasValue)
                Query = (EntityQuery)(m_GetEntityQuery?.Invoke(__instance, new object[] { new ComponentType[] { typeof(CReachabilityModifier), typeof(CPosition) } }));
                if (!Query.HasValue)
                    return;

            using NativeArray<CReachabilityModifier> modifiers = Query.Value.ToComponentDataArray<CReachabilityModifier>(Allocator.Temp);
            using NativeArray<CPosition> positions = Query.Value.ToComponentDataArray<CPosition>(Allocator.Temp);

            for (int i = 0; i < modifiers.Length; i++)
            {
                CReachabilityModifier modifier = modifiers[i];
                CPosition position = positions[i];

                IntVector3 posFrom = position.Position;
                IntVector3 posTo = position.Position + modifier.Orientation.ToOffset();

                if ((posFrom == from && posTo == to) || (modifier.TwoWay && (posFrom == to && posTo == from)))
                {
                    __result = true;
                    return;
                }
            }
        }
    }
}
