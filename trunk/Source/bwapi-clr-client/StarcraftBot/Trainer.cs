using System;
using System.Collections.Generic;
using StarcraftBot.EvolutionaryAlgorithms; 
using StarcraftBot.UnitAgents;

namespace StarcraftBot
{
    /// <summary>
    /// Trainer class, used to train UnitAgentOptimizedProperties or play a game using a preloaded XML-file.  
    /// @author (Thomas Willer Sandberg) 
    /// @version (1.0, January 2011)
    /// </summary>
    class Trainer : StarcraftBot
    {
        private readonly String _unitAgentXMLFileName;
        private readonly int _numberOfRepopulations;
        private readonly int _totalNumberOfRepopulations;
        private readonly int _populationSize;
        private readonly int _numberOfTrials;
        private readonly EvolutionaryAlgorithm _ea;
        private readonly int _gameSpeed;
        private int saveCount = 1;
        private int _nuberOfCrossOvers;
        private Boolean _saveXMLInSameFile = false;
        private int _startGenerationOn;
        private int _percentOfOpPropsToLoad;

        static void Main()//string[] args)
        {
            //const int numberOfRepopulations = 20;
            //const int populationSize = 100;
            const int totalNumberOfRepopulations =24;
            const int startGenerationOn = 0; //The next generation after the one we have last time (can be used after a crash or if we just want to continue evolving from previous evolved data)
            const int percentOfOpPropsToLoad = 100; //How big a percentage of the unit agent optimized properties from the XML file should be loaded into a new generation.
            const int populationSize = 100;//40;//80;//60;
            const int numberOfTrials = 1;
            const int nuberOfCrossOvers = 0;
            const int gameSpeed = -1;//0; //0 is fastest. -1 Resets the speed to normal speed. 
            const string unitAgentXMLFileName = Settings.Settings.XMLSerializeName;

            Console.WriteLine("StarCraft Brood War Bot TWSandberg ver. 1.0");
            try
            {
                //Train from start with totally random values.
                //new Trainer(totalNumberOfRepopulations, populationSize, numberOfTrials, gameSpeed, false, nuberOfCrossOvers);

                //Train with allready loaded and trained data (from XML, Standard name: !StarCraftUnitAgent.xml). Sometimes with a number behind, indicating how many generations that had been run.
                //new Trainer(totalNumberOfRepopulations, startGenerationOn, percentOfOpPropsToLoad, populationSize, numberOfTrials, unitAgentXMLFileName, gameSpeed, nuberOfCrossOvers);

                //Play with one type of agent
                //new Trainer(1, populationSize, unitAgentXMLFileName, gameSpeed);

                //Play with multiple agents (Is tested with Goliaths and Vultures)
                new Trainer(1, populationSize, gameSpeed);

                //FOR TEST ONLY 
                //Play with totally random values for 1 round.
                //new Trainer(1, populationSize, numberOfTrials, gameSpeed, true, nuberOfCrossOvers);

                //PLay with totally random chromosomes for a number of rounds (without repopulation).
                //new Trainer(totalNumberOfRepopulations, populationSize, numberOfTrials, gameSpeed, true, nuberOfCrossOvers);
            }
            catch (Exception e)
            {
                Console.WriteLine("error occurred: {0}\n{1}", e.Message, e.StackTrace);
                Logger.Logger.AddAndPrint("error occurred: e.Message");
                Logger.Logger.AddAndPrint("StackTrace: e.StackTrace");
            }
        }

