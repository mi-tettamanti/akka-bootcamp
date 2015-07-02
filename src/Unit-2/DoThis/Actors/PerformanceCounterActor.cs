using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace ChartApp.Actors
{
    /// <summary>
    /// Actor responsible for monitoring a specific <see cref="PerformanceCounter" />
    /// </summary>
    public class PerformanceCounterActor : UntypedActor
    {
        private readonly string seriesName;
        private readonly Func<PerformanceCounter> performanceCounterGenerator;
        private PerformanceCounter counter;

        private readonly HashSet<IActorRef> subscriptions = new HashSet<IActorRef>();
        private readonly ICancelable cancelPublishing = new Cancelable(Context.System.Scheduler);

        public PerformanceCounterActor(string seriesName, Func<PerformanceCounter> performanceCounterGenerator)
        {
            this.seriesName = seriesName;
            this.performanceCounterGenerator = performanceCounterGenerator;
        }

        #region Actor lifecycle methods

        protected override void PreStart()
        {
            base.PreStart();

            counter = performanceCounterGenerator();
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(250), Self,
                new GatherMetrics(), Self, cancelPublishing);
        }

        protected override void PostStop()
        {
            try
            {
                // terminate the scheduled task

                cancelPublishing.Cancel(false);
                counter.Dispose();
            }
            catch
            {
                // don't care about additional "ObjectDisposed" exceptions
            }
            finally
            {
                base.PostStop();
            }
        }

        #endregion

        protected override void OnReceive(object message)
        {
            if (message is GatherMetrics)
            {
                // publish latest counter value to all subsccribers

                var metric = new Metric(seriesName, counter.NextValue());

                foreach (var sub in subscriptions)
                    sub.Tell(metric);
            }
            else if (message is SubscribeCounter)
            {
                // add a subscription for this counter
                // (it's parent's job to filter by counter types)

                var sc = message as SubscribeCounter;
                subscriptions.Add(sc.Subscriber);
            }
            else if (message is UnsubscribeCounter)
            {
                // remove a subscription from this counter

                var uc = message as UnsubscribeCounter;
                subscriptions.Remove(uc.Subscriber);
            }
        }
    }
}
