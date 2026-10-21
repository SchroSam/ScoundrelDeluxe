using UnityEngine;

public class WeaponButton : MonoBehaviour
{
    public bool isElfWeapon = false;
    public bool isReadied = false;
    public bool isFireBall = false;
    public void WeaponSelected()
    {
        ScoundrelGame.instance.SelectWeaponToggle(this);
    }
}
