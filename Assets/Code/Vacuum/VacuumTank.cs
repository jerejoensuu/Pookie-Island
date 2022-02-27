using UnityEngine;

public class VacuumTank : MonoBehaviour {
    
    [SerializeField] VacuumController vacuum;

    GameObject type;
    int gauge = 0;

    void Awake() {
        type = new GameObject();
    }

    public bool AddToTank(GameObject obj) {
        if (type.tag == obj.tag) {
            Debug.Log(gauge);
            return GaugeAdd(35);
        }

        type = Instantiate(obj);
        type.SetActive(false);
        SetGauge(type.tag == "PookieBullet" ? 100 : 35);
        return true;
    }

    public void Eject() {
        int size = GaugeSubstract(type.tag == "PookieBullet" ? 100 : 35);
        if (size <= 0) return;
        GameObject obj = Instantiate(type);
        obj.transform.position = vacuum.nozzle.transform.position;
        obj.SetActive(true);

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.useGravity = true;
        float force = 5;
        rb.AddForce(vacuum.player.model.transform.forward * force, ForceMode.Impulse);
        Debug.Log($"Ejected {size}");
    }

    int GetGauge() {
        return gauge;
    }

    void SetGauge(int value) {
        gauge = value;
    }

    bool GaugeAdd(int value) {
        if (gauge < 100) {
            gauge += value;
            if (gauge > 100) gauge = 100;
            return true;
        } else {
            return false;
        }
    }

    int GaugeSubstract(int value) {
        if (gauge > value) {
            gauge -= value;
            return value;
        } else {
            int toEject = gauge;
            gauge = 0;
            return toEject;
        }
    }
}
