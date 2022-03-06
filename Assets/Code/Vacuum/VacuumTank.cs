using UnityEngine;

public class VacuumTank : MonoBehaviour {
    
    [SerializeField] VacuumController vacuum;

    [HideInInspector] public DamageElement.DamageType type;
    int gauge = 0;
    public GameObject firePookiePrefab;
    public GameObject icePookiePrefab;
    public GameObject waterPookiePrefab;
    public GameObject bulletPookiePrefab;
    public GameObject currentlyHeldInTank; //only use for non pookies

    public GameObject carriedObject;

    public bool AddCharacterToTank(PullableCharacter character) {
        if (currentlyHeldInTank != null) {
            return false;
        } 
        if (character.type == type) {
            return GaugeAdd(type == DamageElement.DamageType.BULLET ? 100 : 35);
        }
        type = character.type;
        SetGauge(type == DamageElement.DamageType.BULLET ? 100 : 35);
        return true;
    }

    public bool AddObjectToTank(GameObject obj) {
        if (gauge > 0) return false;
        currentlyHeldInTank = obj;
        SetGauge(100);
        obj.SetActive(false);
        return true;
    }

    public bool CarryObject(GameObject obj, Rigidbody rb) {
        if (carriedObject != null) return false;
        carriedObject = obj;
        // obj.transform.position = vacuum.nozzle.transform.position;
        obj.transform.parent = vacuum.player.model.transform;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.useGravity = false;
        obj.GetComponent<Collider>().enabled = false;

        vacuum.pull = false;
        if (!vacuum.player.inputReader.manualAiming) vacuum.player.vcamera.aiming = false;

        return true;
    }

    public void Eject() {
        if (carriedObject != null) DropObject();
        else if (currentlyHeldInTank == null) EjectPookie();
        else EjectObject();
    }

    public void EjectPookie() {
        if (gauge == 0) return;
        int size;
        GameObject obj;
        switch (type) {
            case DamageElement.DamageType.FIRE:
                size = GaugeSubstract(35);
                obj = Instantiate(firePookiePrefab);
                break;
            case DamageElement.DamageType.ICE:
                size = GaugeSubstract(35);
                obj = Instantiate(icePookiePrefab);
                break;
            case DamageElement.DamageType.WATER:
                size = GaugeSubstract(35);
                obj = Instantiate(waterPookiePrefab);
                break;
            case DamageElement.DamageType.BULLET:
                size = GaugeSubstract(100);
                obj = Instantiate(bulletPookiePrefab);
                break;
            default:
                return;
        }
        obj.transform.position = vacuum.nozzle.transform.position;
        obj.SetActive(true);
        vacuum.PutOnCooldown(obj, 120);
    }
    public void EjectObject() {
        SetGauge(0);
        currentlyHeldInTank.transform.position = vacuum.nozzle.transform.position;
        currentlyHeldInTank.SetActive(true);
        vacuum.PutOnCooldown(currentlyHeldInTank, 120);
        Rigidbody rb = currentlyHeldInTank.GetComponent<Rigidbody>();
        rb.useGravity = true;
        float force = 5;
        rb.AddForce(vacuum.player.model.transform.forward * force, ForceMode.Impulse);
        currentlyHeldInTank = null;
    }

    void DropObject() {
        Rigidbody rb = carriedObject.GetComponent<Rigidbody>();
        carriedObject.transform.parent = null;
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
        carriedObject.GetComponent<Collider>().enabled = true;

        float force = 5;
        rb.velocity = vacuum.player.movement.controller.velocity;
        rb.AddForce(vacuum.player.model.transform.forward * force, ForceMode.Impulse);

        carriedObject = null;
    }

    public int GetGauge() {
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
        }
        return false;
    }

    public int GaugeSubstract(int value) {
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
