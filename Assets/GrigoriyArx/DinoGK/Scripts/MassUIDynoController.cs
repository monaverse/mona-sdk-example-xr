using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MassUIDynoController : MonoBehaviour
{

    [Serializable]
    private class AnimationButtonDescription
    {
        public string name;
        public int state;
        public string overrideBlendShape;
        public Button button;
    }

    [Serializable]
    private class BlendShapeButtonDescription
    {
        public string name;
        public Button button;
    }

    [Header("Base settings")]
    [SerializeField] private AnimationButtonDescription[] dynoAnimations;
    [SerializeField] private BlendShapeButtonDescription[] eyeAnimations;
    [SerializeField] private Vector2 dynoMinMaxScale;
    [SerializeField] private float eyeShapeChangingSpeed;
    [Header("References")] [SerializeField] private Animator dynoAnimator;
    [SerializeField] private SkinnedMeshRenderer dynoRenderer;
    [SerializeField] private SkinnedMeshRenderer eyeLeft, eyeRight;
    [SerializeField] private Transform dynoTransform;
    [SerializeField] private Transform youngDynoLeftEye, youngDynoRightEye;
    [SerializeField] private Transform oldDynoLeftEye, oldDynoRightEye;
    [SerializeField] private Scrollbar growthScroll;

    public bool SemiWaterDino = false;
    public GameObject GlobalLand;
    public GameObject GlobalWater;
    public GameObject TempIslands;

    private int _blendShapesCount;
    private float[] _eyeBlendShapesTargets;
    private static readonly int State = Animator.StringToHash("State");
    private static readonly int Reset = Animator.StringToHash("Reset");
    private bool waterDino = false;

    private void Awake()
    {
        _blendShapesCount = eyeLeft.sharedMesh.blendShapeCount;
        _eyeBlendShapesTargets = new float[_blendShapesCount];
        
        for (int i = 0; i < dynoAnimations.Length; i++)
        {
            var animation = dynoAnimations[i];
            animation.button.GetComponentInChildren<TextMeshProUGUI>().text = animation.name;
        }
    }

    private void Start()
    {
        for (int i = 0; i < dynoAnimations.Length; i++)
        {
            var animation = dynoAnimations[i];
            var state = animation.state;
            var targetBlendShape = animation.overrideBlendShape;
            animation.button.onClick.AddListener(() => SwitchAnimation(state, targetBlendShape));
        }

        for (int i = 0; i < eyeAnimations.Length; i++)
        {
            var eyeAnimation = eyeAnimations[i];
            var targetShapeName = eyeAnimation.name;
            eyeAnimation.button.onClick.AddListener(() => SwitchEyeShape(targetShapeName));
        }
        
        growthScroll.onValueChanged.AddListener(SetGrowth);
        
        SetGrowth(growthScroll.value);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < _blendShapesCount; i++)
        {
            var from = eyeLeft.GetBlendShapeWeight(i);
            var to = _eyeBlendShapesTargets[i];
            eyeLeft.SetBlendShapeWeight(i, Mathf.Lerp(from, to, Time.fixedDeltaTime * eyeShapeChangingSpeed));
            eyeRight.SetBlendShapeWeight(i, Mathf.Lerp(from, to, Time.fixedDeltaTime * eyeShapeChangingSpeed));
        }
    }

    private void SetGrowth(float t)
    {
        t = 1 - t;
        dynoTransform.localScale = Vector3.one * Mathf.Lerp(dynoMinMaxScale.x, dynoMinMaxScale.y, t);
        
        eyeLeft.transform.position = Vector3.Lerp(youngDynoLeftEye.position, oldDynoLeftEye.position, t);
        eyeRight.transform.position = Vector3.Lerp(youngDynoRightEye.position, oldDynoRightEye.position, t);
        
        eyeLeft.transform.localScale = Vector3.Lerp(youngDynoLeftEye.localScale, oldDynoLeftEye.localScale, t);
        eyeRight.transform.localScale = Vector3.Lerp(youngDynoRightEye.localScale, oldDynoRightEye.localScale, t);
        
        eyeLeft.transform.rotation = Quaternion.Lerp(youngDynoLeftEye.rotation, oldDynoLeftEye.rotation, t);
        eyeRight.transform.rotation = Quaternion.Lerp(youngDynoRightEye.rotation, oldDynoRightEye.rotation, t);

        dynoRenderer.SetBlendShapeWeight(0, (1 - t) * 100);
    }

    private void SwitchEyeShape(string targetShapeName)
    {
        for (int i = 0; i < _blendShapesCount; i++)
        {
            var shapeWeight = eyeLeft.GetBlendShapeWeight(i);
            if (shapeWeight<=0) continue;
            _eyeBlendShapesTargets[i] = 0;
        }
        
        if (targetShapeName == "Neutral")
            return;

        var targetShapeIndex = GetBlendShapeIndex(targetShapeName);
        _eyeBlendShapesTargets[targetShapeIndex] = 100;
    }

    private int GetBlendShapeIndex(string name)
    {
        return eyeLeft.sharedMesh.GetBlendShapeIndex(name);
    }

    private void SwitchAnimation(int targetState, string targetBlendShape)
    {
        if ((dynoAnimator.GetInteger(State)!=0)&& (dynoAnimator.GetInteger(State) < 97))
            dynoAnimator.SetTrigger(Reset);
        dynoAnimator.SetInteger(State, targetState);
        if (!string.IsNullOrEmpty(targetBlendShape) && !string.IsNullOrWhiteSpace(targetBlendShape))
            SwitchEyeShape(targetBlendShape);

        /*------------------------------------------------------------------------
                if (SemiWaterDino)
                {
                    if ((targetState == 99) && (!waterDino))
                    {
                        TempIslands.active = false;
                        waterDino = true;

                    }
                    if ((targetState == 98) && (waterDino))
                    {
                        TempIslands.active = true;
                        waterDino = false;
                        //GlobalWater.active = false;
                    }
                }
                else
                {

                    if (targetState == 10)
                    {
                        GlobalLand.active = false;
                        GlobalWater.active = true;
                        TempIslands.active = false;
                    }
                    else if ((targetState != 10) && (targetState < 90))
                    {
                        GlobalLand.active = true;
                        GlobalWater.active = false;
                        if (!waterDino) TempIslands.active = true;
                    }
                }

        *///---------------------------------------------------------------------


            if ((targetState == 99))// && (!waterDino))
            {
                TempIslands.active = false;
                waterDino = true;

            }
            if ((targetState == 98))// && (waterDino))
            {
                TempIslands.active = true;
                waterDino = false;
            }

            if (targetState == 10)
            {
                GlobalLand.active = false;
                GlobalWater.active = true;
                TempIslands.active = false;
            }
            else if ((targetState != 10)&&(!waterDino))
            {
                GlobalLand.active = true;
                GlobalWater.active = false;
                TempIslands.active = true;
            }            
            else if ((targetState != 10)&&(waterDino))
            {
                GlobalLand.active = true;
                GlobalWater.active = false;
                TempIslands.active = false;
            }
            



    }
}