        /// <summary>
        /// Create a complete new population with totally random values for all chromosomes.
        /// </summary>
        /// <param name="numberOfRepopulations"></param>
        /// <param name="populationSize"></param>
        /// <param name="numberOfTrials"></param>
        /// <param name="gameSpeed"></param>
        /// <param name="tTest"></param>
        /// <param name="nuberOfCrossOvers">Make one point cross over on the specified number of best candidates, and randomly pick the half of the new chromosomes.</param>
        public Trainer(int numberOfRepopulations, int populationSize, int numberOfTrials, int gameSpeed, Boolean tTest, int nuberOfCrossOvers)
        {
            _numberOfRepopulations = numberOfRepopulations;
            _populationSize = populationSize;
            _numberOfTrials = numberOfTrials;
            _nuberOfCrossOvers = nuberOfCrossOvers;
            _ea = new EvolutionaryAlgorithm(_populationSize);
            _gameSpeed = gameSpeed;

            if (tTest)
                PlayWithRandomChromosomes();//TTest();
            else
                Train();
        }

        /// <summary>
        /// Create a population of existing chromosomes from a file with a possible mixture of new random chromosomes.
        /// </summary>
        /// <param name="totalNumberOfRepopulations"></param>
        /// <param name="startGenerationOn"></param>
        /// <param name="percentOfOpPropsToLoad"></param>
        /// <param name="populationSize"></param>
        /// <param name="numberOfTrials"></param>
        /// <param name="unitAgentXMLFileName"></param>
        /// <param name="gameSpeed"></param>
        /// <param name="nuberOfCrossOvers">Make one point cross over on the specified number of best candidates, and randomly pick the half of the new chromosomes.</param>
        public Trainer(int totalNumberOfRepopulations, int startGenerationOn, int percentOfOpPropsToLoad, int populationSize, int numberOfTrials, String unitAgentXMLFileName, int gameSpeed, int nuberOfCrossOvers)
        {
            if (totalNumberOfRepopulations > startGenerationOn)
                _numberOfRepopulations = totalNumberOfRepopulations - startGenerationOn;
            else
            {
                Logger.Logger.AddAndPrint("numberOfRepopulations needs to be bigger than startGenerationOn.");
                throw new Exception("numberOfRepopulations needs to be bigger than startGenerationOn.");
            }
            
            _populationSize = populationSize;
            _numberOfTrials = numberOfTrials;
            _nuberOfCrossOvers = nuberOfCrossOvers;
            _unitAgentXMLFileName = unitAgentXMLFileName;
            _startGenerationOn = startGenerationOn;

            _gameSpeed = gameSpeed;
            _ea = new EvolutionaryAlgorithm(_populationSize, percentOfOpPropsToLoad, _unitAgentXMLFileName);
            
             Train();
        }

        /// <summary>
        /// Use the best optimized agent to play StarCraft Broodwar (using the fittest unit agent optimized properties loaded from an XML file).
        /// </summary>
        /// <param name="numberOfRestarts"></param>
        /// <param name="populationSize"></param>
        /// <param name="unitAgentXMLFileName"></param>
        /// <param name="gameSpeed"></param>
        public Trainer(int numberOfRestarts, int populationSize, String unitAgentXMLFileName, int gameSpeed)
        {
            _numberOfRepopulations = numberOfRestarts;
            _populationSize = populationSize;
            _unitAgentXMLFileName = unitAgentXMLFileName;

            _gameSpeed = gameSpeed;

            _ea = new EvolutionaryAlgorithm(_populationSize, _unitAgentXMLFileName);

             PlayWithoutTraining(_gameSpeed, numberOfRestarts, true);//loadAgent: TRUE: CALL PLAY and use the current best optimized XMLfile, else use test method to load some standard values.
        }

        /// <summary>
        /// Use the best optimized agents to play StarCraft Broodwar (using the fittest unit agent optimized properties loaded from an XML file).
        /// </summary>
        /// <param name="numberOfRestarts"></param>
        /// <param name="populationSize"></param>
        /// <param name="gameSpeed"></param>
        public Trainer(int numberOfRestarts, int populationSize, int gameSpeed)
        {
            _numberOfRepopulations = numberOfRestarts;
            _populationSize = populationSize;
            _unitAgentXMLFileName = "";

            var xmlFileNames = new List<String>() {"Terran_Goliath.xml", "Terran_Vulture.xml", "Terran_SiegeTank.xml" };

            _gameSpeed = gameSpeed;

            _ea = new EvolutionaryAlgorithm(_populationSize, xmlFileNames);

            PlayWithoutTraining(_gameSpeed, numberOfRestarts, true);//loadAgent: TRUE: CALL PLAY and use the current best optimized XMLfile, else use test method to load some standard values.
        }

