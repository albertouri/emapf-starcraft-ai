using System;
using System.Collections.Generic;
using System.Linq;
using StarcraftBot.UnitAgents;
using StarcraftBot.UnitAgents.Terran;
using StarcraftBot.Wrapper;

namespace StarcraftBot
{
    /// <summary>
    /// BotEvents class...
    /// @author Thomas Willer Sandberg (http://twsandberg.dk/)
    /// @version (1.0, January 2011)
    /// </summary>
    class BotEvents
    {
        private static List<Unit> _allUnits;
        private static List<UnitAgent> _myWorkers;
        private static List<List<UnitAgent>> _myArmy; //All the squads will be in this list.
        //private static List<UnitAgent> _mySquad;//The squad is a subset of myArmy, and represents a squad that is sent to attack the enemy.
        private static TacticalAssaultAgent _taa;
        private static Boolean _gameRestarted;
        public static Boolean RanOutOfTime { get; set; }

        public static List<UnitAgentOptimizedProperties> OptimizedPropertiesGeneList { get; set; }

        //public static UnitAgentOptimizedProperties OptimizedPropertiesGene { get; set; }
        public static Boolean Training { get; set; }

        public static void OnStart()
        {
            ResetUnitLists(); //Clean all the lists, so we secure that all units in the different lists exists and none is added twice to the same list.
            AddAllUnitsToSeparateLists(); //Add the units to separate lists.
            RanOutOfTime = false;
            _gameRestarted = false;
        }

        public static bool OnSendText(string text)
        {
           // Console.WriteLine("Text typed in StarCraft " + text);
            //Util.Logger.Instance.Log("Thomas W. Sandberg typed: " + text););)
            return true;
        }

        public static void OnReceiveText(Player player, string text)
        {}

        public static void OnPlayerLeft(Player player)
        {}

        public static void OnNukeDetect(Position target)
        {}

        public static void OnUnitDiscover(Unit unit)
        {}

        public static void OnUnitEvade(Unit unit)
        {}

        /// <summary>
        /// Clean all the lists, so we secure that all units in the different lists exists and none is added twice to the same list.
        /// </summary>
        public static void ResetUnitLists()
        {
            _allUnits = Game.GetAllUnits();
            _myArmy = new List<List<UnitAgent>>(); //All the bots army units.
            _myWorkers = new List<UnitAgent>();
        }

        public static UnitAgent ConvertUnitToUnitAgent(Unit unit)
        {
            UnitAgent unitAgent;

            switch (unit.UnitTypeEnum)
            {
                case UnitTypes.Terran_SCV:
                    unitAgent = new Terran_SCV_Agent(unit, GetOptimizedValuesToUnitAgent(unit.UnitTypeEnum.ToString()));
                    break;
                case UnitTypes.Terran_Marine:
                    unitAgent = new Terran_Marine_Agent(unit, GetOptimizedValuesToUnitAgent(unit.UnitTypeEnum.ToString()));
                    break;
                case UnitTypes.Terran_Firebat:
                    unitAgent = new Terran_Firebat_Agent(unit, GetOptimizedValuesToUnitAgent(unit.UnitTypeEnum.ToString()));
                    break;
                case UnitTypes.Terran_Medic:
                    unitAgent = new Terran_Medic_Agent(unit, GetOptimizedValuesToUnitAgent(unit.UnitTypeEnum.ToString()));
                    break;
                case UnitTypes.Terran_Goliath:
                    unitAgent = new Terran_Goliath_Agent(unit, GetOptimizedValuesToUnitAgent(unit.UnitTypeEnum.ToString()));
                    break;
                case UnitTypes.Terran_Vulture:
                    unitAgent = new Terran_Vulture_Agent(unit, GetOptimizedValuesToUnitAgent(unit.UnitTypeEnum.ToString()));
                    break;
                case UnitTypes.Terran_TankTurretTankMode:
                case UnitTypes.Terran_SiegeTankSiegeTurret:
                case UnitTypes.Terran_SiegeTankTankMode:
                case UnitTypes.Terran_SiegeTankSiegeMode:
                    unitAgent = new Terran_SiegeTank_Agent(unit, GetOptimizedValuesToUnitAgent(unit.UnitTypeEnum.ToString()));
                    break;
                default:
                    unitAgent = new UnitAgent(unit, GetOptimizedValuesToUnitAgent(unit.UnitTypeEnum.ToString()));
                    break;
            }

            unitAgent.EmotionalMode = UnitAgent.EmotionalModeEnum.Exploration;

            return unitAgent;
        }

