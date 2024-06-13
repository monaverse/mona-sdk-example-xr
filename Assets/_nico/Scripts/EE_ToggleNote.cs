using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//require audiosource component
[RequireComponent(typeof(AudioSource))]
public class EE_ToggleNote : H_DetectCollision
{
    //for long pads
    public bool isPlaying = false;
    public bool isColliding = false;
    float playMinTime = 1f;
    float playTime = 0f;
    Vector3 originalScale;
    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update(){

    }

    protected override void CollisionEvent(){
        if (!isColliding && (Time.time - playTime) > playMinTime) {
            isPlaying = !isPlaying;
        if (isPlaying) {
            TurnOn();
        } else {
            TurnOff();
        }
        playTime = Time.time;
        isColliding = true;
    }
}

    protected override void CollisionExitEvent(){
        isColliding = false;
    }

    void TurnOn(){
        transform.localScale = originalScale * 1.5f;
        GetComponent<AudioSource>().Play();
    }

    void TurnOff(){
        transform.localScale = originalScale;
        GetComponent<AudioSource>().Stop();
    }
}