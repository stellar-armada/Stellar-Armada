using System.Collections.Generic;
using Mirror;
using StellarArmada.Game;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Teams
{
    public class Team : NetworkBehaviour
    {
        public uint teamId;

        // public int pointsToSpend; leaving here so we can add this mechanic later
        public string teamName;
        public Color color;
        public int playerSlots;
        public Texture insignia;
        public List<PlayerController> players = new List<PlayerController>();
        public List<NetworkEntity> entities = new List<NetworkEntity>();

        public delegate void TeamEvent();

        public TeamEvent OnEntitiesUpdated;

        // Hardcoded 3 groups into each team
        public List<List<NetworkEntity>> groups = new List<List<NetworkEntity>>
        {
            new List<NetworkEntity>(),
            new List<NetworkEntity>(),
            new List<NetworkEntity>(),
        };

        public void ChangeEntityGroup(NetworkEntity networkEntity, int group)
        {
            // Check if it's already in a group
            for (int i = 0; i < groups.Count; i++)
            {
                // If it is, check if the group is the same as the one we're trying to change to
                if (groups[i].Contains(networkEntity))
                {
                    // If it isn't, remove from the old group and add to the new group
                    // Otherwise do nothing
                    if (i != group)
                    {
                        RemoveEntityFromGroup(networkEntity, i);
                    }
                }
            }
        }

        public void AddEntityToGroup(NetworkEntity networkEntity, int group)
        {
            groups[group].Add(networkEntity);

            for (int i = 0; i < groups.Count; i++)
            {
                if (i == group) continue;
                if (groups[i].Contains(networkEntity))
                {
                    RemoveEntityFromGroup(networkEntity, i);
                }
            }
        }

        public void RemoveEntityFromGroup(NetworkEntity networkEntity, int group)
        {
            groups[group].Remove(networkEntity);
        }

        [Command]
        public void CmdAddPlayer(uint playerId)
        {
            AddPlayer(playerId);
            RpcAddPlayer(playerId);
        }

        [ClientRpc]
        public void RpcAddPlayer(uint playerId)
        {
            if (!isServer)
            {
            AddPlayer(playerId);
            PlayerManager.GetPlayerById(playerId).SetTeamId(teamId);
            }
        }

        void AddPlayer(uint playerId)
        {
            PlayerController playerController= PlayerManager.GetPlayerById(playerId);
            players.Add(playerController);
            playerController.SetTeamId(teamId);
        }

        public void RemovePlayer(PlayerController playerController)
        {
            players.Remove(playerController);
        }

        public void AddEntity(NetworkEntity networkEntity)
        {
            entities.Add(networkEntity);
            OnEntitiesUpdated?.Invoke();
        }


        public void RemoveEntity(NetworkEntity networkEntity)
        {
            entities.Remove(networkEntity);
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public Team GetTeam()
        {
            return this;
        }

        public uint GetId()
        {
            return teamId;
        }

        public string GetName()
        {
            return teamName;
        }

        public bool IsEnemy(Team team)
        {
            if (team != this) return true;
            return false;
        }

        public bool IsFriend(Team team)
        {
            if (team == this) return true;
            return false;
        }
    }
}