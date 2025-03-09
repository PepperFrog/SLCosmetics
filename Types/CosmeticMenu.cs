using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SLCosmetics.Types
{
    public abstract class CosmeticMenu
    {

        public abstract string Name { get; }

        public abstract int IdStart { get; }

        public TextInputSetting OutputLine;

        private List<SettingBase> _settings;

        public virtual List<SettingBase> GetSettings(Player _plr = null)
        {

            return new List<SettingBase>()
            {
                new TextInputSetting(1,"<color=red>Developer, I found a buge...</color>."),
            };
        }

        public virtual List<SettingBase> CompileSettings(Player _plr = null)
        {
            List<SettingBase> _preoffsetSettings = GetSettings(_plr);

            // Adjust all of our settings to match their respective ID offset
            for (int i = 0; i < _preoffsetSettings.Count(); i++)
            {
                _preoffsetSettings[i].Id += IdStart;
            }

            // Return Compiled List
            return new List<SettingBase>() {
                new HeaderSetting(Name),
                OutputLine,
            }.Concat(_preoffsetSettings).ToList();
        }

        public virtual void Activate() 
        {
            OutputLine = new TextInputSetting(IdStart, "<align=\"center\">Interact with a Setting</align>");

            _settings = CompileSettings();

            SettingBase.Register(_settings);
        }

        public virtual void Deactivate()
        {
            if (_settings is not null)
            {
                SettingBase.Unregister(null,_settings);
            }

            _settings = null;

            OutputLine = null;
        }

        public virtual bool TryGetSetting<T>(Player _plr, int _id, out T _setting) where T : SettingBase => SettingBase.TryGetSetting<T>(_plr, IdStart + _id, out _setting);

        public void SetOutput(string _output, Player _plr)
        {
            OutputLine.Base.SendTextUpdate($"<align=\"center\">{_output}</align>", false, (_hub) => _hub == _plr.ReferenceHub);
        }

        public delegate Action<Player, SettingBase> CosmeticChanged(Player _plr, string _settingBase);

        public delegate Action<Player, SettingBase> CosmeticEnabled(Player _plr, string _settingBase);

        public abstract bool GetEnabledOnSpawn(Player _plr);
        public abstract string[] GetCosmetic(Player _plr);
    }
}
