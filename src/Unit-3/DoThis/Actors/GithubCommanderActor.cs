using System;
using System.Linq;
using Akka.Actor;
using Akka.Routing;

namespace GithubActors.Actors
{
    /// <summary>
    /// Top-level actor responsible for coordinating and launching repo-processing jobs
    /// </summary>
    public class GithubCommanderActor : ReceiveActor, IWithUnboundedStash
    {
        #region Message classes

        public class CanAcceptJob
        {
            public CanAcceptJob(RepoKey repo)
            {
                Repo = repo;
            }

            public RepoKey Repo { get; private set; }
        }

        public class AbleToAcceptJob
        {
            public AbleToAcceptJob(RepoKey repo)
            {
                Repo = repo;
            }

            public RepoKey Repo { get; private set; }
        }

        public class UnableToAcceptJob
        {
            public UnableToAcceptJob(RepoKey repo)
            {
                Repo = repo;
            }

            public RepoKey Repo { get; private set; }
        }

        #endregion

        private IActorRef coordinator;
        private IActorRef canAcceptJobSender;

        private RepoKey repoJob;

        private int pendingJobReplies;

        public GithubCommanderActor()
        {
            Ready();
        }

        private void Ready()
        {
            Receive<CanAcceptJob>(job =>
            {
                coordinator.Tell(job);
                repoJob = job.Repo;

                BecomeAsking();
            });
        }

        private void BecomeAsking()
        {
            canAcceptJobSender = Sender;

            // block, but ask the router for the number of routees. Avoids magic numbers.
            pendingJobReplies = coordinator.Ask<Routees>(new GetRoutees()).Result.Members.Count();
            Become(Asking);

            // send ourselves a ReceiveTimeout message if no message within 3 seconds
            Context.SetReceiveTimeout(TimeSpan.FromSeconds(3));
        }

        private void Asking()
        {
            // stash any subsequent requests
            Receive<CanAcceptJob>(job => Stash.Stash());

            Receive<UnableToAcceptJob>(job =>
            {
                pendingJobReplies--;
                
                if (pendingJobReplies == 0)
                {
                    canAcceptJobSender.Tell(job);
                    BecomeReady();
                }
            });

            Receive<AbleToAcceptJob>(job =>
            {
                canAcceptJobSender.Tell(job);

                //start processing messages
                Sender.Tell(new GithubCoordinatorActor.BeginJob(job.Repo));

                //launch the new window to view results of the processing
                Context.ActorSelection(ActorPaths.MainFormActor.Path).Tell(
                    new MainFormActor.LaunchRepoResultsWindow(job.Repo, Sender));

                BecomeReady();
            });

            // add this inside the GithubCommanderActor.Asking method
            // means at least one actor failed to respond
            Receive<ReceiveTimeout>(timeout =>
            {
                canAcceptJobSender.Tell(new UnableToAcceptJob(repoJob));

                BecomeReady();
            });
        }

        private void BecomeReady()
        {
            Become(Ready); 
            Stash.UnstashAll();

            // cancel ReceiveTimeout
            Context.SetReceiveTimeout(null);
        } 

        protected override void PreStart()
        {
            // create a broadcast router who will ask all of them if they're available for work
            coordinator = 
                Context.ActorOf(Props.Create(() => new GithubCoordinatorActor()).WithRouter(FromConfig.Instance), 
                ActorPaths.GithubCoordinatorActor.Name);

            base.PreStart();
        }

        protected override void PreRestart(Exception reason, object message)
        {
            //kill off the old coordinator so we can recreate it from scratch
            coordinator.Tell(PoisonPill.Instance);
            base.PreRestart(reason, message);
        }

        #region IActorStash Members

        public IStash Stash { get; set; }

        #endregion
    }
}
