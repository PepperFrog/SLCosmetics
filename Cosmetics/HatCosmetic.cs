using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using InventorySystem.Items.Pickups;
using SLCosmetics.Types;
using SLCosmetics.Types.Hats;
using UnityEngine;
using static PlayerList;

namespace SLCosmetics.Cosmetics
{
    public class HatCosmetic : CosmeticHandler
    {
        public static new CosmeticHandler Instance { get; set; }

        public override void RegisterCosmetic()
        {
            Instance = this;

            Menu = new HatMenu();

            base.RegisterCosmetic();
        }

        public static new CosmeticComponent CreateCosmetic(HatInfo _cosmeticInfo)
        {
            // Removes Existing Hat Cosmetic For Owner
            Player _owner = _cosmeticInfo.Owner;

            // Create a Pickup for the Hat Item
            Pickup _newPickup = Pickup.Create(_cosmeticInfo.ItemType);
            _newPickup.Transform.localScale = _cosmeticInfo.Scale;
            _newPickup.Spawn(Vector3.zero, Quaternion.identity);
            _cosmeticInfo.Pickup = _newPickup;

            // Lock the Item to Prevent Picking It Up
            PickupSyncInfo _pickupSync = _newPickup.Info;
            _pickupSync.Locked = true;
            _newPickup.Info = _pickupSync;

            // Remove the Physics
            Rigidbody _rb = _newPickup.Rigidbody;
            _rb.useGravity = false;
            _rb.isKinematic = false;
            _rb.detectCollisions = false;

            // Create the Hat Component
            HatComponent _component = _owner.GameObject.AddComponent<HatComponent>();
            _component.HatInfo = _cosmeticInfo;
            //_component.VisibleToSelf = false;

            Instance.PlayerLinkedCosmetics.Add(_owner.UserId, _component);
            return _component;
        }

        public static new CosmeticComponent AssignNewCosmetic(Player _player, string[] _params)
        {
            Instance.RemoveCosmetic(_player);

            HatInfo _hatInfo = null;
            switch (_params[0].ToLower())
            {
                // None
                case "none":
                    break;
                // Classic Hat (SCP 268)
                case "hat": case "268": case "scp268": case "scp-268":
                    _hatInfo = new(_player, ItemType.SCP268, new(0, -0.2f, 0.125f), Quaternion.Euler(-90, 0, 90), new(.9f,.9f,.9f));
                    break;
                // SCP-500
                case "pill": case "pills": case "500": case "scp500": case "scp-500":
                    _hatInfo = new(_player, ItemType.SCP500, new(0, .075f, 0));
                    break;
                // Lightbulb (SCP 2176)
                case "light": case "bulb": case "lightbulb": case "2176": case "scp2176": case "scp-2176":
                    _hatInfo = new(_player, ItemType.SCP2176);
                    break;
                // SCP-207
                case "soda": case "cola": case "coke": case "207": case "scp207": case "scp-207":
                    _hatInfo = new(_player, ItemType.SCP207, new(0, 0, .225f), Quaternion.Euler(-90, 0, 0));
                    break;
                // Anti-SCP-207
                case "acola": case "anticola": case "anti207":
                    _hatInfo = new(_player, ItemType.AntiSCP207, new(0, 0, .225f), Quaternion.Euler(-90, 0, 0));
                    break;
                // Medkit
                case "medkit":
                    _hatInfo = new(_player, ItemType.Medkit, new(0, .1f, 0));
                    break;
                // Adrenaline
                case "adrenaline":
                    _hatInfo = new(_player, ItemType.Adrenaline, new(0, .1f, 0));
                    break;
                // Butter
                case "butter":
                    _hatInfo = new(_player, ItemType.KeycardScientist, new(0, .1f, 0), Quaternion.Euler(0, 90, 0), new(2.5f, 21f, 2.5f));
                    break;
            }

            // Invalid Hat? Return
            if (_hatInfo == null) return null;

            return CreateCosmetic(_hatInfo);
        }

        public override CosmeticComponent _assign(Player _player, string[] _options) => AssignNewCosmetic(_player, _options);

        public static new HatComponent GetCosmetic(ItemPickupBase _input)
        {
            Pickup _pickup = Pickup.Get(_input);
            foreach (Player _plr in Player.List)
            {
                if (_plr.GameObject.TryGetComponent(out HatComponent _hatComp) && _hatComp.HatInfo.Pickup == _pickup)
                {
                    return _hatComp;
                }
            }
            return null;
        }

        public override void RemoveCosmetic(string _playerId)
        {
            if (Instance.PlayerLinkedCosmetics.TryGetValue(_playerId, out CosmeticComponent _component))
            {
                RemoveCosmetic(_component);
            }
        }
    }
}