using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using UnityEngine;

namespace SLCosmetics.Types.Hats
{
    public class HatInfo : CosmeticInfo
    {
        public ItemType ItemType { get; set; }
        public Pickup Pickup { get; set; }

        public HatInfo(Player _owner, ItemType _itemType, Vector3? _p = null, Quaternion? _r = null, Vector3? _s = null) : base(_owner,_p,_r,_s)
        {
            ItemType = _itemType;
        }
    }
}
