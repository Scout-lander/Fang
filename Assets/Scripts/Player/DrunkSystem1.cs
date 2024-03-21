using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DrunkSystem : MonoBehaviour
{
    public float maxDrunk = 100;
    public float startingDrunk = 100;
    public float DrunkRegenRate = 5;
    public Image DrunkBar;
    public float currentDrunk;
    public TMP_Text DrunkText;
    public float soberRate = 25f;
    public bool IsDrunk;
    public ParticleSystem drunkPartical;
    PlayerStats player;

    // Start is called before the first frame update
    void Start()
    {
        currentDrunk = startingDrunk;
        UpdateDrunkUI();
    }

    // Update is called once per frame
    void Update()
    {
        RegenerateDrunk();
        CheckForDrunk(); // Call the method here if it's meant to be called in Update

    }

    private void RegenerateDrunk()
    {
        if(currentDrunk < maxDrunk)
        {
            currentDrunk += DrunkRegenRate * Time.deltaTime;
            currentDrunk = Mathf.Clamp(currentDrunk, 0f, maxDrunk);
            UpdateDrunkUI();
        }
    }

    public void UpdateDrunkUI()
    {
        if(DrunkBar != null)
        {
            DrunkBar.fillAmount = currentDrunk / maxDrunk;
        }

        if(DrunkText != null)
        {
            DrunkText.text = (int)currentDrunk + "/" + maxDrunk;
        }
    }

    public void CheckForDrunk()
    {
        if (IsDrunk)
            Drunk();

        if(Input.GetKeyDown(KeyCode.Z) && currentDrunk > 99)
        {
            IsDrunk = !IsDrunk;
            drunkPartical.Play();
        }

        if(IsDrunk && currentDrunk < 1)
            IsDrunk = false;
            drunkPartical.Stop();

    }
    public void Drunk()
    {
        currentDrunk -= soberRate * Time.deltaTime;
            if (currentDrunk < 0)
                currentDrunk = 0;
    }
}
