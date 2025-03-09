using Exiled.API.Features;
using SLCosmetics.Types;
using SLCosmetics.Types.Glows;
using SLCosmetics.Types.Hats;
using UnityEngine;
using static PlayerList;
using Light = Exiled.API.Features.Toys.Light;

namespace SLCosmetics.Cosmetics
{
    public class GlowCosmetic : CosmeticHandler
    {
        public static new CosmeticHandler Instance { get; set; }

        public override void RegisterCosmetic()
        {
            Instance = this;

            Menu = new GlowMenu();

            base.RegisterCosmetic();
        }

        public static new CosmeticComponent CreateCosmetic(GlowInfo _cosmeticInfo)
        {
            Player _owner = _cosmeticInfo.Owner;

            // Create AdminToy Light
            Light _newLight = Light.Create(_owner.Position,null,null,true,_cosmeticInfo.Color);
            _newLight.Range = 1.15f;
            _newLight.Intensity = 5f;
            _newLight.ShadowType = LightShadows.None;
            _cosmeticInfo.Light = _newLight;

            // Create the Glow Component
            GlowComponent _component = _owner.GameObject.AddComponent<GlowComponent>();
            _component.GlowInfo = _cosmeticInfo;

            Instance.PlayerLinkedCosmetics.Add(_owner.UserId, _component);
            return _component;
        }

        public static new CosmeticComponent AssignNewCosmetic(Player _player, string[] _params)
        {
            Instance.RemoveCosmetic(_player);

            GlowInfo _glowInfo = null;

            if (ColorUtility.TryParseHtmlString(_params[0], out Color _newCol)) 
            {
                _glowInfo = new(_player,_newCol);
            } else
            {
                return null;
            }

            return CreateCosmetic(_glowInfo);
        }

        public override CosmeticComponent _assign(Player _player, string[] _options) => AssignNewCosmetic(_player, _options);

        public override void RemoveCosmetic(string _playerId)
        {
            if (Instance.PlayerLinkedCosmetics.TryGetValue(_playerId, out CosmeticComponent _component))
            {
                RemoveCosmetic(_component);
            }
        }
    }
}
