//<<<<<<< HEAD
﻿using System.Collections;
//=======
using System.Collections;
//>>>>>>> master
using System.Collections.Generic;
using UnityEngine;


public class SwitchComponent : MonoBehaviour
{
    public void TurnOffCollider(GameObject go)
    {
        go.GetComponent<Collider>().enabled = false;
    }

    public void TurnOnCollider(GameObject go)
    {
        go.GetComponent<Collider>().enabled = true;
    }
}
