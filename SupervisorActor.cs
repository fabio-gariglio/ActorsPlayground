using Akka.Actor;
using Akka.Event;

namespace ActorsPlayground
{
    public class SupervisorActor : UntypedActor
    {
        protected ILoggingAdapter Log = Context.GetLogger();

        protected override void PreStart() => Log.Info("Supervisor started.");
        protected override void PostStop() => Log.Info("Supervisor stopeed.");

        protected override void OnReceive(object message)
        {
            Log.Info($"Received message: {message}");
            Log.Info(Self.ToString());
        }

        public static Props Props() => Akka.Actor.Props.Create<SupervisorActor>();
    }
}
