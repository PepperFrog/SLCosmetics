using HarmonyLib;
using InventorySystem.Items.Pickups;
using Mirror;
using SLCosmetics.Cosmetics;
using SLCosmetics.Types;
using SLCosmetics.Types.Hats;
using System;

namespace SLCosmetics.Patches
{
    public class HatPatches
    {
        [HarmonyPatch(typeof(PickupPhysicsModule), nameof(PickupPhysicsModule.ServerSetSyncData))]
        internal static class ServerSetSyncData
        {
            [HarmonyPrefix]
            private static bool Prefix(ref PickupPhysicsModule __instance, Action<NetworkWriter> writerMethod)
            {
                if (HatCosmetic.GetCosmetic(__instance.Pickup) is HatComponent _hatComponent)
                {
                    if (_hatComponent._hasSyncedPickup)
                        return false;
                    else
                    {
                        _hatComponent._hasSyncedPickup = true;
                    }
                }
                return true;
            }
        }
    }
}
