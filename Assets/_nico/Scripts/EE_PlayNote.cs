using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//require audiosource component
[RequireComponent(typeof(AudioSource))]
public class EE_PlayNote : H_DetectCollision
{
    //AUDIO
    public AudioSource audioSource;
    private float lastCollisionTime = 0f;
    private float collisionThreshold = 0.2f;
    public bool interruptPlaying = false;

    void Start(){
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

    }

    protected override void CollisionEvent(){
        if (Time.time - lastCollisionTime < collisionThreshold) {
            return;
        }

        lastCollisionTime = Time.time;

        if(interruptPlaying){
            PlayNote();
        }else{
            if(!audioSource.isPlaying){
                PlayNote();
            }
        }
    }

    void PlayNote(){
        audioSource.Play();
        if(GetComponent<EE_AnimateBounce>() != null){
            GetComponent<EE_AnimateBounce>().AnimateVisual();
        }
    }
}
