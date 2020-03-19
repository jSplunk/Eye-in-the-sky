using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculatePathToDestination : MonoBehaviour {

    [SerializeField] Transform TargetTransform;
    Navigation nav;
    bool found;
    // Use this for initialization
    void Start () {
        nav = GetComponent<Navigation>();
	}
	
	// Update is called once per frame
	void LateUpdate () {

        if (found) return;

        StartCoroutine(nav.SetDestination(TargetTransform.position));
        if (!nav.pathPending && nav.fullPath.Count > 0)
        {
            //navMeshAgent.SetDestination(trans.position);
            found = true;
        }
        else
        {
            nav.pathPending = false;
            nav.remainingDistance = 0;
        }
    }
}
