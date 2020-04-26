using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;


//Car object in the A*/JPS+ implementation
public class OtherCar : MonoBehaviour {

    [SerializeField] Vector3 TargetPosition;
    Navigation nav;
    Vector3 StartPosition = new Vector3(0, 0, 0);
    NavMeshAgent navMeshAgent;
    
    bool isGoing;
    bool isCalc;
    bool isDestroyed;
    float timer;
    float speed;
    bool render;
    bool first;
    [SerializeField] bool debug;
	// Use this for initialization
	void Start ()
    {
        StartPosition = new Vector3(0,0,0);
        TargetPosition = new Vector3(0,0,0);

        isGoing = false;

        isCalc = false;

        render = false;

        debug = false;

        first = false;

        isDestroyed = false;

        timer = 5.0f;

        StartCoroutine(Init());
    }
	
	// Update is called once per frame
	void Update () {

        if (isDestroyed) return;

        if (!navMeshAgent.isOnNavMesh)
        {
            TransformControllerOtherNavigation.Instance.CarsDestroyed.Invoke(this);
            Destroy(gameObject);
            isDestroyed = true;
            return;
        }
        else if(navMeshAgent.isOnNavMesh && !first)
        {
            Render();
            first = true;
        }

        if (nav == null) return;

        if (!nav.pathPending && nav.fullPath.Count < 1)
        {
            CalcPath();
        }

        if (debug)
        {
            for (int i = 0; i < navMeshAgent.path.corners.Length - 1; i++)
                Debug.DrawLine(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);

            for (int i = 0; i < nav.fullPath.Count - 1; i++)
                Debug.DrawLine(nav.fullPath[i].worldPosition, nav.fullPath[i + 1].worldPosition);
        }

    }

    public void SetStartPosition(Vector3 pos)
    {
        StartPosition = pos;
    }

    public void SetTargetPosition(Vector3 pos)
    {
        TargetPosition = pos;
        isGoing = true;
        
    }

    void CalcPath()
    {
        
        Transform trans = TransformControllerOtherNavigation.Instance.GetRandomTransform();
        StartCoroutine(nav.SetDestination(transform.position, trans.position));
        if (!nav.pathPending && nav.fullPath.Count > 0)
        {
            StartPosition = transform.position;
            TransformControllerOtherNavigation.Instance.CarsReady.Invoke();
        }
        else
        {
            nav.pathPending = false;
            nav.remainingDistance = 0;
        }
    }


    void Render()
    {
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = true;
        }
        render = true;
    }

    IEnumerator Init()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        float x, y, z;

        x = Random.Range(0.0f, 1.0f);
        y = Random.Range(0.0f, 1.0f);
        z = Random.Range(0.0f, 1.0f);

        Color c = new Color(x, y, z);

        GetComponentInChildren<Renderer>().material.SetColor("_Color", c);

        speed = Random.Range(5, 10);

        navMeshAgent.speed = speed;

        foreach(MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = false;
        }

        nav = GetComponent<Navigation>();

        yield return null;
    }

    //Can be used to move car without using a NavMesh
    void move(Vector3 destination)
    {
        float startTime = Time.time;

        float length = Vector3.Distance(transform.position, destination);

        float distCovered = (Time.time - startTime) * speed;

        float fracJourney = distCovered / length;

        Vector3.Lerp(transform.position, destination, fracJourney);
    }
}
