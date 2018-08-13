using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate.Internal.Core;
using nUpdate.Updating;

namespace nUpdate.Test
{
    [TestClass]
    public class ConditionsAtLeastOneTest
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
        [TestCategory("Conditions.Mode.AtLeastOne.FirstScenario")]
        public void ConditionCheckWithFirstScenarioMustWorkWell_1()
        {
            SetFirstConfigScenario();
            _clientConditions.Add(new KeyValuePair<string, string>("R", "east"));
            _clientConditions.Add(new KeyValuePair<string, string>("CNR", "23654"));

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
        [TestCategory("Conditions.Mode.AtLeastOne.FirstScenario")]
        public void ConditionCheckWithFirstScenarioMustWorkWell_2()
        {
            SetFirstConfigScenario(); //1.2.0.0 and 1.3.0.0
            _clientConditions.Add(new KeyValuePair<string, string>("R", "west"));
            _clientConditions.Add(new KeyValuePair<string, string>("CNR", "23654"));

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
        [TestCategory("Conditions.Mode.AtLeastOne.FirstScenario")]
        public void ConditionCheckWithFirstScenarioMustWorkWell_3()
        {
            SetFirstConfigScenario(); //1.2.0.0 and 1.3.0.0
            _clientConditions.Add(new KeyValuePair<string, string>("R", "east"));
            _clientConditions.Add(new KeyValuePair<string, string>("CNR", "56478"));

            List<string> sutWishedUpdates = new List<string> { "1.2.0.0", "1.3.0.0" };

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
        [TestCategory("Conditions.Mode.AtLeastOne.FirstScenario")]
        public void ConditionCheckWithFirstScenarioMustWorkWell_4()
        {
            SetFirstConfigScenario(); //1.2.0.0 and 1.3.0.0
            _clientConditions.Add(new KeyValuePair<string, string>("R", "west"));
            _clientConditions.Add(new KeyValuePair<string, string>("CNR", "10394"));

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
        [TestCategory("Conditions.Mode.AtLeastOne.SecondScenario")]
        public void ConditionCheckWithSecondScenarioMustWorkWell_1()
        {
            SetSecondConfigScenario(); //1.2.0.0 and 1.3.0.0
            _clientConditions.Add(new KeyValuePair<string, string>("CC", "3"));
            _clientConditions.Add(new KeyValuePair<string, string>("CNR", "23654"));

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
        [TestCategory("Conditions.Mode.AtLeastOne.SecondScenario")]
        public void ConditionCheckWithSecondScenarioMustWorkWell_2()
        {
            SetSecondConfigScenario(); //1.2.0.0 and 1.3.0.0
            _clientConditions.Add(new KeyValuePair<string, string>("CC", "4"));
            _clientConditions.Add(new KeyValuePair<string, string>("CNR", "23654"));

            // ReSharper disable once CollectionNeverUpdated.Local
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
        [TestCategory("Conditions.Mode.AtLeastOne.SecondScenario")]
        public void ConditionCheckWithSecondScenarioMustWorkWell_3()
        {
            SetSecondConfigScenario(); //1.2.0.0 and 1.3.0.0
            _clientConditions.Add(new KeyValuePair<string, string>("CC", "3"));
            _clientConditions.Add(new KeyValuePair<string, string>("CNR", "56478"));

            List<string> sutWishedUpdates = new List<string> { "1.2.0.0", "1.3.0.0" };

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
        [TestCategory("Conditions.Mode.AtLeastOne.SecondScenario")]
        public void ConditionCheckWithSecondScenarioMustWorkWell_4()
        {
            SetSecondConfigScenario(); //1.2.0.0 and 1.3.0.0
            _clientConditions.Add(new KeyValuePair<string, string>("CC", "1"));
            _clientConditions.Add(new KeyValuePair<string, string>("CNR", "10394"));

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
        [TestCategory("Conditions.Mode.AtLeastOne.ThirdScenario")]
        public void ConditionCheckWithThirdScenarioMustWorkWell_1()
        {
            SetThirdConfigScenario(); //1.2.0.0 and 1.3.0.0
            _clientConditions.Add(new KeyValuePair<string, string>("P", "special"));
            
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
        [TestCategory("Conditions.Mode.AtLeastOne.ThirdScenario")]
        public void ConditionCheckWithThirdScenarioMustWorkWell_2()
        {
            SetThirdConfigScenario(); //1.2.0.0 and 1.3.0.0
            _clientConditions.Add(new KeyValuePair<string, string>("P", "test"));

            // ReSharper disable once CollectionNeverUpdated.Local
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
        [TestCategory("Conditions.Mode.AtLeastOne.ThirdScenario")]
        public void ConditionCheckWithThirdScenarioMustWorkWell_3()
        {
            SetThirdConfigScenario(); //1.2.0.0 and 1.3.0.0
            _clientConditions.Add(new KeyValuePair<string, string>("P", "secure"));

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


       






        [TestMethod]
        [TestCategory("Conditions.Mode.AtLeastOne")]
        public void ConditionCheckWithoutClientValuesButWithRemoteValuesMustWorkWell()
        {
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition> 
                {new RolloutCondition("P","secure")}));
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>()));

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
        [TestCategory("Conditions.Mode.AtLeastOne")]
        public void ConditionCheckWithoutClientValuesAndWithoutRemoteValuesMustWorkWell()
        {
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>()));
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>()));

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






        [TestMethod]
        [TestCategory("Conditions")]
        [Description("If no conditions are defined in the package, we always pull the update")]
        public void ConditionCheckWithoutRemoteValuesMustAllwayReturnTrue()
        {
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>()));

            UpdateResult sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            Assert.IsTrue(sut.CheckConditions(_clientConditions, _updateConfigs.First()));
        }

        [TestMethod]
        [TestCategory("Conditions")]
        [Description("If no conditions are defined in the package, we always pull the update")]
        public void ConditionCheckWithoutRemoteValuesMustReturnAllUpdates()
        {
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>()));
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>()));
            _updateConfigs.Add(CreateConfig("1.3.4.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>()));

            UpdateResult sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            List<string> sutWishedUpdates = new List<string> { "1.2.0.0", "1.3.0.0", "1.3.4.0" };
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
        [TestCategory("Conditions")]
        public void ConditionCheckCaseUnsensitiveValueOnClientsideTest()
        {
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>
                {new RolloutCondition("R","east"),new RolloutCondition("CNR","23654",true)}));
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>
                {new RolloutCondition("R","west"),new RolloutCondition("CNR","23654",true)}));
            
            _clientConditions.Add(new KeyValuePair<string, string>("R", "eAsT"));

            UpdateResult sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            List<string> sutWishedUpdates = new List<string> { "1.2.0.0" };
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
        [TestCategory("Conditions")]
        public void ConditionCheckCaseUnsensitiveValueOnServersideTest()
        {
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>
                {new RolloutCondition("R","EaSt"),new RolloutCondition("CNR","23654",true)}));
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>
                {new RolloutCondition("R","weST"),new RolloutCondition("CNR","23654",true)}));
            
            _clientConditions.Add(new KeyValuePair<string, string>("R", "east"));

            UpdateResult sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            List<string> sutWishedUpdates = new List<string> { "1.2.0.0" };
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
            //Rollout to all customers in region „east”, but without customer with Nr. „23654" not matter in what region the customer is.
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition> 
                {new RolloutCondition("R","east"),new RolloutCondition("CNR","23654",true)}));

            //Rollout to all customers in regions „east” and „west”, but without customers with Nr. „36587" and „32578" not matter in what region the customers are.
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition> 
            {new RolloutCondition("R","east"),new RolloutCondition("R","west"),
                new RolloutCondition("CNR","36587",true),new RolloutCondition("CNR","32578",true)}));
        }

        private void SetSecondConfigScenario()
        {
            //Rollout to all customers in customer cyrcle „3" but without customer with Nr. „23654" not matter in what customercyrcle the customer is.
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition> 
                {new RolloutCondition("CC","3"),new RolloutCondition("CNR","23654",true)}));

            //Rollout to all customers in customer cyrcle „1", „3", but without customer cyrcle „4".
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition> 
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
