using System;
using System.Collections.Generic;
using System.Linq;
using StarcraftBot.UnitAgents;
using SWIG.BWAPIC;

namespace StarcraftBot
{
    /// <summary>
    /// To run this: USE MONO-2.8 Command Prompt (or newer) in Admin mode together with chaoslauncher and StarCraft: Brood War 1.16.1. 
    /// This class is the main agent class that holds and calls all theSWIG.BWAPI IStarCraftBot events.
    /// The OnStart event is the first method that gets callled.
    /// The OnFrame event is the main game loop that gets called in each game frame from the 
    /// StarCraft game through the bwapi-mono-bridge (http://code.google.com/p/bwapi-mono-bridge/). 
    /// See more information and documentation about theSWIG.BWAPI here: http://code.google.com/p/bwapi/.
    /// BWAPI version used: BWAPI-clr-client Beta 3.3.
    /// @agent Author: Thomas Willer Sandberg (http://twsandberg.dk/)
    /// @version (1.0, January 2011)
    /// </summary>
    class StarcraftBot
    {
        public static int Fitness { get; set; }

        protected static void Reconnect()
        {

            while (!bwapiclient.BWAPIClient.connect())
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        protected static int TotalUnitBuildCount(SWIG.BWAPI.UnitType unitType)
        {
            return SWIG.BWAPI.bwapi.Broodwar.self().completedUnitCount(unitType) + SWIG.BWAPI.bwapi.Broodwar.self().deadUnitCount(unitType);
        }

        protected static int TotalUnitBuildScore(SWIG.BWAPI.UnitType unitType)
        {
            return TotalUnitBuildCount(unitType) * unitType.buildScore();
        }

        /// <summary>
        /// Calculates the total unit build count for all the units owned by this player.
        /// </summary>
        /// <returns></returns>
        protected static int TotalUnitBuildCountForAllOwnUnits()
        {
            //int totalUnitBuildScore
            //foreach (BWAPI.UnitType unitType inSWIG.BWAPI.bwapi.allUnitTypes())
                //totalUnitBuildScore += TotalUnitBuildScore(unitType);
            return SWIG.BWAPI.bwapi.allUnitTypes().Sum(unitType => TotalUnitBuildScore(unitType));
        }

        protected static int TotalUnitDestroyScore(SWIG.BWAPI.UnitType unitType)
        {
            return SWIG.BWAPI.bwapi.Broodwar.self().killedUnitCount(unitType) * 
                unitType.destroyScore();
        }

        /// <summary>
        /// Calculates the total unit destroy score for all the units owned by this player.
        /// </summary>
        /// <returns></returns>
        protected static int TotalUnitDestroyScoreForAllOwnUnits()
        {
            return SWIG.BWAPI.bwapi.allUnitTypes().Sum(unitType => TotalUnitDestroyScore(unitType));
        }

        /// <summary>
        /// The total unit score shown in the end score board.
        /// </summary>
        /// <returns></returns>
       public static int TotalUnitScore()
        {
            return TotalUnitBuildCountForAllOwnUnits() + TotalUnitDestroyScoreForAllOwnUnits();
        }

       protected static int TotalUnitLossScore(SWIG.BWAPI.UnitType unitType)
       {
           return SWIG.BWAPI.bwapi.Broodwar.self().deadUnitCount(unitType) * unitType.destroyScore();
       }

       /// <summary>
       /// Calculates the total unit loss score for all the units owned/lost by this player.
       /// </summary>
       /// <returns></returns>
       protected static int TotalUnitLossScoreForAllOwnUnits()
       {
           return SWIG.BWAPI.bwapi.allUnitTypes().Sum(unitType => TotalUnitLossScore(unitType));
       }

        protected static int TotalHitpointsLeft()
        {
            return SWIG.BWAPI.bwapi.Broodwar.self().getUnits().Count > 0 ? SWIG.BWAPI.bwapi.Broodwar.self().getUnits().Sum(unit => unit.getHitPoints() + unit.getShields()) : 0;
        }

        protected static void CalculateFitnessScore()
       {
            Fitness = TotalUnitScore() - TotalUnitLossScoreForAllOwnUnits(); 
            if (!BotEvents.RanOutOfTime)
                Fitness += TotalHitpointsLeft() * 10; //TotalUnitScore() - TotalUnitLossScoreForAllOwnUnits() + TotalHitpointsLeft() * 10; 
       }

        protected static void Play(List<UnitAgentOptimizedProperties> optimizedPropertiesGeneList, int gameSpeed, Boolean complemapinformation, Boolean training, Boolean userInput)//UnitAgentOptimizedProperties optimizedPropertiesGene, int gameSpeed, Boolean complemapinformation, Boolean training, Boolean userInput)
        {
            while (!SWIG.BWAPI.bwapi.Broodwar.isInGame())
            {
                bwapiclient.BWAPIClient.update();
                if (!bwapiclient.BWAPIClient.isConnected())
                {
                    Console.WriteLine("Reconnecting...");
                    Reconnect();
                }
            } //wait for game

            SWIG.BWAPI.bwapi.Broodwar.setLocalSpeed(gameSpeed);
            if (userInput)
                SWIG.BWAPI.bwapi.Broodwar.enableFlag((int)SWIG.BWAPI.Flag_Enum.UserInput); //Turn User Input On/Off

            if (complemapinformation)
                SWIG.BWAPI.bwapi.Broodwar.enableFlag((int)SWIG.BWAPI.Flag_Enum.CompleteMapInformation);

            while (SWIG.BWAPI.bwapi.Broodwar.isInGame())
            {
                foreach (SWIG.BWAPI.Event e in SWIG.BWAPI.bwapi.Broodwar.getEvents())
                {
                    SWIG.BWAPI.EventType_Enum et = e.type;
                    switch (et)
                    {
                        case SWIG.BWAPI.EventType_Enum.MatchStart:
                            //Very IMPORTANT. This sets the actual optimized properties to the agent.
                            if (optimizedPropertiesGeneList != null && optimizedPropertiesGeneList.Count > 0)
                            {
                                BotEvents.Training = training;
                                BotEvents.OptimizedPropertiesGeneList = optimizedPropertiesGeneList;
                                BotEvents.OnStart();
                            }
                            else
                                Logger.Logger.AddAndPrint("optimizedPropertiesGeneList is null or has zero elements in OnStart Event");
                            break;
                        case SWIG.BWAPI.EventType_Enum.MatchEnd:
                            CalculateFitnessScore();
                            BotEvents.OnEnd(e.isWinner);
                            //Util.Logger.Instance.Log("Game Over. I " + ((e.isWinner) ? "Won." : "Lost."));
                            break;
                        case SWIG.BWAPI.EventType_Enum.MatchFrame:
                            //if (AnalysisDone && GameEnded == false)
                                BotEvents.OnFrame();
                            break;
                        case SWIG.BWAPI.EventType_Enum.MenuFrame:
                            break;
                        case SWIG.BWAPI.EventType_Enum.SendText:
                            BotEvents.OnSendText(e.text);
                            break;
                        case SWIG.BWAPI.EventType_Enum.ReceiveText:
                            //client.onReceiveText(e.player, e.text);
                            break;
                        case SWIG.BWAPI.EventType_Enum.PlayerLeft:
                            //client.onPlayerLeft(e.player);
                            break;
                        case SWIG.BWAPI.EventType_Enum.NukeDetect:
                            //client.onNukeDetect(e.position);
                            break;
                        case SWIG.BWAPI.EventType_Enum.UnitDiscover:
                            // client.onUnitDiscover(e.unit);
                            break;
                        case SWIG.BWAPI.EventType_Enum.UnitEvade:
                            // client.onUnitEvade(e.unit);
                            break;
                        case SWIG.BWAPI.EventType_Enum.UnitShow:
                            if (e.unit.getPlayer() == SWIG.BWAPI.bwapi.Broodwar.self())
                            {
                                BotEvents.OnUnitShow(new Wrapper.Unit(e.unit));
                               //SWIG.BWAPI.bwapi.Broodwar.printf("Unit Shown: [" + e.unit.getType().getName() + "] at [" + e.unit.getPosition().xConst() + "," + e.unit.getPosition().yConst() + "]");
                            }
                            break;
                        case SWIG.BWAPI.EventType_Enum.UnitHide:
                            if (e.unit.getPlayer() == SWIG.BWAPI.bwapi.Broodwar.self())
                            {
                                //enemies.Remove(unit);
                                BotEvents.OnUnitHide(new Wrapper.Unit(e.unit));
                               //SWIG.BWAPI.bwapi.Broodwar.printf("Unit Hidden: [" + e.unit.getType().getName() + "] at [" + e.unit.getPosition().xConst() + "," + e.unit.getPosition().yConst() + "]");
                            }
                            break;
                        case SWIG.BWAPI.EventType_Enum.UnitCreate:

                            if (e.unit.getPlayer() != SWIG.BWAPI.bwapi.Broodwar.self())
                            {
                                //myUnits.Add(new BW.Unit(unit));
                                BotEvents.OnUnitCreate(new Wrapper.Unit(e.unit));
                               // SWIG.BWAPI.bwapi.Broodwar.printf("Unit Created: [" + e.unit.getType().getName() + "] at [" + e.unit.getPosition().xConst() + "," + e.unit.getPosition().yConst() + "]");
                            }
                            break;
                        case SWIG.BWAPI.EventType_Enum.UnitDestroy:
                            BotEvents.OnUnitDestroy(new Wrapper.Unit(e.unit));
                            break;
                        case SWIG.BWAPI.EventType_Enum.UnitMorph:
                            BotEvents.OnUnitMorph(new Wrapper.Unit(e.unit));
                            break;
                        case SWIG.BWAPI.EventType_Enum.UnitRenegade:
                            BotEvents.OnUnitRenegade(new Wrapper.Unit(e.unit));
                            break;
                        case SWIG.BWAPI.EventType_Enum.SaveGame:
                            BotEvents.OnSaveGame(e.text);
                            break;
                        default:
                            break;
                    }
                }

                bwapiclient.BWAPIClient.update();
                if (!bwapiclient.BWAPIClient.isConnected())
                {
                    Console.WriteLine("Reconnecting...\n");
                    Reconnect();
                }
            }
            //return;
        }
    }
}