using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCommander.Match
{
    public class MatchManager : MonoBehaviour
    {
        public void CmdCreateNewMatch()
        {
            var matchObj = Instantiate(matchPrefab, GameManager.instance.transform);
            Match m = matchObj.GetComponent<Match>();
            m.InitLocalMatchWithStoredSettings();
        }

        private void OnDestroy()
        {
            Match.ClearDelegates();
        }
        
    }
}