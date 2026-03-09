using UnityEngine;

public class PlayerHealth : Health
{
    protected override void Start()
    {
        base.Start();
    }
    public override void Heal()
    {
        return;
    }
    public override void Die()
    {
        return;
    }
}
