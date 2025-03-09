using CustomPlayerEffects;
using Exiled.API.Features;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp096;
using PlayerRoles.PlayableScps.Scp939;
using SLCosmetics.Types;
using SLCosmetics.Types.Hats;

namespace SLCosmetics.Cosmetics
{
    public static class CosmeticExtensions
    {
        /// <summary>
        /// Checks a Player's role to see if this Cosmetic Type Is Allowed on the role
        /// </summary>
        /// <param name="_plr"><see cref="Player"/>:The Player whose Role will be Checked</param>
        /// <returns><see cref="bool"/>: True if the Role is Allowed; Otherwise, False.</returns>
        public static bool CheckRoleValid(this CosmeticHandler _handler, Player _plr) => _handler.CheckRoleValid(_plr.Role.Type);

        /// <summary>
        /// Checks a Player's role to see if this Cosmetic Type Is Allowed on the role
        /// </summary>
        /// <param name="_role"><see cref="RoleTypeId"/>:The Role to check for</param>
        /// <returns><see cref="bool"/>: True if the Role is Allowed; Otherwise, False.</returns>
        public static bool CheckRoleValid(this CosmeticHandler _handler,RoleTypeId _role) => !_handler.IncompatibleRoles.Contains(_role);

        /// <summary>
        /// Checks if a Player (Based on Component) is Visible to 
        /// </summary>
        /// <param name="_plr"><see cref="Player"/>:The Player to test on whether or not the cosmetic should be visible</param>
        /// <param name="_showSelf"><see cref="bool"/>:Should we let the player see themselves wearing this cosmetic?</param>
        /// <returns><see cref="bool"/>: True if the Cosmetic Should be Visible to the Player; Otherwise, False.</returns>
        public static bool IsVisibleTo(this CosmeticComponent _component, Player _plr, bool _showSelf = true) 
        {
            // IsPlayerNPC is 2nd because it has a lot of null checks.
            if (_plr == null || _plr.UserId == null || _plr.IsDead) return false;

            Player _cosmeticPlr = Player.Get(_component.gameObject);

            // Player is invisible? Don't Show
            if (_cosmeticPlr.TryGetEffect(Exiled.API.Enums.EffectType.Invisible, out StatusEffectBase stEff) && stEff.Intensity > 0) return false;

            // Same player with the cosmetic is viewing it? Show Self Parameter
            else if (_plr.ReferenceHub == _cosmeticPlr.ReferenceHub) return _showSelf;

            // The players are on the same team? Show
            else if (PlayerRolesUtils.GetTeam(_plr.Role) == PlayerRolesUtils.GetTeam(_cosmeticPlr.Role)) return true;

            // Otherwise Calculate the Visibility of the Player to determine.
            else if (_plr.ReferenceHub.roleManager.CurrentRole is FpcStandardRoleBase _role)
                switch (_plr.Role.Type)
                {
                    case RoleTypeId.Scp939 when _role.VisibilityController is Scp939VisibilityController vision && !vision.ValidateVisibility(_cosmeticPlr.ReferenceHub):
                        return false;
                    case RoleTypeId.Scp096 when _role.VisibilityController is Scp096VisibilityController vision && !vision.ValidateVisibility(_cosmeticPlr.ReferenceHub):
                        return false;
                    case RoleTypeId.Scp106:
                        return false;
                    default:
                        return true;
                }

            return false;
        }

        /// <summary>
        /// Taken from MapEditorReborn. Per-Player Network Spawn for NetworkIdentities
        /// </summary>
        /// <param name="networkIdentity"></param>
        public static void SpawnNetworkIdentity(this Player player, NetworkIdentity networkIdentity)
        {
            Server.SendSpawnMessage.Invoke(null, new object[2] { networkIdentity, player.Connection });
        }

        /// <summary>
        /// Taken from MapEditorReborn. Per-Player Network Destroy for NetworkIdentities
        /// </summary>
        /// <param name="networkIdentity"></param>
        public static void DestroyNetworkIdentity(this Player player, NetworkIdentity networkIdentity)
        {
            player.Connection.Send(new ObjectDestroyMessage
            {
                netId = networkIdentity.netId
            });
        }
    }
}
