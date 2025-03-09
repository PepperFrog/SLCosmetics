using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SLCosmetics.Cosmetics;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;
using PlrEvent = Exiled.Events.Handlers.Player;

namespace SLCosmetics.Types
{
    public abstract class CosmeticHandler
    {
        // Private
        private Dictionary<string, CosmeticComponent> _playerLinkedCosmetics = new Dictionary<string, CosmeticComponent>();
        private CoroutineHandle _updateLoop;

        // Public
        public static CosmeticHandler Instance { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public CosmeticMenu Menu { get; set; }

        public Dictionary<string, CosmeticComponent> PlayerLinkedCosmetics
        {
            get
            {
                // Handle Attempts to Fetch Unassigned Cosmetic Components
                if (_playerLinkedCosmetics is null)
                    _playerLinkedCosmetics = new Dictionary<string, CosmeticComponent>();

                // Clear out any Destroyed Entries from the Dictionary
                _playerLinkedCosmetics = _playerLinkedCosmetics.Where(Entry => Player.TryGet(Entry.Key, out Player _check) && (Entry.Value != null && !Entry.Value.Destroyed)).ToDictionary(x => x.Key, x => x.Value);

                // Return the Dictionary
                return _playerLinkedCosmetics;
            }
            set
            {
                _playerLinkedCosmetics = value;
            }
        }
        public List<RoleTypeId> IncompatibleRoles = new()
        {
            RoleTypeId.None,
            RoleTypeId.Scp079,
            RoleTypeId.Spectator,
            RoleTypeId.Overwatch,
            RoleTypeId.Filmmaker
        };

        public virtual void RegisterCosmetic()
        {
            // Create a Empty Dictionary of Cosmetics
            PlayerLinkedCosmetics = new Dictionary<string, CosmeticComponent>();

            // Register Update Coroutine
            _updateLoop = Timing.RunCoroutine(UpdateDistributor());

            // Activate Cosmetic Menu
            Menu.Activate();

            // Register Events for Automatically Assigning/Unassigning Roles
            PlrEvent.ChangingRole += PlayerChangingRole;

            Log.Info($"Registered - {this.GetType()}");
        }

        public virtual void UnregisterCosmetic()
        {
            // Unregister the Update Coroutine
            Timing.KillCoroutines(_updateLoop);

            // Destroy the Components that are still registered
            foreach (CosmeticComponent _component in PlayerLinkedCosmetics.Values)
            {
                Object.DestroyImmediate(_component);
            }

            // Clear the Dictionary of Cosmetics
            PlayerLinkedCosmetics = new Dictionary<string, CosmeticComponent>();

            // Deactivate Cosmetic Menu
            Menu.Deactivate();
            Menu = null;

            // Register Events for Automatically Assigning/Unassigning Roles
            PlrEvent.ChangingRole -= PlayerChangingRole;

            Log.Info($"Unregistered - {this.GetType()}");
        }

        public IEnumerator<float> UpdateDistributor()
        {
            for (; ; )
            {
                yield return Timing.WaitForOneFrame;

                // To Avoid Excessive Coroutines and Handlers, Update Every Cosmetic Component in a Designated Coroutine
                foreach (CosmeticComponent _component in this.PlayerLinkedCosmetics.Values)
                {
                    _component.OnUpdate();
                }
            }
        }

        private void PlayerChangingRole(ChangingRoleEventArgs _args)
        {
            if (_args.Player.IsNPC) return;

            // Check if the Player is switching to a New Role Valid for this Cosmetic
            if (this.CheckRoleValid(_args.NewRole))
            {
                // Check if the Last Role was Invalid
                if (!this.CheckRoleValid(_args.Player.Role))
                {
                    // Check if the Player has Enable On Spawn
                    if (Menu.GetEnabledOnSpawn(_args.Player))
                    {
                        Timing.CallDelayed(0.5f, () =>
                        {
                            _assign(_args.Player, Menu.GetCosmetic(_args.Player));
                        });
                    }
                }
            }
            else
            {
                RemoveCosmetic(_args.Player);
            }
        }

        public delegate CosmeticComponent CreateCosmetic(CosmeticInfo _cosmeticInfo);
        public delegate CosmeticComponent AssignNewCosmetic(Player _player, string _option);
        public abstract CosmeticComponent _assign(Player _player, string[] _options);

        public virtual void RemoveCosmetic(CosmeticComponent _component) => Object.DestroyImmediate(_component);
        public virtual void RemoveCosmetic(Player _player) => RemoveCosmetic(_player.UserId);
        public virtual void RemoveCosmetic(string _playerId) {
            if (Instance.PlayerLinkedCosmetics.TryGetValue(_playerId, out CosmeticComponent _component)) {
                RemoveCosmetic(_component);
            }
        }

        public delegate object GetCosmetic(object _p);
    }
}