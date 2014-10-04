using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using FluentAssertions;
using SpicyTaco.MultiStyle;
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
        [InlineData("foo    ", new[] { "foo" })]
        [InlineData(" foo", new[] { "foo" })]
        [InlineData("", new string[] { })]
        [InlineData("   ", new string[] { })]
        [InlineData(null, new string[] { })]
        public void ParseStyleNamesFromString(string input, IEnumerable<string> output)
        {
            var result = Multi.Parse(input);

            result.ShouldBeEquivalentTo(output);
        }

        [Fact]
        public void CopyStyle()
        {
            var style = new Style(typeof(FrameworkElement));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, 10));
            style.Setters.Add(new EventSetter());
            style.Triggers.Add(new DataTrigger());
            style.Resources.Add("key", new object());
            style.BasedOn = new Style();

            var result = Multi.Clone(style);

            result.Should().NotBeSameAs(style);
            result.ShouldBeEquivalentTo(style);
        }

        [Fact]
        public void MergeStyles()
        {
            var style1 = new Style(typeof(Button));
            style1.Setters.Add(new Setter(FrameworkElement.WidthProperty, 10));
            var style2 = new Style(typeof(Button));
            style2.Setters.Add(new Setter(FrameworkElement.HeightProperty, 10));
            style2.Setters.Add(new Setter(FrameworkElement.WidthProperty, 20));
            var result = Multi.Merge(style1, style2);

            result.Setters.Should().HaveCount(3);
        }
    }
}
