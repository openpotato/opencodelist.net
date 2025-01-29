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

using System;

namespace OpenCodeList
{
    /// <summary>
    /// Semantic version type, following closely https://semver.org
    /// </summary>
    public class SemanticVersion : IEquatable<SemanticVersion>, IComparable<SemanticVersion>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        /// <param name="major">Major version part</param>
        /// <param name="minor">Minor version part</param>
        /// <param name="patch">Patch version part</param>
        /// <param name="preRelease">Additional label for pre-release</param>
        public SemanticVersion(int major, int minor, int patch, string preRelease = null)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            PreRelease = preRelease;
        }

        /// <summary>
        /// Major version part
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        /// Minor version part
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        /// Patch version part
        /// </summary>
        public int Patch { get; set; }

        /// <summary>
        /// Additional label for pre-release
        /// </summary>
        public string PreRelease { get; set; }

        /// <summary>
        /// Parses a version string into a <see cref="SemanticVersion"/> instance.
        /// </summary>
        /// <param name="version">String formatted version</param>
        public static SemanticVersion From(string version)
        {
            var parts = version.Split('-');
            var versionNumbers = parts[0].Split('.');

            var major = versionNumbers.Length > 0 ? int.Parse(versionNumbers[0]) : 0;
            var minor = versionNumbers.Length > 1 ? int.Parse(versionNumbers[1]) : 0;
            var patch = versionNumbers.Length > 2 ? int.Parse(versionNumbers[2]) : 0;
            var preRelease = parts.Length > 1 ? parts[1] : null;

            return new SemanticVersion(major, minor, patch, preRelease);
        }

        /// <summary>
        /// Determines whether two versions are not equal.
        /// </summary>
        /// <param name="left">The first version to compare</param>
        /// <param name="right">The second version to compare</param>
        /// <returns>True, if left not equal to right; otherwise, false.</returns>
        public static bool operator !=(SemanticVersion left, SemanticVersion right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Compare two versions using less than
        /// </summary>
        /// <param name="left">The first version to compare</param>
        /// <param name="right">The second version to compare</param>
        /// <returns>true, if left is the less than right; otherwise, false.</returns>
        public static bool operator <(SemanticVersion left, SemanticVersion right)
        {
            if (left is null) return right != null;
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Compare two versions using less or equal than
        /// </summary>
        /// <param name="left">The first version to compare</param>
        /// <param name="right">The second version to compare</param>
        /// <returns>True, if left is the less or equal than right; otherwise, false.</returns>
        public static bool operator <=(SemanticVersion left, SemanticVersion right) => left == right || left < right;

        /// <summary>
        /// Determines whether two versions are equal.
        /// </summary>
        /// <param name="left">The first version to compare</param>
        /// <param name="right">The second version to compare</param>
        /// <returns>True, if left equal to right; otherwise, false.</returns>
        public static bool operator ==(SemanticVersion left, SemanticVersion right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Compare two versions using greater than
        /// </summary>
        /// <param name="left">The first version to compare</param>
        /// <param name="right">The second version to compare</param>
        /// <returns>True, if left is the greater than right; otherwise, false.</returns>
        public static bool operator >(SemanticVersion left, SemanticVersion right)
        {
            if (right is null) return left != null; 
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Compare two versions using greater or equal than
        /// </summary>
        /// <param name="left">The first version to compare</param>
        /// <param name="right">The second version to compare</param>
        /// <returns>True, if left is the greater or equal than right; otherwise, false.</returns>
        public static bool operator >=(SemanticVersion left, SemanticVersion right) => left == right || left > right;

        /// <summary>
        /// Compares the current version instance with another version and returns an integer that 
        /// indicates whether the current instance precedes, follows, or occurs in the same position 
        /// in the sort order as the other version.
        /// </summary>
        /// <param name="other">A version to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the versions being compared</returns>
        public int CompareTo(SemanticVersion other)
        {
            if (Major != other.Major) return Major.CompareTo(other.Major);
            if (Minor != other.Minor) return Minor.CompareTo(other.Minor);
            if (Patch != other.Patch) return Patch.CompareTo(other.Patch);

            if (string.IsNullOrEmpty(PreRelease) && !string.IsNullOrEmpty(other.PreRelease)) return 1;
            if (!string.IsNullOrEmpty(PreRelease) && string.IsNullOrEmpty(other.PreRelease)) return -1;

            return string.Compare(PreRelease, other.PreRelease, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether two versions are equal.
        /// </summary>
        /// <param name="other">The version to compare with the current version.</param>
        /// <returns>True if the specified version is equal to the current version; otherwise, false.</returns>
        public bool Equals(SemanticVersion other)
        {
            if (other is null) return false;
            return CompareTo(other) == 0;
        }

        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified Object is equal to the current Object; otherwise, false.</returns>
        public override bool Equals(object obj) => Equals(obj as SemanticVersion);

        /// <summary>
        /// Serves as a hash function for the entity object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Patch, PreRelease);
        }

        /// <summary>
        /// The version as string
        /// </summary>
        /// <returns>String formatted version</returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(PreRelease)
                ? $"{Major}.{Minor}.{Patch}"
                : $"{Major}.{Minor}.{Patch}-{PreRelease}";
        }
    }
}