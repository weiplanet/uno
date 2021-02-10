﻿#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;
using Uno.Extensions;
using Uno.Logging;

namespace Uno.UI.Extensions
{
	public static partial class UIElementExtensions
	{
		/// <summary>
		/// Get a display name for the element for debug purposes
		/// </summary>
		internal static string GetDebugName(this object? elt)
			=> elt switch
			{
				null => "--null--",
				FrameworkElement fwElt when fwElt.Name.HasValue() => $"{fwElt.Name} -{elt.GetHashCode():X8}",
				_ => $"{elt.GetType().Name} -{elt.GetHashCode():X8}",
			};

		internal static string GetDebugIdentifier(this object? elt)
			=> $"{new string('\t', elt.GetDebugDepth())} [{elt.GetDebugName()}]";

		internal static int GetDebugDepth(this object? elt) =>
			elt switch
			{
				null => 0,
#if NETSTANDARD
				UIElement fwElt => fwElt.Depth,
#endif
				_ => elt.GetParent()?.GetDebugDepth() + 1?? 0,
			};

			internal static Thickness GetPadding(this UIElement uiElement)
		{
			if(uiElement is FrameworkElement fe && fe.TryGetPadding(out var padding))
			{
				return padding;
			}

			var property = uiElement.FindDependencyPropertyUsingReflection<Thickness>("PaddingProperty");
			return property != null && uiElement.GetValue(property) is Thickness t ? t : default;
		}

		internal static Thickness GetBorderThickness(this UIElement uiElement)
		{
			if (uiElement is FrameworkElement fe && fe.TryGetBorderThickness(out var borderThickness))
			{
				return borderThickness;
			}

			var property = uiElement.FindDependencyPropertyUsingReflection<Thickness>("BorderThicknessProperty");
			return property != null && uiElement.GetValue(property) is Thickness t ? t : default;
		}

		internal static bool SetPadding(this UIElement uiElement, Thickness padding)
		{
			if (uiElement is FrameworkElement fe && fe.TrySetPadding(padding))
			{
				return true;
			}

			var property = uiElement.FindDependencyPropertyUsingReflection<Thickness>("PaddingProperty");
			if(property != null)
			{
				uiElement.SetValue(property, padding);
				return true;
			}

			return false;
		}

		internal static bool SetBorderThickness(this UIElement uiElement, Thickness borderThickness)
		{
			if (uiElement is FrameworkElement fe && fe.TrySetBorderThickness(borderThickness))
			{
				return true;
			}

			var property = uiElement.FindDependencyPropertyUsingReflection<Thickness>("BorderThicknessProperty");
			if (property != null)
			{
				uiElement.SetValue(property, borderThickness);
				return true;
			}

			return false;
		}

		private static Dictionary<(Type type, string property), DependencyProperty?>? _dependencyPropertyReflectionCache;

		internal static DependencyProperty? FindDependencyPropertyUsingReflection<TProperty>(this UIElement uiElement, string propertyName)
		{
			var type = uiElement.GetType();
			var propertyType = typeof(TProperty);
			var key = (ownerType: type, propertyName);

			_dependencyPropertyReflectionCache ??= new Dictionary<(Type, string), DependencyProperty?>(2);

			if (_dependencyPropertyReflectionCache.TryGetValue(key, out var property))
			{
				return property;
			}

			property =
				type
					.GetTypeInfo()
					.GetDeclaredProperty(propertyName)
					?.GetValue(null) as DependencyProperty
				?? type
					.GetTypeInfo()
					.GetDeclaredField(propertyName)
					?.GetValue(null) as DependencyProperty;

			if (property == null)
			{
				uiElement.Log().Warn($"The {propertyName} dependency property does not exist on {type}");
			}
			else if (property.Type != propertyType)
			{
				uiElement.Log().Warn($"The {propertyName} dependency property {type} is not of the {propertyType} Type.");
				property = null;
			}

			_dependencyPropertyReflectionCache[key] = property;

			return property;
		}
	}
}
