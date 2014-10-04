using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace SpicyTaco.MultiStyle
{
    public class MultiStyleExtension : MarkupExtension
    {
        readonly IEnumerable<string> _resourceKeys;

        public MultiStyleExtension(string styleNames)
        {
            if (styleNames == null) throw new ArgumentNullException("styleNames");

            _resourceKeys = Parse(styleNames);

            if (!_resourceKeys.Any()) throw new ArgumentException("No input resource keys specified.");
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

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var resultStyle = new Style();

            foreach (var currentResourceKey in _resourceKeys)
            {
                var currentStyle = 
                    new StaticResourceExtension(currentResourceKey)
                        .ProvideValue(serviceProvider) as Style;

                if (currentStyle == null)
                {
                    throw new InvalidOperationException("Could not find style with resource key " + currentResourceKey + ".");
                }

                resultStyle = Merge(resultStyle, currentStyle);
            }
            return resultStyle;
        }
    }
}