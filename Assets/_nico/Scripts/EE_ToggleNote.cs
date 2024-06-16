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
    Quaternion originalRotation;
    bool rotBackStart = false;
    bool isRotating = false;
    float rotBackTime = 0f;
    void Start()
    {
        originalScale = transform.localScale;
        originalRotation = transform.localRotation;
    }

    void Update(){
        if(!isRotating){
            if(rotBackTime < 1f){
                rotBackTime += Time.deltaTime * 1f;
                transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, rotBackTime);
            }else{
                transform.localRotation = originalRotation;
            }
        }
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
        if(GetComponent<EE_AnimateTo>() != null){
            GetComponent<EE_AnimateTo>().AnimateToTarget();
        }
        isRotating = true;
    }

    void TurnOff(){
        transform.localScale = originalScale;
        GetComponent<AudioSource>().Stop();
        if(GetComponent<EE_AnimateTo>() != null){
            GetComponent<EE_AnimateTo>().AnimateBackToOriginal();
        }
        rotBackStart = true;
        rotBackTime = 0f;
        isRotating = false;
    }
}