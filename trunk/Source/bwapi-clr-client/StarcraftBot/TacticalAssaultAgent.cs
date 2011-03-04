using System;
using System.Collections.Generic;
using System.Linq;
using StarcraftBot.MathHelpers;
using StarcraftBot.Wrapper;
using StarcraftBot.UnitAgents;

namespace StarcraftBot
{
    /// <summary>
    /// Tactical Assault Agent class...
    /// @author Thomas Willer Sandberg (http://twsandberg.dk/)
    /// @version (1.0, January 2011)
    /// </summary>
    class TacticalAssaultAgent 
    {
        public TacticalAssaultAgent(List<UnitAgent>  mySquad)//ref List<UnitAgent> mySquad)//(List<UnitAgent> mySquad, List<Unit> enemyUnits, int maxDistance)
        { 
            MySquad = mySquad;
        }

        public void FindBestGoalsForAllUnitsInSquad()//Position position)
        {
            if (MySquad != null && MySquad.Count > 0)
            {
                foreach (UnitAgent unitAgent in MySquad)
                    FindAndSetOptimalGoalForUnitAgent(unitAgent);
                //unitAgent.GoalPosition = SquadMainGoalPosition;
            }
            else
            {
                Logger.Logger.AddAndPrint("MySquad is null in method FindBestGoalsForAllUnitsInSquad");
                throw new ArgumentNullException("MySquad");
            }
        }

        public void FindAndSetOptimalGoalForUnitAgent(UnitAgent unitAgent)
        {
            if (unitAgent == null)
            {
                Logger.Logger.AddAndPrint("unitAgent is null in method SetGoalPositionForUnitAgentToClosestEnemy");
                throw new ArgumentNullException("unitAgent");
            }
                
            Unit enemyUnitToAttack = MyMath.GetClosestEnemyUnit(unitAgent.MyUnit);//unitAgent.GetClosestEnemyUnit();
            if (enemyUnitToAttack != null)
            {
                unitAgent.GoalUnitToAttack = enemyUnitToAttack;
                unitAgent.GoalPosition = enemyUnitToAttack.Position;
            }
        }

        /// <summary>
        /// Checks if there are any units near the specified unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns>True if there are any units near the specified unit.</returns>
        public Boolean AnyFriendsNear(Unit unit)
        {
            return MySquad.Any(u => unit.GetDistanceToPosition(u.MyUnit.Position) < 2 && unit.GetDistanceToPosition(u.MyUnit.Position) != 0);
            /*foreach (UnitAgent u in MySquad)
                if (unit.GetDistanceToPosition(u.MyUnit.Position) < 2 && unit.GetDistanceToPosition(u.MyUnit.Position) != 0)
                    return true;
            return false;*/
        }

        /// <summary>
        /// The tactical assault agent will deside which action that would be best for the squad.
        /// For instance find and attack closest enemy units using the squad (team) of own units. 
        /// A tactic could be to attack the units with lowest health first, and using medics to 
        /// heal the most injured melee units first.
        /// </summary>
        public void ExecuteBestActionForSquad()
        {
            foreach (UnitAgent myUnitAgent in MySquad)
            {
                FindBestGoalsForAllUnitsInSquad();
                myUnitAgent.ExecuteBestActionForUnitAgent(MySquad);
            }
        }

        /***********************************************************************
         * All the properties and variables for the TacticalAssaultAgent class *
         ***********************************************************************/
        public List<UnitAgent> MySquad { get; set; }


        /***********************************************************************
         * NOT IMPLEMENTED METHODS                                             *
         ***********************************************************************/
        /// <summary>
        /// Run towards the assault squad, to assist the assault squad in battle.
        /// (Hígh potential field in a radius around the assault squad, and lower PF the farther the support team are from the squad.)
        /// Quote from StarCraft: "Warriors has engaged the enemy".
        /// </summary>
        /// <param name="supportSquad">To assist the assault squad (Can be one to many.)</param>
        public void SupportExistingSquad(List<Unit> supportSquad)
        {
            throw new NotImplementedException();
            //TODO: Run towards the assault squad, to assist the assault squad in battle.
            //Hígh potential field in a radius around the assault squad, and lower PF the farther the support team are from the squad.
        }
    }
}