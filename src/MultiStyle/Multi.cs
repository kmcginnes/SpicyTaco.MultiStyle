using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SpicyTaco.MultiStyle
{
    public class Multi
    {
        public static readonly DependencyProperty StylesProperty = DependencyProperty.RegisterAttached(
            "Styles", typeof(string), typeof(Multi), new PropertyMetadata(default(string), OnStylesChanged));

        static void OnStylesChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var styleableControl = dependencyObject as FrameworkElement;
            if (styleableControl == null) return;

            var resultStyle = new Style();

            var styleNames = Parse(GetStyles(dependencyObject)).ToArray();

            if (!styleNames.Any()) throw new ArgumentException("No input resource keys specified.");

            foreach (var currentResourceKey in styleNames)
            {
                var currentStyle = styleableControl.FindResource(currentResourceKey) as Style;

                if (currentStyle == null)
                {
                    throw new InvalidOperationException("Could not find style with resource key " + currentResourceKey + ".");
                }

                resultStyle = Merge(resultStyle, currentStyle);
            }
            styleableControl.Style = resultStyle;
        }

        public static void SetStyles(DependencyObject element, string value)
        {
            element.SetValue(StylesProperty, value);
        }

        public static string GetStyles(DependencyObject element)
        {
            return (string)element.GetValue(StylesProperty);
        }

        public static IEnumerable<string> Parse(string styleNames)
        {
            return (styleNames ?? string.Empty).Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static Style Merge(Style style1, Style style2)
        {
            if (style1 == null)
            {
                throw new ArgumentNullException("style1");
            }
            if (style2 == null)
            {
                throw new ArgumentNullException("style2");
            }

            var result = Clone(style1);

            if (result.TargetType.IsAssignableFrom(style2.TargetType))
            {
                result.TargetType = style2.TargetType;
            }

            if (style2.BasedOn != null)
            {
                Merge(result, style2.BasedOn);
            }

            foreach (SetterBase currentSetter in style2.Setters)
            {
                result.Setters.Add(currentSetter);
            }

            foreach (TriggerBase currentTrigger in style2.Triggers)
            {
                result.Triggers.Add(currentTrigger);
            }

            // This code is only needed when using DynamicResources.
            foreach (object key in style2.Resources.Keys)
            {
                result.Resources[key] = style2.Resources[key];
            }

            return result;
        }

        public static Style Clone(Style source)
        {
            var result = new Style(source.TargetType);
            result.BasedOn = source.BasedOn;
            source.Setters.ToList().ForEach(x => result.Setters.Add(x));
            source.Triggers.ToList().ForEach(x => result.Triggers.Add(x));
            foreach (var key in source.Resources.Keys)
            {
                result.Resources[key] = source.Resources[key];
            }

            return result;
        }
    }
}