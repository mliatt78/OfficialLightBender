using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GUN : Item
{
    public abstract override void Use();

    public GameObject bulletImpactPrefab;
    
}
