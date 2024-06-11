using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EA_InstrumentNode_Drum : EA_InstrumentNode
{
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    public override void OnInitialized(EA_Sequencer _sequencer){
        base.OnInitialized(_sequencer);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void RegisterNode(){
        sequencer.RegisterNode();
        PlayNode();
    }

    public override void PlayNode(){
        base.PlayNode();
    }

}
