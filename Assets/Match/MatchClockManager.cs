using Mirror;

namespace SpaceCommander.Match
{

    public class MatchClockManager : NetworkBehaviour
    {
        public static MatchClockManager instance; // private singleton with public GetCurrentMatch access
        
        public MatchClock clock;

        float CurrentTime;

        private void Update()
        {
            if (isServer)
            {
                clock.Update();
                CurrentTime = clock.GetMatchTime();
            }
        }

    }
}
