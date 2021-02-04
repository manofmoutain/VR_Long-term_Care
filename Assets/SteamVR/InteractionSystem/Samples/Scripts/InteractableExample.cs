//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Demonstrates how to create a simple interactable object
//
//=============================================================================

using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem.Sample
{
	//-------------------------------------------------------------------------
	[RequireComponent( typeof( Interactable ) )]
	public class InteractableExample : MonoBehaviour
    {
        private TextMesh generalText;
        private TextMesh hoveringText;
        private Vector3 oldPosition;
		private Quaternion oldRotation;

		private float attachTime;

		private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & ( ~Hand.AttachmentFlags.SnapOnAttach ) & (~Hand.AttachmentFlags.DetachOthers) & (~Hand.AttachmentFlags.VelocityMovement);

        private Interactable interactable;

		//-------------------------------------------------
		void Awake()
		{
			var textMeshs = GetComponentsInChildren<TextMesh>();
            generalText = textMeshs[0];
            hoveringText = textMeshs[1];

            generalText.text = "沒有懸浮的手";
            hoveringText.text = "懸浮: False";

            interactable = this.GetComponent<Interactable>();
		}


		/// <summary>
		/// 當手的Collider進入物件的Collider(不用掛腳本，SteamVR內建的API)
		/// </summary>
		/// <param name="hand"></param>
		private void OnHandHoverBegin( Hand hand )
		{
			generalText.text = "懸浮於其上的手: " + hand.name;
		}


		/// <summary>
		/// 當手的Collider離開物件的Collider後執行(不用掛腳本，SteamVR內建的API)
		/// </summary>
		/// <param name="hand"></param>
		private void OnHandHoverEnd( Hand hand )
		{
			generalText.text = "沒有手觸碰到"+gameObject.name;
		}



		/// <summary>
		/// 當手的Collider持續接觸物件的Collider(不用掛腳本，SteamVR內建的API)
		/// </summary>
		/// <param name="hand"></param>
		private void HandHoverUpdate( Hand hand )
		{
            GrabTypes startingGrabType = hand.GetGrabStarting();
            bool isGrabEnding = hand.IsGrabEnding(this.gameObject);

            if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
            {
                // Save our position/rotation so that we can restore it when we detach
				//儲存物件舊的位置與旋轉資訊，以便之後可以回歸
                oldPosition = transform.position;
                oldRotation = transform.rotation;

				// Call this to continue receiving HandHoverUpdate messages,
				// and prevent the hand from hovering over anything else
				//持續接收HandHoverUpdate送來的資訊，以免手跟第二個物件做懸浮的互動(避免觸碰後引發Hover的腳本)
				hand.HoverLock(interactable);

                // Attach this object to the hand
				//手握住這個物件
                hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
            }
            else if (isGrabEnding)
            {
                // Detach this object from the hand
				//鬆開物件
                hand.DetachObject(gameObject);

				// Call this to undo HoverLock
				//解除HoverLock
				hand.HoverUnlock(interactable);

                // Restore position/rotation
				//回復物件位置與旋轉資訊
                transform.position = oldPosition;
                transform.rotation = oldRotation;
            }
		}


		/// <summary>
		/// 當手鬆開物件的順間執行(不用掛腳本，SteamVR內建的API)
		/// </summary>
		/// <param name="hand"></param>
		private void OnAttachedToHand( Hand hand )
        {
            generalText.text = string.Format("Attached: {0}", hand.name);
            attachTime = Time.time;
		}



		/// <summary>
		/// 當手鬆開物件時(不用掛腳本，SteamVR內建的API)
		/// </summary>
		/// <param name="hand"></param>
		private void OnDetachedFromHand( Hand hand )
		{
            generalText.text = string.Format("鬆開物件: {0}", hand.name);
		}


		/// <summary>
		/// 當手握住物件時，持續執行(不用掛腳本，SteamVR內建的API)
		/// </summary>
		/// <param name="hand"></param>
		private void HandAttachedUpdate( Hand hand )
		{
            generalText.text = string.Format("握住物件: {0} :: 時間: {1:F2}", hand.name, (Time.time - attachTime));
		}


        private bool lastHovering = false;
        private void Update()
        {
			//如果物件沒放在手上
            if (interactable.isHovering != lastHovering) //save on the .tostrings a bit
            {
                hoveringText.text = string.Format("Hovering: {0}", interactable.isHovering);
                lastHovering = interactable.isHovering;
            }
        }


		//-------------------------------------------------
		// Called when this attached GameObject becomes the primary attached object
		//-------------------------------------------------
		private void OnHandFocusAcquired( Hand hand )
		{
		}


		//-------------------------------------------------
		// Called when another attached GameObject becomes the primary attached object
		//-------------------------------------------------
		private void OnHandFocusLost( Hand hand )
		{
		}
	}
}