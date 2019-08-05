using SpaceCommander.Player;
using SpaceCommander.UI;

namespace SpaceCommander.Match
{
    public struct MatchCountDownSettings
    {

        public MessageType startingMessage;
        public CountdownType countDownType;
        public MatchCountDownSettings(MessageType startingMessage, CountdownType countDownType)
        {
            this.startingMessage = startingMessage;
            this.countDownType = countDownType;
        }
    }
    
    public enum CountdownType
    {
        PreMatch,
        Match,
        PostMatch
    };

    public class MatchCountdown
    {
        int counter = 5;
        private MatchCountDownSettings settings;
        private Timer t;
        
        public void StartCountdown(MatchClock clock)
        {
            Timer  t = new Timer(clock, 1.2f);

            t.Start();

            MatchMessageManager.instance.CmdRaiseMessageToClients(settings.startingMessage);
            
            t.OnTimerFinished += () => { CountDown(clock); };
        }

        public MatchCountdown(MatchClock clock, MatchCountDownSettings _settings)
        {
            settings = _settings;
            StartCountdown(clock);
        }

        void CountDown(MatchClock clock)
        {
            t = new Timer(clock,1f);
            if (counter > 0)
            {

                MatchMessageManager.instance.CmdRaiseMessageToClients((MessageType) counter);
                counter--;
                
                t.Start();
                t.OnTimerFinished += () => CountDown(clock);
                return;
            }

            if (settings.countDownType == CountdownType.PreMatch)
            {
                t = new Timer(clock, 1.5f);
                t.Start();
                t.OnTimerFinished += DelayedPostCoundown;
            }
            
        }

        void DelayedPostCoundown()
        {
            MatchMessageManager.instance.CmdRaiseMessageToClients(MessageType.MATCH_HAS_BEGUN);
        }
    }
}