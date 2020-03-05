using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Maroon.Physics;
using TMPro;
using UnityEngine;

public class VoltmeterDifferences : MonoBehaviour
{
    public VoltmeterMeasuringPoint positiveMeasuringPoint;
    public VoltmeterMeasuringPoint negativeMeasuringPoint;

    private CoulombLogic _coulombLogic;
    public TextMeshProUGUI textMeshProGUI;

    [Header("Assessment System")]
    public QuantityFloat currentValue;
    
    // Start is called before the first frame update
    void Start()
    {
        var simControllerObject = GameObject.Find("CoulombLogic");
        if (simControllerObject)
            _coulombLogic = simControllerObject.GetComponent<CoulombLogic>();
        Debug.Assert(_coulombLogic != null);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!textMeshProGUI) return;
        if (!positiveMeasuringPoint.isActiveAndEnabled || !negativeMeasuringPoint.isActiveAndEnabled)
            textMeshProGUI.text = "--- " + GetCurrentUnit();
        else
        {
            textMeshProGUI.text = GetDifference() + " " + GetCurrentUnit();
        }
    }
    
    private string GetDifference(){
        currentValue = positiveMeasuringPoint.GetPotentialInMicroVolt() - negativeMeasuringPoint.GetPotentialInMicroVolt();
        return GetCurrentFormattedString();
    }

    private string GetCurrentFormattedString()
    {
        float check = currentValue;
        for (var cnt = 0; Mathf.Abs(check) < 1f && cnt < 2; ++cnt)
        {
            check *= Mathf.Pow(10, 3);
        }

//        Debug.Log("START: " + _currentValue.ToString("F") + " - END: "+ check.ToString("F"));
        return check.ToString("F");   
    }

    private string GetCurrentUnit()
    {
        var unit = "V";
        var check = currentValue;
        if (check > 1f)
            return unit;
        check *= Mathf.Pow(10, 3);
        if (check > 1f)
            return "m" + unit;
        return "\u00B5" + unit;
    }
}
