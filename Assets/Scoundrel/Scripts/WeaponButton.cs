using UnityEngine;

public class WeaponButton : MonoBehaviour
{
    public bool isElfWeapon = false;
    public bool isReadied = false;
    private ScoundrelGame gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<ScoundrelGame>();
    }

    public void weaponSelected()
    {
        gameManager.SelectWeaponToggle(this);
    }
}
