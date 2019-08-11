using System.Collections.Generic;
using Mirror;
using SpaceCommander.Selection;
using UnityEngine;

namespace SpaceCommander.Teams
{
    public class Team : NetworkBehaviour
    {
        public uint teamId;

        // public int pointsToSpend; leaving here so we can add this mechanic later
        public string name;
        public Color color;
        public int playerSlots;
        public Texture insignia;
        public List<IPlayer> players = new List<IPlayer>();
        public List<IEntity> entities = new List<IEntity>();

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

        public void AddPlayer(IPlayer player)
        {
            players.Add(player);
        }

        public void RemovePlayer(IPlayer player)
        {
            players.Remove(player);
        }

        public void AddEntity(IEntity entity)
        {
            entities.Add(entity);
            GroupManager.instance.UpdateGroupManager(teamId);
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
            return name;
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