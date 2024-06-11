// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Meta.XR.MRUtilityKit;

// public class EA_Level_Intro : MonoBehaviour
// {
//     private MRUKAnchor.SceneLabels floorLabel;
//     public EA_Input leftInput, rightInput;
//     public GameObject floorAnchorIntro;
//     // Start is called before the first frame update
//     void Start()
//     {
//         floorLabel = MRUKAnchor.SceneLabels.FLOOR;
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if(rightInput.hitAnchor && rightInput.hitAnchor.name == "FLOOR"){
//             Debug.Log("FLOOR IS FOUND");
//             floorAnchorIntro.SetActive(true);
//             floorAnchorIntro.transform.position = Vector3.Lerp(floorAnchorIntro.transform.position, rightInput.hitPosition, 0.1f);
//             rightInput.SetRayIndicator(true);
//         }else{
//             floorAnchorIntro.SetActive(false);
//             rightInput.SetRayIndicator(false);
//         }
//     }
// }
