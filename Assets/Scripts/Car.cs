using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Car : MonoBehaviour {

    [SerializeField] Vector3 TargetPosition;
    Vector3 StartPosition = new Vector3(0, 0, 0);
    NavMeshAgent navMeshAgent;
    NavMeshPath navMeshPath;
    bool isGoing;
    bool isCalc;
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

        timer = 5.0f;

        StartCoroutine(Init());

        //StartPosition.x = Random.Range(-1000, 1000);

        //StartPosition.y = 0.6f;

        //StartPosition.z = Random.Range(-1000, 1000);
        

       

        
    }
	
	// Update is called once per frame
	void Update () {
        if(!navMeshAgent.isOnNavMesh)
        {
            TransformController.Instance.CarsDestroyed.Invoke(this);
            Destroy(gameObject, 2);
            return;
        }
        else if(navMeshAgent.isOnNavMesh && !first)
        {
            Render();
            first = true;
        }

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 1)
        {
            CalcPath();
        }
        if (navMeshAgent.velocity.magnitude < 1f )
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = 5.0f;
        }

        if(timer < 0 && TransformController.Instance.finishLoaded)
        {
            CalcPath();
            navMeshAgent.Warp(navMeshAgent.transform.position + (navMeshAgent.transform.position.normalized * 2));
            timer = 5.0f;
        }
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            navMeshAgent.SetPath(navMeshPath);
            TransformController.Instance.CarsReady.Invoke();
        }

        if (debug)
        {
            for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
                Debug.DrawLine(navMeshPath.corners[i], navMeshPath.corners[i + 1], Color.red);
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
        navMeshAgent.SetDestination(TargetPosition);
    }

    void CalcPath()
    {
        
        Transform trans = TransformController.Instance.GetRandomTransform();
        navMeshAgent.CalculatePath(trans.position, navMeshPath);
       
        StartPosition = transform.position;

        //isCalc = navMeshAgent.SetDestination(trans.position);
        //isCalc = navMeshAgent.CalculatePath(trans.position, navMeshPath);
    }

    void Render()
    {
        
        GetComponentInChildren<MeshRenderer>().enabled = true;
        render = true;
        
        
    }

    IEnumerator Init()
    {
        StartPosition = transform.position;

        navMeshAgent = GetComponent<NavMeshAgent>();

        float x, y, z;

        x = Random.Range(0.0f, 1.0f);
        y = Random.Range(0.0f, 1.0f);
        z = Random.Range(0.0f, 1.0f);

        Color c = new Color(x, y, z);

        GetComponentInChildren<Renderer>().material.SetColor("_Color", c);

        speed = Random.Range(5, 10);

        navMeshAgent.speed = speed;

        isCalc = true;

        GetComponentInChildren<MeshRenderer>().enabled = false;

        navMeshPath = new NavMeshPath();
        yield return null;
    }
}
