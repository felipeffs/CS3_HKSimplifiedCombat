using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    int currentHealthPoints;
    [SerializeField]
    int maxHealthPoints;

    public int CurrentHealthPoints { get => currentHealthPoints; set => currentHealthPoints = value; }

    void Start()
    {
        currentHealthPoints = maxHealthPoints;
    }

    public void TakeDamage(int damage)
    {

        int previewHealthPoints = currentHealthPoints - damage;
        if (previewHealthPoints > 0)
        {
            currentHealthPoints = previewHealthPoints;
        }
        else
        {
            Debug.Log("Morreu!");
            //Morreu
        }
    }
}
