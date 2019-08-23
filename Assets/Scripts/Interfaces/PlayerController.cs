using Mirror;
using StellarArmada.Teams;
using UnityEngine;
#pragma warning disable 0649
namespace StellarArmada
{
    public abstract class PlayerController: NetworkBehaviour
    {
        public delegate void EventHandler();

        [SyncVar(hook = nameof(UpdateName))] public string playerName;

        [SyncVar(hook = nameof(HandleTeamChange))] public uint teamId = 255;
        
        protected virtual void UpdateName(string nameToChangeTo)
        {
            transform.name = nameToChangeTo;
            playerName = nameToChangeTo;
            EventOnPlayerNameChange?.Invoke();
        }

        public event EventHandler EventOnPlayerNameChange;
        public event EventHandler EventOnPlayerTeamChange;

        public void HandleTeamChange(uint pTeam)
        {
            if (teamId == pTeam) return;
            teamId = pTeam;
            EventOnPlayerTeamChange?.Invoke();
        }
        public abstract PlayerType GetPlayerType();
        
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
