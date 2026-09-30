using System;
using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    
    [SerializeField] private int health;
    [SerializeField] private PlayerController player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = 2;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Axe")
        {
            health--;
        }

        if (health == 0)
        {
            this.gameObject.SetActive(false);
            player.CallAxeBack();
        }
    }
}
