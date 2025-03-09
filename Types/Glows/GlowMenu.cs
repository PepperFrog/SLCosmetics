using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.Permissions.Extensions;
using SLCosmetics.Cosmetics;
using System;
using System.Collections.Generic;

namespace SLCosmetics.Types.Glows
{
    public class GlowMenu : CosmeticMenu
    {
        public override string Name { get; } = "SLCosmetics - Glows";

        public override int IdStart { get; } = 1210;

        public static CosmeticMenu Instance
        {
            get
            {
                return GlowCosmetic.Instance.Menu;
            }
        }

        public override List<SettingBase> GetSettings(Player _plr = null)
        {
            List<string> _options = new List<string>() {"None" };
            if (_plr is null || _plr.CheckPermission("slcosmetics.glows"))
            {
                _options.AddRange(new List<string>() 
                {
                    "Red",
                    "Orange",
                    "Yellow",
                    "Green",
                    "Cyan",
                    "Blue",
                    "Purple",
                    "Magenta"
                });
            }
            return new List<SettingBase>()
            {
                new DropdownSetting(1,"Current Color:",_options,onChanged:CosmeticChanged),
                new TwoButtonsSetting(2,"Enable On Spawn:","Disabled","Enabled",false,"When enabled, you are given the selected cosmetic upon spawning as eligeable roles."),
                new ButtonSetting(3,"Enable Cosmetic:","Grant",0,"Enables the selected cosmetic until you die.",onChanged:CosmeticEnabled),
            };
        }

        public new Action<Player, SettingBase> CosmeticChanged = (_plr, _settingBase) =>
        {
            DropdownSetting _dropdown = _settingBase as DropdownSetting;
            
            if (!GlowCosmetic.Instance.CheckRoleValid(_plr))
            {
                Instance.SetOutput("<color=\"green\">Cosmetic Selected. Change will apply after spawning as a Supported Class</color>", _plr);
                return;
            }

            GlowCosmetic.AssignNewCosmetic(_plr, Instance.GetCosmetic(_plr));
            Instance.SetOutput($"<color=\"green\">Cosmetic Selected and Assigned. (Now {_dropdown.SelectedOption})</color>", _plr);
        };

        public new Action<Player, SettingBase> CosmeticEnabled = (_plr, _settingBase) =>
        {
            if (!GlowCosmetic.Instance.CheckRoleValid(_plr))
            {
                Instance.SetOutput("<color=\"red\">Could Not Grant Cosmetic - Invalid Role</color>", _plr);
                return;
            }

            GlowCosmetic.AssignNewCosmetic(_plr, Instance.GetCosmetic(_plr));
            Instance.SetOutput($"<color=\"green\">Cosmetic Assigned. (Now {Instance.GetCosmetic(_plr)[0]})</color>", _plr);
        };

        public override bool GetEnabledOnSpawn(Player _plr)
        {
            if (Instance.TryGetSetting(_plr, 2, out TwoButtonsSetting _plrTwoButton))
                return _plrTwoButton.IsSecond;
            return false;
        }

        public override string[] GetCosmetic(Player _plr)
        {
            if (Instance.TryGetSetting(_plr, 1, out DropdownSetting _plrDropdown))
                return new string[] { _plrDropdown.SelectedOption };
            return new string[] { "" };
        }
    }
}
