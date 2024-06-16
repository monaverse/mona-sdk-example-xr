using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EE_AnimateBounce : H_DetectCollision
{
    public bool setAnimation = true;
    public bool setScale = true;
    public bool setPosition = false;
    public AnimationCurve scaleCurve;
    public float scaleMultiplier = 1.5f;
    public Vector3 localPositionOffset;
    Vector3 targetPosition, originalPosition;
    public float scaleAnimationDuration = 1f;
    public float scaleAnimationDelay =0f;
    private Vector3 originalScale;
    bool startAnimating = false;
    float animationStartTime = 0;

    public Renderer blinkTarget;

    public bool setBlink, startBlinking;
    public AnimationCurve blinkCurve;
    public float blinkDuration = 1f;
    public float blinkStartTime = 0;
    public float blinkDelay = 0f;
    public Color targetColor = Color.white;
    private Color blinkColor;
    public float blinkValue;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
        targetPosition = transform.localPosition + localPositionOffset;
        originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //use curve to create a tween animation of the scale of this object so that it bounces to 1.2 and then return to 1
        if(setAnimation && startAnimating){
            float scaleValue = scaleCurve.Evaluate(Time.time - animationStartTime - scaleAnimationDelay);
            if(setScale)
                transform.localScale = originalScale * (1 + scaleValue * scaleMultiplier);
            if(setPosition)
                transform.localPosition = new Vector3(
                    originalPosition.x + (targetPosition.x - originalPosition.x) * scaleValue,
                    originalPosition.y + (targetPosition.y - originalPosition.y) * scaleValue,
                    originalPosition.z + (targetPosition.z - originalPosition.z) * scaleValue
                );
        }

        if(setBlink && startBlinking){
            blinkValue = blinkCurve.Evaluate(Time.time - blinkStartTime - blinkDelay);
            //lerp blinkColor from black to targetColor
            blinkColor = Color.Lerp(Color.black, targetColor, blinkValue);
            SetMaterials(blinkColor);
        }else{
            SetMaterials(Color.black);
        }
    }

    void SetMaterials(Color _c){
        if(!blinkTarget) return;
        //set all colors of the materials
        foreach(Material mat in blinkTarget.materials){
            mat.SetColor("_EmissionColor", _c);
        }
    }

    protected override void CollisionEvent(){
        AnimateVisual();
    }

    public void AnimateVisual(){

        //set animationStartTime to now
        if(setAnimation){
            animationStartTime = Time.time;
            startAnimating = true;
            Invoke("ResetAnimation", scaleAnimationDuration + 2);
        }

        if(setBlink){
            blinkStartTime = Time.time;
            startBlinking = true;   
            Invoke("ResetBlinking", blinkDuration + 2);
        }
    }

    void ResetAnimation(){
        startAnimating = false;
    }

    void ResetBlinking(){
        startBlinking = false;
    }

}
