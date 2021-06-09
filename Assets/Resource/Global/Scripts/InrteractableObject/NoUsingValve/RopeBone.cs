using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractableObject
{

    /// <summary>
    /// 繩子骨架
    /// </summary>
    public class RopeBone : MonoBehaviour
    {
        public bool toFreeze;
        public new Rigidbody rigidbody;

        private void Awake()
        {
            //設定骨架連接
            GetComponent<CharacterJoint>().connectedBody = transform.parent.GetComponent<Rigidbody>();
        }

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();

        }


        public void FreezePositionFixed()
        {


            if (toFreeze)
            {
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }

        }

        public void FreezePositionDetach()
        {
            if (toFreeze)
            {
                rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }

        }

    }
}
