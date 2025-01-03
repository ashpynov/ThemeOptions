using ThemeOptions.Models;
//using NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using System.Collections.Generic;
using Playnite.SDK.Data;

namespace Tests.Models
{

    public class VariableComparer : IEqualityComparer<Variable>
    {
        public bool Equals(Variable x, Variable y)
        {
            if (x == null || y == null) return false;
            return
                x.Type == y.Type &&
                x.Value == y.Value &&
                x.Title == y.Title &&
                x.LocKey == y.LocKey &&
                x.Default == y.Default &&
                x.Description == y.Description &&
                x.Preview == y.Preview &&
                Equals(x.Slider, y.Slider) &&
                x.TitleStyle == y.TitleStyle &&
                x.Style == y.Style;
        }

        public int GetHashCode(Variable obj)
        {
            if (obj == null) return 0;
            return CombineHashCodes(
                            obj.Type?.GetHashCode() ?? 0,
                                    obj.Value?.GetHashCode() ?? 0,
                                    obj.Title?.GetHashCode() ?? 0,
                                    obj.LocKey?.GetHashCode() ?? 0,
                                    obj.Default?.GetHashCode() ?? 0,
                                    obj.Description?.GetHashCode() ?? 0,
                                    obj.Preview?.GetHashCode() ?? 0,
                                    obj.Slider?.GetHashCode() ?? 0,
                                    obj.TitleStyle?.GetHashCode() ?? 0,
                                    obj.Style?.GetHashCode() ?? 0);
        }

        private static int CombineHashCodes(params int[] hashCodes)
        {
            int hash = 17;
            foreach (var code in hashCodes)
            {
                hash = hash * 31 + code;
            }
            return hash;
        }
    }

    public class VariableValueComparer : IEqualityComparer<VariableValue>
    {
        public bool Equals(VariableValue x, VariableValue y)
        {
            if (x == null || y == null) return false;
            return
                x.Type == y.Type &&
                x.Value == y.Value;
        }

        public int GetHashCode(VariableValue obj)
        {
            if (obj == null) return 0;
            return CombineHashCodes(
                            obj.Type?.GetHashCode() ?? 0,
                                    obj.Value?.GetHashCode() ?? 0);
        }

        private static int CombineHashCodes(params int[] hashCodes)
        {
            int hash = 17;
            foreach (var code in hashCodes)
            {
                hash = hash * 31 + code;
            }
            return hash;
        }
    }

    [TestClass]
    public class VariableValuesSerialization
    {

        private VariablesValues setupVariablesValues1()
        => new VariablesValues
            {
                { "stringKey1", new Variable() { Type = "String", Value = "value1" } },
                { "booleanKey2",  new Variable() { Type = "Boolean", Value = "True" } },
                { "int32Key3", new Variable() { Type = "Int32", Value = "123" } },
                { "doubleKey4", new Variable() { Type = "Double", Value = "123.45" } },
                { "visibilityKey5", new Variable() { Type = "Visibility", Value = "Visible" } },
                { "timespanKey6", new Variable() { Type = "TimeSpan", Value = "00:01:00" } },
                { "colorKey7", new Variable() { Type = "Color", Value = "#FF0000" } },
                { "thicknessKey8", new Variable() { Type = "Thickness", Value = "1,2,3,4" } },
                { "durationKey9", new Variable() { Type = "Duration", Value = "00:00:10" } },
                { "cornerRadiusKey10", new Variable() { Type = "CornerRadius", Value = "5,5,5,5" } },
                { "solidColorBrushKey11", new Variable() { Type = "SolidColorBrush", Value = "#FF0000" } }
            };

        private VariablesValues setupVariablesValues2()
        => new VariablesValues(new List<KeyValuePair<string, Variable>>()
            {
                new KeyValuePair<string, Variable>( "key1", new Variable() { Type = "String", Value = "value1Key1" } ),
                new KeyValuePair<string, Variable>( "key2", new Variable() { Type = "Boolean", Value = "False" } ),
                new KeyValuePair<string, Variable>( "key3", new Variable() { Type = "Int32", Value = "999" } ),
                new KeyValuePair<string, Variable>( "key4", new Variable() { Type = "Double", Value = "123.45" } ),
                new KeyValuePair<string, Variable>( "key5", new Variable() { Type = "Visibility", Value = "Collapsed" } ),
                new KeyValuePair<string, Variable>( "key6", new Variable() { Type = "TimeSpan", Value = "00:01:00" } ),
                new KeyValuePair<string, Variable>( "key7", new Variable() { Type = "Color", Value = "#FF0000" } ),
                new KeyValuePair<string, Variable>( "key8", new Variable() { Type = "Thickness", Value = "1,2,3,4" } ),
                new KeyValuePair<string, Variable>( "key9", new Variable() { Type = "Duration", Value = "00:00:10" } ),
                new KeyValuePair<string, Variable>( "key10", new Variable() { Type = "CornerRadius", Value = "5,5,5,5" } ),
                new KeyValuePair<string, Variable>( "key11", new Variable() { Type = "SolidColorBrush", Value = "#FF0000" } )
        });

