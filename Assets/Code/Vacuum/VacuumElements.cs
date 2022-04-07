using System.Collections;  
using Bolt;  
using UnityEngine; 
  
public class VacuumElements : MonoBehaviour {  
  
    [SerializeField] VacuumController vacuum;  
  
    public GameObject bullet;  
    public GameObject waterCollider;  
  
    public GameObject fireEffect;  
    public GameObject waterEffectForward; 
    public GameObject waterEffectDown; 
  
    [HideInInspector] public bool use;  
    bool timerRunning, watering;  
    public float timerSpeed = 20; 
 
    [SerializeField] float JetGravity = 0.225f; 
    [SerializeField] float JetForce = 70; 
    [HideInInspector] public float jetGravity = 1; 
 
    void Update() {  
        if (use) Use();  
    }  
  
    void Use() {  
        switch (vacuum.tank.pookieType) {  
            case DamageElement.DamageType.BULLET:  
                ShootBullet();  
                break;  
            case DamageElement.DamageType.FIRE:  
                SprayFire();  
                break;  
            case DamageElement.DamageType.ICE:  
                SprayIce();  
                break;  
            case DamageElement.DamageType.WATER:  
                SprayWater();  
                break;  
            default:  
                Debug.LogWarning($"No vacuum action set for type: {vacuum.tank.pookieType}");  
                break;  
        }  
    }  
  
    void ShootBullet() { 
        // if (vacuum.tank.gauge < 25) return;  
 
        vacuum.elements.use = false; 
        vacuum.player.anim.animator.SetTrigger("shoot");  
  
        vacuum.tank.GaugeSubstract(25);  
        GameObject obj = Instantiate(bullet);  
        obj.transform.position = vacuum.nozzle.transform.position;  
  
        Rigidbody rb = obj.GetComponent<Rigidbody>();  
        float force = 50;  
        rb.AddForce(vacuum.player.model.transform.forward * force, ForceMode.Impulse);  
    }  
  
    void SprayFire() {  
        if (vacuum.tank.gauge <= 0) return;  
        fireEffect.SetActive(true); 
        if (!timerRunning) StartCoroutine(Timer());  
  
        foreach(RaycastHit hit in vacuum.CastShootingRays("fire")) {  
            CustomEvent.Trigger(hit.transform.gameObject, "DestroyFire");  
        }  
    }  
  
    void SprayIce() {  
        if (vacuum.tank.gauge <= 0) return;  
        if (!timerRunning) StartCoroutine(Timer());  
  
        foreach(RaycastHit hit in vacuum.CastShootingRays("ice")) {  
            CustomEvent.Trigger(hit.transform.gameObject, "DestroyIce");  
        }  
  
    }  
  
    void SprayWater() {  
        if (vacuum.tank.gauge <= 0) return;  
        if (!timerRunning) StartCoroutine(Timer());  
        if (!watering) { 
            if (vacuum.player.movement.grounded) { 
                StartCoroutine(ForwardJet()); 
            } else { 
                StartCoroutine(DownwardJet()); 
            }  
        }  
    }  
  
    IEnumerator ForwardJet() {  
        watering = true; 
        waterEffectForward.SetActive(true); 
          
        while(use && vacuum.tank.gauge > 0) {  
            GameObject obj = Instantiate(waterCollider);  
            obj.transform.position = vacuum.nozzle.transform.position;  
  
            Rigidbody rb = obj.GetComponent<Rigidbody>();  
            float force = 70;  
            rb.AddForce(vacuum.player.model.transform.forward * force, ForceMode.Impulse);  
  
            yield return new WaitForSeconds(.2f);  
        }  
  
        watering = false;  
    }  
 
    IEnumerator DownwardJet() {  
        watering = true; 
        jetGravity = JetGravity; 
        waterEffectDown.SetActive(true); 
 
        while(use && vacuum.tank.gauge > 0) {  
            GameObject obj = Instantiate(waterCollider);  
            obj.transform.position = vacuum.nozzle.transform.position;  
  
            Rigidbody rb = obj.GetComponent<Rigidbody>();  
            float force = JetForce;  
            rb.AddForce(-vacuum.player.model.transform.up * force, ForceMode.Impulse); 
 
            yield return new WaitForSeconds(.2f);  
        }  
  
        watering = false; 
        jetGravity = 1; 
    }  
  
    IEnumerator Timer() {  
        timerRunning = true;  
        vacuum.player.anim.animator.SetBool("vacuum", true);  
  
        while(use && vacuum.tank.gauge > 0) {  
            vacuum.tank.GaugeSubstract(1);  
            yield return new WaitForSeconds(Time.deltaTime * timerSpeed);  
        }  
  
        timerRunning = false;  
        vacuum.player.anim.animator.SetBool("vacuum", false);  
        fireEffect.SetActive(false); 
        waterEffectForward.SetActive(false); 
        waterEffectDown.SetActive(false); 
    }  
}