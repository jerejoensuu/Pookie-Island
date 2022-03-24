using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VacuumTank : MonoBehaviour {
    
    [SerializeField] VacuumController vacuum;

    public DamageElement.DamageType pookieType {
        get => SaveUtils.currentSaveGame.type;
        set {
            SaveUtils.currentSaveGame.type = value;
            switch (value) {
                case DamageElement.DamageType.FIRE:
                    GaugePookieDisplay.sprite = firePookieUIImage;
                    break;
                case DamageElement.DamageType.ICE:
                    GaugePookieDisplay.sprite = icePookieUIImage;
                    break;
                case DamageElement.DamageType.WATER:
                    GaugePookieDisplay.sprite = waterPookieUIImage;
                    break;
                case DamageElement.DamageType.BULLET:
                    GaugePookieDisplay.sprite = bulletPookieUIImage;
                    break;
            }
        }
    }

    private void Start() {
        //making sure correct values are displayed on scene load
        gauge = gauge;
        pookieType = pookieType;
    }

    public TextMeshProUGUI GaugeUI;
    public Image GaugePookieDisplay;
    public Sprite firePookieUIImage;
    public Sprite icePookieUIImage;
    public Sprite waterPookieUIImage;
    public Sprite bulletPookieUIImage;
    public GameObject firePookiePrefab;
    public GameObject icePookiePrefab;
    public GameObject waterPookiePrefab;
    public GameObject bulletPookiePrefab;
    public GameObject currentlyHeldInTank; //only use for non pookies

    public GameObject carriedObject;

    public int gauge {
        get => SaveUtils.currentSaveGame.gauge;
        private set {
            GaugePookieDisplay.enabled = value != 0;
            SaveUtils.currentSaveGame.gauge = value;
            GaugeUI.text = value.ToString();
        }
    }

    public bool AddCharacterToTank(PullableCharacter character) {
        if (currentlyHeldInTank != null) {
            return false;
        } 
        if (character.type == pookieType && gauge > 0) {
            return GaugeAdd(pookieType == DamageElement.DamageType.BULLET ? 100 : 35);
        }
        pookieType = character.type;
        gauge = pookieType == DamageElement.DamageType.BULLET ? 100 : 35;
        vacuum.player.anim.animator.SetTrigger("vacuumKnockback");
        return true;
    }

    public bool AddObjectToTank(GameObject obj) {
        if (gauge > 0 || currentlyHeldInTank != null) return false;
        currentlyHeldInTank = obj;
        obj.SetActive(false);
        vacuum.player.anim.animator.SetTrigger("vacuumKnockback");
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

        vacuum.player.anim.animator.SetTrigger("vacuumKnockback");
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
        switch (pookieType) {
            case DamageElement.DamageType.FIRE:
                size = GaugeSubstract(35);
                obj = Instantiate(firePookiePrefab, vacuum.nozzle.transform.position, Quaternion.identity);
                break;
            case DamageElement.DamageType.ICE:
                size = GaugeSubstract(35);
                obj = Instantiate(icePookiePrefab, vacuum.nozzle.transform.position, Quaternion.identity);
                break;
            case DamageElement.DamageType.WATER:
                size = GaugeSubstract(35);
                obj = Instantiate(waterPookiePrefab, vacuum.nozzle.transform.position, Quaternion.identity);
                break;
            case DamageElement.DamageType.BULLET:
                size = GaugeSubstract(100);
                obj = Instantiate(bulletPookiePrefab, vacuum.nozzle.transform.position, Quaternion.identity);
                break;
            default:
                return;
        }
        vacuum.player.anim.animator.SetTrigger("shoot");
        obj.SetActive(true);
        vacuum.PutOnCooldown(obj, 120);
    }
    public void EjectObject() {
        gauge = 0;
        currentlyHeldInTank.transform.position = vacuum.nozzle.transform.position;
        currentlyHeldInTank.SetActive(true);
        vacuum.PutOnCooldown(currentlyHeldInTank, 120);
        Rigidbody rb = currentlyHeldInTank.GetComponent<Rigidbody>();
        rb.useGravity = true;
        float force = 5;
        rb.AddForce(vacuum.player.model.transform.forward * force, ForceMode.Impulse);
        currentlyHeldInTank = null;
        vacuum.player.anim.animator.SetTrigger("shoot");
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

    public void ResetTank() {
        gauge = 0;
    }
}
