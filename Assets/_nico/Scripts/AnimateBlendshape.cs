using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateBlendshape : MonoBehaviour
{
    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    int blendShapeCount;
    public int playIndex = 0;
    float playIndexFloat = 0f;
    public float shapeAnimationSpeed = 1f;
    public float materialAnimationSpeed;
    public Material mainMat;
    public float offsetY;


    // Start is called before the first frame update
    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = skinnedMeshRenderer.sharedMesh;
        blendShapeCount = skinnedMesh.blendShapeCount;
        mainMat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // Reset the previous blend shape weight to 0
        skinnedMeshRenderer.SetBlendShapeWeight(playIndex, 0f);

        // Increment the playIndexFloat based on time and speed
        playIndexFloat += Time.deltaTime * shapeAnimationSpeed;
        playIndex = (int)playIndexFloat % blendShapeCount; // Use modulo to loop back to 0

        // Set the current blend shape weight to 100
        skinnedMeshRenderer.SetBlendShapeWeight(playIndex, 100f);

        //animate the offset parameter in tiling of the material
        offsetY += Time.deltaTime * materialAnimationSpeed;

        // Set the new offset values to the material
        //mainMat.SetTextureOffset("_MainTex", new Vector2(0, offsetY));
        mainMat.mainTextureOffset = new Vector2(0, -offsetY);
    }

    // Update is called once per frame
    // void Update()
    // {

    //     if(playIndex > 0){
    //         skinnedMeshRenderer.SetBlendShapeWeight(playIndex-1, 0f);
    //     }

    //     if(playIndex == 0){
    //         skinnedMeshRenderer.SetBlendShapeWeight(blendShapeCount - 1, 0f);
    //     }

    //     skinnedMeshRenderer.SetBlendShapeWeight(playIndex, 100f);

    //     playIndexFloat += Time.deltaTime * speed;
    //     playIndex = (int)playIndexFloat;

    //     if(playIndex > blendShapeCount-1){
    //         playIndex = 0;
    //     }
        
    // }
}
