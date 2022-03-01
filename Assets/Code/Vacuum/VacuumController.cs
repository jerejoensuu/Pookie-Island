using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VacuumController : MonoBehaviour {
    
    public PlayerController player;

    public GameObject nozzle;
    public VacuumTank tank;
    public VacuumElements elements;

    public float nozzleSize = 0.3f;
    public float rayDensity = 20;
    public float spread = 5;
    public float distance = 2;
    public float pullForce = 1;

    List<Vector3> points;
    List<Vector3> targets;

    RaycastHit[] rayHits;
    List<GameObject> hitObjects = new List<GameObject>();
    List<Vector3> hitPositions = new List<Vector3>();

    Dictionary<GameObject, int> onCooldown = new Dictionary<GameObject, int>();
    bool counting;

    internal bool pull;


    void Start() {
    }

    void Update() {
        if (pull) {
            CastRays();
            PullObjects();
            ClearList();
        }

        if (onCooldown.Count > 0 && !counting) StartCoroutine(CountCooldown());
    }

    void CastRays() {
        PlotRayPoints();
        Vector3 dir;
        for (int i = 0; i < points.Count; i++) {
            Vector3 origin = points[i];
            Vector3 target = targets[i] + (Quaternion.AngleAxis(nozzle.transform.eulerAngles.y, Vector3.up) * Vector3.forward * distance);
            dir = Vector3.Normalize(target - origin);

            Debug.DrawRay(origin, dir * distance, new Color(0.5f, 0.5f, 0.5f, 0.04f));
            rayHits = Physics.RaycastAll(origin, dir, distance);
            for (int hit = 0; hit < rayHits.Length; hit++) {
                StoreHit(rayHits[hit]);
            }

        }
    }

    public void CastRays(string tag, out RaycastHit[] rayResults) {
        rayResults = null;
        PlotRayPoints();
        Vector3 dir;
        for (int i = 0; i < points.Count; i++) {
            Vector3 origin = points[i];
            Vector3 target = targets[i] + (Quaternion.AngleAxis(nozzle.transform.eulerAngles.y, Vector3.up) * Vector3.forward * distance);
            dir = Vector3.Normalize(target - origin);

            Color c = Color.black;
            switch (tag) {
                case "fire":
                    c = new Color(1f, 0f, 0f, 0.08f);
                    break;
                case "ice":
                    c = new Color(0.42f, 0.6f, 0.9f, 0.08f);
                    break;
            }
            Debug.DrawRay(origin, dir * distance, c);

            rayHits = Physics.RaycastAll(origin, dir, distance);
            for (int hit = 0; hit < rayHits.Length; hit++) {
                StoreHit(rayHits[hit]);
            }
            rayResults = rayHits;
        }
    }

    void PullObjects() {
        Rigidbody rb;
        Vector3 center;
        List<GameObject> forTank = new List<GameObject>();
        foreach (GameObject hitObject in hitObjects) {
            rb = hitObject.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.velocity = Vector3.zero;

            center = nozzle.transform.position + -nozzle.transform.up * Vector3.Distance(hitObject.transform.position, nozzle.transform.position);
            hitObject.transform.position = Vector3.Lerp(hitObject.transform.position, center, pullForce * 0.5f * Time.deltaTime);
            hitObject.transform.position = Vector3.MoveTowards(hitObject.transform.position, nozzle.transform.position, pullForce * Time.deltaTime);
            
            // Transition to tank:
            if (Vector3.Distance(hitObject.transform.position, nozzle.transform.position) < 0.5f) {
                forTank.Add(hitObject);
            }
        }

        foreach (GameObject obj in forTank) {
            if (tank.AddToTank(obj)) {
                Destroy(obj);
            } else {
                RejectObject(obj);
            }
        }
    }

    void RejectObject(GameObject obj) {
        PutOnCooldown(obj);
        hitObjects.Remove(obj);
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.useGravity = true;
        float force = 3;
        rb.AddForce(new Vector3(Random.Range(-1, 1), 0.2f, Random.Range(-1, 1)) * force, ForceMode.Impulse);
    }

    void StoreHit(RaycastHit hit) {
        GameObject newHitObject = hit.collider.gameObject;
        if (newHitObject.GetComponent<Rigidbody>() == null || onCooldown.ContainsKey(newHitObject)) return;
        foreach (GameObject oldHitObject in hitObjects) {
            if (oldHitObject == newHitObject) return;
        }
        hitObjects.Add(newHitObject);
        hitPositions.Add(hit.point);
    }

    void ClearList() {
        foreach (GameObject hitObject in hitObjects) {
            hitObject.GetComponent<Rigidbody>().useGravity = true;
        }
        hitObjects = new List<GameObject>();
        hitPositions = new List<Vector3>();
    }

    void PlotRayPoints() {
        Vector3 nozzlePos = nozzle.transform.position;
        points = new List<Vector3>();
        targets = new List<Vector3>();
        int ringNum = 10;
        int outerRing = 360;

        // Center points:
        points.Add(nozzlePos);
        targets.Add(nozzlePos + (Quaternion.AngleAxis(nozzle.transform.eulerAngles.y, Vector3.up) * Vector3.forward * distance));

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
                Vector3 dir = points[points.Count - 1] - points[0];
                points[points.Count - 1] = Quaternion.AngleAxis(nozzle.transform.eulerAngles.y, Vector3.up) * dir + points[0];

                targets.Add(new Vector3(
                    nozzlePos.x + radius * spread * Mathf.Sin(angle * Mathf.Deg2Rad),
                    nozzlePos.y + radius * spread * Mathf.Cos(angle * Mathf.Deg2Rad),
                    nozzlePos.z
                ));
                dir = targets[targets.Count - 1] - points[points.Count - 1];
                targets[targets.Count - 1] = Quaternion.AngleAxis(nozzle.transform.eulerAngles.y, Vector3.up) * dir + points[points.Count - 1];
            }
        }
    }

    public void PutOnCooldown(GameObject obj, int cooldown = 90) {
        onCooldown.Add(obj, cooldown);
    }

    IEnumerator CountCooldown() {
        counting = true;
        List<GameObject> toRemove = new List<GameObject>();
        while (onCooldown.Count > 0) {
            foreach(GameObject key in onCooldown.Keys.ToList()) {
                onCooldown[key]--;
                if (onCooldown[key] == 0) onCooldown.Remove(key);
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }

        counting = false;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        if (Application.isPlaying) {
            foreach (Vector3 point in hitPositions) {
                Gizmos.DrawSphere(point, 0.05f);
            }
        }
        
    }
}