using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_InstrumentNode : H_DetectCollision
{
    public AudioSource audio;
    public EA_Sequencer sequencer;
    public bool isPlayingNode;

    public virtual void OnInitialized(EA_Sequencer _sequencer){
        sequencer = _sequencer;
        audio = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    protected override void CollisionEvent(){
        if(!isPlayingNode){
            PlayNode();
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(isPlayingNode){
            if(!audio.isPlaying){
                isPlayingNode = false;
            }
        }
    }

    public virtual void PlayNode(){
        audio.Play();
        isPlayingNode = true;
    }
}
