using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EE_AnimateTo : H_DetectCollision
{
public Vector3 targetSize;
public float animateToTargetTime;
public float animateToOriginalTime;
public AnimationCurve animateToCurve, animateBackCurve;
private Vector3 originalSize;
private float timeSinceStarted;
private bool isAnimatingToTarget;
private bool isAnimatingToOriginal;

private bool toggled;

// Start is called before the first frame update
void Start()
{
    originalSize = transform.localScale;
    timeSinceStarted = 0f;
    isAnimatingToTarget = false;
    isAnimatingToOriginal = false;
}

// Update is called once per frame
void Update()
{
    if (isAnimatingToTarget)
    {
        float t = timeSinceStarted / animateToTargetTime;
        float curveValue = animateToCurve.Evaluate(t);
        transform.localScale = Vector3.Lerp(originalSize, targetSize, curveValue);
        timeSinceStarted += Time.deltaTime;
        if (t >= 1.0f)
        {
            isAnimatingToTarget = false;
            timeSinceStarted = 0f;
        }
    }
    else if (isAnimatingToOriginal)
    {
        float t = timeSinceStarted / animateToOriginalTime;
        float curveValue = animateBackCurve.Evaluate(t);
        transform.localScale = Vector3.Lerp(targetSize, originalSize, curveValue);
        timeSinceStarted += Time.deltaTime;
        if (t >= 1.0f)
        {
            isAnimatingToOriginal = false;
            timeSinceStarted = 0f;
        }
    }
}

protected override void CollisionEvent(){
    toggled = !toggled;
    if(toggled){
        AnimateToTarget();
    }else{
        AnimateBackToOriginal();
    }
}

public void AnimateToTarget()
{
    if(isAnimatingToOriginal) return;
    isAnimatingToTarget = true;
    isAnimatingToOriginal = false;
    timeSinceStarted = 0f;
}

public void AnimateBackToOriginal()
{
    if(isAnimatingToTarget) return;
    isAnimatingToOriginal = true;
    isAnimatingToTarget = false;
    timeSinceStarted = 0f;
}
}
