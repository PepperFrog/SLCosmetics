using Exiled.API.Features;
using UnityEngine;

namespace SLCosmetics.Types
{
    public class CosmeticInfo
    {
        public Player Owner { get; set; }
        public Vector3 PositionOffset { get; set; }
        public Quaternion RotationOffset { get; set; }
        public Vector3 Scale { get; set; }

        public CosmeticInfo(Player _owner, Vector3? _positionOffset = null, Quaternion? _rotationOffset = null, Vector3? _scale = null)
        {
            Owner = _owner;
            PositionOffset = _positionOffset ?? Vector3.zero;
            RotationOffset = _rotationOffset ?? Quaternion.identity;
            Scale = _scale ?? Vector3.one;
        }
    }
}
