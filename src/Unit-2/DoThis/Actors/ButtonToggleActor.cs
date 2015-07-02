using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Akka.Actor;

namespace ChartApp.Actors
{
    /// <summary>
    /// Actor responsible for managing button toggles
    /// </summary>
    public class ButtonToggleActor : UntypedActor
    {
        #region Message Types

        /// <summary>
        /// Toggles this button on or off and sends an appropriate message
        /// to the <see cref="PerformanceCounterCoordinatorActor" />
        /// </summary>
        public class Toggle { }

        #endregion

        private readonly CounterType myCounterType;
        private bool isToggledOn;
        private readonly Button myButton;
        private readonly IActorRef coordinatorActor;

        public ButtonToggleActor(IActorRef coordinatorActor, Button myButton,
            CounterType myCounterType, bool isToggledOn)
        {
            this.coordinatorActor = coordinatorActor;
            this.myButton = myButton;
            this.isToggledOn = isToggledOn;
            this.myCounterType = myCounterType;
        }

        protected override void OnReceive(object message)
        {
            if (message is Toggle && isToggledOn)
            {
                // toggle is currently on

                // stop watching this counter
                coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Unwatch(myCounterType));

                FlipToggle();
            }
            else if (message is Toggle && !isToggledOn)
            {
                // toggle is currently off

                // start watching this counter
                coordinatorActor.Tell(new PerformanceCounterCoordinatorActor.Watch(myCounterType));

                FlipToggle();
            }
            else
                Unhandled(message);
        }

        private void FlipToggle()
        {
            // flip the toggle
            isToggledOn = !isToggledOn;

            // change the text od the button
            myButton.Text = string.Format("{0} ({1})", myCounterType.ToString().ToUpperInvariant(),
                isToggledOn ? "ON" : "OFF");
        }
    }
}
