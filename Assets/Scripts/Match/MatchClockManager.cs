using Mirror;

#pragma warning disable 0649
namespace SpaceCommander.Match
{

    public class MatchClockManager : NetworkBehaviour
    {
        public static MatchClockManager instance; // private singleton with public GetCurrentMatch access
        
        public MatchClock clock;

        float CurrentTime;

        private void Update()
        {
            if (isServer && clock != null)
            {
                clock.Update();
                CurrentTime = clock.GetMatchTime();
            }
        }

    }
}
