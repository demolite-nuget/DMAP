using System.Linq.Expressions;
using System.Reflection;
using DMAP.Net.Helpers;
using DMAP.Net.Interfaces;

namespace DMAP.Net.Implementation;

public abstract class AbstractDMapper<T1, T2> : IDMapper<T1, T2>
{
	private Dictionary<string, Action<T1, T2>> ForwardMap { get; set; } = [];

	private Dictionary<string, Action<T2, T1>> BackwardMap { get; set; } = [];

	public void Map(T1 source, T2 destination)
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(destination);

		foreach (var forwardAction in ForwardMap.Values)
		{
			forwardAction(source, destination);
		}
	}

	public void Map(T2 source, T1 destination)
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(destination);

		foreach (var backwardAction in BackwardMap.Values)
		{
			backwardAction(source, destination);
		}
	}

	protected void MapProperty<T1Prop, T2Prop>(
		Expression<Func<T1, T1Prop>> sourceAccessor,
		Expression<Func<T2, T2Prop>> destinationAccessor,
		Func<T1Prop, T2Prop>? forwardConverter = null,
		Func<T2Prop, T1Prop>? backwardConverter = null
	)
	{
		var sourceProperty = sourceAccessor.GetPropertyFromExpression();
		var destinationProperty = destinationAccessor.GetPropertyFromExpression();

		if (sourceProperty.GetType() != destinationProperty.GetType() &&
			forwardConverter is null &&
			backwardConverter is null)
		{
			throw new NotSupportedException(
				"Mapping properties of different types without a converter is not supported."
			);
		}

		var forwardGetter = CreatePropertyGetter<T1, T1Prop>(sourceProperty);
		var forwardSetter = CreatePropertySetter<T2, T2Prop>(destinationProperty);
		var forwardAction = CreateMappingAction(forwardGetter, forwardConverter, forwardSetter);

		ForwardMap.TryAdd(sourceProperty.Name, forwardAction);

		var backwardGetter = CreatePropertyGetter<T2, T2Prop>(sourceProperty);
		var backwardSetter = CreatePropertySetter<T1, T1Prop>(destinationProperty);
		var backwardAction = CreateMappingAction(backwardGetter, backwardConverter, backwardSetter);

		BackwardMap.TryAdd(sourceProperty.Name, backwardAction);
	}

	private static Action<TA, TB> CreateMappingAction<TA, TAProp, TB, TBProp>(
		Func<TA, TAProp> getter,
		Func<TAProp, TBProp>? converter,
		Action<TB, TBProp> forwardSetter
	)
	{
		return (source, dest)
			=>
		{
			try
			{
				var inValue = getter.Invoke(source);

				if (typeof(TAProp) == typeof(TBProp) && converter is null)
				{
					forwardSetter.Invoke(dest, (TBProp)(object)inValue!);
					return;
				}

				var outValue = converter!.Invoke(inValue);
				forwardSetter.Invoke(dest, outValue);
			}
			catch (Exception ex)
			{
				// TODO: Implement custom logging which is subscribable via the LogHelper Class.
			}
		};
	}

	/// <summary>
	/// Returns a setter action for the output type.
	/// </summary>
	/// <param name="propertyInfo">PropertyInfo for the property to be set.</param>
	/// <param name="customSetter">A custom setter, if defined.</param>
	/// <typeparam name="T">Type of class</typeparam>
	/// <typeparam name="TProp">Type of property</typeparam>
	/// <returns>A setter action.</returns>
	private static Action<T, TProp> CreatePropertySetter<T, TProp>(
		PropertyInfo propertyInfo,
		Action<T, TProp>? customSetter = null
	)
		=> customSetter ?? propertyInfo.SetMethod!.CreateDelegate<Action<T, TProp>>();

	/// <summary>
	/// Returns a setter action for the output type.
	/// </summary>
	/// <param name="propertyInfo">PropertyInfo for the property to be set.</param>
	/// <param name="customSetter">A custom setter, if defined.</param>
	/// <typeparam name="T">Type of class</typeparam>
	/// <typeparam name="TProp">Type of property</typeparam>
	/// <returns>A setter action.</returns>
	private static Func<T, TProp> CreatePropertyGetter<T, TProp>(PropertyInfo propertyInfo)
		=> propertyInfo.GetMethod!.CreateDelegate<Func<T, TProp>>();

	/// <summary>
	/// Returns a setter action for the input type.
	/// </summary>
	/// <param name="propertyInfo">PropertyInfo for the property to be set.</param>
	/// <param name="customSetter">A custom setter, if defined.</param>
	/// <typeparam name="TOutProp">Property type to be set.</typeparam>
	/// <returns>A setter action.</returns>
	private static Action<T1, TInProp> GetBackwardSetter<TInProp>(
		PropertyInfo propertyInfo,
		Action<T1, TInProp>? customSetter = null
	)
		=> customSetter ?? propertyInfo.SetMethod!.CreateDelegate<Action<T1, TInProp>>();
}