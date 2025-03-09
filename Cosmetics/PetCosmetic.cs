using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using PlayerRoles.Subroutines;
using PlayerStatsSystem;
using SLCosmetics.Types;
using SLCosmetics.Types.Pets;
using System.Collections.Generic;
using UnityEngine;
using PlrEvent = Exiled.Events.Handlers.Player;

namespace SLCosmetics.Cosmetics
{
    public class PetCosmetic : CosmeticHandler
    {
        public static new CosmeticHandler Instance { get; set; }

        public static readonly Dictionary<string,string> AvailableColors = new Dictionary<string, string>() 
        {
            ["White"] = "#FFFFFF",
            ["Black"] = "#000000",
            ["Pink"] = "#FF96DE",
            ["Red"] = "#C50000",
            ["Brown"] = "#944710",
            ["Silver"] = "#A0A0A0",
            ["LightGreen"] = "#32CD32",
            ["Crimson"] = "#DC143C",
            ["Cyan"] = "#00B7EB",
            ["Aqua"] = "#00FFFF",
            ["DeepPink"] = "#FF1493",
            ["Tomato"] = "#FF6448",
            ["Yellow"] = "#FAFF86",
            ["Magenta"] = "#FF0090",
            ["BlueGreen"] = "#4DFFB8",
            ["Orange"] = "#FF9966",
            ["Lime"] = "#BFFF00",
            ["Green"] = "#228B22",
            ["Emerald"] = "#50C878",
            ["Carmine"] = "#960018",
            ["Nickel"] = "#727472",
            ["Mint"] = "#98FB98",
            ["ArmyGreen"] = "#4B5320",
            ["Pumpkin"] = "#EE7600",
        };

        public override void RegisterCosmetic()
        {
            Instance = this;

            Menu = new PetMenu();

            // Events
            PlrEvent.ChangingRole += PlayerChangingRole;
            PlrEvent.TriggeringTesla += PlayerTriggeringTesla;
            PlrEvent.Handcuffing += PlayerHandcuffing;
            PlrEvent.DroppingItem += PlayerDroppingItem;
            PlrEvent.MakingNoise += PlayerMakingNoise;
            PlrEvent.Escaping += PlayerEscaping;

            base.RegisterCosmetic();
        }

        public override void UnregisterCosmetic()
        {
            // Events
            PlrEvent.ChangingRole -= PlayerChangingRole;
            PlrEvent.TriggeringTesla -= PlayerTriggeringTesla;
            PlrEvent.Handcuffing -= PlayerHandcuffing;
            PlrEvent.DroppingItem -= PlayerDroppingItem;
            PlrEvent.MakingNoise -= PlayerMakingNoise;
            PlrEvent.Escaping -= PlayerEscaping;

            base.UnregisterCosmetic();
        }

        public static new CosmeticComponent CreateCosmetic(PetInfo _cosmeticInfo)
        {
            Player _owner = _cosmeticInfo.Owner;

            Npc _spawnedPet = Npc.Spawn($"{_cosmeticInfo.Owner.Nickname}'s Pet",_cosmeticInfo.Role,true,_cosmeticInfo.Owner.Position);
            _spawnedPet.Scale = new Vector3(0.4f,0.4f,0.4f);
            _spawnedPet.ReferenceHub.characterClassManager._godMode = true;
            _spawnedPet.ReferenceHub.playerStats.GetModule<AdminFlagsStat>().SetFlag(AdminFlags.GodMode, true);
            _spawnedPet.MaxHealth = 9999;
            _spawnedPet.Health = 9999;

            _cosmeticInfo.Npc = _spawnedPet;

            // Set Name
            _spawnedPet.CustomInfo = $"<color={_cosmeticInfo.Color}>{_cosmeticInfo.Name}</color>";
            Timing.CallDelayed(0.5f, () =>
            {
                _spawnedPet.RankName = "";
                _spawnedPet.ClearInventory();
                _spawnedPet.Follow(_owner);
            });

            // Create the Pet Component
            PetComponent _component = _owner.GameObject.AddComponent<PetComponent>();
            _component.PetInfo = _cosmeticInfo;

            Instance.PlayerLinkedCosmetics.Add(_owner.UserId, _component);
            return null;
        }

