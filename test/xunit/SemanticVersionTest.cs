#region OpenCodeList.NET - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    OpenCodeList.NET  
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License, Version 2.0. 
 * 
 */
#endregion

using Xunit;

namespace OpenCodeList.XUnit
{
    /// <summary>
    /// Unit tests for <see cref="SemanticVersion"/>.
    /// </summary>
    public class SemanticVersionTest
    {
        [Fact]
        public void Compare_And_Operators_Work_As_Expected()
        {
            var v1 = new SemanticVersion(1, 2, 3);
            var v2 = new SemanticVersion(1, 2, 4);
            var pre = new SemanticVersion(1, 2, 3, "beta");

            Assert.True(v1 < v2);
            Assert.True(v2 > v1);
            Assert.True(pre < v1);
            Assert.True(v1 >= pre);
            Assert.True(v1 != v2);
            Assert.True(v1 == new SemanticVersion(1, 2, 3));
        }

        [Fact]
        public void From_Ignores_Additional_PreRelease_Separators_After_First()
        {
            var version = SemanticVersion.From("3.4.5-alpha-extra");

            Assert.Equal("alpha", version.PreRelease);
        }

        [Fact]
        public void From_Parses_Full_Version()
        {
            var version = SemanticVersion.From("1.2.3");

            Assert.Equal(1, version.Major);
            Assert.Equal(2, version.Minor);
            Assert.Equal(3, version.Patch);
            Assert.Null(version.PreRelease);
        }

        [Fact]
        public void From_Parses_Partial_Version_And_Defaults_Missing_Parts()
        {
            var version = SemanticVersion.From("2.5");

            Assert.Equal(2, version.Major);
            Assert.Equal(5, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Null(version.PreRelease);
        }

        [Fact]
        public void From_Parses_PreRelease()
        {
            var version = SemanticVersion.From("1.2.3-beta");

            Assert.Equal(1, version.Major);
            Assert.Equal(2, version.Minor);
            Assert.Equal(3, version.Patch);
            Assert.Equal("beta", version.PreRelease);
        }

        [Fact]
        public void ToString_Formats_With_And_Without_PreRelease()
        {
            var stable = new SemanticVersion(1, 0, 0);
            var preRelease = new SemanticVersion(1, 0, 0, "rc1");

            Assert.Equal("1.0.0", stable.ToString());
            Assert.Equal("1.0.0-rc1", preRelease.ToString());
        }
    }
}
