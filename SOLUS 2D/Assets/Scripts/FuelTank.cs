using UnityEngine;
using System;
using UnityEngine.UI;

public class FuelTank : MonoBehaviour
{
    public Sprite[] fuelLevels;

    private Movement movementScript;
    private float fuelRemaining;
    private float totalFuel;
    private int listPos;

    private GameObject fuelTank;

    void Start()
    {
        fuelTank = GameObject.FindGameObjectWithTag("FuelTank");
    }

    void Update()
    {
        movementScript = gameObject.GetComponent<Movement>();

        fuelRemaining = movementScript.fuelRemaining;
        totalFuel = movementScript.fuel;

        listPos = (int)Math.Floor(fuelRemaining * (fuelLevels.Length / totalFuel));
        fuelTank.GetComponent<Image>().sprite = fuelLevels[listPos];
    }
}
