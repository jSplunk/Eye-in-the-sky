using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to zoom on the map when game is running
public class OtherZoom : MonoBehaviour {
    
    [SerializeField] private Camera cam;
    private float scroll;

    public float dragSpeed;
    private Vector3 dragOrigin;
    // Use this for initialization
    void Start () {
        dragOrigin = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        dragSpeed = 0;
    }
	
	// Update is called once per frame
	void Update () {

        if (!TransformControllerOtherNavigation.Instance.finishLoaded) return;

        scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetKey(KeyCode.LeftControl))
        {
            StartCoroutine(OnScrollFast());
        }
        else if (scroll != 0 && !Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(OnScroll());
        }
        StartCoroutine(OnMouseDrag());
    }

    private IEnumerator OnScroll()
    {
        if (cam.transform.position.y <= 1800 && cam.transform.position.y >= 50)
        {
            Vector3 pos = cam.transform.position;
            pos.y += (-scroll) * 20;
            cam.transform.position = pos;
        }
        else if (cam.transform.position.y < 50)
        {
            Vector3 pos = cam.transform.position;
            pos.y = 50;
            cam.transform.position = pos;
        }
        else if (cam.transform.position.y > 1800)
        {
            Vector3 pos = cam.transform.position;
            pos.y = 1800;
            cam.transform.position = pos;
        }

        yield return null;
    }

    private IEnumerator OnScrollFast()
    {
        if (cam.transform.position.y <= 1800 && cam.transform.position.y >= 50)
        {
            Vector3 pos = cam.transform.position;
            pos.y += (-scroll) * 500;
            cam.transform.position = pos;
        }
        else if (cam.transform.position.y < 50)
        {
            Vector3 pos = cam.transform.position;
            pos.y = 50;
            cam.transform.position = pos;
        }
        else if (cam.transform.position.y > 1800)
        {
            Vector3 pos = cam.transform.position;
            pos.y = 1800;
            cam.transform.position = pos;
        }
        yield return null;
    }

    private IEnumerator OnMouseDrag()
    {
        if (Input.GetMouseButtonDown(0) && dragSpeed != 2)
        {
            dragSpeed = 2;
            dragOrigin = new Vector3(Screen.width/2, Screen.height / 2, 0);
            yield return null;
        }

        if(Input.GetMouseButtonUp(0) && dragSpeed != 0)
        {
            dragSpeed = 0;
            yield return null;
        }

        if (cam.transform.position.z > 1030 || cam.transform.position.z < -1009)
        {
            Vector3 position = cam.transform.position;
            if (position.z > 1030) position.z = 1030;
            if (position.z < -1009) position.z = -1009;
            cam.transform.position = position;
            dragSpeed = 0;
            yield return null;
        }

        if (cam.transform.position.x > 1009 || cam.transform.position.x < -988)
        {
            Vector3 position = cam.transform.position;
            if (position.x > 1009) position.x = 1009;
            if (position.x < -988) position.x = -988;
            cam.transform.position = position;
            dragSpeed = 0;
            yield return null;
        }


        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);

        cam.transform.Translate(move, Space.World);
    }
}
