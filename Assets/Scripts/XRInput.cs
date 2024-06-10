using Mona.SDK.Brains.Core.Brain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class XRInput : MonoBehaviour
{
    private MonaBrainInput _input;
    private void Start()
    {
        _input = GetComponent<MonaBrainInput>();
    }


    void Update()
    {
        var move = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        _input.ExternalMove = new Vector2(move.x, move.y);
    }
}
