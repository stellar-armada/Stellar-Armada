using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySteer.Behaviors;

public class ShipMovementPredictor : MonoBehaviour
{

    [SerializeField] private GameObject predictionPrefab;

    private List<Transform> predictionMarkers = new List<Transform>();

    [SerializeField] private int maxPredictionMarkers = 20;
    
    [SerializeField] private AutonomousVehicle vehicle;

    [SerializeField] private SteerForPoint steerForPoint;

    void Awake()
    {
        for (int i = 0; i < maxPredictionMarkers; i++)
        {
            Transform t = Instantiate(predictionPrefab).transform;
            predictionMarkers.Add(t);
            t.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (steerForPoint.enabled)
            HandleSteering();
    }

    private float incrementTime = 2f;

    private float minDistance = 50f;
    
    void HandleSteering()
    {
        bool predictionCloseToTarget = false;
        for (int i = 0; i < predictionMarkers.Count; i++)
        {
            // Get position
            Vector3 pos = vehicle.PredictFutureDesiredPosition(i * incrementTime);

            if (Vector3.Distance(steerForPoint.TargetPoint, pos) < minDistance)
            {
                predictionCloseToTarget = true;
            }
            
            // Set positoin 
            predictionMarkers[i].gameObject.SetActive(!predictionCloseToTarget);

            if (!predictionCloseToTarget)
                predictionMarkers[i].position = pos + transform.position;
            
        }
        
    }
    
}
