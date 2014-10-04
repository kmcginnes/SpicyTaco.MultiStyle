using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Extensions;

namespace MultiStyle.Tests
{
    public class MultiStyleShould
    {
        [Fact]
        public void Foo()
        {
            true.Should().BeTrue();
        }

        [Theory]
        [InlineData("foo", new[] { "foo" })]
        [InlineData("foo bar", new[] { "foo", "bar" })]
        [InlineData("baz fiz", new[] { "baz", "fiz" })]
        [InlineData("foo bar baz fiz", new[] { "foo", "bar", "baz", "fiz" })]
        [InlineData("foo ", new[] { "foo" })]
        [InlineData(" foo", new[] { "foo" })]
        [InlineData("", new string[] { })]
        [InlineData("   ", new string[] { })]
        [InlineData(null, new string[] { })]
        public void ParseStyleNamesFromString(string input, IEnumerable<string> output)
        {
            var result = MultiStyleExtension.Parse(input);

            result.Should().ContainInOrder(output);
        }
    }

    public class MultiStyleExtension
    {
        public static IEnumerable<string> Parse(string styleNames)
        {
            return (styleNames ?? string.Empty).Split(' ');
        }
    }
}
