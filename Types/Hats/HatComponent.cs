using Exiled.API.Features;
using InventorySystem.Items.Pickups;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using SLCosmetics.Cosmetics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SLCosmetics.Types.Hats
{
    public class HatComponent : CosmeticComponent
    {
        public HatInfo HatInfo = null;

        public bool _hasSyncedPickup = false;

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (HatInfo.Owner is null) return;

            // Attempt to get the exact positioning for the Hat to sit on the Head
            Transform _camTransform = null;
            Vector3 _headPosOffset = Vector3.zero;
            Vector3 _headRotOffset = Vector3.zero;
            if (HatInfo.Owner.Role.Base is IFpcRole fpcRole)
            {
                List<HitboxIdentity> _HeadHitbox = fpcRole.FpcModule.CharacterModelInstance.Hitboxes.Where(hbox => hbox.name.ToLower().Contains("mixamorig:head")).ToList();
                _camTransform = _HeadHitbox.FirstOrDefault()?.transform ?? HatInfo.Owner.CameraTransform;
                switch (HatInfo.Owner.Role.Type)
                {
                    case RoleTypeId.Scp106:
                        _camTransform = fpcRole.FpcModule.CharacterModelInstance.gameObject.transform.Find("armature/Root/HipsCTRL/Pelvis/Stomach/LowerChest/UpperChest/neck/Head");
                        _headPosOffset = new(0, .20f, -0.02f);
                        _headRotOffset = new(0, 180f, 0);
                        break;
                    case RoleTypeId.Scp096:
                        _camTransform = fpcRole.FpcModule.CharacterModelInstance.gameObject.transform.Find("SCP-096/root/Hips/Spine01/Spine02/Spine03/Neck01/Neck02/head");
                        _headPosOffset = new(0, .125f, -0.025f);
                        break;
                    case RoleTypeId.Scp939:
                        _camTransform = fpcRole.FpcModule.CharacterModelInstance.gameObject.transform.Find("Anims/939Rig/HipControl/DEF-Hips/DEF-Stomach/DEF-Chest/DEF-Neck/DEF-Head");
                        _headPosOffset = new(0, .1f, .025f);
                        break;
                    case RoleTypeId.Scp173:
                        _headPosOffset = new(0, .55f, -.05f);
                        break;
                    case RoleTypeId.Scp049:
                        _headPosOffset = new(0, .125f, -.05f);
                        break;
                    case RoleTypeId.Scp0492:
                        _headPosOffset = new(0, 0, -.1f);
                        break;
                    default:
                        _headPosOffset = new(0, .20f, -.03f);
                        break;
                }
            }

            // If the role isnt an FpcRole, there can't be a head to align to, making this pointless
            if (_camTransform is null) return;

            // Calculate Offsets and Positioning
            Vector3 _eulerAngles = _camTransform.rotation.eulerAngles;
            if (_camTransform == HatInfo.Owner.CameraTransform && PlayerRolesUtils.GetTeam(HatInfo.Owner.Role) == Team.SCPs) _eulerAngles.x = 0;

            Quaternion _finalRotation = Quaternion.Euler(_eulerAngles) * Quaternion.Euler(_headRotOffset) * HatInfo.RotationOffset;
            Vector3 _finalPosition = (_finalRotation * (HatInfo.PositionOffset + _headPosOffset)) + _camTransform.position;

            // Apply to Pickup
            if (HatInfo.Pickup is not null)
            {
                HatInfo.Pickup.Position = _finalPosition;
                HatInfo.Pickup.Rotation = _finalRotation;
                if (HatInfo.Pickup.PhysicsModule is PickupStandardPhysics _standardPhysics) HatInfo.Pickup.PhysicsModule.ServerSendRpc(new Action<NetworkWriter>(_standardPhysics.ServerWriteRigidbody));
            }
        }

        public override void OnDestroy()
        {
            HatInfo.Pickup.Destroy();
            base.OnDestroy();
        }

        public override void ShowToPlayer(Player _plr)
        {
            base.ShowToPlayer(_plr);

            if (HatInfo.Pickup is not null) _plr.SpawnNetworkIdentity(HatInfo.Pickup.Base.netIdentity);
        }

        public override void HideFromPlayer(Player _plr)
        {
            base.HideFromPlayer(_plr);

            if (HatInfo.Pickup is not null) _plr.DestroyNetworkIdentity(HatInfo.Pickup.Base.netIdentity);
        }
    }
}
