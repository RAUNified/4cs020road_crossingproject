using System.Collections.Generic;
using UnityEngine;
using HealthbarGames;

public class VehicleMover : MonoBehaviour
{
    [SerializeField] private List<GameObject> waypoints;
    private int currentWaypointIndex = 0;

    private const float CLOSE_DISTANCE = 1f;
    private const float SPEED = 10.0f;

    [SerializeField] private bool flipLookDirection = false;

    // 🟢 Phase that controls this lane
    [SerializeField] private TrafficLightPhase controlledPhase;

    void Start()
    {
        if (waypoints.Count > 0)
        {
            transform.position = waypoints[currentWaypointIndex].transform.position;
            currentWaypointIndex = 1 % waypoints.Count;
        }
    }

    void Update()
    {
        if (waypoints.Count == 0 || controlledPhase == null) return;

        var phaseState = controlledPhase.GetState();

        // 🛑 Stop if red or yellow (optional: remove yellow to allow vehicles to cross)
        if (phaseState == TrafficLightBase.State.Stop || phaseState == TrafficLightBase.State.PrepareToStop || phaseState == TrafficLightBase.State.PrepareToGo)
        {
            return;
        }

        GameObject target = waypoints[currentWaypointIndex];
        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0;

        float distance = direction.magnitude;

        if (distance > 0)
        {
            Quaternion rotation = Quaternion.LookRotation(
                flipLookDirection ? -direction : direction,
                Vector3.up
            );
            transform.rotation = rotation;
        }

        Vector3 normDirection = direction / distance;
        transform.position += normDirection * SPEED * Time.deltaTime;

        if (distance < CLOSE_DISTANCE)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }
    }
}
