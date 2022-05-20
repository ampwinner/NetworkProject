using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enableComponents[] components;

    // Start is called before the first frame update
    void Start()
    {
        components = FindObjectsOfType<enableComponents>();
        foreach (enableComponents components in components)
        {
            components.enabled = true;
        }
    }

}
