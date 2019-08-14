using System.Collections.Generic;
using Mirror;
using SpaceCommander.Game;
using SpaceCommander.Selection;
using UnityEngine;

#pragma warning disable 0649
namespace SpaceCommander.Teams
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
        public List<IEntity> entities = new List<IEntity>();

        public delegate void TeamEvent();

        public TeamEvent OnEntitiesUpdated;

        // Hardcoded 3 groups into each team
        public List<List<IEntity>> groups = new List<List<IEntity>>
        {
            new List<IEntity>(),
            new List<IEntity>(),
            new List<IEntity>(),
        };

        public void ChangeEntityGroup(IEntity entity, int group)
        {
            // Check if it's already in a group
            for (int i = 0; i < groups.Count; i++)
            {
                // If it is, check if the group is the same as the one we're trying to change to
                if (groups[i].Contains(entity))
                {
                    // If it isn't, remove from the old group and add to the new group
                    // Otherwise do nothing
                    if (i != group)
                    {
                        RemoveEntityFromGroup(entity, i);
                    }
                }
            }
        }

        public void AddEntityToGroup(IEntity entity, int group)
        {
            groups[group].Add(entity);

            for (int i = 0; i < groups.Count; i++)
            {
                if (i == group) continue;
                if (groups[i].Contains(entity))
                {
                    RemoveEntityFromGroup(entity, i);
                }
            }
        }

        public void RemoveEntityFromGroup(IEntity entity, int group)
        {
            groups[group].Remove(entity);
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

        public void AddEntity(IEntity entity)
        {
            entities.Add(entity);
            OnEntitiesUpdated?.Invoke();
        }


        public void RemoveEntity(IEntity entity)
        {
            entities.Remove(entity);
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