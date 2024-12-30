using ThemeOptions.Models;
//using NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using System.Collections.Generic;

namespace Tests.Models
{
    [TestClass]
    public class VariablesToObjectsCast
    {
        [TestMethod]
        [DataRow("string", "value1", "value1")]
        [DataRow("String", "value2", "value2")]
        public void ToString(string typeName, string value, object expected)
        {
            VariableValue variable =  new Variable() { Type = typeName, Value = value };
            object actual = VariablesValues.CastToObject(variable);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("boolean", "True", true)]
        [DataRow("Boolean", "true", true)]
        [DataRow("Boolean", "False", false)]
        [DataRow("Boolean", "Any", false)]
        public void ToBoolean(string typeName, string value, object expected)
        {
            VariableValue variable =  new Variable() { Type = typeName, Value = value };
            object actual = VariablesValues.CastToObject(variable);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("unknown", "555", null)]
        [DataRow("Int32", "555", 555)]
        [DataRow("Int32", "-358", -358)]
        [DataRow("Int32", "2A", 0)]
        [DataRow("Double", "45", 45.0)]
        [DataRow("Double", "98.78", 98.78)]
        [DataRow("Double", "-89.45", -89.45)]
        [DataRow("Double", "4f5", double.NaN)]
        public void ToNumbers(string typeName, string value, object expected)
        {
            VariableValue variable =  new Variable() { Type = typeName, Value = value };
            object actual = VariablesValues.CastToObject(variable);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("Thickness", "1,2,3,4", 1,2,3,4)]
        [DataRow("Thickness", "5", 5,5,5,5)]
        [DataRow("Thickness", "5,6", 0,0,0,0)]
        public void ToThickness(string typeName, string value, double left, double top, double right, double bottom)
        {
            VariableValue variable =  new Variable() { Type = typeName, Value = value };
            object actual = VariablesValues.CastToObject(variable);
            Assert.AreEqual(new Thickness(left, top, right, bottom), actual);
        }

        [TestMethod]
        [DataRow("CornerRadius", "1,2,3,4", 1,2,3,4)]
        [DataRow("CornerRadius", "1,2,3.3,4", 1,2,3.3,4)]
        [DataRow("CornerRadius", "5.1", 5.1,5.1,5.1,5.1)]
        [DataRow("CornerRadius", "5,6", 0,0,0,0)]
        public void ToCornerRadius(string typeName, string value, double left, double top, double right , double bottom)
        {
            VariableValue variable =  new Variable() { Type = typeName, Value = value };
            object actual = VariablesValues.CastToObject(variable);
            Assert.AreEqual(new CornerRadius(left, top, right, bottom), actual);
        }

        [TestMethod]
        [DataRow("Visibility", "Visible", Visibility.Visible)]
        [DataRow("Visibility", "visible", Visibility.Visible)]
        [DataRow("Visibility", "Collapsed", Visibility.Collapsed)]
        [DataRow("Visibility", "Other", Visibility.Collapsed)]
        public void ToVisibility(string typeName, string value, Visibility expected)
        {
            VariableValue variable =  new Variable() { Type = typeName, Value = value };
            object actual = VariablesValues.CastToObject(variable);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("TimeSpan", "00:00:12", 0, 0, 12, 0)]
        [DataRow("TimeSpan", "00:00:34.56", 0, 0, 34, 560)]
        [DataRow("TimeSpan", "aa", 0, 0, 0, 0)]
        public void ToTimeSpan(string typeName, string value, int hh, int mm, int ss, int ms)
        {
            VariableValue variable =  new Variable() { Type = typeName, Value = value };
            object actual = VariablesValues.CastToObject(variable);
            Assert.AreEqual(new TimeSpan(0, hh,mm,ss, ms), actual);
        }
    }
}