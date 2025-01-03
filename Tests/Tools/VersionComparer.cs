using ThemeOptions.Tools;
//using NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using System.Collections.Generic;

namespace Tests.Tools
{
    [TestClass]
    public class VersionComparerTest
    {
        [TestMethod]
        [DataRow("0.1", "0.1.0", 0)]
        [DataRow("0.18", "0.1", 1)]
        [DataRow("0.18", "0.19", -1)]
        [DataRow("0.1", "0.1.1", -1)]
        public void VersionCompareTest(string version1, string version2, int expected)
        {
            Assert.AreEqual(expected, VersionComparer.CompareVersions(version1,version2) );
        }

        [TestMethod]
        [DataRow("0.16", "0.1.0", false)]
        [DataRow("0.16", "0.1", false)]
        [DataRow("0.16", "0.2", false)]
        [DataRow("0.16", "0.16", true)]
        [DataRow("0.16", "0.16.1", true)]
        [DataRow("0.16.1", "0.16", false)]
        [DataRow("0.16.10", "0.16.1", false)]
        public void MinimumVersionTest(string minVersion, string actualVersion, bool expected)
        {
            Assert.AreEqual(expected, VersionComparer.MinimalVersion(minVersion,actualVersion) );
        }
    }
}