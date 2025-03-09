using Exiled.API.Features;
using UnityEngine;
using Light = Exiled.API.Features.Toys.Light;

namespace SLCosmetics.Types.Glows
{
    public class GlowInfo : CosmeticInfo
    {
        public Color Color { get; set; }
        public Light Light { get; set; }

        public GlowInfo(Player _owner, Color _color, Vector3? _p = null, Quaternion? _r = null, Vector3? _s = null) : base(_owner,_p,_r,_s)
        {
            Color = _color;
        }
    }
}
