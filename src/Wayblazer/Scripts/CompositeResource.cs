using System;
using System.Collections.Generic;

namespace Wayblazer;

public class CompositeResource(string name, List<RawResource> inputs) : RawResource(name, CalculateProperties(inputs))
{
	public List<RawResource> Inputs { get; private set; } = inputs;

	private static Dictionary<ResourcePropertyType, ResourceProperty> CalculateProperties(List<RawResource> inputs)
	{
		var combinedProperties = new Dictionary<ResourcePropertyType, ResourceProperty>();
		foreach (var input in inputs)
		{
			foreach (var propertyEntry in input.Properties)
			{
				if (combinedProperties.ContainsKey(propertyEntry.Key))
				{
					combinedProperties[propertyEntry.Key] = CombineProperties(combinedProperties[propertyEntry.Key], propertyEntry.Value);
				}
				else
				{
					combinedProperties[propertyEntry.Key] = propertyEntry.Value;
				}
			}
		}

		return combinedProperties;
	}

	private static ResourceProperty CombineProperties(ResourceProperty propertyOne, ResourceProperty propertyTwo)
	{
		var combinedValue = propertyOne.Type switch
		{
			ResourcePropertyType.Conductivity => Math.Max(propertyOne.Value + propertyTwo.Value * 0.5f, propertyTwo.Value + propertyOne.Value * 0.5f),
			ResourcePropertyType.Reactivity => propertyOne.Value * propertyTwo.Value * 0.5f,
			ResourcePropertyType.Resistance => Math.Min(propertyOne.Value, propertyTwo.Value) * 1.75f,
			ResourcePropertyType.Strength => propertyOne.Value + propertyTwo.Value,
			ResourcePropertyType.Toughness => (propertyOne.Value + propertyTwo.Value) * 0.75f,
			_ => throw new NotImplementedException()
		};

		return new ResourceProperty(propertyOne.Type, combinedValue);
	}
}
