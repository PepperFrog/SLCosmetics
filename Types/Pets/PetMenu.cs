using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.Permissions.Extensions;
using SLCosmetics.Cosmetics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SLCosmetics.Types.Pets
{
    public class PetMenu : CosmeticMenu
    {
        public override string Name { get; } = "SLCosmetics - Pets";

        public override int IdStart { get; } = 1220;

        public static CosmeticMenu Instance
        {
            get
            {
                return PetCosmetic.Instance.Menu;
            }
        }

        public override List<SettingBase> GetSettings(Player _plr = null)
        {
            List<string> _options = new List<string>() {"None" };
            if (_plr is null || _plr.CheckPermission("slcosmetics.pets"))
            {
                _options.AddRange(new List<string>() 
                {
                    "Same-Role"
                });
            }
            return new List<SettingBase>()
            {
                new DropdownSetting(1,"Pet Role:",_options,onChanged:CosmeticChanged),
                new UserTextInputSetting(2,"Pet Name:",characterLimit:25,hintDescription:"Please choose an appropriate name, Server Admins may punish you for inappropriate content."),
                new DropdownSetting(3,"Pet Name Color:",PetCosmetic.AvailableColors.Keys.ToList(),onChanged:CosmeticChanged),
                new TwoButtonsSetting(4,"Enable On Spawn:","Disabled","Enabled",false,"When enabled, you are given the selected cosmetic upon spawning as eligeable roles."),
                new ButtonSetting(5,"Enable Cosmetic:","Grant",0,"Enables the selected cosmetic until you die.",onChanged:CosmeticEnabled),
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

            Instance.SetOutput($"<color=\"green\">Cosmetic Selected. Please re-enable this cosmetic to apply. (Now {_dropdown.SelectedOption})</color>", _plr);
        };

        public new Action<Player, SettingBase> CosmeticEnabled = (_plr, _settingBase) =>
        {
            if (!GlowCosmetic.Instance.CheckRoleValid(_plr))
            {
                Instance.SetOutput("<color=\"red\">Could Not Grant Cosmetic - Invalid Role</color>", _plr);
                return;
            }

            PetCosmetic.AssignNewCosmetic(_plr, Instance.GetCosmetic(_plr));
            Instance.SetOutput($"<color=\"green\">Cosmetic Assigned. (Now {Instance.GetCosmetic(_plr)[0]})</color>", _plr);
        };

        public override bool GetEnabledOnSpawn(Player _plr)
        {
            if (Instance.TryGetSetting(_plr, 4, out TwoButtonsSetting _plrTwoButton))
                return _plrTwoButton.IsSecond;
            return false;
        }

        public override string[] GetCosmetic(Player _plr)
        {
            if (Instance.TryGetSetting(_plr, 1, out DropdownSetting _plrDropdown))
                if (Instance.TryGetSetting(_plr, 2, out UserTextInputSetting _plrPetName))
                    if (Instance.TryGetSetting(_plr, 3, out DropdownSetting _plrNameColor))
                        return new string[] { _plrDropdown.SelectedOption, _plrPetName.Text, _plrNameColor.SelectedOption };
            return new string[] { "" };
        }
    }
}
