using System.Collections;
using System.Collections.Generic;
using Autohand;
using Autohand.Demo;
using UnityEngine;

namespace AutoHandInteract
{
    public class AutoHand_WheelRotator : PhysicsGadgetHingeAngleReader
    {
        public Transform move;
        public Vector3 angle;
        public bool useLocal = false;


        void Update(){
            if(useLocal)
                move.localRotation *= Quaternion.Euler(angle*Time.deltaTime*GetValue());
            else
                move.rotation *= Quaternion.Euler(angle*Time.deltaTime*GetValue());
        }
    }
}

