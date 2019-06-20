// SemanticVersionTest.cs, 01.08.2018
// Copyright (C) Dominic Beger 20.06.2019

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable ObjectCreationAsStatement

namespace nUpdate.Test
{
    [TestClass]
    public class SemanticVersionTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CanCheckVersionValidity1()
        {
            new SemanticVersion("1.0.0."); // Invalid
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CanCheckVersionValidity2()
        {
            new SemanticVersion("1.0"); // Invalid
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CanCheckVersionValidity3()
        {
            new SemanticVersion("1.0.0alpha"); // Invalid
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CanCheckVersionValidity4()
        {
            new SemanticVersion("2.0.0-"); // Invalid
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CanCheckVersionValidity5()
        {
            new SemanticVersion("2.0.0-alpha.1+"); // Invalid
        }

        [TestMethod]
        public void CanConstructVersion1()
        {
            new SemanticVersion("1.0.0");
        }

        [TestMethod]
        public void CanConstructVersion2()
        {
            new SemanticVersion("2.1.0-alpha");
        }

        [TestMethod]
        public void CanConstructVersion3()
        {
            new SemanticVersion("2.1.0-alpha1");
        }

        [TestMethod]
        public void CanConstructVersion4()
        {
            new SemanticVersion("2.1.0-alpha.1");
        }

        [TestMethod]
        public void CanConstructVersion5()
        {
            new SemanticVersion("2.1.0-alpha.beta");
        }

        [TestMethod]
        public void CanConstructVersion6()
        {
            new SemanticVersion("2.1.0-alpha.1.2.3");
        }

        [TestMethod]
        public void CanConstructVersion7()
        {
            new SemanticVersion("2.1.0-0.3.2");
        }

        [TestMethod]
        public void CanConstructVersion8()
        {
            new SemanticVersion("2.1.0-0.x.2.h.1");
        }

        [TestMethod]
        public void CanConstructVersion9()
        {
            new SemanticVersion("2.1.0-beta.1.3.4+exp.build");
        }

        [TestMethod]
        public void CanConstructVersion10()
        {
            new SemanticVersion("2.1.0+exp.build");
        }

        [TestMethod]
        public void CanConstructVersion11()
        {
            new SemanticVersion("2.1.0-beta.-.0.1");
        }

        [TestMethod]
        public void CanCompareVersions1()
        {
            Assert.AreEqual(-1, new SemanticVersion("1.0.0-alpha").CompareTo(new SemanticVersion("1.0.0")));
        }

        [TestMethod]
        public void CanCompareVersions2()
        {
            Assert.AreEqual(-1, new SemanticVersion("1.0.0-alpha").CompareTo(new SemanticVersion("1.0.0-alpha.1")));
        }

        [TestMethod]
        public void CanCompareVersions3()
        {
            Assert.AreEqual(-1,
                new SemanticVersion("1.0.0-alpha.1").CompareTo(new SemanticVersion("1.0.0-alpha.beta")));
        }

        [TestMethod]
        public void CanCompareVersions4()
        {
            Assert.AreEqual(-1, new SemanticVersion("1.0.0-alpha").CompareTo(new SemanticVersion("1.0.0-beta")));
        }

        [TestMethod]
        public void CanCompareVersions5()
        {
            Assert.AreEqual(-1, new SemanticVersion("1.0.0-beta").CompareTo(new SemanticVersion("1.0.0-beta.2")));
        }

        [TestMethod]
        public void CanCompareVersions6()
        {
            Assert.AreEqual(-1, new SemanticVersion("1.0.0-alpha.2").CompareTo(new SemanticVersion("1.0.0-alpha.11")));
        }

        [TestMethod]
        public void CanCompareVersions7()
        {
            Assert.AreEqual(-1, new SemanticVersion("1.0.0-alpha.2").CompareTo(new SemanticVersion("1.0.0")));
        }

        [TestMethod]
        public void CanCompareVersions8()
        {
            Assert.AreEqual(-1, new SemanticVersion("1.0.0-alpha.2").CompareTo(new SemanticVersion("1.0.0-rc.1")));
        }

        [TestMethod]
        public void CanCompareVersions9()
        {
            Assert.AreEqual(-1,
                new SemanticVersion("1.0.0-alpha.2+exp.bla").CompareTo(new SemanticVersion("1.0.0-alpha.11")));
        }

        [TestMethod]
        public void CanCompareVersions10()
        {
            Assert.AreEqual(-1, new SemanticVersion("1.0.0-x.1").CompareTo(new SemanticVersion("1.0.0-x.2")));
        }

        [TestMethod]
        public void CanCompareVersions11()
        {
            Assert.AreEqual(-1, new SemanticVersion("1.0.0-x.1").CompareTo(new SemanticVersion("1.0.0-y.0.1")));
        }

        [TestMethod]
        public void CanCompareVersions12()
        {
            Assert.AreEqual(-1, new SemanticVersion("1.0.0-x.1").CompareTo(new SemanticVersion("2.0.0-x.1")));
        }

        [TestMethod]
        public void CanCompareVersions13()
        {
            Assert.AreEqual(-1, new SemanticVersion("2.1.14").CompareTo(new SemanticVersion("2.1.15")));
        }

        [TestMethod]
        public void CanCompareVersions14()
        {
            Assert.AreEqual(-1, new SemanticVersion("2.1.14").CompareTo(new SemanticVersion("2.2.0")));
        }

        [TestMethod]
        public void CanCompareVersions15()
        {
            Assert.AreEqual(-1, new SemanticVersion("2.1.14").CompareTo(new SemanticVersion("3.0.1")));
        }

        [TestMethod]
        public void CanCompareVersions16()
        {
            Assert.AreEqual(1, new SemanticVersion("1.0.0-rc.1").CompareTo(new SemanticVersion("1.0.0-alpha.2")));
        }

        [TestMethod]
        public void CanCompareVersions17()
        {
            Assert.AreEqual(1, new SemanticVersion("1.0.0-alpha1").CompareTo(new SemanticVersion("1.0.0-alpha.1")));
        }

        [TestMethod]
        public void CanCompareVersions18()
        {
            Assert.AreEqual(0, new SemanticVersion("1.0.0-alpha1").CompareTo(new SemanticVersion("1.0.0-alpha1")));
        }

        [TestMethod]
        public void CanCompareVersions19()
        {
            Assert.AreEqual(0, new SemanticVersion("1.0.0-alpha1+exp.2").CompareTo(new SemanticVersion("1.0.0-alpha1")));
        }

        [TestMethod]
        public void CanCompareVersions20()
        {
            Assert.AreEqual(0, new SemanticVersion("1.2.1").CompareTo(new SemanticVersion("1.2.1+bla")));
        }

        [TestMethod]
        public void CanCompareVersions21()
        {
            Assert.AreEqual(0, new SemanticVersion("1.2.1-beta.0.8.1").CompareTo(new SemanticVersion("1.2.1-beta.0.8.1")));
        }

        [TestMethod]
        public void CanCompareVersions22()
        {
            Assert.AreEqual(1, new SemanticVersion("1.2.0").CompareTo(new SemanticVersion("1.0.1+exp")));
        }

        [TestMethod]
        public void CanCompareVersions23()
        {
            Assert.AreEqual(1, new SemanticVersion("1.2.0-rc").CompareTo(new SemanticVersion("1.2.0-beta.1+exp")));
        }

        [TestMethod]
        public void CanCompareVersions24()
        {
            Assert.AreEqual(1, new SemanticVersion("1.2.0-r").CompareTo(new SemanticVersion("1.2.0-1")));
        }
    }
}