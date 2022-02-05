using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumController : MonoBehaviour {
    
    GameObject nozzle;

    public float nozzleSize = 0.3f;
    public float rayDensity = 20;
    public float spread = 5;
    public float distance = 2;

    List<Vector3> points;
    List<Vector3> targets;

    RaycastHit rayHit;


    void Start() {
        nozzle = transform.Find("Nozzle").gameObject;
    }

    void Update() {
        CastRays();
    }

    void CastRays() {
        PlotRayPoints();
        Vector3 dir;
        for (int i = 0; i < points.Count; i++) {
            Vector3 origin = points[i];
            Vector3 target = targets[i] + Vector3.forward * distance;
            dir = Vector3.Normalize(target - origin);
            if (Physics.Raycast(origin, dir, out rayHit, distance)) {
                Debug.DrawLine(origin, rayHit.point, Color.green);
            } else {
                Debug.DrawRay(origin, dir * distance, Color.red);
            }
        }
    }

    void PlotRayPoints() {
        Vector3 nozzlePos = nozzle.transform.position;
        points = new List<Vector3>();
        targets = new List<Vector3>();
        int ringNum = 10;
        int outerRing = 360;

        points.Add(nozzlePos);
        targets.Add(nozzlePos + Vector3.forward * distance);
        for (int ring = 1; ring < ringNum + 1; ring++) {
            float radius = nozzleSize / ringNum * ring;
            int pointsInRing = Mathf.RoundToInt(outerRing / ringNum * ring * (rayDensity/100));
            for (int point = 0; point < pointsInRing; point++) {
                float angle = 360f / pointsInRing * point;

                points.Add(new Vector3(
                    nozzlePos.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad),
                    nozzlePos.y + radius * Mathf.Cos(angle * Mathf.Deg2Rad),
                    nozzlePos.z
                ));
                targets.Add(new Vector3(
                    nozzlePos.x + radius * spread * Mathf.Sin(angle * Mathf.Deg2Rad),
                    nozzlePos.y + radius * spread * Mathf.Cos(angle * Mathf.Deg2Rad),
                    nozzlePos.z
                ));
            }
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if (Application.isPlaying) {
            foreach (Vector3 point in points) {
                Gizmos.DrawSphere(point, 0.002f);
            }
        }
        
    }
}
