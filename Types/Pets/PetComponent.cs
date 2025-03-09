using Exiled.API.Features;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using SLCosmetics.Cosmetics;
using SLCosmetics.Types.Glows;
using UnityEngine;

namespace SLCosmetics.Types.Pets
{
    public class PetComponent : CosmeticComponent
    {
        public PetInfo PetInfo = null;

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (PetInfo.Owner is null) return;

            if (PetInfo.Owner.ReferenceHub.roleManager.CurrentRole is FpcStandardRoleBase _ownerFPC && _ownerFPC.FpcModule.ModuleReady && PetInfo.Npc.ReferenceHub.roleManager.CurrentRole is FpcStandardRoleBase _petFPC && _petFPC.FpcModule.ModuleReady)
            {
                _petFPC.FpcModule.CurrentMovementState = _ownerFPC.FpcModule.CurrentMovementState;
            }

            if (Vector3.Distance(PetInfo.Owner.Position, PetInfo.Npc.Position) > 10f)
                PetInfo.Npc.Position = PetInfo.Owner.Position;
        }

        public override void OnDestroy()
        {
            if (PetInfo.Npc is not null)
            {
                PetInfo.Npc.ClearInventory();
                PetInfo.Npc.Position = new Vector3(-9999f, -9999f, -9999f);
                PetInfo.Npc.Destroy();
            }

            base.OnDestroy();
        }

        public override void ShowToPlayer(Player _plr)
        {
            base.ShowToPlayer(_plr);

            if (PetInfo.Npc is not null) 
            { 
                _plr.SpawnNetworkIdentity(PetInfo.Npc.NetworkIdentity);
                _plr.ReferenceHub.connectionToClient.Send(new RoleSyncInfo(PetInfo.Npc.ReferenceHub,PetInfo.Npc.Role.Type,_plr.ReferenceHub));
            }
        }

        public override void HideFromPlayer(Player _plr)
        {
            base.HideFromPlayer(_plr);

            if (PetInfo.Npc is not null) _plr.DestroyNetworkIdentity(PetInfo.Npc.NetworkIdentity);
        }
    }
}
