using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class MyCarEvent : UnityEvent<Car>
{
}

public class TransformController : MonoBehaviour {

    Transform[] Locations;
    public GameObject Map;
    [Header("For Spawning Car")]
    [SerializeField] GameObject CarObject;
    [SerializeField] [Range(1.0f, 10000.0f)] int CarAmount;
    [SerializeField] Canvas canvas;
    [SerializeField] Slider slider;
    List<Car> Cars;
    bool first;
    [HideInInspector] public bool finishLoaded;
    public static TransformController Instance;
    [HideInInspector] public UnityEvent CarsReady;
    [HideInInspector] public MyCarEvent CarsDestroyed;
    float timerToCalcCarsPath;
    bool writtenToFile;
    
    // Use this for initialization
    void Awake()
    {
        Instance = this;
        CarsReady = new UnityEvent();
        CarsDestroyed = new MyCarEvent();
        
    }

    void Start () {
        first = false;
        Cars = new List<Car>();

        for (int i = 0; i < CarAmount; i++)
        {
            StartCoroutine(SpawnCar());
        }

        CarsReady.AddListener(UpdateCanvas);

        CarsDestroyed.AddListener(RemoveCar);

        finishLoaded = false;

        writtenToFile = false;

        timerToCalcCarsPath = 0;
    }
	
	// Update is called once per frame
	void Update () {
		if(!first)
        {
            Locations = Map.GetComponentsInChildren<Transform>();
            first = true;
            slider.maxValue = Cars.Count;
        }

        if (slider.maxValue > Cars.Count)
        {
            slider.maxValue = Cars.Count;
        }

        if(slider.value == Cars.Count && !writtenToFile)
        {
            CarsReady.RemoveListener(UpdateCanvas);
            CarsDestroyed.RemoveListener(RemoveCar);
            writtenToFile = true;
            StartCoroutine(TimeToCalcPathsForCars());
            finishLoaded = true;
            canvas.enabled = false;
        }

        if(!writtenToFile)
            timerToCalcCarsPath += Time.deltaTime;
	}

    private IEnumerator SpawnCar()
    {
        CarObject.SetActive(false);

        Quaternion q = Quaternion.identity;

        Vector3 StartPosition = new Vector3();

        StartPosition.x = Random.Range(-1000, 1000);

        StartPosition.y = 0.6f;

        StartPosition.z = Random.Range(-1000, 1000);

        GameObject o = Instantiate(CarObject, StartPosition, q);

        o.SetActive(true);

        Car component = o.GetComponent<Car>();

        Cars.Add(component);

        //component.SetStartPosition(StartPosition);

        yield return null;
    }

    public Transform GetRandomTransform()
    {
        int i = Random.Range(0, Locations.Length - 1);
        return Locations[i];
    }

    void UpdateCanvas()
    {
        slider.value += 1;
    }

    void RemoveCar(Car car)
    {
        Cars.Remove(car);
    }

    IEnumerator TimeToCalcPathsForCars()
    {
        string str = "Milliseconds to calculate path for "+ Cars.Count +" Cars: {" + timerToCalcCarsPath / 1000.0f + "}";
        StreamWriter sw = new StreamWriter("Assets/Logs/LogTimeToCalcPath.txt", true);
        sw.WriteLine(str);
        sw.Close();
        yield return null;
    }
}
