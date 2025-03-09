using Exiled.API.Features;
using SLCosmetics.Cosmetics;
using UnityEngine;

namespace SLCosmetics.Types.Glows
{
    public class GlowComponent : CosmeticComponent
    {
        public GlowInfo GlowInfo = null;

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (GlowInfo.Owner is null) return;

            GlowInfo.Light.Position = GlowInfo.Owner.Position + new Vector3(0f, -0.8f, 0f);

            if (GlowInfo.Owner.CurrentRoom.RoomLightController._flickerDuration > 0f)
                GlowInfo.Light.Position += new Vector3(0,-150,0);
        }

        public override void OnDestroy()
        {
            if (GlowInfo.Light is not null)
                DestroyImmediate(GlowInfo.Light.AdminToyBase.gameObject);

            base.OnDestroy();
        }

        public override void ShowToPlayer(Player _plr)
        {
            base.ShowToPlayer(_plr);

            if (GlowInfo.Light is not null) _plr.SpawnNetworkIdentity(GlowInfo.Light.Base.netIdentity);
        }

        public override void HideFromPlayer(Player _plr)
        {
            base.HideFromPlayer(_plr);

            if (GlowInfo.Light is not null) _plr.DestroyNetworkIdentity(GlowInfo.Light.Base.netIdentity);
        }
    }
}
