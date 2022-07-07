namespace ManiaGen.HTManialink;

public partial class MlComposer : MlComponent
{
    public override void Entry()
    {
        PreMainLabel.Invoke();
        MainLabel.Invoke();
        while (true)
        {
            PreUpdateLabel.Invoke();
            foreach (var ev in Nod.PendingEvents)
            {
                PendingEventLabel.Invoke(ev);
            }
            UpdateLabel.Invoke();
            AnimateLabel.Invoke();
        }
    }
}