        public static new CosmeticComponent AssignNewCosmetic(Player _player, string[] _params)
        {
            Instance.RemoveCosmetic(_player);

            PetInfo _petInfo = null;

            switch (_params[0].ToLower())
            {
                case "none":
                    return null;
                case "same-role":
                    _petInfo = new(_player, _player.Role.Type, _params[1], AvailableColors[_params[2]]);
                    break;
                default:
                    return null; // In the future, allow for any Pet Class
            }

            // Invalid Pet Info? Return null.
            if (_petInfo is null)
                return null;

            return CreateCosmetic(_petInfo);
        }

        public static Player GetCosmeticOwner(Player _npc)
        {
            if (!_npc.IsNPC) return null;

            foreach (Player _plr in Player.List)
            {
                if (_plr.GameObject.TryGetComponent(out PetComponent _petComp) && _petComp.PetInfo.Npc == _npc)
                {
                    return _petComp.PetInfo.Owner;
                }
            }
            return null;
        }

        public static bool IsCosmetic(ReferenceHub refHub)
        {
            foreach (Player player in Player.List)
            {
                if (player.GameObject.TryGetComponent(out PetComponent _petComp))
                {
                    if (_petComp.PetInfo.Npc.ReferenceHub == refHub) return true;
                }
            }
            return false;
        }

        public static bool IsCosmetic(Player play)
        {
            foreach (Player player in Player.List)
            {
                if (player.GameObject.TryGetComponent(out PetComponent petComp))
                {
                    if (petComp.PetInfo.Npc.ReferenceHub == play.ReferenceHub) return true;
                }
            }
            return false;
        }

        public static bool IsCosmetic(SubroutineBase targetTrack)
        {
            targetTrack.Role.TryGetOwner(out ReferenceHub refHub);
            if (refHub != null)
                return IsCosmetic(refHub);
            else
                return false;
        }

        public override CosmeticComponent _assign(Player _player, string[] _options) => AssignNewCosmetic(_player, _options);

        public override void RemoveCosmetic(string _playerId)
        {
            if (Instance.PlayerLinkedCosmetics.TryGetValue(_playerId, out CosmeticComponent _component))
            {
                RemoveCosmetic(_component);
            }
        }

        // Events
        public void PlayerChangingRole(ChangingRoleEventArgs _args)
        {
            if (_args.Player.IsNPC) return;

            if (_args.Player.GameObject.TryGetComponent(out PetComponent _component))
            {
                if (_args.Player.Role.Type == _component.PetInfo.Npc.Role.Type)
                {
                    _component.PetInfo.Role = _args.NewRole;
                    _component.PetInfo.Npc.Role.Set(_args.NewRole, RoleSpawnFlags.None);
                }
            }
        }

        public void PlayerTriggeringTesla(TriggeringTeslaEventArgs _args)
        {
            if (_args.Player.IsNPC && GetCosmeticOwner(_args.Player) is not null)
                _args.IsAllowed = false;
        }

        public void PlayerHandcuffing(HandcuffingEventArgs _args)
        {
            if (_args.Player.IsNPC && GetCosmeticOwner(_args.Player) is not null)
                _args.IsAllowed = false;
        }

        public void PlayerDroppingItem(DroppingItemEventArgs _args)
        {
            if (_args.Player.IsNPC && GetCosmeticOwner(_args.Player) is not null)
                _args.IsAllowed = false;
        }

        public void PlayerMakingNoise(MakingNoiseEventArgs _args)
        {
            if (_args.Player.IsNPC && GetCosmeticOwner(_args.Player) is not null)
                _args.IsAllowed = false;
        }

        public void PlayerEscaping(EscapingEventArgs _args)
        {
            if (_args.Player.IsNPC && GetCosmeticOwner(_args.Player) is not null)
                _args.IsAllowed = false;
        }
    }
}
