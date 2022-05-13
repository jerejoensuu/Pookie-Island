using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VacuumTank : MonoBehaviour {
    
    [SerializeField] VacuumController vacuum;
    public GameObject model;
    private SkinnedMeshRenderer smRenderer;
    public Material MaterialEmpty;
    public Material MaterialBullet;
    public Material MaterialFire;
    public Material MaterialWater;
    private Material[] rendererMats;

    public DamageElement.DamageType pookieType {
        get => SaveUtils.currentSaveGame.type;
        set {
            SaveUtils.currentSaveGame.type = value;
            switch (value) {
                case DamageElement.DamageType.FIRE:
                    GaugePookieDisplay.sprite = firePookieUIImage;
                    PookieAmount.sprite = firePookieMaskUI;
                    break;
                case DamageElement.DamageType.ICE:
                    GaugePookieDisplay.sprite = icePookieUIImage;
                    PookieAmount.sprite = icePookieMaskUI;
                    break;
                case DamageElement.DamageType.WATER:
                    GaugePookieDisplay.sprite = waterPookieUIImage;
                    PookieAmount.sprite = waterPookieMaskUI;
                    break;
                case DamageElement.DamageType.BULLET:
                    GaugePookieDisplay.sprite = bulletPookieUIImage;
                    PookieAmount.sprite = bulletPookieMaskUI;
                    break;
            }
        }
    }

    private void Awake() {
        //making sure correct values are displayed on scene load
        gauge = gauge;
        pookieType = pookieType;
        originalSize = mask.rectTransform.rect.height;
        //Debug.Log("Start: original size = " + originalSize);
        initialized = true;
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

        
        if (model) {
            smRenderer = model.GetComponent<SkinnedMeshRenderer>();
            rendererMats = smRenderer.materials;
        } 
        rendererMats[1] = MaterialEmpty;
        switch (pookieType) {
            case DamageElement.DamageType.BULLET:
                rendererMats[1] = MaterialBullet;
                break;
            case DamageElement.DamageType.FIRE:
                rendererMats[1] = MaterialFire;
                break;
            case DamageElement.DamageType.WATER:
                rendererMats[1] = MaterialWater;
                break;
        }
        smRenderer.materials = rendererMats;
    }

    void Update() {
        if (gauge == 0 && rendererMats[1] != MaterialEmpty) {
            rendererMats[1] = MaterialEmpty;
            smRenderer.materials = rendererMats;
        }
    }


    public TextMeshProUGUI GaugeUI;
    public Image GaugePookieDisplay;
    public Image mask;
    public Image PookieAmount;
    float originalSize;
    public Sprite firePookieUIImage;
    public Sprite icePookieUIImage;
    public Sprite waterPookieUIImage;
    public Sprite bulletPookieUIImage;
    public Sprite firePookieMaskUI;
    public Sprite icePookieMaskUI;
    public Sprite waterPookieMaskUI;
    public Sprite bulletPookieMaskUI;
    public GameObject firePookiePrefab;
    public GameObject icePookiePrefab;
    public GameObject waterPookiePrefab;
    public GameObject bulletPookiePrefab;
    public GameObject currentlyHeldInTank; //only use for non pookies
    public GameObject carriedObject;
    private bool initialized = false;
    public int gauge {
        get => SaveUtils.currentSaveGame.gauge;
        private set {
            GaugePookieDisplay.enabled = value != 0;
            PookieAmount.enabled = value != 0;
            SaveUtils.currentSaveGame.gauge = value;
            GaugeUI.text = value.ToString();
            SetValue(value);
        }
    }

        public void SetValue(float value)
    {
        if (!initialized) return;
        if (value > 100) value = 100;
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalSize * value/100);
        //Debug.Log("setting my mask to " + value + " originalSize = " + originalSize);
    }

    public bool AddCharacterToTank(PullableCharacter character) {
        if (currentlyHeldInTank != null) {
            return false;
        } 
        if (character.type == pookieType && gauge > 0) {
            switch (pookieType) {
                case DamageElement.DamageType.BULLET:
                    return GaugeAdd(25);
                case DamageElement.DamageType.FIRE:
                    return GaugeAdd((int)(35 * character.gameObject.transform.localScale.x * 1.54));
                default:
                    return GaugeAdd((int)(35 * character.gameObject.transform.localScale.x));
            }
        }
        EjectAll();
        pookieType = character.type;
        switch (pookieType) {
            case DamageElement.DamageType.BULLET:
                gauge = 25;
                break;
            case DamageElement.DamageType.FIRE:
                gauge = (int)(35 * character.gameObject.transform.localScale.x * 1.54f);
                break;
            default:
                gauge = (int)(35 * character.gameObject.transform.localScale.x);
                break;
        }
        vacuum.player.anim.animator.SetTrigger("vacuumKnockback");
        switch (pookieType) {
            case DamageElement.DamageType.BULLET:
                rendererMats[1] = MaterialBullet;
                break;
            case DamageElement.DamageType.FIRE:
                rendererMats[1] = MaterialFire;
                break;
            case DamageElement.DamageType.WATER:
                rendererMats[1] = MaterialWater;
                break;
        }
        smRenderer.materials = rendererMats;
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

    void EjectAll() {
        int toSub = pookieType == DamageElement.DamageType.BULLET ? 25 : 35;
        while (gauge > 0) {
            Eject(false);
        }
    }

    public void Eject(bool playAnimation = true) {
        if (carriedObject != null) DropObject();
        else if (currentlyHeldInTank == null) EjectPookie(playAnimation);
        else EjectObject(playAnimation);
    }

    public void EjectPookie(bool playAnimation = true) {
        if (gauge == 0) return;
        int size;
        GameObject obj = new GameObject();
        switch (pookieType) {
            case DamageElement.DamageType.FIRE:
                size = GaugeSubstract(35);
                if (size < 10) break;
                obj = Instantiate(firePookiePrefab, vacuum.nozzle.transform.position, Quaternion.identity);
                obj.transform.localScale = new Vector3(size / 35f * 0.65f, size / 35f * 0.65f, size / 35f * 0.65f);
                break;
            case DamageElement.DamageType.ICE:
                size = GaugeSubstract(35);
                if (size <= 5) break;
                obj = Instantiate(icePookiePrefab, vacuum.nozzle.transform.position, Quaternion.identity);
                obj.transform.localScale = new Vector3(size / 35f, size / 35f, size / 35f);
                break;
            case DamageElement.DamageType.WATER:
                size = GaugeSubstract(35);
                if (size <= 5) break;
                obj = Instantiate(waterPookiePrefab, vacuum.nozzle.transform.position, Quaternion.identity);
                obj.transform.localScale = new Vector3(size / 35f, size / 35f, size / 35f);
                break;
            case DamageElement.DamageType.BULLET:
                size = GaugeSubstract(25);
                obj = Instantiate(bulletPookiePrefab, vacuum.nozzle.transform.position, Quaternion.identity);
                break;
            default:
                return;
        }
        if (playAnimation) vacuum.player.anim.animator.SetTrigger("shoot");
        obj.SetActive(true);
        vacuum.PutOnCooldown(obj, 50);
    }
    public void EjectObject(bool playAnimation = true) {
        gauge = 0;
        currentlyHeldInTank.transform.position = vacuum.nozzle.transform.position;
        currentlyHeldInTank.SetActive(true);
        vacuum.PutOnCooldown(currentlyHeldInTank, 120);
        Rigidbody rb = currentlyHeldInTank.GetComponent<Rigidbody>();
        rb.useGravity = true;
        float force = 5;
        rb.AddForce(vacuum.player.model.transform.forward * force, ForceMode.Impulse);
        currentlyHeldInTank = null;
        if (playAnimation) vacuum.player.anim.animator.SetTrigger("shoot");
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
