using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace Autohand.Demo{
    public class IndexHandControllerLink : MonoBehaviour{
        
        float indexBend;
        float middleBend;
        float ringBend;
        float pinkyBend;
        float thumbBend;

        float GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum finger) {
            switch(finger){
                case SteamVR_Skeleton_FingerIndexEnum.index:
                    return skeletonAction.GetFingerCurl(finger)*indexMax;
                case SteamVR_Skeleton_FingerIndexEnum.middle:
                    return skeletonAction.GetFingerCurl(finger)*middleMax;
                case SteamVR_Skeleton_FingerIndexEnum.ring:
                    return skeletonAction.GetFingerCurl(finger)*ringMax;
                case SteamVR_Skeleton_FingerIndexEnum.pinky:
                    return skeletonAction.GetFingerCurl(finger)*pinkyMax;
                case SteamVR_Skeleton_FingerIndexEnum.thumb:
                    return skeletonAction.GetFingerCurl(finger)*thumbMax;
            };
            return skeletonAction.GetFingerCurl(finger);
        }

        //YOU CAN INCREASE THE FINGER BEND SPEED BY INCREASING THE fingerSmoothSpeed VALUE ON EACH FINGER
        //YOU CAN DISABLE FINGER SWAY BY TURNING SWAY STRENGTH ON HAND TO 0 OR DISABLEIK ENABLED

        public Hand hand;
        public SteamVR_Input_Sources handType;
        public SteamVR_Action_Skeleton skeletonAction;
        [Tooltip("Allows fingers to move while holding an object"), Space]
        public bool freeFingers = true;
        
        [Header("Bend Fingers")]
        public Finger thumb;
        public Finger index;
        public Finger middle;
        public Finger ring;
        public Finger pinky;
        
        public float thumbMax = 0.8f;
        public float indexMax = 1;
        public float middleMax = 1;
        public float ringMax = 1;
        public float pinkyMax = 1;

        [Space]
        
        [Header("Grab Finger Action")]
        [Tooltip("The required fingers to be bent to the required finger bend to call the grab event, all of these fingers needs to be past the required bend value [Represents AND]")]
        public SteamVRFingerBend[] grabFingersRequired;

        [Header("Grab Controller Action")]
        public SteamVR_Action_Boolean grabAction;
        
        [Header("Squeeze Finger Action"), Space, Space, Space]
        [Tooltip("The required fingers to be bent to the required finger bend to call the squeeze event, all of these fingers needs to be past the required bend value [Represents AND]")]
        public SteamVRFingerBend[] squeezeFingersRequired;
        
        [Header("Squeeze Controller Axis")]
        public SteamVR_Action_Single squeezeAction;
        public float requiredSqueeze = 0.8f;

        bool grabbing;
        bool squeezing;
        

        public void FixedUpdate() {
            if(hand.IsGrabbing())
                return;

            bool grab = IsGrabbing();
            if(!grabbing && grab) {
                grabbing = true;
                hand.Grab();
            }

            if(grabbing && !grab) {
                grabbing = false;
                hand.Release();
            }


            bool squeeze = IsSqueezing();
            if(!squeezing && squeeze) {
                squeezing = true;
                hand.Squeeze();
            }

            if(squeezing && !squeeze) {
                squeezing = false;
                hand.Unsqueeze();
            }
            
            if(hand.holdingObj == null) {
                thumb.bendOffset = GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.thumb);
                index.bendOffset = GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.index);
                middle.bendOffset = GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.middle);
                ring.bendOffset = GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.ring);
                pinky.bendOffset = GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.pinky);

                thumb.UpdateFinger();
                index.UpdateFinger();
                middle.UpdateFinger();
                ring.UpdateFinger();
                pinky.UpdateFinger();
            }
            else if(freeFingers && hand.holdingObj.GetComponent<GrabbablePose>() == null){
                thumb.bendOffset = thumb.GetLastHitBend();
                index.bendOffset = index.GetLastHitBend();
                middle.bendOffset = middle.GetLastHitBend();
                ring.bendOffset = ring.GetLastHitBend();
                pinky.bendOffset = pinky.GetLastHitBend();

                if(GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.thumb) < thumb.GetLastHitBend())
                    thumb.bendOffset = GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.thumb);

                if(GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.index) < index.GetLastHitBend())
                    index.bendOffset = GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.index);

                if(GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.middle) < middle.GetLastHitBend())
                    middle.bendOffset = GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.middle);

                if(GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.ring) < ring.GetLastHitBend())
                    ring.bendOffset = GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.ring);

                if(GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.pinky) < pinky.GetLastHitBend())
                    pinky.bendOffset = GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum.pinky);

                thumb.UpdateFinger();
                index.UpdateFinger();
                middle.UpdateFinger();
                ring.UpdateFinger();
                pinky.UpdateFinger();
            }
            
        }


        

        public bool IsGrabbing(){
            bool requiredFingers = true;
            
            if(grabFingersRequired.Length == 0)
                requiredFingers = false;
            else
                for (int i = 0; i < grabFingersRequired.Length; i++){
                    if(GetFingerCurl(grabFingersRequired[i].finger) < grabFingersRequired[i].amount){
                        requiredFingers = false;
                    }
                }

            if(grabAction != null && grabAction.GetState(handType)){
                requiredFingers = true;
            }
            
            return requiredFingers;
        }


        public bool IsSqueezing(){
            bool requiredFingers = true;
            
            if(squeezeFingersRequired.Length == 0)
                requiredFingers = false;
            else
                for (int i = 0; i < squeezeFingersRequired.Length; i++){
                    if (GetFingerCurl(squeezeFingersRequired[i].finger) < squeezeFingersRequired[i].amount){
                        requiredFingers = false;
                    }
                }

            if(squeezeAction != null && squeezeAction.GetAxis(handType) > requiredSqueeze){
                requiredFingers = true;
            }

            return requiredFingers;
        }
    }

    [System.Serializable]
    public struct SteamVRFingerBend {
        public float amount;
        public SteamVR_Skeleton_FingerIndexEnum finger;
    }
}
