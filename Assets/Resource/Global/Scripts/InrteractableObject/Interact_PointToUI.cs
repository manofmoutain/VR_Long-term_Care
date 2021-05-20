using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Valve.VR.Extras;

namespace InteractableObject
{
    public class Interact_PointToUI : MonoBehaviour
    {
        [SerializeField] private SteamVR_LaserPointer laserPointr;
        [SerializeField] private UnityEvent onPointerEnter;
        [SerializeField] private UnityEvent onPointerOut;
        [SerializeField] private UnityEvent onPointerClick;

        private void Awake()
        {
            laserPointr.PointerIn += PointerInside;
            laserPointr.PointerOut += PointerOutside;
            laserPointr.PointerClick += PointerClick;
        }

        void PointerInside(object sender , PointerEventArgs eventArgs)
        {
            if (eventArgs.target.gameObject.layer==5)
            {
                Debug.Log($"Pointer in {eventArgs.target.name}");
                onPointerEnter.Invoke();
            }
        }

        void PointerOutside(object sender , PointerEventArgs eventArgs)
        {
            if (eventArgs.target.gameObject.layer==5)
            {
                onPointerOut.Invoke();
            }
        }

        void PointerClick(object sender , PointerEventArgs eventArgs)
        {
            if (eventArgs.target.gameObject.layer==5)
            {
                onPointerClick.Invoke();
            }
        }
    }
}