        private VariablesValues setupVariablesValuesCommon()
        => new VariablesValues(new List<KeyValuePair<string, Variable>>()
            {
                new KeyValuePair<string, Variable>(  "stringKey1", new Variable() { Type = "String", Value = "value1Key1" } ),
                new KeyValuePair<string, Variable>(  "booleanKey2", new Variable() { Type = "Boolean", Value = "False" } ),
                new KeyValuePair<string, Variable>(  "int32Key3", new Variable() { Type = "Int32", Value = "999" } ),
                new KeyValuePair<string, Variable>(  "doubleKey4", new Variable() { Type = "Double", Value = "123.45" } ),
                new KeyValuePair<string, Variable>(  "visibilityKey5", new Variable() { Type = "Visibility", Value = "Collapsed" } ),
                new KeyValuePair<string, Variable>(  "timeSpanKey6", new Variable() { Type = "TimeSpan", Value = "00:01:00" } ),
                new KeyValuePair<string, Variable>(  "colorKey7", new Variable() { Type = "Color", Value = "#FF0000" } ),
                new KeyValuePair<string, Variable>(  "thicknessKey8", new Variable() { Type = "Thickness", Value = "1,2,3,4" } ),
                new KeyValuePair<string, Variable>(  "durationKey9", new Variable() { Type = "Duration", Value = "00:00:10" } ),
                new KeyValuePair<string, Variable>(  "cornerRadiusKey10", new Variable() { Type = "CornerRadius", Value = "5,5,5,5" } ),
                new KeyValuePair<string, Variable>(  "solidColorBrushKey11", new Variable() { Type = "SolidColorBrush", Value = "#FF0000" } )
            });

        [TestMethod]
        public void ToDynamicPropertiesTest()
        {
            var dynamicProperties = setupVariablesValues1().ToDynamicProperties();
        }

        [TestMethod]
        public void MergeTest()
        {
            var actual = setupVariablesValues1().Merge(setupVariablesValuesCommon());
        }

        [TestMethod]
        public void FormatResourceDictionaryTest()
        {
            var values = setupVariablesValues2();
            var resource = values.FormatResourceDictionary();
        }

        [TestMethod]
        public void FromDynamicPropertiesTest()
        {
            var start = setupVariablesValues1().Merge(setupVariablesValuesCommon());
            var expected = setupVariablesValues1()
                            .Merge(setupVariablesValuesCommon())
                            .Merge(setupVariablesValues2());
            var dynSource = setupVariablesValuesCommon().Merge(setupVariablesValues2());
            var dyn = dynSource.ToDynamicProperties();
            var actual = start.FromDynamicProperties(dyn);

            foreach (var v in actual)
            {
                Assert.AreEqual(
                    expected[v.Key],
                    actual[v.Key],
                    comparer: new VariableValueComparer(),
                    message: $"{v.Key} are not equal: expected ({expected[v.Key].Type}:{expected[v.Key].Value}),  actual:({actual[v.Key].Type}:{actual[v.Key].Value})" );
            }
        }

        [TestMethod]
        public void FromDynamicPropertiesUpdateOnlyTest()
        {
            var start = setupVariablesValues2();
            var expected = setupVariablesValues2();
            var dynSource = setupVariablesValuesCommon().Merge(setupVariablesValues2());
            var dyn = dynSource.ToDynamicProperties();
            var actual = start.FromDynamicProperties(dyn, updateOnly: true);

            foreach (var v in actual)
            {
                Assert.AreEqual(
                    expected[v.Key],
                    actual[v.Key],
                    comparer: new VariableValueComparer(),
                    message: $"{v.Key} are not equal: expected ({expected[v.Key].Type}:{expected[v.Key].Value}),  actual:({actual[v.Key].Type}:{actual[v.Key].Value})" );
            }
        }
    }
}