        /// <summary>
        /// Get the optimized values to an unit agent. If there exists different optimized values for the unit agents, these will be used, else just the first 
        /// UnitAgentOptimizedProperties will be used.
        /// </summary>
        /// <param name="unitTypeName"></param>
        /// <returns></returns>
        public static UnitAgentOptimizedProperties GetOptimizedValuesToUnitAgent(String unitTypeName)//ref UnitAgent unitAgent)
        {
            if (OptimizedPropertiesGeneList != null && OptimizedPropertiesGeneList.Count > 0)
            {
                foreach (UnitAgentOptimizedProperties opropTmp in OptimizedPropertiesGeneList)
                {
                    if (!String.IsNullOrEmpty(opropTmp.UnitTypeName))
                    {
                        if (opropTmp.UnitTypeName.Equals(unitTypeName))
                            return opropTmp;
                    }
                    else
                        opropTmp.UnitTypeName = unitTypeName;
                }

                return OptimizedPropertiesGeneList[0];
            }
            Console.WriteLine("OptimizedPropertiesGeneList was null or had 0 UnitAgentOptimizedProperties in GetOptimizedValuesToUnitAgent in BotEvents.");
            Logger.Logger.AddAndPrint("OptimizedPropertiesGeneList was null or had 0 UnitAgentOptimizedProperties in GetOptimizedValuesToUnitAgent in BotEvents.");

            return null;
        }

        /// <summary>
        /// Returns true if the specified unit is a worker else false.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        private static Boolean IsWorker(Unit unit)
        {
            return unit.UnitTypeEnum == UnitTypes.Terran_SCV || unit.UnitTypeEnum == UnitTypes.Zerg_Drone || unit.UnitTypeEnum == UnitTypes.Protoss_Probe;
        }

        /// <summary>
        /// Add all units to separate lists. For instance will there be a list with own/my units and one for enemyUnits.
        /// </summary>
        private static void AddAllUnitsToSeparateLists()
        {
            var mySquad = new List<UnitAgent>();

            foreach (var unit in _allUnits) // Game.PlayerSelf.GetUnits IS EQUAL TO MyOwn Units
            {
                if (unit.IsMine)
                {
                    if (IsWorker(unit)) //Add all worker units to one workers.
                        _myWorkers.Add(ConvertUnitToUnitAgent(unit));//new UnitAgent(unit));
                    else //Add all military units to one squad.
                    {
                        //_myArmy.Add(ConvertUnitToUnitAgent(unit));
                        mySquad.Add(ConvertUnitToUnitAgent(unit));
                    }
                }
            }
            _myArmy.Add(mySquad);
            _taa = new TacticalAssaultAgent(_myArmy[0]); //Add the only existing squad.
        }

        public static void OnFrame()
        {
            if (!SWIG.BWAPI.bwapi.Broodwar.isInGame() || _gameRestarted)
                return;

            if (Training && SWIG.BWAPI.bwapi.Broodwar.getFrameCount() > 3200)
            {
                RanOutOfTime = true;
                Reset();
            }

            if (SWIG.BWAPI.bwapi.Broodwar.getFrameCount() != 0 && SWIG.BWAPI.bwapi.Broodwar.getFrameCount() % 5 == 0 && AnyUnitsLeftOnTeams())// && !_gameRestarted && AnyUnitsLeftOnTeams())//&& Game.PlayerSelf.GetUnits().Count > 0)//5
                _taa.ExecuteBestActionForSquad();
        }

