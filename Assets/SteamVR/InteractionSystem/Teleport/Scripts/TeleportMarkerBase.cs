//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Base class for all the objects that the player can teleport to
//
//=============================================================================

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	public abstract class TeleportMarkerBase : MonoBehaviour
	{

		[Tooltip("鎖住的情況下，仍然會顯示移動範圍框，但無法移動至此區")]public bool locked = false;
		[Tooltip("未打勾的情況下，不會顯示移動範圍框，也無法移動至此區")]public bool markerActive = true;

		//-------------------------------------------------
		public virtual bool showReticle
		{
			get
			{
				return true;
			}
		}


		//-------------------------------------------------
		public void SetLocked( bool locked )
		{
			this.locked = locked;

			UpdateVisuals();
		}


		//-------------------------------------------------
		public virtual void TeleportPlayer( Vector3 pointedAtPosition )
		{
		}


		//-------------------------------------------------
		public abstract void UpdateVisuals();

		//-------------------------------------------------
		public abstract void Highlight( bool highlight );

		//-------------------------------------------------
		public abstract void SetAlpha( float tintAlpha, float alphaPercent );

		//-------------------------------------------------
		public abstract bool ShouldActivate( Vector3 playerPosition );

		//-------------------------------------------------
		public abstract bool ShouldMovePlayer();
	}
}
