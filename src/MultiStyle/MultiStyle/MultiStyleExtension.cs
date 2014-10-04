using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace SpicyTaco.MultiStyle
{
    [MarkupExtensionReturnType(typeof(Style))]
    public class MultiStyleExtension : MarkupExtension
    {
        [ConstructorArgument("resourceKeys")]
        public string ResourceKeys { get; set; }

        public MultiStyleExtension() { }

        public MultiStyleExtension(string resourceKeys)
        {
            if (resourceKeys == null) throw new ArgumentNullException("resourceKeys");

            ResourceKeys = resourceKeys;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var resultStyle = new Style();

            var styleNames = Parse(ResourceKeys).ToArray();

            if (!styleNames.Any()) throw new ArgumentException("No input resource keys specified.");

            foreach (var currentResourceKey in styleNames)
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