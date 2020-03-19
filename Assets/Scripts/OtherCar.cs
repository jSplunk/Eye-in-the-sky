using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

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

        //StartPosition.x = Random.Range(-1000, 1000);

        //StartPosition.y = 0.6f;

        //StartPosition.z = Random.Range(-1000, 1000);
        

       

        
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

       

        //if (!nav.pathPending && nav.fullPath.Count > 0)
        //{
        //    foreach (Node n in nav.fullPath)
        //    {
        //        move(n.worldPosition);
        //    }
        //}

        
        //if (navMeshAgent.velocity.magnitude < 1f )
        //{
        //    timer -= Time.deltaTime;
        //}
        //else
        //{
        //    timer = 5.0f;
        //}

        //if(timer < 0 && TransformControllerOtherNavigation.Instance.finishLoaded)
        //{
        //    StartCoroutine(CalcPath());
        //    navMeshAgent.Warp(navMeshAgent.transform.position + (navMeshAgent.transform.position.normalized * 2));
        //    timer = 5.0f;
        //}

        if (debug)
        {
            for (int i = 0; i < navMeshAgent.path.corners.Length - 1; i++)
                Debug.DrawLine(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);

            for (int i = 0; i < nav.fullPath.Count - 1; i++)
                Debug.DrawLine(nav.fullPath[i].worldPosition, nav.fullPath[i + 1].worldPosition);
        }

        if (nav == null) return;

        if (!nav.pathPending && nav.fullPath.Count < 1)
        {
            CalcPath();
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

    void Go()
    {
        StartPosition = transform.position;
    }

    void CalcPath()
    {
        
        Transform trans = TransformControllerOtherNavigation.Instance.GetRandomTransform();
        StartCoroutine(nav.SetDestination(trans.position));
        if (!nav.pathPending && nav.fullPath.Count > 0)
        {
            //navMeshAgent.SetDestination(trans.position);
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
        
        GetComponentInChildren<MeshRenderer>().enabled = true;
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

        GetComponentInChildren<MeshRenderer>().enabled = false;

        nav = GetComponent<Navigation>();

        yield return null;
    }

    void move(Vector3 destination)
    {
        float startTime = Time.time;

        float length = Vector3.Distance(transform.position, destination);

        float distCovered = (Time.time - startTime) * speed;

        float fracJourney = distCovered / length;

        Vector3.Lerp(transform.position, destination, fracJourney);
    }
}
