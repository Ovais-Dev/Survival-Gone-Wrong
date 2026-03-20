using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerShooting playerShooting;

    [Header("Weapons")]
    [SerializeField] private List<WeaponData> weapons = new List<WeaponData>();
    [SerializeField] private int currentWeaponIndex = 0;

    [Header("Input")]
    [SerializeField] private InputActionReference scrollAction;

    [Header("Settings")]
    [SerializeField] private float scrollSwitchCooldown = 0.15f;

    private float lastScrollTime;

    private void Start()
    {
        if (playerShooting == null)
            playerShooting = GetComponent<PlayerShooting>();

        if (weapons == null || weapons.Count == 0)
        {
            Debug.LogWarning("WeaponHandler: No weapons assigned.");
            return;
        }

        currentWeaponIndex = Mathf.Clamp(currentWeaponIndex, 0, weapons.Count - 1);
        EquipWeapon(currentWeaponIndex);
    }

    private void OnEnable()
    {
        if (scrollAction != null)
            scrollAction.action.Enable();
    }

    private void OnDisable()
    {
        if (scrollAction != null)
            scrollAction.action.Disable();
    }

    private void Update()
    {
        HandleScrollInput();
        HandleNumberKeyInput();
    }

    private void HandleScrollInput()
    {
        if (scrollAction == null || weapons == null || weapons.Count <= 1)
            return;

        if (Time.time < lastScrollTime + scrollSwitchCooldown)
            return;

        Vector2 scrollValue = scrollAction.action.ReadValue<Vector2>();

        if (scrollValue.y > 0.1f)
        {
            NextWeapon();
            lastScrollTime = Time.time;
        }
        else if (scrollValue.y < -0.1f)
        {
            PreviousWeapon();
            lastScrollTime = Time.time;
        }
    }
    private void HandleNumberKeyInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            EquipWeapon(0);

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            EquipWeapon(1);

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            EquipWeapon(2);
    }
    public void NextWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
        EquipWeapon(currentWeaponIndex);
    }

    public void PreviousWeapon()
    {
        currentWeaponIndex--;

        if (currentWeaponIndex < 0)
            currentWeaponIndex = weapons.Count - 1;

        EquipWeapon(currentWeaponIndex);
    }

    public void EquipWeapon(int index)
    {
        if (weapons == null || weapons.Count == 0)
            return;

        if (index < 0 || index >= weapons.Count)
            return;

        currentWeaponIndex = index;
        WeaponData selectedWeapon = weapons[currentWeaponIndex];

        if (playerShooting != null)
            playerShooting.SetWeapon(selectedWeapon);

        Debug.Log("Equipped Weapon: " + selectedWeapon.name);
    }

    public WeaponData GetCurrentWeapon()
    {
        if (weapons == null || weapons.Count == 0)
            return null;

        return weapons[currentWeaponIndex];
    }
}