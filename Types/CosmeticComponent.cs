using Exiled.API.Features;
using SLCosmetics.Cosmetics;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SLCosmetics.Types
{
    public abstract class CosmeticComponent : MonoBehaviour
    {
        // Private
        private List<string> _invisibleTo = new List<string>();

        // Public
        public bool Destroyed = false;
        public bool VisibleToSelf = true;
        public List<string> CosmeticInvisibleTo
        {
            get
            {
                if (_invisibleTo is not null)
                    _invisibleTo = _invisibleTo.Where(uid => Player.TryGet(uid, out Player plr)).ToList();

                if (_invisibleTo is null)
                    _invisibleTo = new List<string>();

                return _invisibleTo;
            }
            set
            {
                _invisibleTo = value;
            }
        }

        public virtual void OnUpdate()
        {
            // Delete this Component if there is not an Attached Player
            if (Destroyed || !Player.TryGet(gameObject, out Player plr))
            {
                OnDestroy();
                DestroyImmediate(this);
                return;
            }

            // Change the Visibility of the Component
            foreach (Player _plr in Player.List)
            {
                if (_plr.IsNPC) continue;

                if (this.IsVisibleTo(_plr, VisibleToSelf))
                {
                    if (CosmeticInvisibleTo.Contains(_plr.UserId)) {
                        ShowToPlayer(_plr);
                        CosmeticInvisibleTo.Remove(_plr.UserId);
                    }
                } else 
                {
                    if (!CosmeticInvisibleTo.Contains(_plr.UserId))
                    {
                        HideFromPlayer(_plr);
                        CosmeticInvisibleTo.Add(_plr.UserId);
                    }
                }
            }
        }

        public virtual void OnDestroy()
        {
            Destroyed = true;
        }

        public virtual void ShowToPlayer(Player _plr) 
        { 
        
        }

        public virtual void HideFromPlayer(Player _plr) 
        {
        
        }
    }
}