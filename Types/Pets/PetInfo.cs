using Exiled.API.Features;
using PlayerRoles;
using UnityEngine;

namespace SLCosmetics.Types.Pets
{
    public class PetInfo : CosmeticInfo
    {
        public Npc Npc { get; set; }

        public RoleTypeId Role { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public PetInfo(Player _owner, RoleTypeId _role, string _name, string _color,Vector3? _p = null, Quaternion? _r = null, Vector3? _s = null) : base(_owner, _p, _r, _s)
        {
            Role = _role;
            Name = _name;
            Color = _color;
        }
    }
}
