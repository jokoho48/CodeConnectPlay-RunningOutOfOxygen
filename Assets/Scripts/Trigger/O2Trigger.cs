public class O2Trigger : Triggerable
{
    public override void OnTrigger()
    {
        base.OnTrigger();
        if (!GameManager.Instance.PlayerAlive) return;
        GameManager.Instance.oxygenAmount = 100;
        GameManager.Instance.UpdateOxygenDisplay();
    }
}