        public static void OnUnitCreate(Unit unit)
        {
            //Util.Logger.Instance.Log("unit create: " + unit.UnitTypeEnum);
        }

        /// <summary>
        /// Remove the destroyed unit from the list where the unit is placed.
        /// </summary>
        /// <param name="unit"></param>
        private static void RemoveDeadUnitFromList(Unit unit)
        {
            if (unit.IsMine)
            {
                if (IsWorker(unit))
                {
                    foreach (UnitAgent ua in _myWorkers)
                    {
                        _myWorkers.Remove(ua);
                        return;
                    }
                }
                else
                {
                    foreach (List<UnitAgent> squad in _myArmy)
                    {
                        foreach (UnitAgent ua in squad.Where(ua => ua.MyUnit.ID == unit.ID))
                        {
                            squad.Remove(ua);
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if any of the (two) teams has any units left. 
        /// </summary>
        /// <returns>True if both teams has units left. False if just one of the teams has no units left</returns>
        public static Boolean AnyUnitsLeftOnTeams()
        {
            //Logger.AddAndPrint("Game.PlayerEnemy.GetUnits().Count: " + Game.PlayerEnemy.GetUnits().Count + "Game.PlayerSelf.GetUnits().Count: " + Game.PlayerSelf.GetUnits().Count);
            return SWIG.BWAPI.bwapi.Broodwar.self().getUnits().Count > 0 &&SWIG.BWAPI.bwapi.Broodwar.enemy().getUnits().Count > 0;
        }

        internal static void OnUnitHide(Unit unit)
        {
           // Util.Logger.Instance.Log("unit hide: " + unit.UnitTypeEnum.ToString());

            if (unit.IsMine)
            {
                if (unit.UnitTypeEnum == UnitTypes.Terran_Goliath || unit.UnitTypeEnum == UnitTypes.Terran_SiegeTankSiegeMode || unit.UnitTypeEnum == UnitTypes.Terran_SiegeTankTankMode || unit.UnitTypeEnum == UnitTypes.Terran_SiegeTankSiegeTurret || unit.UnitTypeEnum == UnitTypes.Terran_TankTurretTankMode)
                    RemoveDeadUnitFromList(unit);
            }
        }

        internal static void OnUnitShow(Unit unit)
        {
            //Util.Logger.Instance.Log("unit show: " + unit.UnitTypeEnum.ToString());
        }

        internal static void Reset()
        {
            _allUnits = null;
            _myArmy = null; //All the bots army units.
           // _mySquad = null;//new List<Unit>();
            _myWorkers = null;
            _taa = null;
            
            //
            OptimizedPropertiesGeneList = null;
            //

            System.Threading.Thread.Sleep(100);
            //System.Console.WriteLine("getFrameCount: " + SWIG.BWAPI.bwapi.Broodwar.getFrameCount());
            _gameRestarted = true;
            Game.RestartSinglePlayerGame();
        }

        internal static void OnEnd(bool isWinner)
        {
            if (Training && !_gameRestarted)
                Reset();
        }

        public static void OnUnitDestroy(Unit unit)
        {
            if (unit.IsMine)
                RemoveDeadUnitFromList(unit);
        }

        internal static void OnUnitMorph(Unit unit)
        {
            //Util.Logger.Instance.Log("unit morph: " + unit.UnitTypeEnum.ToString());
        }

        public static void OnUnitRenegade(Unit unit)
        {
            //Util.Logger.Instance.Log("unit renegade: " + unit.UnitTypeEnum.ToString());
        }

        public static void OnSaveGame(string gameName)
        {
            //Util.Logger.Instance.Log("Save Game as: " + gameName);
        }
    }
}
