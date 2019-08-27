using Mirror;
using StellarArmada.Teams;
using UnityEngine;
#pragma warning disable 0649
namespace StellarArmada.Player
{
    // The player controller is the base class for any "player"
    // This is confusing and could be rethought a little bit, since we think of players as humans
    // (but that would be the HumanPlayerController inheriting class)
    // AI players can also inherit from this class
    
    public abstract class PlayerController: NetworkBehaviour
    {
        // Generic event handler for this class
        public delegate void PlayerEvent();

        // Set the player name. When set by the server, call the UpdateName callback
        [SyncVar(hook = nameof(UpdateName))] public string playerName;

        // Set the player's team
        //UInts are used here, mostly because uints are used for net IDs and incremented entity index values
        [SyncVar(hook = nameof(HandleTeamChange))] public uint teamId = 255;
        
        public event PlayerEvent EventOnPlayerNameChange;
        public event PlayerEvent EventOnPlayerTeamChange;
        
        // Called when the server updates the player's name variable
        protected virtual void UpdateName(string nameToChangeTo)
        {
            transform.name = nameToChangeTo;
            playerName = nameToChangeTo;
            EventOnPlayerNameChange?.Invoke();
        }

        // Called when the server updates the player's team
        public void HandleTeamChange(uint pTeam)
        {
            if (teamId == pTeam) return;
            teamId = pTeam;
            EventOnPlayerTeamChange?.Invoke();
        }
        public abstract PlayerType GetPlayerType(); // Human, AI?
        
        public PlayerController GetPlayer() => this;
        public GameObject GetGameObject() => gameObject;

        public virtual uint GetId() => netId;

        public virtual string GetName() => name;

        public virtual bool IsEnemy(PlayerController playerController) => playerController.GetTeamId() != teamId;

        public virtual Team GetTeam() => TeamManager.instance.GetTeamByID(teamId);
        public virtual uint GetTeamId() => teamId;
        public virtual void SetTeamId(uint newTeamId) => teamId = newTeamId;

        [Command]
        public virtual void CmdSetTeam(uint _team) => teamId = _team;
        
        [Command]
        public virtual void CmdSetUserName(string newUserName)
        {
            playerName = newUserName;
        }
    }
}
