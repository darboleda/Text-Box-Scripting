using System.Collections;

public abstract class ScriptCommand<TApplication, TActivation> where TApplication : ICommandApplicationContext
                                                               where TActivation : ICommandActivationContext
{
    public virtual IEnumerator Activate(TActivation context) { yield break; }
    public virtual bool Skip(TActivation context) { return true; }

    public virtual void Apply(TApplication context) { }
    public virtual string Evaluate(TApplication context) { return string.Empty; }
}

public abstract class ScriptCommand : ScriptCommand<ICommandApplicationContext, ICommandActivationContext> { }
