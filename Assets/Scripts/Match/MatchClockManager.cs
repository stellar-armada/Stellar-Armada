using Mirror;

#pragma warning disable 0649
namespace StellarArmada.Match
{
    // Networked manager that syncs the match clock
    // TO-DO: Implement, integrate and comment!
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
