using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate.Internal.Core;
using nUpdate.Updating;

namespace nUpdate.Test
{
    [TestClass]
    public class ConditionsAllTest
    {
        private List<KeyValuePair<string, string>> _clientConditions;
        private List<UpdateConfiguration> _updateConfigs;


        [TestInitialize]
        public void InitTest()
        {
            _clientConditions = new List<KeyValuePair<string, string>>();
            _updateConfigs = new List<UpdateConfiguration>();
        }

        

        [TestMethod]
        [TestCategory("Conditions.Mode.All.FirstScenario")]
        public void ConditionCheckWithFirstScenarioMustWorkWell_1()
        {
            SetFirstConfigScenario();
            _clientConditions.Add(new KeyValuePair<string, string>("R", "east"));
            _clientConditions.Add(new KeyValuePair<string, string>("CNR", "23654"));

            List<string> sutWishedUpdates = new List<string> { "1.2.0.0" };

            UpdateResult sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);
            
            List<string> sutUpdateResults = new List<string>();
            foreach (UpdateConfiguration c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }
            Assert.AreEqual(sutWishedUpdates.Count(), sutUpdateResults.Count);

            foreach (string wu in sutWishedUpdates)
            {
                Assert.IsTrue(sutUpdateResults.Any(r => r.Equals(wu)));
            }
        }

        [TestMethod]
        [TestCategory("Conditions.Mode.All.FirstScenario")]
        public void ConditionCheckWithFirstScenarioMustWorkWell_2()
        {
            SetFirstConfigScenario(); //1.2.0.0 and 1.3.0.0
            _clientConditions.Add(new KeyValuePair<string, string>("R", "east"));
            _clientConditions.Add(new KeyValuePair<string, string>("CNR", "36448"));

            List<string> sutWishedUpdates = new List<string> { "1.3.0.0" };

            UpdateResult sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            List<string> sutUpdateResults = new List<string>();
            foreach (UpdateConfiguration c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }
            Assert.AreEqual(sutWishedUpdates.Count(), sutUpdateResults.Count);

            foreach (string wu in sutWishedUpdates)
            {
                Assert.IsTrue(sutUpdateResults.Any(r => r.Equals(wu)));
            }
        }


        [TestMethod]
        [TestCategory("Conditions.Mode.All.FirstScenario")]
        public void ConditionCheckWithFirstScenarioMustWorkWell_3()
        {
            SetFirstConfigScenario(); //1.2.0.0 and 1.3.0.0
            _clientConditions.Add(new KeyValuePair<string, string>("R", "east"));
            _clientConditions.Add(new KeyValuePair<string, string>("CNR", "36447"));

            List<string> sutWishedUpdates = new List<string>();

            UpdateResult sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            List<string> sutUpdateResults = new List<string>();
            foreach (UpdateConfiguration c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }
            Assert.AreEqual(sutWishedUpdates.Count(), sutUpdateResults.Count);

            foreach (string wu in sutWishedUpdates)
            {
                Assert.IsTrue(sutUpdateResults.Any(r => r.Equals(wu)));
            }
        }


      
        [TestMethod]
        [TestCategory("Conditions.Mode.All.SecondScenario")]
        public void ConditionCheckWithSecondScenarioMustWorkWell_1()
        {
            SetSecondConfigScenario(); //1.2.0.0 and 1.3.0.0
            _clientConditions.Add(new KeyValuePair<string, string>("CC", "3"));
           
            List<string> sutWishedUpdates = new List<string> { "1.2.0.0" };

            UpdateResult sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            List<string> sutUpdateResults = new List<string>();
            foreach (UpdateConfiguration c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }
            Assert.AreEqual(sutWishedUpdates.Count(), sutUpdateResults.Count);

            foreach (string wu in sutWishedUpdates)
            {
                Assert.IsTrue(sutUpdateResults.Any(r => r.Equals(wu)));
            }
        }





        [TestMethod]
        [TestCategory("Conditions.Mode.All")]
        public void ConditionCheckWithoutClientValuesButWithRemoteValuesMustWorkWell()
        {
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.All, new List<RolloutCondition> 
                {new RolloutCondition("P","secure")}));
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.All, new List<RolloutCondition>()));

            List<string> sutWishedUpdates = new List<string> { "1.3.0.0"};
            UpdateResult sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            List<string> sutUpdateResults = new List<string>();
            foreach (UpdateConfiguration c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }
            Assert.AreEqual(sutWishedUpdates.Count(), sutUpdateResults.Count);

            foreach (string wu in sutWishedUpdates)
            {
                Assert.IsTrue(sutUpdateResults.Any(r => r.Equals(wu)));
            }
        }


        [TestMethod]
        [TestCategory("Conditions.Mode.All")]
        public void ConditionCheckWithoutClientValuesAndWithoutRemoteValuesMustWorkWell()
        {
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.All, new List<RolloutCondition>()));
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.All, new List<RolloutCondition>()));

            List<string> sutWishedUpdates = new List<string> {"1.2.0.0", "1.3.0.0"};
            UpdateResult sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            List<string> sutUpdateResults = new List<string>();
            foreach (UpdateConfiguration c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }
            Assert.AreEqual(sutWishedUpdates.Count(), sutUpdateResults.Count);

            foreach (string wu in sutWishedUpdates)
            {
                Assert.IsTrue(sutUpdateResults.Any(r => r.Equals(wu)));
            }
        }





        private void SetFirstConfigScenario()
        {
           _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.All, new List<RolloutCondition> 
                {new RolloutCondition("R","east"),new RolloutCondition("CNR","23654")}));

           _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.All, new List<RolloutCondition> 
            {new RolloutCondition("R","east"),new RolloutCondition("CNR","36448")}));
        }

        private void SetSecondConfigScenario()
        {
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.All, new List<RolloutCondition> 
                {new RolloutCondition("CC","3"),new RolloutCondition("CNR","23654",true)}));

            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.All, new List<RolloutCondition> 
            {new RolloutCondition("CC","1"),new RolloutCondition("CC","3"),new RolloutCondition("CC","4",true)}));
        }


        private void SetThirdConfigScenario()
        {
            //Here a test maybe with a password for updatepackage. Password „secure” is needed.
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition> 
                {new RolloutCondition("P","secure")}));

            //Here a test maybe with passwords for a updatepackage. Password „secure” or „special” is valid.
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition> 
                {new RolloutCondition("P","secure"),new RolloutCondition("P","special")}));
        }



        private UpdateConfiguration CreateConfig(string version, RolloutConditionMode conditionMode, List<RolloutCondition> conditions, bool isnecessary = true)
        {
            UpdateConfiguration config = new UpdateConfiguration()
            {
                LiteralVersion = version,
                RolloutConditionMode = conditionMode,
                RolloutConditions = conditions,
                NecessaryUpdate = isnecessary
            };
            return config;
        }

    }
}
