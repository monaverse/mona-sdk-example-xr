using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoControll : MonoBehaviour
{

    Animator anim;
    [Header("Age (0..10)")] [SerializeField] float DinoAge = 10;
    [SerializeField] private float BabyScale = 0.5f;
    [SerializeField] private SkinnedMeshRenderer dinoRenderer;
    [SerializeField] private SkinnedMeshRenderer eyeLeft, eyeRight;
    [SerializeField] private float eyeShapeChangingSpeed=10f;
    [SerializeField] private Transform dinoTransform;
    [SerializeField] private Transform youngDinoLeftEye, youngDinoRightEye;
    [SerializeField] private Transform oldDinoLeftEye, oldDinoRightEye;


    private Vector2 dinoMinMaxScale;
    private int dinoState = 0;
    private int _blendShapesCount;
    private float[] _eyeBlendShapesTargets;
    private int eyeShape = 0;


    private void Awake()
    {
        _blendShapesCount = eyeLeft.sharedMesh.blendShapeCount;
        _eyeBlendShapesTargets = new float[_blendShapesCount];
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        dinoMinMaxScale.x = this.transform.localScale.x * BabyScale;
        dinoMinMaxScale.y = this.transform.localScale.x;
        

    }

    void Update()
    {
        if (dinoState == 17) dinoState = 0;
        if (dinoState < 0) dinoState = 16;
        if (Input.GetKeyDown("d"))
        {
            if (dinoState < 18)
            {
                dinoState++;
                if (dinoState == 13) dinoState++;
                SwitchAnimation(dinoState);
            }
        }
        if (Input.GetKeyDown("a"))
        {
            if (dinoState > 0)
            {
                dinoState--;
                if (dinoState == 13) dinoState--;
                SwitchAnimation(dinoState);
            }
        }

        if (Input.GetKeyDown("e"))
        {
            if (DinoAge < 10)
            {
                DinoAge++;
                SetGrowth(DinoAge * 0.1f);
            }
        }

        if (Input.GetKeyDown("q"))
        {
            if (DinoAge > 0)
            {
                DinoAge--;
                SetGrowth(DinoAge * 0.1f);
            }
        }

        if (Input.GetKeyDown("c"))
        {
            if (eyeShape >= 0)
            {
                eyeShape++;
                if (eyeShape > _blendShapesCount-1) eyeShape = 0;
                Debug.Log(eyeShape);
                SwitchEyeShape(eyeShape);
            }
        }

        if (Input.GetKeyDown("z"))
        {
            if (eyeShape <= _blendShapesCount)
            {
                eyeShape--;
                if (eyeShape < 0) eyeShape = _blendShapesCount-1;
                Debug.Log(eyeShape);
                SwitchEyeShape(eyeShape);
            }
        }

        if (Input.GetKeyDown("1"))
        {
            //dinoState = 98;
            SwitchAnimation(98);
        }

        if (Input.GetKeyDown("2"))
        {
            //dinoState = 99;
            SwitchAnimation(99);
        }
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

    void SetGrowth(float t)
    {
        dinoTransform.localScale = Vector3.one * Mathf.Lerp(dinoMinMaxScale.x, dinoMinMaxScale.y, t);

        eyeLeft.transform.position = Vector3.Lerp(youngDinoLeftEye.position, oldDinoLeftEye.position, t);
        eyeRight.transform.position = Vector3.Lerp(youngDinoRightEye.position, oldDinoRightEye.position, t);

        eyeLeft.transform.localScale = Vector3.Lerp(youngDinoLeftEye.localScale, oldDinoLeftEye.localScale, t);
        eyeRight.transform.localScale = Vector3.Lerp(youngDinoRightEye.localScale, oldDinoRightEye.localScale, t);

        eyeLeft.transform.rotation = Quaternion.Lerp(youngDinoLeftEye.rotation, oldDinoLeftEye.rotation, t);
        eyeRight.transform.rotation = Quaternion.Lerp(youngDinoRightEye.rotation, oldDinoRightEye.rotation, t);

        dinoRenderer.SetBlendShapeWeight(0, (1 - t) * 100);
    }
    
    private void SwitchAnimation(int targetState)
    {
        if ((anim.GetInteger("State") != 0) && (anim.GetInteger("State") < 97)) anim.SetTrigger("Reset");
        anim.SetInteger("State", targetState);
    }

    private void SwitchEyeShape(int targetShape)
    {
        for (int i = 0; i < _blendShapesCount; i++)
        {
            var shapeWeight = eyeLeft.GetBlendShapeWeight(i);
            if (shapeWeight <= 0) continue;
            _eyeBlendShapesTargets[i] = 0;
        }

        if (targetShape == 0)
            return;

        _eyeBlendShapesTargets[targetShape] = 100;
    }
}
