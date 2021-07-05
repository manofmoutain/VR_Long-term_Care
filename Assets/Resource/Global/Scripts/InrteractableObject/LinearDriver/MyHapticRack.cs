using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

namespace InteractableObject
{
    [RequireComponent(typeof(MyInteractable))]
    public class MyHapticRack : MonoBehaviour
    {
        [Tooltip( "The linear mapping driving the haptic rack" )]
        		public Interact_LinearMapping linearMapping;

        		[Tooltip( "The number of haptic pulses evenly distributed along the mapping" )]
        		public int teethCount = 128;

        		[Tooltip( "Minimum duration of the haptic pulse" )]
        		public int minimumPulseDuration = 500;

        		[Tooltip( "Maximum duration of the haptic pulse" )]
        		public int maximumPulseDuration = 900;

        		[Tooltip( "This event is triggered every time a haptic pulse is made" )]
        		public UnityEvent onPulse;

        		private Hand hand;
        		private int previousToothIndex = -1;

        		//-------------------------------------------------
        		void Awake()
        		{
        			if ( linearMapping == null )
        			{
        				linearMapping = GetComponent<Interact_LinearMapping>();
        			}
        		}


        		//-------------------------------------------------
        		private void OnHandHoverBegin( Hand hand )
        		{
        			this.hand = hand;
        		}


        		//-------------------------------------------------
        		private void OnHandHoverEnd( Hand hand )
        		{
        			this.hand = null;
        		}


        		//-------------------------------------------------
        		void Update()
        		{
        			int currentToothIndex = Mathf.RoundToInt( linearMapping.value * teethCount - 0.5f );
        			if ( currentToothIndex != previousToothIndex )
        			{
        				Pulse();
        				previousToothIndex = currentToothIndex;
        			}
        		}


        		//-------------------------------------------------
        		private void Pulse()
        		{
        			if ( hand && (hand.isActive) && ( hand.GetBestGrabbingType() != GrabTypes.None ) )
        			{
        				ushort duration = (ushort)Random.Range( minimumPulseDuration, maximumPulseDuration + 1 );
        				hand.TriggerHapticPulse( duration );

        				onPulse.Invoke();
        			}
        		}
    }
}

