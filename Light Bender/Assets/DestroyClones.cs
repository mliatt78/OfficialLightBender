using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyClones : MonoBehaviour
{
    // Start is called before the first frame update
    public float destructiontime = 4f;
    void Start()
    {
        Destroy(gameObject, destructiontime);
    }

   
}
