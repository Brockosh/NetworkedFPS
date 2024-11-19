using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{

    //[SyncVar]
    private string displayName = "Loading...";


    void Start()
    {
        DontDestroyOnLoad(this);
    }







}
