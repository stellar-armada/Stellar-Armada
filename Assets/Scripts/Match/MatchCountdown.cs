using StellarArmada.Match.Messaging;

#pragma warning disable 0649
namespace StellarArmada.Match
{
    // TO-DO: Integrate and comment
    public struct MatchCountDownSettings
    {
        public MatchMessageType startingMessage;
        public CountdownType countDownType;
        public MatchCountDownSettings(MatchMessageType startingMessage, CountdownType countDownType)
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
        private MatchTimer t;
        
        public void StartCountdown(MatchClock clock)
        {
            MatchTimer  t = new MatchTimer(clock, 1.2f);

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
            t = new MatchTimer(clock,1f);
            if (counter > 0)
            {

                MatchMessageManager.instance.CmdRaiseMessageToClients((MatchMessageType) counter);
                counter--;
                
                t.Start();
                t.OnTimerFinished += () => CountDown(clock);
                return;
            }

            if (settings.countDownType == CountdownType.PreMatch)
            {
                t = new MatchTimer(clock, 1.5f);
                t.Start();
                t.OnTimerFinished += DelayedPostCoundown;
            }
            
        }

        void DelayedPostCoundown()
        {
            MatchMessageManager.instance.CmdRaiseMessageToClients(MatchMessageType.MATCH_HAS_BEGUN);
        }
    }
}