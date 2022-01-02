// ConditionsAtLeastOneTest.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            var sutWishedUpdates = new List<string> {"1.3.0.0"};
            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutUpdateResults = new List<string>();
            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);

            foreach (var wu in sutWishedUpdates)
            {
                Assert.IsTrue(sutUpdateResults.Any(r => r.Equals(wu)));
            }
        }

        [TestMethod]
        [TestCategory("Conditions.Mode.AtLeastOne.FirstScenario")]
        public void ConditionCheckWithFirstScenarioMustWorkWell_2()
        {
            SetFirstConfigScenario(); // 1.2.0.0 and 1.3.0.0
            _clientConditions.Add(new KeyValuePair<string, string>("R", "west"));
            _clientConditions.Add(new KeyValuePair<string, string>("CNR", "23654"));

            var sutWishedUpdates = new List<string> {"1.3.0.0"};

            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutUpdateResults = new List<string>();
            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);

            foreach (var wu in sutWishedUpdates)
            {
                Assert.IsTrue(sutUpdateResults.Any(r => r.Equals(wu)));
            }
        }


        [TestMethod]
        [TestCategory("Conditions.Mode.AtLeastOne.FirstScenario")]
        public void ConditionCheckWithFirstScenarioMustWorkWell_3()
        {
            SetFirstConfigScenario(); // 1.2.0.0 and 1.3.0.0
            _clientConditions.Add(new KeyValuePair<string, string>("R", "east"));
            _clientConditions.Add(new KeyValuePair<string, string>("CNR", "56478"));

            var sutWishedUpdates = new List<string> {"1.2.0.0", "1.3.0.0"};

            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutUpdateResults = new List<string>();
            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);

            foreach (var wu in sutWishedUpdates)
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

            var sutWishedUpdates = new List<string> {"1.3.0.0"};

            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutUpdateResults = new List<string>();
            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);

            foreach (var wu in sutWishedUpdates)
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

            var sutWishedUpdates = new List<string> {"1.3.0.0"};

            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutUpdateResults = new List<string>();
            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);

            foreach (var wu in sutWishedUpdates)
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
            var sutWishedUpdates = new List<string>();

            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutUpdateResults = new List<string>();
            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);

            foreach (var wu in sutWishedUpdates)
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

            var sutWishedUpdates = new List<string> {"1.2.0.0", "1.3.0.0"};

            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutUpdateResults = new List<string>();
            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);

            foreach (var wu in sutWishedUpdates)
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

            var sutWishedUpdates = new List<string> {"1.3.0.0"};

            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutUpdateResults = new List<string>();
            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);

            foreach (var wu in sutWishedUpdates)
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

            var sutWishedUpdates = new List<string> {"1.3.0.0"};

            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutUpdateResults = new List<string>();
            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);

            foreach (var wu in sutWishedUpdates)
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
            var sutWishedUpdates = new List<string>();
            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutUpdateResults = new List<string>();
            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);

            foreach (var wu in sutWishedUpdates)
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

            var sutWishedUpdates = new List<string> {"1.2.0.0", "1.3.0.0"};
            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutUpdateResults = new List<string>();
            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);

            foreach (var wu in sutWishedUpdates)
            {
                Assert.IsTrue(sutUpdateResults.Any(r => r.Equals(wu)));
            }
        }


        [TestMethod]
        [TestCategory("Conditions.Mode.AtLeastOne")]
        public void ConditionCheckWithoutClientValuesButWithRemoteValuesMustWorkWell()
        {
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>
                {new RolloutCondition("P", "secure")}));
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>()));

            var sutWishedUpdates = new List<string> {"1.3.0.0"};
            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutUpdateResults = new List<string>();
            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);

            foreach (var wu in sutWishedUpdates)
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

            var sutWishedUpdates = new List<string> {"1.2.0.0", "1.3.0.0"};
            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutUpdateResults = new List<string>();
            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);

            foreach (var wu in sutWishedUpdates)
            {
                Assert.IsTrue(sutUpdateResults.Any(r => r.Equals(wu)));
            }
        }


        [TestMethod]
        [TestCategory("Conditions")]
        [System.ComponentModel.Description("If no conditions are defined in the package, we always pull the update")]
        public void ConditionCheckWithoutRemoteValuesMustAllwayReturnTrue()
        {
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>()));

            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            Assert.IsTrue(sut.CheckConditions(_clientConditions, _updateConfigs.First()));
        }

        [TestMethod]
        [TestCategory("Conditions")]
        [System.ComponentModel.Description("If no conditions are defined in the package, we always pull the update")]
        public void ConditionCheckWithoutRemoteValuesMustReturnAllUpdates()
        {
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>()));
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>()));
            _updateConfigs.Add(CreateConfig("1.3.4.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>()));

            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutWishedUpdates = new List<string> {"1.2.0.0", "1.3.0.0", "1.3.4.0"};
            var sutUpdateResults = new List<string>();

            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);
            foreach (var wu in sutWishedUpdates)
            {
                Assert.IsTrue(sutUpdateResults.Any(r => r.Equals(wu)));
            }
        }


        [TestMethod]
        [TestCategory("Conditions")]
        public void ConditionCheckCaseUnsensitiveValueOnClientsideTest()
        {
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>
                {new RolloutCondition("R", "east"), new RolloutCondition("CNR", "23654", true)}));
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>
                {new RolloutCondition("R", "west"), new RolloutCondition("CNR", "23654", true)}));

            _clientConditions.Add(new KeyValuePair<string, string>("R", "eAsT"));

            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutWishedUpdates = new List<string> {"1.2.0.0"};
            var sutUpdateResults = new List<string>();

            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);
            foreach (var wu in sutWishedUpdates)
            {
                Assert.IsTrue(sutUpdateResults.Any(r => r.Equals(wu)));
            }
        }

        [TestMethod]
        [TestCategory("Conditions")]
        public void ConditionCheckCaseUnsensitiveValueOnServersideTest()
        {
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>
                {new RolloutCondition("R", "EaSt"), new RolloutCondition("CNR", "23654", true)}));
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>
                {new RolloutCondition("R", "weST"), new RolloutCondition("CNR", "23654", true)}));

            _clientConditions.Add(new KeyValuePair<string, string>("R", "east"));

            var sut = new UpdateResult(_updateConfigs, new UpdateVersion(1, 0, 0, 0), false, false,
                _clientConditions);

            var sutWishedUpdates = new List<string> {"1.2.0.0"};
            var sutUpdateResults = new List<string>();

            foreach (var c in _updateConfigs)
            {
                if (sut.CheckConditions(_clientConditions, c))
                {
                    sutUpdateResults.Add(c.LiteralVersion);
                }
            }

            Assert.AreEqual(sutWishedUpdates.Count, sutUpdateResults.Count);
            foreach (var wu in sutWishedUpdates)
            {
                Assert.IsTrue(sutUpdateResults.Any(r => r.Equals(wu)));
            }
        }

        private void SetFirstConfigScenario()
        {
            // Roll out to all customers in region „east”, but without customer „23654", no matter in what region the customer is.
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>
                {new RolloutCondition("R", "east"), new RolloutCondition("CNR", "23654", true)}));

            // Roll out to all customers in regions „east” and „west”, but without customers „36587" and „32578", no matter in what region they are.
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>
            {
                new RolloutCondition("R", "east"), new RolloutCondition("R", "west"),
                new RolloutCondition("CNR", "36587", true), new RolloutCondition("CNR", "32578", true)
            }));
        }

        private void SetSecondConfigScenario()
        {
            // Roll out to all customers in customer circle „3", but without customer „23654".
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>
                {new RolloutCondition("CC", "3"), new RolloutCondition("CNR", "23654", true)}));

            // Roll out to all customers in customer circle „1" and „3", but not customer circle „4".
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>
            {
                new RolloutCondition("CC", "1"), new RolloutCondition("CC", "3"), new RolloutCondition("CC", "4", true)
            }));
        }


        private void SetThirdConfigScenario()
        {
            // Password „secure” is needed.
            _updateConfigs.Add(CreateConfig("1.2.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>
                {new RolloutCondition("P", "secure")}));

            // Password „secure” or „special” is valid.
            _updateConfigs.Add(CreateConfig("1.3.0.0", RolloutConditionMode.AtLeastOne, new List<RolloutCondition>
                {new RolloutCondition("P", "secure"), new RolloutCondition("P", "special")}));
        }

        private UpdateConfiguration CreateConfig(string version, RolloutConditionMode conditionMode,
            List<RolloutCondition> conditions, bool isnecessary = true)
        {
            var config = new UpdateConfiguration()
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