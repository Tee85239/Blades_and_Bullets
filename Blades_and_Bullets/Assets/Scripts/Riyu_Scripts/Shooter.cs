using UnityEngine;
using Game.Collectibles.Player;
using UnityEngine.InputSystem;
using System;

public class Shooting : MonoBehaviour
{
    [SerializeField] private GameObject slash;
    [SerializeField] private GameObject focusSlash;
    [SerializeField] private GameObject specialSlash;
    [SerializeField] private float power = 1;
    private float shootTime = .3f;
    private float shootTimeMax = .1f;
    private bool specialSlashAnimation = false;
    private float specialSlashTimeMax = .5f;
    private float specialSlashTime = 0f;
    private bool specialSlashReady;
    private GameObject slashInstance;
    private PlayerResourceInventory inventory;  
    private float damage = 2;
    private bool _specialSlashActive;


    void Start()
    {
        GameControllerScript.AbilityActiveStatus += AbilityActiveStatus;
        inventory = Player.Instance.GetComponent<PlayerResourceInventory>();
        _specialSlashActive = true;
    }
    
    void Update()
    {
        shootTime -= Time.deltaTime;
        specialSlashTime -= Time.deltaTime;

        if (specialSlashTime < 0f)
        {
            specialSlash.SetActive(false);
            specialSlashAnimation = false;
            
        }


        if (Player.Instance.moveState != Player.MoveState.Death)
        {   
            if((Keyboard.current.cKey.isPressed || Keyboard.current.commaKey.wasPressedThisFrame) && _specialSlashActive == true)
            {
                SpecialSlash();
            }
            if (shootTime <= 0f && !specialSlashAnimation)
            {
                HandleShoot();
            }
        }
        power = Mathf.Clamp(inventory.Power, 2f, 100f);
        damage = Mathf.Lerp(3f, 20f, Mathf.InverseLerp(2f, 100f, inventory.Power));
    }
    private void AbilityActiveStatus(object sender, EventArgs e)
    {
        _specialSlashActive = true;
    }
    private void SpecialSlash()
    {
        specialSlash.GetComponent<SlashScript>().SetDamage(damage);
        specialSlashAnimation = true;
        specialSlash.SetActive(true);
        specialSlashTime = specialSlashTimeMax;
        _specialSlashActive = false;
    }

    private void HandleShoot()
    {
        if (Keyboard.current.spaceKey.isPressed || Keyboard.current.zKey.isPressed || Keyboard.current.slashKey.isPressed)
        {
            switch (Player.Instance.moveState)
            {
                default:
            case Player.MoveState.Normal:
                slashInstance = Instantiate(slash, transform.position, transform.rotation);
                slashInstance.GetComponent<SlashScript>().SetDamage(damage * 1f);
                break;
            case Player.MoveState.Focused:
                slashInstance = Instantiate(focusSlash, transform.position, transform.rotation);
                slashInstance.GetComponent<SlashScript>().SetDamage(damage * 1.5f);
                break;     
            }
            slashInstance.transform.localScale *= Mathf.Lerp(0.35f, 1f, Mathf.InverseLerp(2f, 100f, power));

            // FireBullets();
            shootTime = shootTimeMax;
        }
    }
    
    private void OnDestroy()
    {
        GameControllerScript.AbilityActiveStatus -= AbilityActiveStatus;
    }
}
