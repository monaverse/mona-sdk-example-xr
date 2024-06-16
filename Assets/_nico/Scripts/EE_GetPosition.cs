using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EE_GetPosition : MonoBehaviour
{
    public static EE_GetPosition Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public GameObject headset;
    public Vector3 headsetPosition;
    public GameObject instrumentOrigin;
    // Start is called before the first frame update
    void Start()
    {
        headsetPosition = headset.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(headsetPosition == Vector3.zero)
        {
            headsetPosition = headset.transform.position;
            instrumentOrigin.transform.position = headsetPosition;
        }
    }
}
