using Exiled.Events.EventArgs.Scp106;
using Exiled.Events.Handlers;

namespace SLCosmetics
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core.UserSettings;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using MEC;
    using SLCosmetics.Cosmetics;
    using SLCosmetics.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UserSettings.ServerSpecific;
    using UserSettings.UserInterfaceSettings;
    using PlrEvent = Exiled.Events.Handlers.Player;
    public class Main : Plugin<Config.Config>
    {
        // Public
        public override string Name => "SLCosmetics";
        public override string Author => "GatoDeveloper";
        public override Version Version => new(1, 0, 0);
        public static Main Instance { get; set; }
        //public static string HarmonyPath => "gatodeveloper.SLCosmetics";

        // Private
        //private Harmony _harmony;
        private List<CosmeticHandler> _cosmeticHandlers = new();

        public override void OnEnabled()
        {
            Instance = this;

            // Register All CosmeticHandlers
            _cosmeticHandlers = new()
            {
                //new HatCosmetic(),
                new GlowCosmetic(),
                //new PetCosmetic()
            };
            foreach (var _handler in _cosmeticHandlers)
            {
                _handler.RegisterCosmetic();
            }

            // Patch Everything with Harmony
            /*
            if (_harmony is null)
                _harmony = new(HarmonyPath);
            _harmony.PatchAll();
            */
            // Events
            PlrEvent.Verified += PlayerVerify;
            PlrEvent.ChangingGroup += PlayerChangingGroup;
            Scp106.Stalking += OnStalking;
            Scp106.ExitStalking += OnExitStalking;

            // Finally announce the plugin as enabled
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            // Disable All CosmeticHandlers
            foreach (CosmeticHandler _handler in _cosmeticHandlers)
            {
                _handler.UnregisterCosmetic();
            }
            _cosmeticHandlers = null;

            // Unpatch Everything with Harmony
            /*
            if (_harmony is not null)
                _harmony.UnpatchAll(HarmonyPath);
            */
            // Events
            PlrEvent.Verified -= PlayerVerify;
            PlrEvent.ChangingGroup -= PlayerChangingGroup;
            Scp106.Stalking -= OnStalking;
            Scp106.ExitStalking -= OnExitStalking;

            // Finally announce the plugin as disabled
            base.OnDisabled();
        }

        private static void OnStalking(StalkingEventArgs ev)
        {
            if(ev.IsAllowed)
                ev.Player.SessionVariables["cos_stlk"] = true;
        }
        
        private static void OnExitStalking(ExitStalkingEventArgs ev)
        {
            if(ev.IsAllowed)
                ev.Player.SessionVariables["cos_stlk"] = false;
        }

        // This is all meant to handle the issue of Menus competing for each other. Instead, I insert it like this.
        private static void PlayerVerify(VerifiedEventArgs _args) => SendMenuToPlayer(_args.Player);
        private static void PlayerChangingGroup(ChangingGroupEventArgs _args) => SendMenuToPlayer(_args.Player);
        private static void SendMenuToPlayer(Player _plr)
        {
            if (_plr.IsNPC) return;

            Timing.CallDelayed(0.1f, () =>
            {
                List<SettingBase> _registeredSettings = SettingBase.List.ToList();
                foreach (CosmeticHandler _handler in Instance._cosmeticHandlers)
                {
                    List<SettingBase> _menuSettings = _handler.Menu.CompileSettings(_plr);
                    foreach (SettingBase _newSetting in _menuSettings)
                    {
                        int _index = _registeredSettings.FindIndex(_setting => _setting.Id == _newSetting.Id);
                        if (_index != -1) 
                        {
                            _registeredSettings[_index] = _newSetting;
                        } else
                        {
                            Log.Error($"Couldn't Find Existing Match for {_newSetting.Id}");
                        }
                    }
                }
                SettingBase.SendToPlayer(_plr, _registeredSettings);
                Timing.CallDelayed(0.1f, () =>
                {
                    foreach (CosmeticHandler _handler in Instance._cosmeticHandlers)
                    {
                        _handler.Menu.SetOutput(_handler.Menu.OutputLine.Label, _plr);
                    }
                });
            });
        }
    }
}