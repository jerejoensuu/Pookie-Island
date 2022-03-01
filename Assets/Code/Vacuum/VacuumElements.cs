using System.Collections;
using UnityEngine;

public class VacuumElements : MonoBehaviour
{

    [SerializeField] VacuumController vacuum;

    public GameObject bullet;

    [HideInInspector] public bool use;
    bool timerRunning;
    public float timerSpeed = 20;

    void Start() {
        
    }

    void Update() {
        if (use) Use();
        if (!use) StopCoroutine(Timer());
    }

    void Use() {
        switch (vacuum.tank.type.tag) {
            case "PookieBullet":
                ShootBullet();
                break;
            case "PookieFire":
                SprayFire();
                break;
            case "PookieIce":
                SprayIce();
                break;
            default:
                Debug.LogWarning($"No vacuum action set for type: {vacuum.tank.type.tag}");
                break;
        }
    }

    void ShootBullet() {
        if (vacuum.tank.GetGauge() < 100) return;

        vacuum.tank.GaugeSubstract(100);
        GameObject obj = Instantiate(bullet);
        obj.transform.position = vacuum.nozzle.transform.position;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        float force = 25;
        rb.AddForce(vacuum.player.model.transform.forward * force + Vector3.up * 3, ForceMode.Impulse);
    }

    void SprayFire() {
        if (vacuum.tank.GetGauge() <= 0) return;
        if (!timerRunning) StartCoroutine(Timer());

        RaycastHit[] rayResults;
        vacuum.CastRays("fire", out rayResults);

        foreach(RaycastHit hit in rayResults) {
            // Do something
        }

    }

    void SprayIce() {
        if (vacuum.tank.GetGauge() <= 0) return;
        if (!timerRunning) StartCoroutine(Timer());

        RaycastHit[] rayResults;
        vacuum.CastRays("ice", out rayResults);

        foreach(RaycastHit hit in rayResults) {
            // Do something
        }

    }

    IEnumerator Timer() {
        timerRunning = true;

        while(vacuum.tank.GetGauge() > 0) {
            vacuum.tank.GaugeSubstract(1);
            yield return new WaitForSeconds(Time.deltaTime * timerSpeed);
        }

        timerRunning = false;
    }
}
