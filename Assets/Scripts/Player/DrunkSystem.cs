using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrunkSystem : MonoBehaviour
{
    public float startingDrunk = 100;
    public Image DrunkBar;
    public float currentDrunk;
    public TMP_Text DrunkText;
    public bool IsDrunk;
    public ParticleSystem drunkParticle;
    PlayerStats player;
    public BuffData drunkBuffData; // Reference to the BuffData scriptable object for drunk buff

    private void Start()
    {
        currentDrunk = startingDrunk;
        player = GetComponent<PlayerStats>(); // Assuming PlayerStats is attached to the same GameObject
        UpdateDrunkUI();
    }

    private void Update()
    {
        RegenerateDrunk();
        CheckForDrunk();
    }

    private void RegenerateDrunk()
    {
        if (currentDrunk < player.Stats.maxDrunk)
        {
            currentDrunk += player.Stats.drunkAutoRegen * Time.deltaTime;
            currentDrunk = Mathf.Clamp(currentDrunk, 0f, player.Stats.maxDrunk);
            UpdateDrunkUI();
        }
    }

    private void UpdateDrunkUI()
    {
        if (DrunkBar != null)
        {
            DrunkBar.fillAmount = currentDrunk / player.Stats.maxDrunk;
        }

        if (DrunkText != null)
        {
            DrunkText.text = ((int)currentDrunk) + "/" + player.Stats.maxDrunk;
        }
    }

    private void CheckForDrunk()
    {
        if (IsDrunk)
        {
            Drunk();
        }

        if (Input.GetKeyDown(KeyCode.Z) && currentDrunk > 99)
        {
            ToggleDrunkState();
        }

        if (IsDrunk && currentDrunk < 1)
        {
            ToggleDrunkState();
        }
    }

    public void DoDrunk()
    {
        if (currentDrunk > 99)
        {
            ToggleDrunkState();
        }
    }

    private void ToggleDrunkState()
    {
        IsDrunk = !IsDrunk;
        if (IsDrunk)
        {
            drunkParticle.Play();
            // Add Drunk buff here when drunk state is activated
            player.AddBuff(drunkBuffData);
        }
        else
        {
            drunkParticle.Stop();
            // Remove Drunk buff here when drunk state is deactivated
            player.RemoveBuff(drunkBuffData);
        }
    }

    private void Drunk()
    {
        currentDrunk -= player.Stats.soberRate * Time.deltaTime;
        if (currentDrunk < 0)
        {
            currentDrunk = 0;
        }
    }
}
