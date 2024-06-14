using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EE_Turtle : MonoBehaviour
{
    public Animator turtleAnimator;
    public bool isAlive;
    //when player touches turtle, it is not alive
    public float shelterTime =1f;
    public float rotateSpeed = 0.2f;
    public float timeSinceLastTouch;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //once turtle is not alive, increment timeSinceLastTouch, if it is greater than shelterTime, it becomes alive again
        if(!isAlive){
            timeSinceLastTouch += Time.deltaTime;
            if(timeSinceLastTouch > shelterTime){
                isAlive = true;
                //set the boolean called 'shelter' to false
                turtleAnimator.SetBool("shelter", false);
            }
        }else{
            //rotate on z axis if it is alive with speed rotateSpeed
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        }
    }

    public void TouchedByPlayer(){
        isAlive = false;
        timeSinceLastTouch = 0;
        //set the boolean called 'shelter' to true
        turtleAnimator.SetBool("shelter", true);
    }

}
