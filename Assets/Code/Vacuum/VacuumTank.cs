using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumTank : MonoBehaviour {
    
    [SerializeField] VacuumController vacuum;

    GameObject type;
    int gauge = 0;

    public bool AddToTank(GameObject obj) {
        if (type.tag == obj.tag) {
            return GaugeAdd(35);
        }

        type = obj;
        SetGauge(type.tag == "PookieBullet" ? 100 : 35);
        return true;
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
            return true;
        } else {
            return false;
        }
    }

    bool GaugeSubstract(int value) {
        throw new System.NotImplementedException();
    }
}
