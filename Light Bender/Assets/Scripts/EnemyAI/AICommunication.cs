using System;
using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using Photon.Pun;
using UnityEngine;

public class AICommunication : MonoBehaviour
{
    [NonSerialized] public static AIController NeedsHelpRed;
    [NonSerialized] public static AIController NeedsHelpBlue;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SendMessage(AIController sender)
    {
        if (sender.team == 1)
            NeedsHelpRed = sender;
        else
            NeedsHelpBlue = sender;
    }
}
