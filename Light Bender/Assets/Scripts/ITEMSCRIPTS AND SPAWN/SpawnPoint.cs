using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject graphics;

    void Awake()
    {
        graphics.SetActive(false);
    }
}
