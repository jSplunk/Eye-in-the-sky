using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

    public static MapController Instance;

    public GameObject Map;
    [HideInInspector]
    public Transform[] MapTransforms;
    [HideInInspector]
    public bool isEmpty;
    bool first = false;

    void Awake()
    {
        Instance = this;
        isEmpty = true;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!first)
        {
            StartCoroutine(SetMapTransforms());
            first = true;
        }
    }

    IEnumerator SetMapTransforms()
    {
        
        isEmpty = false;
        yield return null;
    }
}
