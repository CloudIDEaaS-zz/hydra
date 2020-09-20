using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    [Flags]
    public enum NpmVersionComparison
    {
        NoComparison = 0,
        NotEquals = 1,
        Equals = 1 << 2,
        GreaterThan = 1 << 3,
        LessThan = 1 << 4,
        Matches = 1 << 5
    }

    [DebuggerDisplay(" { VersionString } ")]
    public class NpmVersion
    {
        public string Version { get; }
        public string Prefix { get; }
        public string VersionTuple { get; }
        public int? Major { get; }
        public int? Minor { get; }
        public int? Patch { get; }
        public NpmVersion VersionRangeLower { get; }
        public NpmVersion VersionRangeHigher { get; }
        public bool MatchesAll { get; }
        public bool MajorWildcard { get; }
        public bool MinorWildcard { get; }
        public bool PatchWildcard { get; }

        public NpmVersion(string versionString)
        {
            string major;
            string minor;
            string patch;
            string tagid;
            var regexVersion = new Regex(@"^(?<prefix>[\^~><*xX]|\>=|\<=)?" +
                                            @"(?<version>" +
                                                @"(?<major>\d+|[*xX])" +
                                                @"(\.(?<minor>\d+||[*xX]))?" +
                                                @"(\.(?<patch>\d+||[*xX]))?" +
                                                @"(\-(?<tag>\w+?)\.(?<tagid>\d+)" +
                                             ")?" +
                                           ")?$");

            if (!versionString.IsNullWhiteSpaceOrEmpty())
            {
                if (regexVersion.IsMatch(versionString))
                {
                    var match = regexVersion.Match(versionString);

                    this.Prefix = match.GetGroupValue("prefix");

                    if (this.Prefix.IsOneOf("*", "x", "X"))
                    {
                        this.MatchesAll = true;
                    }
                    else
                    {
                        this.VersionTuple = match.GetGroupValue("version");
                        major = match.GetGroupValue("major");
                        minor = match.GetGroupValue("minor");
                        patch = match.GetGroupValue("patch");
                        tagid = match.GetGroupValue("tagid");

                        this.Tag = match.GetGroupValue("tag");

                        if (!major.IsNullWhiteSpaceOrEmpty())
                        {
                            if (major.IsOneOf("*", "x", "X"))
                            {
                                this.MajorWildcard = true;
                            }
                            else
                            {
                                this.Major = int.Parse(major);
                            }
                        }

                        if (!minor.IsNullWhiteSpaceOrEmpty())
                        {
                            if (minor.IsOneOf("*", "x", "X"))
                            {
                                this.MinorWildcard = true;
                            }
                            else
                            {
                                this.Minor = int.Parse(minor);
                            }
                        }

                        if (!patch.IsNullWhiteSpaceOrEmpty())
                        {
                            if (patch.IsOneOf("*", "x", "X"))
                            {
                                this.PatchWildcard = true;
                            }
                            else
                            {
                                this.Patch = int.Parse(patch);
                            }
                        }

                        if (!tagid.IsNullWhiteSpaceOrEmpty())
                        {
                            this.TagId = int.Parse(tagid);
                        }
                    }
                }
                else
                {
                    var regexHyphenRange = new Regex(@"(?<versionLower>" +
                                                         @"(?<majorLower>\d+)" +
                                                         @"(\.(?<minorLower>\d+))?" +
                                                         @"(\.(?<patchLower>\d+))?" +
                                                      ")" +
                                                      @"\s*\-\s*" +
                                                      @"(?<versionUpper>" +
                                                         @"(?<majorUpper>\d+)" +
                                                         @"(\.(?<minorUpper>\d+))?" +
                                                         @"(\.(?<patchUpper>\d+))?" +
                                                      ")$");

                    if (regexHyphenRange.IsMatch(versionString))
                    {
                        var match = regexHyphenRange.Match(versionString);

                        this.VersionRangeLower = match.GetGroupValue("versionLower");
                        this.VersionRangeHigher = match.GetGroupValue("versionUpper");
                    }
                    else
                    {
                        DebugUtils.Break();
                    }
                }

                this.Version = versionString;
            }
        }

        public static implicit operator NpmVersion(string versionPackageString)
        {
            return new NpmVersion(versionPackageString);
        }

        public static bool operator ==(NpmVersion version1, NpmVersion version2)
        {
            bool equals;
            NpmVersionComparison comparison;

            if (CompareExtensions.CheckNullEquality(version1, version2, out equals))
            {
                return equals;
            }

            comparison = NpmVersion.GetComparison(version1, version2);

            return comparison.HasFlag(NpmVersionComparison.Equals);
        }

        public static bool operator !=(NpmVersion version1, NpmVersion version2)
        {
            bool equals;
            NpmVersionComparison comparison;

            if (CompareExtensions.CheckNullEquality(version1, version2, out equals))
            {
                return !equals;
            }

            comparison = NpmVersion.GetComparison(version1, version2);

            return !comparison.HasFlag(NpmVersionComparison.Equals);
        }

        public static bool operator <(NpmVersion version1, NpmVersion version2)
        {
            var comparison = NpmVersion.GetComparison(version1, version2);

            return comparison.HasFlag(NpmVersionComparison.LessThan);
        }

        public static bool operator >(NpmVersion version1, NpmVersion version2)
        {
            var comparison = NpmVersion.GetComparison(version1, version2);

            return comparison.HasFlag(NpmVersionComparison.GreaterThan);
        }

        public static bool operator <=(NpmVersion version1, NpmVersion version2)
        {
            var comparison = NpmVersion.GetComparison(version1, version2);

            return comparison.HasAnyFlag(NpmVersionComparison.LessThan | NpmVersionComparison.Equals);
        }

        public static bool operator >=(NpmVersion version1, NpmVersion version2)
        {
            var comparison = NpmVersion.GetComparison(version1, version2);

            return comparison.HasAnyFlag(NpmVersionComparison.GreaterThan | NpmVersionComparison.Equals);
        }

        public bool Matches(NpmVersion versionMatch)
        {
            var comparison = NpmVersion.GetComparison(this, versionMatch);

            return comparison.HasFlag(NpmVersionComparison.Matches);
        }

        public bool HasRangeComparison
        {
            get
            {
                if (this.VersionRangeLower != null && this.VersionRangeHigher != null)
                {
                    return true;
                }
                else
                {
                    switch (this.Prefix)
                    {
                        case "<":
                        case ">":
                        case "<=":
                        case ">=":
                            return true;
                        default:
                            return false;
                    }
                }
            }
        }

        public bool HasWildcard
        {
            get
            {
                if (BoolExtensions.AnyAreValueTrue(this.MajorWildcard, this.MinorWildcard, this.PatchWildcard))
                {
                    return true;
                }

                return false;
            }
        }

        public NpmVersion ToPaddedVersion()
        {
            return string.Format("{0}.{1}.{2}", this.Major.NullToZero(), this.Minor.NullToZero(), this.Patch.NullToZero());
        }

        public Regex GetRegexFromWildchard()
        {
            var pattern = this.VersionTuple.RegexReplace("[*xX]", @"\d+?");

            return new Regex(pattern);
        }

        public static NpmVersionComparison GetComparison(NpmVersion version1, NpmVersion version2)
        {
            bool? majorMatch = null;
            bool? minorMatch = null;
            bool? patchMatch = null;
            bool? major1Greater = null;
            bool? minor1Greater = null;
            bool? patch1Greater = null;
            var comparison = NpmVersionComparison.NoComparison;
            var skipDeeperComparison = false;

            if (version1.MatchesAll || version2.MatchesAll)
            {
                comparison = NpmVersionComparison.Matches;
                skipDeeperComparison = true;
            }
            else if (version1.HasWildcard || version2.HasWildcard)
            {
                NpmVersion npmWildcard = null;
                NpmVersion npmMatch = null;
                Regex regex = null;

                if (version1.HasWildcard)
                {
                    npmWildcard = version1;
                    npmMatch = version2.ToPaddedVersion();
                }
                else if (version2.HasWildcard)
                {
                    npmWildcard = version2;
                    npmMatch = version1.ToPaddedVersion();
                }
                else
                {
                    DebugUtils.Break();
                }

                regex = npmWildcard.GetRegexFromWildchard();

                if (regex.IsMatch(npmMatch.VersionTuple))
                {
                    comparison = NpmVersionComparison.Matches;
                }
                else
                {
                    comparison = NpmVersionComparison.NotEquals;
                }

                skipDeeperComparison = true;
            }
            else if (version1.VersionRangeLower != null && version1.VersionRangeHigher != null)
            {
                if (version2 >= version1.VersionRangeLower && version2 <= version1.VersionRangeHigher)
                {
                    comparison = NpmVersionComparison.Matches;
                }
                else
                {
                    comparison = NpmVersionComparison.NotEquals;
                }

                skipDeeperComparison = true;
            }
            else if (version2.VersionRangeLower != null && version2.VersionRangeHigher != null)
            {
                if (version1 >= version2.VersionRangeLower && version1 <= version2.VersionRangeHigher)
                {
                    comparison = NpmVersionComparison.Matches;
                }
                else
                {
                    comparison = NpmVersionComparison.NotEquals;
                }

                skipDeeperComparison = true;
            }

            if (!skipDeeperComparison)
            {
                if (version1.Version == null)
                {
                    if (version2.Version == null)
                    {
                        comparison = NpmVersionComparison.Equals | NpmVersionComparison.Matches;
                        skipDeeperComparison = true;
                    }
                    else
                    {
                        comparison = NpmVersionComparison.GreaterThan | NpmVersionComparison.Matches;
                        skipDeeperComparison = true;
                    }
                }
                else if (version2.Version != null)
                {
                    if (version1.Version == version2.Version)
                    {
                        comparison = NpmVersionComparison.Equals | NpmVersionComparison.Matches;

                        if (!version1.HasRangeComparison && !version2.HasRangeComparison)
                        {
                            skipDeeperComparison = true;
                        }
                    }
                }

                if (version1.Major == null)
                {
                    if (version2.Major == null)
                    {
                        comparison = NpmVersionComparison.Equals | NpmVersionComparison.Matches;
                        skipDeeperComparison = true;
                    }
                    else
                    {
                        comparison = NpmVersionComparison.LessThan;
                        skipDeeperComparison = true;
                    }
                }
            }

            if (!skipDeeperComparison)
            {
                if (version1.Major == null && version2.Major != null)
                {
                    major1Greater = false;
                }
                else if (version1.Major != null && version2.Major == null)
                {
                    major1Greater = true;
                }
                else if (version1.Major == version2.Major)
                {
                    majorMatch = true;

                    if (version1.Minor != null)
                    {
                        if (version2.Minor != null)
                        {
                            if (version1.Minor == version2.Minor)
                            {
                                minorMatch = true;  // majors and minors match

                                if (version1.Patch != null)
                                {
                                    if (version2.Patch != null)
                                    {
                                        if (version1.Patch == version2.Patch)
                                        {
                                            patchMatch = true;  // majors, minors, and patches match
                                        }
                                        else
                                        {
                                            patch1Greater = version1.Patch > version2.Patch;
                                        }
                                    }
                                    else
                                    {
                                        patch1Greater = true;
                                    }
                                }
                                else if (version2.Patch != null)
                                {
                                    patch1Greater = false;  // v1 has no patch, v2 does
                                }
                                else
                                {
                                    // both have no patch
                                    patch1Greater = false;
                                }
                            }
                            else
                            {
                                minor1Greater = version1.Minor > version2.Minor;
                            }
                        }
                        else
                        {
                            minor1Greater = true;
                        }
                    }
                    else if (version2.Minor != null)
                    {
                        minor1Greater = false;  // v1 has no minor, v2 does
                    }
                    else
                    {
                        // both have no minor
                        minor1Greater = false;
                    }
                }
                else
                {
                    major1Greater = version1.Major > version2.Major;
                    majorMatch = false;
                }

                if (version1.Prefix != string.Empty && version2.Prefix != string.Empty)
                {
                    comparison = NpmVersionComparison.NotEquals; // versions did not equal above
                }
                else if (version1.Prefix == string.Empty && version2.Prefix == string.Empty)
                {
                    if (patch1Greater.IsValueTrue())
                    {
                        comparison = NpmVersionComparison.GreaterThan;
                    }
                    else if (minor1Greater.IsValueTrue())
                    {
                        comparison = NpmVersionComparison.GreaterThan;
                    }
                    else if (major1Greater.IsValueTrue())
                    {
                        comparison = NpmVersionComparison.GreaterThan;
                    }
                    else if (patch1Greater.IsValueFalse())
                    {
                        comparison = NpmVersionComparison.LessThan;
                    }
                    else if (minor1Greater.IsValueFalse())
                    {
                        comparison = NpmVersionComparison.LessThan;
                    }
                    else if (major1Greater.IsValueFalse())
                    {
                        comparison = NpmVersionComparison.LessThan;
                    }
                    else
                    {
                        DebugUtils.Break();
                        comparison = NpmVersionComparison.LessThan;
                    }
                }
                else
                { 
                    // left has the prefix, matching to right.  Left in general must be less than or equal to right

                    switch (version1.Prefix)
                    {
                        case "^":

                            if (version2.VersionTuple == version1.VersionTuple)
                            {
                                comparison = NpmVersionComparison.Equals | NpmVersionComparison.Matches;
                            }
                            else if (major1Greater.IsValueTrue())
                            {
                                comparison = NpmVersionComparison.GreaterThan;
                            }
                            else if (major1Greater.IsValueFalse())
                            {
                                comparison = NpmVersionComparison.LessThan;
                            }
                            else if (majorMatch.IsValueTrue())
                            {
                                if (BoolExtensions.AnyAreValueTrue(patch1Greater, minor1Greater))
                                {
                                    comparison = NpmVersionComparison.GreaterThan;
                                }
                                else if (BoolExtensions.AnyAreValueFalse(patch1Greater, minor1Greater))
                                {
                                    comparison = NpmVersionComparison.LessThan | NpmVersionComparison.Matches;
                                }
                                else if (minorMatch.IsValueTrue())
                                {
                                    comparison = NpmVersionComparison.Matches;
                                }
                                else
                                {
                                    DebugUtils.Break();
                                }
                            }

                            break;

                        case "~":

                            if (version2.VersionTuple == version1.VersionTuple)
                            {
                                comparison = NpmVersionComparison.Equals | NpmVersionComparison.Matches;
                            }
                            else if (BoolExtensions.AnyAreValueTrue(major1Greater, minor1Greater))
                            {
                                comparison = NpmVersionComparison.GreaterThan;
                            }
                            else if (BoolExtensions.AnyAreValueFalse(major1Greater, minor1Greater))
                            {
                                comparison = NpmVersionComparison.LessThan;
                            }
                            else if (minorMatch.IsValueTrue())
                            {
                                if (patch1Greater.IsValueTrue())
                                {
                                    comparison = NpmVersionComparison.GreaterThan;
                                }
                                else if (patch1Greater.IsValueFalse())
                                {
                                    comparison = NpmVersionComparison.LessThan | NpmVersionComparison.Matches;
                                }
                                else if (patchMatch.IsValueTrue())
                                {
                                    comparison = NpmVersionComparison.Matches;
                                }
                                else
                                {
                                    DebugUtils.Break();
                                }
                            }

                            break;

                        case ">":
                            
                            if (version1.VersionTuple == version2.VersionTuple)
                            {
                                comparison = NpmVersionComparison.Equals;
                            }
                            else if (BoolExtensions.AnyAreValueFalse(major1Greater, minor1Greater, patch1Greater))
                            {
                                comparison = NpmVersionComparison.GreaterThan | NpmVersionComparison.Matches;
                            }
                            else
                            {
                                comparison = NpmVersionComparison.LessThan;
                            }

                            break;

                        case "<":

                            if (version1.VersionTuple == version2.VersionTuple)
                            {
                                comparison = NpmVersionComparison.Equals;
                            }
                            else if (BoolExtensions.AnyAreValueTrue(major1Greater, minor1Greater, patch1Greater))
                            {
                                comparison = NpmVersionComparison.LessThan | NpmVersionComparison.Matches;
                            }
                            else
                            {
                                comparison = NpmVersionComparison.GreaterThan;
                            }

                            break;

                        case ">=":

                            if (version1.VersionTuple == version2.VersionTuple)
                            {
                                comparison = NpmVersionComparison.Equals | NpmVersionComparison.Matches;
                            }
                            else if (BoolExtensions.AnyAreValueFalse(major1Greater, minor1Greater, patch1Greater))
                            {
                                comparison = NpmVersionComparison.GreaterThan | NpmVersionComparison.Matches;
                            }
                            else
                            {
                                comparison = NpmVersionComparison.LessThan;
                            }

                            break;

                        case "<=":

                            if (version1.VersionTuple == version2.VersionTuple)
                            {
                                comparison = NpmVersionComparison.Equals | NpmVersionComparison.Matches;
                            }
                            else if (BoolExtensions.AnyAreValueTrue(major1Greater, minor1Greater, patch1Greater))
                            {
                                comparison = NpmVersionComparison.LessThan | NpmVersionComparison.Matches;
                            }
                            else
                            {
                                comparison = NpmVersionComparison.GreaterThan;
                            }

                            break;
                    }

                    // right has the prefix, matching to left.  Right in general must be less than or equal to left

                    switch (version2.Prefix)
                    {
                        case "^":

                            if (version2.VersionTuple == version1.VersionTuple)
                            {
                                comparison = NpmVersionComparison.Equals | NpmVersionComparison.Matches;
                            }
                            else if (major1Greater.IsValueTrue())
                            {
                                comparison = NpmVersionComparison.LessThan;
                            }
                            else if (majorMatch.IsValueTrue())
                            {
                                if (BoolExtensions.AnyAreValueTrue(patch1Greater, minor1Greater))
                                {
                                    comparison = NpmVersionComparison.LessThan;
                                }
                                else if (BoolExtensions.AnyAreValueFalse(patch1Greater, minor1Greater))
                                {
                                    comparison = NpmVersionComparison.GreaterThan | NpmVersionComparison.Matches;
                                }
                                else if (minorMatch.IsValueTrue())
                                {
                                    comparison = NpmVersionComparison.Matches;
                                }
                                else
                                {
                                    DebugUtils.Break();
                                }
                            }

                            break;

                        case "~":

                            if (version2.VersionTuple == version1.VersionTuple)
                            {
                                comparison = NpmVersionComparison.Equals | NpmVersionComparison.Matches;
                            }
                            else if (BoolExtensions.AnyAreValueTrue(major1Greater, minor1Greater))
                            {
                                comparison = NpmVersionComparison.LessThan;
                            }
                            else if (BoolExtensions.AnyAreValueFalse(major1Greater, minor1Greater))
                            {
                                comparison = NpmVersionComparison.GreaterThan;
                            }
                            else if (minorMatch.IsValueTrue())
                            {
                                if (patch1Greater.IsValueTrue())
                                {
                                    comparison = NpmVersionComparison.LessThan;
                                }
                                else if (patch1Greater.IsValueFalse())
                                {
                                    comparison = NpmVersionComparison.GreaterThan | NpmVersionComparison.Matches;
                                }
                                else if (patchMatch.IsValueTrue())
                                {
                                    comparison = NpmVersionComparison.Matches;
                                }
                                else
                                {
                                    DebugUtils.Break();
                                }
                            }

                            break;

                        case ">":

                            if (version1.VersionTuple == version2.VersionTuple)
                            {
                                comparison = NpmVersionComparison.Equals;
                            }
                            else if (BoolExtensions.AnyAreValueTrue(major1Greater, minor1Greater, patch1Greater))
                            {
                                comparison = NpmVersionComparison.GreaterThan | NpmVersionComparison.Matches;
                            }
                            else
                            {
                                comparison = NpmVersionComparison.LessThan;
                            }

                            break;

                        case "<":

                            if (version1.VersionTuple == version2.VersionTuple)
                            {
                                comparison = NpmVersionComparison.Equals;
                            }
                            else if (BoolExtensions.AnyAreValueFalse(major1Greater, minor1Greater, patch1Greater))
                            {
                                comparison = NpmVersionComparison.LessThan | NpmVersionComparison.Matches;
                            }
                            else
                            {
                                comparison = NpmVersionComparison.GreaterThan;
                            }

                            break;

                        case ">=":

                            if (version1.VersionTuple == version2.VersionTuple)
                            {
                                comparison = NpmVersionComparison.Equals | NpmVersionComparison.Matches;
                            }
                            else if (BoolExtensions.AnyAreValueTrue(major1Greater, minor1Greater, patch1Greater))
                            {
                                comparison = NpmVersionComparison.GreaterThan | NpmVersionComparison.Matches;
                            }
                            else
                            {
                                comparison = NpmVersionComparison.LessThan;
                            }

                            break;

                        case "<=":

                            if (version1.VersionTuple == version2.VersionTuple)
                            {
                                comparison = NpmVersionComparison.Equals | NpmVersionComparison.Matches;
                            }
                            else if (BoolExtensions.AnyAreValueFalse(major1Greater, minor1Greater, patch1Greater))
                            {
                                comparison = NpmVersionComparison.LessThan | NpmVersionComparison.Matches;
                            }
                            else
                            {
                                comparison = NpmVersionComparison.GreaterThan;
                            }

                            break;
                    }
                }
            }

            /*
                public enum NpmVersionComparison
                {
                    NoComparison = 0,
                    NotEquals = 1,
                    Equals = 1 << 2,
                    GreaterThan = 1 << 3,
                    LessThan = 1 << 4,
                    Matches = 1 << 5
                }
            */

            Debug.Assert(comparison != NpmVersionComparison.NoComparison);
            Debug.Assert(!(comparison.HasFlag(NpmVersionComparison.GreaterThan) && comparison.HasFlag(NpmVersionComparison.LessThan)));
            Debug.Assert(!(comparison.HasFlag(NpmVersionComparison.Equals) && comparison.HasFlag(NpmVersionComparison.NotEquals)));
            Debug.Assert(!(comparison.HasFlag(NpmVersionComparison.Equals) && comparison.HasFlag(NpmVersionComparison.GreaterThan)));
            Debug.Assert(!(comparison.HasFlag(NpmVersionComparison.Equals) && comparison.HasFlag(NpmVersionComparison.LessThan)));

            return comparison;
        }

        public string VersionString
        {
            get
            {
                return "v" + this.Version;
            }
        }

        public int TagId { get; }
        public string Tag { get; }

        public override bool Equals(object obj)
        {
            return (obj as NpmVersion) == this;
        }

        public override int GetHashCode()
        {
            return this.Version.GetHashCode();
        }
    }
}
