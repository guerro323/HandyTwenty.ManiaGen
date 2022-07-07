namespace ManiaGen.HTManialink;

public partial class MlComposer : MlComponent
{
    public override void Entry()
    {
        PreMainLabel.Invoke();
        MainLabel.Invoke();
        while (true)
        {
            Thread.Yield(); // poor-man yield (I will perhaps convert the Entry point to an async task so I can do await Task.Yield()?)
            
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