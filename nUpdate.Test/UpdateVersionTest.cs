// UpdateVersionTest.cs, 01.08.2018
// Copyright (C) Dominic Beger 17.06.2019

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nUpdate.Updating;

namespace nUpdate.Test
{
    [TestClass]
    public class UpdateVersionTest
    {
        [TestMethod]
        public void CanCheckVersionValidity()
        {
            var alphaVersion = new UpdateVersion("1.0.0.0a1");
            var betaVersion = new UpdateVersion("1.0.0.0b1");
            var releaseCandidateVersion = new UpdateVersion("1.0.0.0rc1");
            var normalVersion = new UpdateVersion("1.0.0.0");
            //var firstInvalidVersion = new UpdateVersion("1.0.0."); // Invalid
            //var secondInvalidVersion = new UpdateVersion("1.0.0.0c1"); // Invalid
        }

        [TestMethod]
        public void CanGetHighestVersion()
        {
            var versionArray = new[]
                {"1.0.0.0", "1.1.0.0", "1.2.0.0a1", "1.3.0.0b1", "1.2.0.0b3", "1.3.0.0b3", "1.3.0.1b97", "1.1.1.0"};
            var versions = versionArray.Select(entry => new UpdateVersion(entry)).ToList();

            Assert.AreEqual("1.3.0.1b97", UpdateVersion.GetHighestUpdateVersion(versions).ToString());
        }

        [TestMethod]
        public void CanConstructOneDigitVersion()
        {
            var version = new UpdateVersion("1");
            Assert.AreEqual("1.0.0.0", version.ToString());
        }

        [TestMethod]
        public void CanConstructTwoDigitVersion()
        {
            var version = new UpdateVersion("1.0");
            Assert.AreEqual("1.0.0.0", version.ToString());
        }

        [TestMethod]
        public void CanConstructThreeDigitVersion()
        {
            var version = new UpdateVersion("1.0.0");
            Assert.AreEqual("1.0.0.0", version.ToString());
        }

        [TestMethod]
        public void CanConstructFourDigitVersion()
        {
            var version = new UpdateVersion("1.0.0.0");
            Assert.AreEqual("1.0.0.0", version.ToString());
        }

        [TestMethod]
        public void CanConstructOneDigitAlphaVersion()
        {
            var version = new UpdateVersion("1a1");
            Assert.AreEqual("1.0.0.0a1", version.ToString());
        }

        [TestMethod]
        public void CanConstructOneDigitAlphaVersionWithoutDevelopmentalStage()
        {
            var version = new UpdateVersion("1a");
            Assert.AreEqual("1.0.0.0a", version.ToString());
        }

        [TestMethod]
        public void CanGetCorrectReleaseCandidateVersionString()
        {
            var version = new UpdateVersion("1.2rc2");
            Assert.AreEqual("1.2.0.0rc2", version.ToString());
        }

        [TestMethod]
        public void CanConstructWithSemanticVersion()
        {
            var firstVersion = new UpdateVersion("1.0-a");
            Assert.AreEqual("1.0.0.0a", firstVersion.ToString());

            var secondVersion = new UpdateVersion("1.0-a.2");
            Assert.AreEqual("1.0.0.0a2", secondVersion.ToString());

            var thirdVersion = new UpdateVersion("1.0a.2");
            Assert.AreEqual("1.0.0.0a2", thirdVersion.ToString());
        }

        [TestMethod]
        public void CanGetCorrectSemanticVersion()
        {
            var firstVersion = new UpdateVersion("1.2a2");
            Assert.AreEqual("1.2-a.2", firstVersion.SemanticVersion);

            var secondVersion = new UpdateVersion("1.2a");
            Assert.AreEqual("1.2-a", secondVersion.SemanticVersion);

            var thirdVersion = new UpdateVersion("1.2");
            Assert.AreEqual("1.2.0.0", thirdVersion.SemanticVersion);
        }

        [TestMethod]
        public void CanCompareVersionsCorrectly()
        {
            var firstVersion = new UpdateVersion("1.2");
            var secondVersion = new UpdateVersion("1.3.0.0");
            Assert.AreEqual(firstVersion < secondVersion, true);

            var thirdVersion = new UpdateVersion("1.3a1");
            var fourthVersion = new UpdateVersion("1.3.0.0a");
            Assert.AreEqual(thirdVersion > fourthVersion, true);

            var fifthVersion = new UpdateVersion("1.4.0.0-b1");
            var sixthVersion = new UpdateVersion("1.4.0.0rc");
            var seventhVersion = new UpdateVersion("1.4.0.0-rc.1");
            var eighthVersion = new UpdateVersion("1.4.0.0");
            Assert.AreEqual(
                fifthVersion < sixthVersion && sixthVersion < seventhVersion && seventhVersion < eighthVersion, true);
        }

        /// <summary>
        ///     Determines whether this instance [can create version from full text].
        /// </summary>
        [TestMethod]
        public void CanCreateVersionFromFullText()
        {
            const string firstFullText = "0.1.0.0 Alpha 1";
            Assert.AreEqual(UpdateVersion.FromFullText(firstFullText).ToString(), "0.1.0.0a1");

            const string secondFullText = "0.1.0.0 ReleaseCandidate";
            Assert.AreEqual(UpdateVersion.FromFullText(secondFullText).ToString(), "0.1.0.0rc");

            const string thirdFullText = "0.1.0.0";
            Assert.AreEqual(UpdateVersion.FromFullText(thirdFullText).ToString(), "0.1.0.0");
        }
    }
}