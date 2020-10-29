public class O2Trigger : Triggerable
{
    public override void DoTrigger()
    {
        base.DoTrigger();
        if (!GameManager.Instance.PlayerAlive) return;
        GameManager.Instance.oxygenAmount = 100;
        GameManager.Instance.UpdateOxygenDisplay();
    }
}
