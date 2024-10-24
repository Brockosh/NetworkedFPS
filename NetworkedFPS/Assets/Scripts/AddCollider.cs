using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCollider : MonoBehaviour
{


    private void Start()
    {
        gameObject.AddComponent<MeshCollider>();
    }
}
