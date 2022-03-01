using UnityEngine;

public class VacuumElements : MonoBehaviour
{

    [SerializeField] VacuumController vacuum;

    public GameObject bullet;

    [HideInInspector] public bool use;

    void Start() {
        
    }

    void Update() {
        if (use) Use();
    }

    void Use() {
        switch (vacuum.tank.type.tag) {
            case "PookieBullet":
                ShootBullet();
                break;
            default:
                Debug.LogWarning($"No vacuum action set for type: {vacuum.tank.type.tag}");
                break;
        }
    }

    bool ShootBullet() {
        if (vacuum.tank.GetGauge() < 100) return false;

        vacuum.tank.GaugeSubstract(100);
        GameObject obj = Instantiate(bullet);
        obj.transform.position = vacuum.nozzle.transform.position;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        float force = 25;
        rb.AddForce(vacuum.player.model.transform.forward * force + Vector3.up * 3, ForceMode.Impulse);

        return true;
    }
}
