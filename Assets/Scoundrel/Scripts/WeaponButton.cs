using UnityEngine;

public class WeaponButton : MonoBehaviour
{
    public bool isElfWeapon = false;
    public bool isReadied = false;
    public void weaponSelected()
    {
        ScoundrelGame.instance.SelectWeaponToggle(this);
    }
}