        /// <summary>
        /// TEST METHOD for running with predefined unit agent optimized properties.
        /// </summary>
        /// <param name="unitTypeName"></param>
        /// <returns></returns>
        public static UnitAgentOptimizedProperties GetOptimizedValuesToUnitAgent(String unitTypeName)//ref UnitAgent unitAgent)
        {
            var opProp = new UnitAgentOptimizedProperties();
            opProp.UnitTypeName = unitTypeName;

            //Force
            opProp.ForceOwnUnitsRepulsion = 100;
            opProp.ForceEnemyUnitsRepulsion = 200;
            opProp.ForceMSD = 240;
            opProp.ForceSquadAttraction = 10;
            opProp.ForceMapCenterAttraction = 20;
            opProp.ForceMapEdgeRepulsion = 50;
            opProp.ForceWeaponCoolDownRepulsion = 800;

            //ForceStep
            opProp.ForceStepOwnUnitsRepulsion = 0.6;
            opProp.ForceStepEnemyUnitsRepulsion = 1.2;
            opProp.ForceStepMSD = 0.24;
            opProp.ForceStepSquadAttraction = 0.1;
            opProp.ForceStepMapCenterAttraction = 0.09;
            opProp.ForceStepMapEdgeRepulsion = 0.3;
            opProp.ForceStepWeaponCoolDownRepulsion = 6.4;

            //Range
            opProp.RangeOwnUnitsRepulsion = 8;
            opProp.RangePercentageSquadAttraction = 5;
            opProp.RangePecentageMapCenterAttraction = 5;
            opProp.RangeMapEdgeRepulsion = 96;
            opProp.RangeWeaponCooldownRepulsion = 160;

            return opProp;
        }

