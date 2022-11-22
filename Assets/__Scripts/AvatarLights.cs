using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarLights : MonoBehaviour
{
    public GameObject MenuLights;
    public GameObject[] CardLights;

    public void SetCardLightsOn(int rarity)
    {
        MenuLights.SetActive(false);
        foreach (GameObject light in CardLights) light.SetActive(false);
        CardLights[rarity].SetActive(true);
    }
    public void SetMenuLightsOn()
    {
        MenuLights.SetActive(true);
        foreach (GameObject light in CardLights) light.SetActive(false);
    }
}
