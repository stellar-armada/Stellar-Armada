using System.Collections.Generic;
using System.Linq;
using Mirror;
using StellarArmada.Entities;
using StellarArmada.Entities.Ships;
using StellarArmada.Levels;
using StellarArmada.Match;
using StellarArmada.Player;
using UnityEngine;

#pragma warning disable 0649
namespace StellarArmada.Teams
{
    // Team object, instantiated and spaned over the network by the match server manager after the server's read the scenario data in
    // Contains all ships for teams. Organizes ships into three battle groups (List<List<NetworkEntity>> groups)
    // Also contains references to players, team specific colors, etc.
    
    public class SyncListShipPrototype : SyncList<ShipPrototype> { }

    public class SyncListShipType : SyncList<ShipType> { }

    public class Team : NetworkBehaviour
    {
        [SyncVar (hook=nameof(HandleTeamSet))] public uint teamId; // unique ID incremented when team is created

        public SyncListShipPrototype prototypes = new SyncListShipPrototype();
        
        // public int pointsToSpend; leaving here so we can add this mechanic later
        [SyncVar] public string teamName;
        [SyncVar] public Color color;
        [SyncVar] public int playerSlots;
        public SyncListShipType availableShipTypes = new SyncListShipType();
        [SyncVar] public int pointsToSpend;
        
        public Texture insignia;
        public List<PlayerController> players = new List<PlayerController>();
        public List<NetworkEntity> entities = new List<NetworkEntity>();
        
        public delegate void TeamEvent();

        public TeamEvent OnEntitiesUpdated;

        void Start()
        {
            //InitializeShipProtoypes();
            TeamManager.instance.teams.Add(this);
            if (insignia == null) HandleTeamSet(teamId);
            
        }

        [Server]
        public void UpdatePrototype(int id, int groupId)
        {
            // Get prototype
            ShipPrototype proto = prototypes.Single(p => p.id == id);

            int index = prototypes.IndexOf(proto);
            
            // change group value
            proto.group = groupId;
            
            // get index of prototype and dirty
            prototypes[index] = proto;
        }

        void HandleTeamSet(uint teamId)
        {
            insignia = TeamManager.instance.templates[teamId].insignia;
        }
       

        void InitializeShipProtoypes(){
        Scenario currentScenario = MatchScenarioManager.instance.GetCurrentScenario();
        
            for (int g = 0; g < currentScenario.teamInfo[teamId].fleetBattleGroups.Count; g++)
            {
                foreach (var shipKeyVal in currentScenario.teamInfo[teamId].fleetBattleGroups[g])
                {
                    for (int numShips = 0; numShips < shipKeyVal.Value; numShips++)
                    {
                        // For each ship, instantiate for current team
                        ShipPrototype p = new ShipPrototype();
                        p.shipType = shipKeyVal.Key;
                        p.group = g;
                        p.id = ShipPrototype.prototypeEntityIncrement++;
                        prototypes.Add(p);
                    }
                }
            }
        }

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