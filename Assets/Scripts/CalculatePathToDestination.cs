using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Testing algorithm for other level
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

        StartCoroutine(nav.SetDestination(transform.position, TargetTransform.position));
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
