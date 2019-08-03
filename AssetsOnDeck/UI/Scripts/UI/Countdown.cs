using SpaceCommander.Player;
using SpaceCommander.UI;

namespace SpaceCommander.Game
{
    public struct CountDownSettings
    {

        public MessageType startingMessage;
        public CountdownType countDownType;
        public CountDownSettings(MessageType startingMessage, CountdownType countDownType)
        {
            this.startingMessage = startingMessage;
            this.countDownType = countDownType;
        }
    }
    
    public enum CountdownType
    {
        Prematch,
        Match,
        Postmatch
    };

    public class Countdown
    {
        int counter = 5;
        private CountDownSettings settings;
        private Timer t;
        
        public void StartCountdown(MatchClock clock)
        {
            Timer  t = new Timer(clock, 1.2f);

            t.Start();

            Match.GetCurrentMatch().RaiseMessageToClients(settings.startingMessage);
            
            t.OnTimerFinished += () => { CountDown(clock); };
        }

        public Countdown(MatchClock clock, CountDownSettings _settings)
        {
            settings = _settings;
            StartCountdown(clock);
        }

        void CountDown(MatchClock clock)
        {
            t = new Timer(clock,1f);
            if (counter > 0)
            {

                Match.GetCurrentMatch().RaiseMessageToClients((MessageType) counter);
                counter--;
                
                t.Start();
                t.OnTimerFinished += () => CountDown(clock);
                return;
            }

            if (settings.countDownType == CountdownType.Prematch)
            {
                t = new Timer(clock, 1.5f);
                t.Start();
                t.OnTimerFinished += DelayedPostCoundown;
                
                Match.GetCurrentMatch().RaiseMessageToClients(MessageType.CD_FIGHT);
            }
            
        }

        void DelayedPostCoundown()
        {
                    Match.GetCurrentMatch().RaiseMessageToClients(MessageType.MATCH_HAS_BEGUN);
        }
    }
}