        public void PlayWithoutTraining(int gameSpeed, int numberOfRestarts, Boolean loadAgent)
        {
            try
            {
               SWIG.BWAPI.bwapi.BWAPI_init();
                Console.WriteLine("Waiting for StarCraft Broodwar to be started from Chaoslauncher.");
                Reconnect();
                Console.WriteLine("The bot is now connected to StarCraft Broodwar");
                Console.WriteLine("Waiting for the match to begin");

                for (int i = 0; i < numberOfRestarts; i++)
                {                     
                    //Play(optimizedPropertiesGene, -1, true);
                    Console.WriteLine("run number: " + i);
                    Console.WriteLine("The bot is getting ready for battle!");
                   SWIG.BWAPI.bwapi.Broodwar.sendText("GL HF");//Good Luck Have Fun

                    if (loadAgent)
                        Play(_ea.OptimizedPropertiesGenes, gameSpeed, true, false, true);
                    else
                        Play(new List<UnitAgentOptimizedProperties>() { GetOptimizedValuesToUnitAgent("TESTUNIT") }, gameSpeed, true, false, true);
                    
                    Console.WriteLine("Game ended");
                    Console.WriteLine("Fitness Score After end game: " + Fitness);
                    Logger.Logger.AddAndPrint("" + Fitness);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                Console.WriteLine(e.StackTrace);
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += LastChanceHandler;
            }
        }

        public void TTest()
        {
            try
            {
               SWIG.BWAPI.bwapi.BWAPI_init();
                Console.WriteLine("Waiting for StarCraft Broodwar to be started from Chaoslauncher."); //Connecting...");
                Reconnect();

                Console.WriteLine("The bot is now connected to StarCraft Broodwar");
                Console.WriteLine("Waiting for the match to begin");
  
                Logger.Logger.AddAndPrint(
                    "c1;c2;c3;c4;c5;c6;c7;c8;c9;c10;c11;c12;c13;c14;c15;c16;c17;c18;c19;c20;c21;c22;c23;c24;c25;c26;c27;c28;c29;c30;c31;c32;c33;c34;c35;c36;c37;c38;c39;c40;c41;c42;c43;c44;c45;c46;c47;c48;c49;c50;c51;c52;c53;c54;c55;c56;c57;c58;c59;c60;c61;c62;c63;c64;c65;c66;c67;c68;c69;c70;c71;c72;c73;c74;c75;c76;c77;c78;c79;c80;c81;c82;c83;c84;c85;c86;c87;c88;c89;c90;c91;c92;c93;c94;c95;c96;c97;c98;c99;c100");
                for (int i = 1; i <= _numberOfRepopulations; i++)
                {
                    Console.WriteLine("Round " + i);
                    int indexOfOptimizedPropertiesGenes = 0;
                    foreach (UnitAgentChromosome optimizedPropertiesChromosome in _ea.Population)
                    {
                        //UnitAgentOptimizedProperties optimizedPropertiesGene = _ea.OptimizedPropertiesGenes[indexOfOptimizedPropertiesGenes];
                        var optimizedPropertiesGeneList = new List<UnitAgentOptimizedProperties>() { _ea.OptimizedPropertiesGenes[indexOfOptimizedPropertiesGenes] };
                        
                        Play(optimizedPropertiesGeneList, _gameSpeed, true, true, false); //Should take a parameter representing UnitAgentOptimizedProperties

                        Logger.Logger.AddAndPrintSingleLine("" + Fitness + ";");

                        if ((indexOfOptimizedPropertiesGenes % 10) == 0)
                            Console.Write(".");

                        indexOfOptimizedPropertiesGenes++;
                    }
                     Logger.Logger.AddAndPrint("");
                }

                //Console.WriteLine("...SAVING FOR THE LAST TIME...");
                //_ea.SaveOptimizedPropertiesToXMLFile();
                //Console.WriteLine("...SAVING DONE...");
                //Console.WriteLine("...The EA has finished. Good bye for this time...");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                Console.WriteLine(e.StackTrace);
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += LastChanceHandler;
            }
        }


        public void PlayWithRandomChromosomes()
        {
            try
            {
                SWIG.BWAPI.bwapi.BWAPI_init();
                Console.WriteLine("Waiting for StarCraft Broodwar to be started from Chaoslauncher."); //Connecting...");
                Reconnect();
                int fitnessScore = 0;

                Console.WriteLine("The bot is now connected to StarCraft Broodwar");
                Console.WriteLine("Waiting for the match to begin");

                Logger.Logger.AddAndPrintSingleLine(
                    "c1;c2;c3;c4;c5;c6;c7;c8;c9;c10;c11;c12;c13;c14;c15;c16;c17;c18;c19;c20;c21;c22;c23;c24;c25;c26;c27;c28;c29;c30;c31;c32;c33;c34;c35;c36;c37;c38;c39;c40;c41;c42;c43;c44;c45;c46;c47;c48;c49;c50;c51;c52;c53;c54;c55;c56;c57;c58;c59;c60;c61;c62;c63;c64;c65;c66;c67;c68;c69;c70;c71;c72;c73;c74;c75;c76;c77;c78;c79;c80;c81;c82;c83;c84;c85;c86;c87;c88;c89;c90;c91;c92;c93;c94;c95;c96;c97;c98;c99;c100");

                for (int i = 1; i <= _numberOfRepopulations; i++)
                {
                    Console.WriteLine("Round " + i);
                    Logger.Logger.AddAndPrint("");
                    int indexOfOptimizedPropertiesGenes = 0;

                    foreach (UnitAgentChromosome optimizedPropertiesChromosome in _ea.Population)
                    {
                        //UnitAgentOptimizedProperties optimizedPropertiesGene = _ea.OptimizedPropertiesGenes[indexOfOptimizedPropertiesGenes];
                        var optimizedPropertiesGeneList = new List<UnitAgentOptimizedProperties>() { _ea.OptimizedPropertiesGenes[indexOfOptimizedPropertiesGenes] };

                        //for (int trials = 0; trials < 2; trials++)
                        //{
                        Play(optimizedPropertiesGeneList, _gameSpeed, true, true, false); //Should take a parameter representing UnitAgentOptimizedProperties
                            fitnessScore += Fitness;
                       // }
                        optimizedPropertiesChromosome.Fitness = fitnessScore;
                        fitnessScore = 0;

                        Logger.Logger.AddAndPrintSingleLine("" + optimizedPropertiesChromosome.Fitness + ";");
                        Console.WriteLine("Fitness score: " + optimizedPropertiesChromosome.Fitness);

                        if ((indexOfOptimizedPropertiesGenes % 10) == 0)
                            Console.Write(".");

                        indexOfOptimizedPropertiesGenes++;
                    }
                }
                
                Console.WriteLine("...The EA has finished. Good bye for this time...");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                Console.WriteLine(e.StackTrace);
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += LastChanceHandler;
            }
        }

        /// <summary>
        /// Train the population of candidates.
        /// </summary>
        public void Train()
        {
            try
            {
               SWIG.BWAPI.bwapi.BWAPI_init();
                Console.WriteLine("Waiting for StarCraft Broodwar to be started from Chaoslauncher."); //Connecting...");
                Reconnect();
                int fitnessScore = 0;


                Console.WriteLine("The bot is now connected to StarCraft Broodwar");
                Console.WriteLine("Waiting for the match to begin");

                Boolean repopulate = false;

                if (String.IsNullOrEmpty(_unitAgentXMLFileName))
                    Logger.Logger.AddAndPrint("Best ; Worst ; Average");

                //Only repopulate from start in first generation, if data has been loaded from a file and fitness values exists. 
                else if (_ea.OptimizedPropertiesGenes != null && _ea.OptimizedPropertiesGenes[0].Fitness != 0) 
                {
                    repopulate = true;
                }

                for (int i = 0; i <= _numberOfRepopulations; i++)
                {
                    var roundNr = i + _startGenerationOn;
                    Console.WriteLine("Round " + roundNr);

                    if (repopulate)
                        _ea.Repopulate(_nuberOfCrossOvers); 
   
                    int indexOfOptimizedPropertiesGenes = 0;	
                    foreach (UnitAgentChromosome optimizedPropertiesChromosome in _ea.Population)
                    {
                        if (_ea.OptimizedPropertiesGenes != null)
                        {
                            //UnitAgentOptimizedProperties optimizedPropertiesGene = _ea.OptimizedPropertiesGenes[indexOfOptimizedPropertiesGenes];
                            var optimizedPropertiesGeneList = new List<UnitAgentOptimizedProperties>(){_ea.OptimizedPropertiesGenes[indexOfOptimizedPropertiesGenes]};
    
                            for (int trials = 0; trials < _numberOfTrials; trials++)
                            {
                                Play(optimizedPropertiesGeneList,_gameSpeed, true, true, false); //optimizedPropertiesGene, _gameSpeed, true, true, false); 
                                fitnessScore += Fitness;
                                //optimizedPropertiesChromosome.Fitness 
                            }
                        
                        optimizedPropertiesChromosome.Fitness = fitnessScore;
                        //ONLY NECESSARY IF WE NEED TO SAVE THE FIRST RANDOM POPULATION WITH THEIR FITNESS SCORE 
                        if (!_saveXMLInSameFile)//i == 0 && //AFTER FIRST ROUND THIS WILL HAPPEN AUTOMATICALLY, BECAUSE THE UnitAgentChromosome WILL BE CONVERTED TO UnitAgentOptimizedProperties in the repopulate medtod.
                            _ea.OptimizedPropertiesGenes[indexOfOptimizedPropertiesGenes].Fitness = fitnessScore;                      
                        ////
                        fitnessScore = 0;

                        if ((indexOfOptimizedPropertiesGenes % 10) == 0)
                            Console.Write(".");

                        indexOfOptimizedPropertiesGenes++;
                        }
                        else
                        {
                            Logger.Logger.AddAndPrint("The _ea.OptimizedPropertiesGenes is null in Trainer.cs->Train().");
                            throw new Exception("The _ea.OptimizedPropertiesGenes is null in Trainer.cs->Train().");
                        }
                    }

                    //if (i > 0)//Convert and add all the Chromosomes in the population to the UnitAgentOptimizedProperties List. Not the first time, because this will automatically be called from the _ga on init.
                    //{
                        //if ((i % saveCount) == 0) //Save the best calculated agent after each 10th repopulation.
                       // {
                            
                            if (!_saveXMLInSameFile)
                            {
                                if (_ea.OptimizedPropertiesGenes != null)
                                {
                                    _ea.OptimizedPropertiesGenes.Sort();
                                    var rank = 1;
                                    foreach (UnitAgentOptimizedProperties uaop in _ea.OptimizedPropertiesGenes)
                                        uaop.Rank = rank++;
                                }
                                int genPrintNumber = i + _startGenerationOn;
                                _ea.UnitAgentXMLFileHandler.Filename = "!StarCraftUnitAgent" + genPrintNumber + ".xml";
                                Console.WriteLine("...SAVING...");
                                _ea.SaveOptimizedPropertiesToXMLFile();
                                Console.WriteLine("...SAVING DONE...");
                            }
                            else if ((i % saveCount) == 0)//Save the best calculated agent after each n repopulation.
                            {
                                Console.WriteLine("...SAVING...");
                                _ea.SaveOptimizedPropertiesToXMLFile();
                                Console.WriteLine("...SAVING DONE...");
                            }
                            
                            _ea.LogBestWorstAverageFitness(); //Sort population, print to console best / worst fitness and log best, worst and average fitness to log file.
                            repopulate = true;//Make sure to repopulate from start in generation 2, if not already set to true, else we will have no evolution.
                       //}
                    //}
                }

                if (_saveXMLInSameFile)//ONLY NECESSARY WHEN NOT SAVING AFTER EACH RUN. 
                { 
                    Console.WriteLine("...SAVING FOR THE LAST TIME...");
                    _ea.SortPopulationAfterFitness();
                    _ea.ConvertPopulationToUnitAgentOptimizedProperties();
                    _ea.SaveOptimizedPropertiesToXMLFile();
                    Console.WriteLine("...SAVING DONE...");
                }
                Console.WriteLine("...The EA has finished training. Good bye for this time.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                Console.WriteLine(e.StackTrace);
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += LastChanceHandler;
            }
        }

        private static void LastChanceHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var ex = ((Exception)(args.ExceptionObject));

            Logger.Logger.Filename = Settings.Settings.LastChanceHandlerError;
            Logger.Logger.AddAndPrint("****** LastChanceHandler ******");
            Logger.Logger.AddAndPrint("ExceptionType: " + ex.GetType().Name);
            Logger.Logger.AddAndPrint("HelpLine: " + ex.HelpLink);
            Logger.Logger.AddAndPrint("Message: " + ex.Message);
            Logger.Logger.AddAndPrint("Source: " +ex.Source);
            Logger.Logger.AddAndPrint("StackTrace: " + ex.StackTrace);
            Logger.Logger.AddAndPrint("TargetSite: " + ex.TargetSite);

            Console.WriteLine("****** LastChanceHandler ******");
            Console.WriteLine("ExceptionType: {0}", ex.GetType().Name);
            Console.WriteLine("HelpLine: {0}", ex.HelpLink);
            Console.WriteLine("Message: {0}", ex.Message);
            Console.WriteLine("Source: {0}", ex.Source);
            Console.WriteLine("StackTrace: {0}", ex.StackTrace);
            Console.WriteLine("TargetSite: {0}", ex.TargetSite);

            Logger.Logger.Filename = Settings.Settings.LogFilename;
        }
    }
}