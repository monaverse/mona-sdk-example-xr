using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class H_DetectCollision : MonoBehaviour
{
    public string targetStringContains = "Hand"; // name of target collision must contain this string
    public UnityEvent collisionEnterEvent, collisionExitEvent;
    public GameObject collidingObject;
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(gameObject.name + "Colliding with " + collision.gameObject.name);
        if(collision.gameObject.name.Contains(targetStringContains)){
            collidingObject = collision.gameObject;
            collisionEnterEvent?.Invoke();
        }

        //call the CollisionEvent that can be overriden by children scripts
        CollisionEvent();
    }

    protected virtual void CollisionEvent(){
        Debug.Log("Collision Event");
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log(gameObject.name + "Colliding with " + collision.gameObject.name);
        if(collision.gameObject == collidingObject){
            collisionExitEvent?.Invoke();
            collidingObject = null;
        }

        CollisionExitEvent();
    }

    protected virtual void CollisionExitEvent(){
        Debug.Log("Collision Exit Event");
    }
}
