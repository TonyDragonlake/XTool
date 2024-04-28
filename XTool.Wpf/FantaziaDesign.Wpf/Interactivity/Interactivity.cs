using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FantaziaDesign.Wpf.Interactivity
{
	public abstract class AttachableCollection<T> : FreezableCollection<T>, IAttachedObject where T : DependencyObject, IAttachedObject
	{
		protected DependencyObject AssociatedObject
		{
			get
			{
				base.ReadPreamble();
				return this.associatedObject;
			}
		}

		internal AttachableCollection()
		{
			((INotifyCollectionChanged)this).CollectionChanged += this.OnCollectionChanged;
			this.snapshot = new Collection<T>();
		}

		protected abstract void OnAttached();

		protected abstract void OnDetaching();

		internal abstract void ItemAdded(T item);

		internal abstract void ItemRemoved(T item);

		[Conditional("DEBUG")]
		private void VerifySnapshotIntegrity()
		{
			bool flag = base.Count == this.snapshot.Count;
			if (flag)
			{
				for (int i = 0; i < base.Count; i++)
				{
					if (base[i] != this.snapshot[i])
					{
						return;
					}
				}
			}
		}

		private void VerifyAdd(T item)
		{
			if (this.snapshot.Contains(item))
			{
				throw new InvalidOperationException("DuplicateItemInCollection");
			}
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (var obj in e.NewItems)
					{
						T t = (T)((object)obj);
						try
						{
							this.VerifyAdd(t);
							this.ItemAdded(t);
						}
						finally
						{
							this.snapshot.Insert(base.IndexOf(t), t);
						}
					}
					return;
					break;
				case NotifyCollectionChangedAction.Remove:
					goto IL_13A;
				case NotifyCollectionChangedAction.Replace:
					break;
				case NotifyCollectionChangedAction.Move:
					return;
				case NotifyCollectionChangedAction.Reset:
					goto IL_18D;
				default:
					return;
			}
			foreach (object obj2 in e.OldItems)
			{
				T item = (T)((object)obj2);
				this.ItemRemoved(item);
				this.snapshot.Remove(item);
			}
			foreach (var obj3 in e.NewItems)
			{
				T t2 = (T)((object)obj3);
				try
				{
					this.VerifyAdd(t2);
					this.ItemAdded(t2);
				}
				finally
				{
					this.snapshot.Insert(base.IndexOf(t2), t2);
				}
			}
			return;
		IL_13A:
			foreach (var obj4 in e.OldItems)
			{
				T item2 = (T)((object)obj4);
				this.ItemRemoved(item2);
				this.snapshot.Remove(item2);

			}
			return;
		IL_18D:
			foreach (T item3 in this.snapshot)
			{
				this.ItemRemoved(item3);
			}
			this.snapshot = new Collection<T>();
			foreach (T item4 in this)
			{
				this.VerifyAdd(item4);
				this.ItemAdded(item4);
			}
		}

		DependencyObject IAttachedObject.AssociatedObject
		{
			get
			{
				return this.AssociatedObject;
			}
		}

		public void Attach(DependencyObject dependencyObject)
		{
			if (dependencyObject != this.AssociatedObject)
			{
				if (this.AssociatedObject != null)
				{
					throw new InvalidOperationException();
				}
				if (Interaction.ShouldRunInDesignMode || !(bool)base.GetValue(DesignerProperties.IsInDesignModeProperty))
				{
					base.WritePreamble();
					this.associatedObject = dependencyObject;
					base.WritePostscript();
				}
				this.OnAttached();
			}
		}

		public void Detach()
		{
			this.OnDetaching();
			base.WritePreamble();
			this.associatedObject = null;
			base.WritePostscript();
		}

		private Collection<T> snapshot;

		private DependencyObject associatedObject;
	}

	public abstract class Behavior : Animatable, IAttachedObject
	{
		internal event EventHandler AssociatedObjectChanged;

		protected Type AssociatedType
		{
			get
			{
				base.ReadPreamble();
				return this.associatedType;
			}
		}

		protected DependencyObject AssociatedObject
		{
			get
			{
				base.ReadPreamble();
				return this.associatedObject;
			}
		}

		internal Behavior(Type associatedType)
		{
			this.associatedType = associatedType;
		}

		protected virtual void OnAttached()
		{
		}

		protected virtual void OnDetaching()
		{
		}

		protected override Freezable CreateInstanceCore()
		{
			Type type = base.GetType();
			return (Freezable)Activator.CreateInstance(type);
		}

		private void OnAssociatedObjectChanged()
		{
			if (this.AssociatedObjectChanged != null)
			{
				this.AssociatedObjectChanged(this, new EventArgs());
			}
		}

		DependencyObject IAttachedObject.AssociatedObject
		{
			get
			{
				return this.AssociatedObject;
			}
		}

		public void Attach(DependencyObject dependencyObject)
		{
			if (dependencyObject != this.AssociatedObject)
			{
				if (this.AssociatedObject != null)
				{
					throw new InvalidOperationException("CannotHostBehaviorMultipleTimes");
				}
				if (dependencyObject != null && !this.AssociatedType.IsAssignableFrom(dependencyObject.GetType()))
				{
					throw new InvalidOperationException("TypeConstraintViolated");
				}
				base.WritePreamble();
				this.associatedObject = dependencyObject;
				base.WritePostscript();
				this.OnAssociatedObjectChanged();
				this.OnAttached();
			}
		}

		public void Detach()
		{
			this.OnDetaching();
			base.WritePreamble();
			this.associatedObject = null;
			base.WritePostscript();
			this.OnAssociatedObjectChanged();
		}

		private Type associatedType;

		private DependencyObject associatedObject;
	}

	public sealed class BehaviorCollection : AttachableCollection<Behavior>
	{
		internal BehaviorCollection()
		{
		}

		protected override void OnAttached()
		{
			foreach (Behavior behavior in this)
			{
				behavior.Attach(base.AssociatedObject);
			}
		}

		protected override void OnDetaching()
		{
			foreach (Behavior behavior in this)
			{
				behavior.Detach();
			}
		}

		internal override void ItemAdded(Behavior item)
		{
			if (base.AssociatedObject != null)
			{
				item.Attach(base.AssociatedObject);
			}
		}

		internal override void ItemRemoved(Behavior item)
		{
			if (((IAttachedObject)item).AssociatedObject != null)
			{
				item.Detach();
			}
		}

		protected override Freezable CreateInstanceCore()
		{
			return new BehaviorCollection();
		}
	}

	public abstract class Behavior<T> : Behavior where T : DependencyObject
	{
		protected Behavior() : base(typeof(T))
		{
		}

		protected new T AssociatedObject
		{
			get
			{
				return (T)((object)base.AssociatedObject);
			}
		}
	}

	public enum CustomPropertyValueEditor
	{
		Element,
		Storyboard,
		StateName,
		ElementBinding,
		PropertyBinding
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class CustomPropertyValueEditorAttribute : Attribute
	{
		public CustomPropertyValueEditor CustomPropertyValueEditor { get; private set; }

		public CustomPropertyValueEditorAttribute(CustomPropertyValueEditor customPropertyValueEditor)
		{
			this.CustomPropertyValueEditor = customPropertyValueEditor;
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
	public sealed class DefaultTriggerAttribute : Attribute
	{
		public Type TargetType
		{
			get
			{
				return this.targetType;
			}
		}

		public Type TriggerType
		{
			get
			{
				return this.triggerType;
			}
		}

		public IEnumerable Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		public DefaultTriggerAttribute(Type targetType, Type triggerType, object parameter) : this(targetType, triggerType, new object[]
		{
			parameter
		})
		{
		}

		public DefaultTriggerAttribute(Type targetType, Type triggerType, params object[] parameters)
		{
			if (!typeof(TriggerBase).IsAssignableFrom(triggerType))
			{
				throw new ArgumentException("DefaultTriggerAttributeInvalidTriggerTypeSpecified");
			}
			this.targetType = targetType;
			this.triggerType = triggerType;
			this.parameters = parameters;
		}

		public TriggerBase Instantiate()
		{
			object obj = null;
			try
			{
				obj = Activator.CreateInstance(this.TriggerType, this.parameters);
			}
			catch
			{
			}
			return (TriggerBase)obj;
		}

		private Type targetType;

		private Type triggerType;

		private object[] parameters;
	}

	public static class DependencyObjectHelper
	{
		public static IEnumerable<DependencyObject> GetSelfAndAncestors(this DependencyObject dependencyObject)
		{
			while (dependencyObject != null)
			{
				yield return dependencyObject;
				dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
			}
			yield break;
		}
	}

	public sealed class EventObserver : IDisposable
	{
		public EventObserver(EventInfo eventInfo, object target, Delegate handler)
		{
			if (eventInfo == null)
			{
				throw new ArgumentNullException("eventInfo");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			this.eventInfo = eventInfo;
			this.target = target;
			this.handler = handler;
			this.eventInfo.AddEventHandler(this.target, handler);
		}

		public void Dispose()
		{
			this.eventInfo.RemoveEventHandler(this.target, this.handler);
		}

		private EventInfo eventInfo;

		private object target;

		private Delegate handler;
	}

	public class EventTrigger : EventTriggerBase<object>
	{
		public EventTrigger()
		{
		}

		public EventTrigger(string eventName)
		{
			this.EventName = eventName;
		}

		public string EventName
		{
			get
			{
				return (string)base.GetValue(EventTrigger.EventNameProperty);
			}
			set
			{
				base.SetValue(EventTrigger.EventNameProperty, value);
			}
		}

		protected override string GetEventName()
		{
			return this.EventName;
		}

		private static void OnEventNameChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			((EventTrigger)sender).OnEventNameChanged((string)args.OldValue, (string)args.NewValue);
		}

		public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register("EventName", typeof(string), typeof(EventTrigger), new FrameworkPropertyMetadata("Loaded", new PropertyChangedCallback(EventTrigger.OnEventNameChanged)));
	}

	public abstract class EventTriggerBase : TriggerBase
	{
		protected sealed override Type AssociatedObjectTypeConstraint
		{
			get
			{
				AttributeCollection attributes = TypeDescriptor.GetAttributes(base.GetType());
				TypeConstraintAttribute typeConstraintAttribute = attributes[typeof(TypeConstraintAttribute)] as TypeConstraintAttribute;
				if (typeConstraintAttribute != null)
				{
					return typeConstraintAttribute.Constraint;
				}
				return typeof(DependencyObject);
			}
		}

		protected Type SourceTypeConstraint
		{
			get
			{
				return this.sourceTypeConstraint;
			}
		}

		public object SourceObject
		{
			get
			{
				return base.GetValue(EventTriggerBase.SourceObjectProperty);
			}
			set
			{
				base.SetValue(EventTriggerBase.SourceObjectProperty, value);
			}
		}

		public string SourceName
		{
			get
			{
				return (string)base.GetValue(EventTriggerBase.SourceNameProperty);
			}
			set
			{
				base.SetValue(EventTriggerBase.SourceNameProperty, value);
			}
		}

		public object Source
		{
			get
			{
				object obj = base.AssociatedObject;
				if (this.SourceObject != null)
				{
					obj = this.SourceObject;
				}
				else if (this.IsSourceNameSet)
				{
					obj = this.SourceNameResolver.Object;
					if (obj != null && !this.SourceTypeConstraint.IsAssignableFrom(obj.GetType()))
					{
						throw new InvalidOperationException("RetargetedTypeConstraintViolated");
					}
				}
				return obj;
			}
		}

		private NameResolver SourceNameResolver
		{
			get
			{
				return this.sourceNameResolver;
			}
		}

		private bool IsSourceChangedRegistered
		{
			get
			{
				return this.isSourceChangedRegistered;
			}
			set
			{
				this.isSourceChangedRegistered = value;
			}
		}

		private bool IsSourceNameSet
		{
			get
			{
				return !string.IsNullOrEmpty(this.SourceName) || base.ReadLocalValue(EventTriggerBase.SourceNameProperty) != DependencyProperty.UnsetValue;
			}
		}

		private bool IsLoadedRegistered { get; set; }

		internal EventTriggerBase(Type sourceTypeConstraint) : base(typeof(DependencyObject))
		{
			this.sourceTypeConstraint = sourceTypeConstraint;
			this.sourceNameResolver = new NameResolver();
			this.RegisterSourceChanged();
		}

		protected abstract string GetEventName();

		protected virtual void OnEvent(EventArgs eventArgs)
		{
			base.InvokeActions(eventArgs);
		}

		private void OnSourceChanged(object oldSource, object newSource)
		{
			if (base.AssociatedObject != null)
			{
				this.OnSourceChangedImpl(oldSource, newSource);
			}
		}

		internal virtual void OnSourceChangedImpl(object oldSource, object newSource)
		{
			if (string.IsNullOrEmpty(this.GetEventName()))
			{
				return;
			}
			if (string.Compare(this.GetEventName(), "Loaded", StringComparison.Ordinal) != 0)
			{
				if (oldSource != null && this.SourceTypeConstraint.IsAssignableFrom(oldSource.GetType()))
				{
					this.UnregisterEvent(oldSource, this.GetEventName());
				}
				if (newSource != null && this.SourceTypeConstraint.IsAssignableFrom(newSource.GetType()))
				{
					this.RegisterEvent(newSource, this.GetEventName());
				}
			}
		}

		protected override void OnAttached()
		{
			base.OnAttached();
			DependencyObject associatedObject = base.AssociatedObject;
			Behavior behavior = associatedObject as Behavior;
			FrameworkElement frameworkElement = associatedObject as FrameworkElement;
			this.RegisterSourceChanged();
			if (behavior != null)
			{
				associatedObject = ((IAttachedObject)behavior).AssociatedObject;
				behavior.AssociatedObjectChanged += this.OnBehaviorHostChanged;
			}
			else
			{
				if (this.SourceObject == null)
				{
					if (frameworkElement != null)
					{
						goto IL_5C;
					}
				}
				try
				{
					this.OnSourceChanged(null, this.Source);
					goto IL_68;
				}
				catch (InvalidOperationException)
				{
					goto IL_68;
				}
			IL_5C:
				this.SourceNameResolver.NameScopeReferenceElement = frameworkElement;
			}
		IL_68:
			bool flag = string.Compare(this.GetEventName(), "Loaded", StringComparison.Ordinal) == 0;
			if (flag && frameworkElement != null && !Interaction.IsElementLoaded(frameworkElement))
			{
				this.RegisterLoaded(frameworkElement);
			}
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			Behavior behavior = base.AssociatedObject as Behavior;
			FrameworkElement frameworkElement = base.AssociatedObject as FrameworkElement;
			try
			{
				this.OnSourceChanged(this.Source, null);
			}
			catch (InvalidOperationException)
			{
			}
			this.UnregisterSourceChanged();
			if (behavior != null)
			{
				behavior.AssociatedObjectChanged -= this.OnBehaviorHostChanged;
			}
			this.SourceNameResolver.NameScopeReferenceElement = null;
			bool flag = string.Compare(this.GetEventName(), "Loaded", StringComparison.Ordinal) == 0;
			if (flag && frameworkElement != null)
			{
				this.UnregisterLoaded(frameworkElement);
			}
		}

		private void OnBehaviorHostChanged(object sender, EventArgs e)
		{
			this.SourceNameResolver.NameScopeReferenceElement = (((IAttachedObject)sender).AssociatedObject as FrameworkElement);
		}

		private static void OnSourceObjectChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			EventTriggerBase eventTriggerBase = (EventTriggerBase)obj;
			object @object = eventTriggerBase.SourceNameResolver.Object;
			if (args.NewValue == null)
			{
				eventTriggerBase.OnSourceChanged(args.OldValue, @object);
				return;
			}
			if (args.OldValue == null && @object != null)
			{
				eventTriggerBase.UnregisterEvent(@object, eventTriggerBase.GetEventName());
			}
			eventTriggerBase.OnSourceChanged(args.OldValue, args.NewValue);
		}

		private static void OnSourceNameChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			EventTriggerBase eventTriggerBase = (EventTriggerBase)obj;
			eventTriggerBase.SourceNameResolver.Name = (string)args.NewValue;
		}

		private void RegisterSourceChanged()
		{
			if (!this.IsSourceChangedRegistered)
			{
				this.SourceNameResolver.ResolvedElementChanged += this.OnSourceNameResolverElementChanged;
				this.IsSourceChangedRegistered = true;
			}
		}

		private void UnregisterSourceChanged()
		{
			if (this.IsSourceChangedRegistered)
			{
				this.SourceNameResolver.ResolvedElementChanged -= this.OnSourceNameResolverElementChanged;
				this.IsSourceChangedRegistered = false;
			}
		}

		private void OnSourceNameResolverElementChanged(object sender, NameResolvedEventArgs e)
		{
			if (this.SourceObject == null)
			{
				this.OnSourceChanged(e.OldObject, e.NewObject);
			}
		}

		private void RegisterLoaded(FrameworkElement associatedElement)
		{
			if (!this.IsLoadedRegistered && associatedElement != null)
			{
				associatedElement.Loaded += new RoutedEventHandler(this.OnEventImpl);
				this.IsLoadedRegistered = true;
			}
		}

		private void UnregisterLoaded(FrameworkElement associatedElement)
		{
			if (this.IsLoadedRegistered && associatedElement != null)
			{
				associatedElement.Loaded -= new RoutedEventHandler(this.OnEventImpl);
				this.IsLoadedRegistered = false;
			}
		}

		private void RegisterEvent(object obj, string eventName)
		{
			Type type = obj.GetType();
			EventInfo @event = type.GetEvent(eventName);
			if (@event == null)
			{
				if (this.SourceObject != null)
				{
					throw new ArgumentException("EventTriggerCannotFindEventName");
				}
				return;
			}
			else
			{
				if (EventTriggerBase.IsValidEvent(@event))
				{
					this.eventHandlerMethodInfo = typeof(EventTriggerBase).GetMethod("OnEventImpl", BindingFlags.Instance | BindingFlags.NonPublic);
					@event.AddEventHandler(obj, Delegate.CreateDelegate(@event.EventHandlerType, this, this.eventHandlerMethodInfo));
					return;
				}
				if (this.SourceObject != null)
				{
					throw new ArgumentException("EventTriggerBaseInvalid");
				}
				return;
			}
		}

		private static bool IsValidEvent(EventInfo eventInfo)
		{
			Type eventHandlerType = eventInfo.EventHandlerType;
			if (typeof(Delegate).IsAssignableFrom(eventInfo.EventHandlerType))
			{
				MethodInfo method = eventHandlerType.GetMethod("Invoke");
				ParameterInfo[] parameters = method.GetParameters();
				return parameters.Length == 2 && typeof(object).IsAssignableFrom(parameters[0].ParameterType) && typeof(EventArgs).IsAssignableFrom(parameters[1].ParameterType);
			}
			return false;
		}

		private void UnregisterEvent(object obj, string eventName)
		{
			if (string.Compare(eventName, "Loaded", StringComparison.Ordinal) == 0)
			{
				FrameworkElement frameworkElement = obj as FrameworkElement;
				if (frameworkElement != null)
				{
					this.UnregisterLoaded(frameworkElement);
					return;
				}
			}
			else
			{
				this.UnregisterEventImpl(obj, eventName);
			}
		}

		private void UnregisterEventImpl(object obj, string eventName)
		{
			Type type = obj.GetType();
			if (this.eventHandlerMethodInfo == null)
			{
				return;
			}
			EventInfo @event = type.GetEvent(eventName);
			@event.RemoveEventHandler(obj, Delegate.CreateDelegate(@event.EventHandlerType, this, this.eventHandlerMethodInfo));
			this.eventHandlerMethodInfo = null;
		}

		private void OnEventImpl(object sender, EventArgs eventArgs)
		{
			this.OnEvent(eventArgs);
		}

		internal void OnEventNameChanged(string oldEventName, string newEventName)
		{
			if (base.AssociatedObject != null)
			{
				FrameworkElement frameworkElement = this.Source as FrameworkElement;
				if (frameworkElement != null && string.Compare(oldEventName, "Loaded", StringComparison.Ordinal) == 0)
				{
					this.UnregisterLoaded(frameworkElement);
				}
				else if (!string.IsNullOrEmpty(oldEventName))
				{
					this.UnregisterEvent(this.Source, oldEventName);
				}
				if (frameworkElement != null && string.Compare(newEventName, "Loaded", StringComparison.Ordinal) == 0)
				{
					this.RegisterLoaded(frameworkElement);
					return;
				}
				if (!string.IsNullOrEmpty(newEventName))
				{
					this.RegisterEvent(this.Source, newEventName);
				}
			}
		}

		private Type sourceTypeConstraint;

		private bool isSourceChangedRegistered;

		private NameResolver sourceNameResolver;

		private MethodInfo eventHandlerMethodInfo;

		public static readonly DependencyProperty SourceObjectProperty = DependencyProperty.Register("SourceObject", typeof(object), typeof(EventTriggerBase), new PropertyMetadata(new PropertyChangedCallback(EventTriggerBase.OnSourceObjectChanged)));

		public static readonly DependencyProperty SourceNameProperty = DependencyProperty.Register("SourceName", typeof(string), typeof(EventTriggerBase), new PropertyMetadata(new PropertyChangedCallback(EventTriggerBase.OnSourceNameChanged)));
	}

	public abstract class EventTriggerBase<T> : EventTriggerBase where T : class
	{
		protected EventTriggerBase() : base(typeof(T))
		{
		}

		public new T Source
		{
			get
			{
				return (T)((object)base.Source);
			}
		}

		internal sealed override void OnSourceChangedImpl(object oldSource, object newSource)
		{
			base.OnSourceChangedImpl(oldSource, newSource);
			this.OnSourceChanged(oldSource as T, newSource as T);
		}

		protected virtual void OnSourceChanged(T oldSource, T newSource)
		{
		}
	}

	public interface IAttachedObject
	{
		DependencyObject AssociatedObject { get; }

		void Attach(DependencyObject dependencyObject);

		void Detach();
	}

	public static class Interaction
	{
		internal static bool ShouldRunInDesignMode { get; set; }

		public static TriggerCollection GetTriggers(DependencyObject obj)
		{
			TriggerCollection triggerCollection = (TriggerCollection)obj.GetValue(Interaction.TriggersProperty);
			if (triggerCollection == null)
			{
				triggerCollection = new TriggerCollection();
				obj.SetValue(Interaction.TriggersProperty, triggerCollection);
			}
			return triggerCollection;
		}

		public static BehaviorCollection GetBehaviors(DependencyObject obj)
		{
			BehaviorCollection behaviorCollection = (BehaviorCollection)obj.GetValue(Interaction.BehaviorsProperty);
			if (behaviorCollection == null)
			{
				behaviorCollection = new BehaviorCollection();
				obj.SetValue(Interaction.BehaviorsProperty, behaviorCollection);
			}
			return behaviorCollection;
		}

		private static void OnBehaviorsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			BehaviorCollection behaviorCollection = (BehaviorCollection)args.OldValue;
			BehaviorCollection behaviorCollection2 = (BehaviorCollection)args.NewValue;
			if (behaviorCollection != behaviorCollection2)
			{
				if (behaviorCollection != null && ((IAttachedObject)behaviorCollection).AssociatedObject != null)
				{
					behaviorCollection.Detach();
				}
				if (behaviorCollection2 != null && obj != null)
				{
					if (((IAttachedObject)behaviorCollection2).AssociatedObject != null)
					{
						throw new InvalidOperationException("CannotHostBehaviorCollectionMultipleTimes");
					}
					behaviorCollection2.Attach(obj);
				}
			}
		}

		private static void OnTriggersChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			TriggerCollection triggerCollection = args.OldValue as TriggerCollection;
			TriggerCollection triggerCollection2 = args.NewValue as TriggerCollection;
			if (triggerCollection != triggerCollection2)
			{
				if (triggerCollection != null && ((IAttachedObject)triggerCollection).AssociatedObject != null)
				{
					triggerCollection.Detach();
				}
				if (triggerCollection2 != null && obj != null)
				{
					if (((IAttachedObject)triggerCollection2).AssociatedObject != null)
					{
						throw new InvalidOperationException("CannotHostTriggerCollectionMultipleTimes");
					}
					triggerCollection2.Attach(obj);
				}
			}
		}

		internal static bool IsElementLoaded(FrameworkElement element)
		{
			return element.IsLoaded;
		}

		private static readonly DependencyProperty TriggersProperty = DependencyProperty.RegisterAttached("ShadowTriggers", typeof(TriggerCollection), typeof(Interaction), new FrameworkPropertyMetadata(new PropertyChangedCallback(Interaction.OnTriggersChanged)));

		private static readonly DependencyProperty BehaviorsProperty = DependencyProperty.RegisterAttached("ShadowBehaviors", typeof(BehaviorCollection), typeof(Interaction), new FrameworkPropertyMetadata(new PropertyChangedCallback(Interaction.OnBehaviorsChanged)));
	}

	public sealed class InvokeCommandAction : TriggerAction<DependencyObject>
	{
		public string CommandName
		{
			get
			{
				base.ReadPreamble();
				return this.commandName;
			}
			set
			{
				if (this.CommandName != value)
				{
					base.WritePreamble();
					this.commandName = value;
					base.WritePostscript();
				}
			}
		}

		public ICommand Command
		{
			get
			{
				return (ICommand)base.GetValue(InvokeCommandAction.CommandProperty);
			}
			set
			{
				base.SetValue(InvokeCommandAction.CommandProperty, value);
			}
		}

		public object CommandParameter
		{
			get
			{
				return base.GetValue(InvokeCommandAction.CommandParameterProperty);
			}
			set
			{
				base.SetValue(InvokeCommandAction.CommandParameterProperty, value);
			}
		}

		protected override void Invoke(object parameter)
		{
			if (base.AssociatedObject != null)
			{
				ICommand command = this.ResolveCommand();
				if (command != null && command.CanExecute(this.CommandParameter))
				{
					command.Execute(this.CommandParameter);
				}
			}
		}

		private ICommand ResolveCommand()
		{
			ICommand result = null;
			if (this.Command != null)
			{
				result = this.Command;
			}
			else if (base.AssociatedObject != null)
			{
				Type type = base.AssociatedObject.GetType();
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
				foreach (PropertyInfo propertyInfo in properties)
				{
					if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType) && string.Equals(propertyInfo.Name, this.CommandName, StringComparison.Ordinal))
					{
						result = (ICommand)propertyInfo.GetValue(base.AssociatedObject, null);
					}
				}
			}
			return result;
		}

		private string commandName;

		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandAction), null);

		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandAction), null);
	}

	internal sealed class NameResolvedEventArgs : EventArgs
	{
		public object OldObject
		{
			get
			{
				return this.oldObject;
			}
		}

		public object NewObject
		{
			get
			{
				return this.newObject;
			}
		}

		public NameResolvedEventArgs(object oldObject, object newObject)
		{
			this.oldObject = oldObject;
			this.newObject = newObject;
		}

		private object oldObject;

		private object newObject;
	}

	internal sealed class NameResolver
	{
		public event EventHandler<NameResolvedEventArgs> ResolvedElementChanged;

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				DependencyObject @object = this.Object;
				this.name = value;
				this.UpdateObjectFromName(@object);
			}
		}

		public DependencyObject Object
		{
			get
			{
				if (string.IsNullOrEmpty(this.Name) && this.HasAttempedResolve)
				{
					return this.NameScopeReferenceElement;
				}
				return this.ResolvedObject;
			}
		}

		public FrameworkElement NameScopeReferenceElement
		{
			get
			{
				return this.nameScopeReferenceElement;
			}
			set
			{
				FrameworkElement oldNameScopeReference = this.NameScopeReferenceElement;
				this.nameScopeReferenceElement = value;
				this.OnNameScopeReferenceElementChanged(oldNameScopeReference);
			}
		}

		private FrameworkElement ActualNameScopeReferenceElement
		{
			get
			{
				if (this.NameScopeReferenceElement == null || !Interaction.IsElementLoaded(this.NameScopeReferenceElement))
				{
					return null;
				}
				return this.GetActualNameScopeReference(this.NameScopeReferenceElement);
			}
		}

		private DependencyObject ResolvedObject { get; set; }

		private bool PendingReferenceElementLoad { get; set; }

		private bool HasAttempedResolve { get; set; }

		private void OnNameScopeReferenceElementChanged(FrameworkElement oldNameScopeReference)
		{
			if (this.PendingReferenceElementLoad)
			{
				oldNameScopeReference.Loaded -= this.OnNameScopeReferenceLoaded;
				this.PendingReferenceElementLoad = false;
			}
			this.HasAttempedResolve = false;
			this.UpdateObjectFromName(this.Object);
		}

		private void UpdateObjectFromName(DependencyObject oldObject)
		{
			DependencyObject resolvedObject = null;
			this.ResolvedObject = null;
			if (this.NameScopeReferenceElement != null)
			{
				if (!Interaction.IsElementLoaded(this.NameScopeReferenceElement))
				{
					this.NameScopeReferenceElement.Loaded += this.OnNameScopeReferenceLoaded;
					this.PendingReferenceElementLoad = true;
					return;
				}
				if (!string.IsNullOrEmpty(this.Name))
				{
					FrameworkElement actualNameScopeReferenceElement = this.ActualNameScopeReferenceElement;
					if (actualNameScopeReferenceElement != null)
					{
						resolvedObject = (actualNameScopeReferenceElement.FindName(this.Name) as DependencyObject);
					}
				}
			}
			this.HasAttempedResolve = true;
			this.ResolvedObject = resolvedObject;
			if (oldObject != this.Object)
			{
				this.OnObjectChanged(oldObject, this.Object);
			}
		}

		private void OnObjectChanged(DependencyObject oldTarget, DependencyObject newTarget)
		{
			if (this.ResolvedElementChanged != null)
			{
				this.ResolvedElementChanged(this, new NameResolvedEventArgs(oldTarget, newTarget));
			}
		}

		private FrameworkElement GetActualNameScopeReference(FrameworkElement initialReferenceElement)
		{
			FrameworkElement frameworkElement = initialReferenceElement;
			if (this.IsNameScope(initialReferenceElement))
			{
				frameworkElement = ((initialReferenceElement.Parent as FrameworkElement) ?? frameworkElement);
			}
			return frameworkElement;
		}

		private bool IsNameScope(FrameworkElement frameworkElement)
		{
			FrameworkElement frameworkElement2 = frameworkElement.Parent as FrameworkElement;
			if (frameworkElement2 != null)
			{
				object obj = frameworkElement2.FindName(this.Name);
				return obj != null;
			}
			return false;
		}

		private void OnNameScopeReferenceLoaded(object sender, RoutedEventArgs e)
		{
			this.PendingReferenceElementLoad = false;
			this.NameScopeReferenceElement.Loaded -= this.OnNameScopeReferenceLoaded;
			this.UpdateObjectFromName(this.Object);
		}

		private string name;

		private FrameworkElement nameScopeReferenceElement;
	}

	public class PreviewInvokeEventArgs : EventArgs
	{
		public bool Cancelling { get; set; }
	}

	public abstract class TargetedTriggerAction : TriggerAction
	{
		public object TargetObject
		{
			get
			{
				return base.GetValue(TargetedTriggerAction.TargetObjectProperty);
			}
			set
			{
				base.SetValue(TargetedTriggerAction.TargetObjectProperty, value);
			}
		}

		public string TargetName
		{
			get
			{
				return (string)base.GetValue(TargetedTriggerAction.TargetNameProperty);
			}
			set
			{
				base.SetValue(TargetedTriggerAction.TargetNameProperty, value);
			}
		}

		protected object Target
		{
			get
			{
				object obj = base.AssociatedObject;
				if (this.TargetObject != null)
				{
					obj = this.TargetObject;
				}
				else if (this.IsTargetNameSet)
				{
					obj = this.TargetResolver.Object;
				}
				if (obj != null && !this.TargetTypeConstraint.IsAssignableFrom(obj.GetType()))
				{
					throw new InvalidOperationException("RetargetedTypeConstraintViolated");
				}
				return obj;
			}
		}

		protected sealed override Type AssociatedObjectTypeConstraint
		{
			get
			{
				AttributeCollection attributes = TypeDescriptor.GetAttributes(base.GetType());
				TypeConstraintAttribute typeConstraintAttribute = attributes[typeof(TypeConstraintAttribute)] as TypeConstraintAttribute;
				if (typeConstraintAttribute != null)
				{
					return typeConstraintAttribute.Constraint;
				}
				return typeof(DependencyObject);
			}
		}

		protected Type TargetTypeConstraint
		{
			get
			{
				base.ReadPreamble();
				return this.targetTypeConstraint;
			}
		}

		private bool IsTargetNameSet
		{
			get
			{
				return !string.IsNullOrEmpty(this.TargetName) || base.ReadLocalValue(TargetedTriggerAction.TargetNameProperty) != DependencyProperty.UnsetValue;
			}
		}

		private NameResolver TargetResolver
		{
			get
			{
				return this.targetResolver;
			}
		}

		private bool IsTargetChangedRegistered
		{
			get
			{
				return this.isTargetChangedRegistered;
			}
			set
			{
				this.isTargetChangedRegistered = value;
			}
		}

		internal TargetedTriggerAction(Type targetTypeConstraint) : base(typeof(DependencyObject))
		{
			this.targetTypeConstraint = targetTypeConstraint;
			this.targetResolver = new NameResolver();
			this.RegisterTargetChanged();
		}

		internal virtual void OnTargetChangedImpl(object oldTarget, object newTarget)
		{
		}

		protected override void OnAttached()
		{
			base.OnAttached();
			DependencyObject associatedObject = base.AssociatedObject;
			Behavior behavior = associatedObject as Behavior;
			this.RegisterTargetChanged();
			if (behavior != null)
			{
				associatedObject = ((IAttachedObject)behavior).AssociatedObject;
				behavior.AssociatedObjectChanged += this.OnBehaviorHostChanged;
			}
			this.TargetResolver.NameScopeReferenceElement = (associatedObject as FrameworkElement);
		}

		protected override void OnDetaching()
		{
			Behavior behavior = base.AssociatedObject as Behavior;
			base.OnDetaching();
			this.OnTargetChangedImpl(this.TargetResolver.Object, null);
			this.UnregisterTargetChanged();
			if (behavior != null)
			{
				behavior.AssociatedObjectChanged -= this.OnBehaviorHostChanged;
			}
			this.TargetResolver.NameScopeReferenceElement = null;
		}

		private void OnBehaviorHostChanged(object sender, EventArgs e)
		{
			this.TargetResolver.NameScopeReferenceElement = (((IAttachedObject)sender).AssociatedObject as FrameworkElement);
		}

		private void RegisterTargetChanged()
		{
			if (!this.IsTargetChangedRegistered)
			{
				this.TargetResolver.ResolvedElementChanged += this.OnTargetChanged;
				this.IsTargetChangedRegistered = true;
			}
		}

		private void UnregisterTargetChanged()
		{
			if (this.IsTargetChangedRegistered)
			{
				this.TargetResolver.ResolvedElementChanged -= this.OnTargetChanged;
				this.IsTargetChangedRegistered = false;
			}
		}

		private static void OnTargetObjectChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			TargetedTriggerAction targetedTriggerAction = (TargetedTriggerAction)obj;
			targetedTriggerAction.OnTargetChanged(obj, new NameResolvedEventArgs(args.OldValue, args.NewValue));
		}

		private static void OnTargetNameChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			TargetedTriggerAction targetedTriggerAction = (TargetedTriggerAction)obj;
			targetedTriggerAction.TargetResolver.Name = (string)args.NewValue;
		}

		private void OnTargetChanged(object sender, NameResolvedEventArgs e)
		{
			if (base.AssociatedObject != null)
			{
				this.OnTargetChangedImpl(e.OldObject, e.NewObject);
			}
		}

		private Type targetTypeConstraint;

		private bool isTargetChangedRegistered;

		private NameResolver targetResolver;

		public static readonly DependencyProperty TargetObjectProperty = DependencyProperty.Register("TargetObject", typeof(object), typeof(TargetedTriggerAction), new FrameworkPropertyMetadata(new PropertyChangedCallback(TargetedTriggerAction.OnTargetObjectChanged)));

		public static readonly DependencyProperty TargetNameProperty = DependencyProperty.Register("TargetName", typeof(string), typeof(TargetedTriggerAction), new FrameworkPropertyMetadata(new PropertyChangedCallback(TargetedTriggerAction.OnTargetNameChanged)));
	}

	public abstract class TargetedTriggerAction<T> : TargetedTriggerAction where T : class
	{
		protected TargetedTriggerAction() : base(typeof(T))
		{
		}

		protected new T Target
		{
			get
			{
				return (T)((object)base.Target);
			}
		}

		internal sealed override void OnTargetChangedImpl(object oldTarget, object newTarget)
		{
			base.OnTargetChangedImpl(oldTarget, newTarget);
			this.OnTargetChanged(oldTarget as T, newTarget as T);
		}

		protected virtual void OnTargetChanged(T oldTarget, T newTarget)
		{
		}

	}

	[DefaultTrigger(typeof(UIElement), typeof(EventTrigger), "MouseLeftButtonDown")]
	[DefaultTrigger(typeof(ButtonBase), typeof(EventTrigger), "Click")]
	public abstract class TriggerAction : Animatable, IAttachedObject
	{
		public bool IsEnabled
		{
			get
			{
				return (bool)base.GetValue(TriggerAction.IsEnabledProperty);
			}
			set
			{
				base.SetValue(TriggerAction.IsEnabledProperty, value);
			}
		}

		protected DependencyObject AssociatedObject
		{
			get
			{
				base.ReadPreamble();
				return this.associatedObject;
			}
		}

		protected virtual Type AssociatedObjectTypeConstraint
		{
			get
			{
				base.ReadPreamble();
				return this.associatedObjectTypeConstraint;
			}
		}

		internal bool IsHosted
		{
			get
			{
				base.ReadPreamble();
				return this.isHosted;
			}
			set
			{
				base.WritePreamble();
				this.isHosted = value;
				base.WritePostscript();
			}
		}

		internal TriggerAction(Type associatedObjectTypeConstraint)
		{
			this.associatedObjectTypeConstraint = associatedObjectTypeConstraint;
		}

		internal void CallInvoke(object parameter)
		{
			if (this.IsEnabled)
			{
				this.Invoke(parameter);
			}
		}

		protected abstract void Invoke(object parameter);

		protected virtual void OnAttached()
		{
		}

		protected virtual void OnDetaching()
		{
		}

		protected override Freezable CreateInstanceCore()
		{
			Type type = base.GetType();
			return (Freezable)Activator.CreateInstance(type);
		}

		DependencyObject IAttachedObject.AssociatedObject
		{
			get
			{
				return this.AssociatedObject;
			}
		}

		public void Attach(DependencyObject dependencyObject)
		{
			if (dependencyObject != this.AssociatedObject)
			{
				if (this.AssociatedObject != null)
				{
					throw new InvalidOperationException("CannotHostTriggerActionMultipleTimes");
				}
				if (dependencyObject != null && !this.AssociatedObjectTypeConstraint.IsAssignableFrom(dependencyObject.GetType()))
				{
					throw new InvalidOperationException("TypeConstraintViolated");
				}
				base.WritePreamble();
				this.associatedObject = dependencyObject;
				base.WritePostscript();
				this.OnAttached();
			}
		}

		public void Detach()
		{
			this.OnDetaching();
			base.WritePreamble();
			this.associatedObject = null;
			base.WritePostscript();
		}

		private bool isHosted;

		private DependencyObject associatedObject;

		private Type associatedObjectTypeConstraint;

		public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(TriggerAction), new FrameworkPropertyMetadata(true));
	}

	public class TriggerActionCollection : AttachableCollection<TriggerAction>
	{
		internal TriggerActionCollection()
		{
		}

		protected override void OnAttached()
		{
			foreach (TriggerAction triggerAction in this)
			{
				triggerAction.Attach(base.AssociatedObject);
			}
		}

		protected override void OnDetaching()
		{
			foreach (TriggerAction triggerAction in this)
			{
				triggerAction.Detach();
			}
		}

		internal override void ItemAdded(TriggerAction item)
		{
			if (item.IsHosted)
			{
				throw new InvalidOperationException("CannotHostTriggerActionMultipleTimes");
			}
			if (base.AssociatedObject != null)
			{
				item.Attach(base.AssociatedObject);
			}
			item.IsHosted = true;
		}

		internal override void ItemRemoved(TriggerAction item)
		{
			if (((IAttachedObject)item).AssociatedObject != null)
			{
				item.Detach();
			}
			item.IsHosted = false;
		}

		protected override Freezable CreateInstanceCore()
		{
			return new TriggerActionCollection();
		}
	}

	public abstract class TriggerAction<T> : TriggerAction where T : DependencyObject
	{
		protected TriggerAction() : base(typeof(T))
		{
		}

		protected new T AssociatedObject
		{
			get
			{
				return (T)((object)base.AssociatedObject);
			}
		}

		protected sealed override Type AssociatedObjectTypeConstraint
		{
			get
			{
				return base.AssociatedObjectTypeConstraint;
			}
		}
	}

	[ContentProperty("Actions")]
	public abstract class TriggerBase : Animatable, IAttachedObject
	{
		internal TriggerBase(Type associatedObjectTypeConstraint)
		{
			this.associatedObjectTypeConstraint = associatedObjectTypeConstraint;
			TriggerActionCollection value = new TriggerActionCollection();
			base.SetValue(TriggerBase.ActionsPropertyKey, value);
		}

		protected DependencyObject AssociatedObject
		{
			get
			{
				base.ReadPreamble();
				return this.associatedObject;
			}
		}

		protected virtual Type AssociatedObjectTypeConstraint
		{
			get
			{
				base.ReadPreamble();
				return this.associatedObjectTypeConstraint;
			}
		}

		public TriggerActionCollection Actions
		{
			get
			{
				return (TriggerActionCollection)base.GetValue(TriggerBase.ActionsProperty);
			}
		}

		public event EventHandler<PreviewInvokeEventArgs> PreviewInvoke;

		protected void InvokeActions(object parameter)
		{
			if (this.PreviewInvoke != null)
			{
				PreviewInvokeEventArgs previewInvokeEventArgs = new PreviewInvokeEventArgs();
				this.PreviewInvoke(this, previewInvokeEventArgs);
				if (previewInvokeEventArgs.Cancelling)
				{
					return;
				}
			}
			foreach (TriggerAction triggerAction in this.Actions)
			{
				triggerAction.CallInvoke(parameter);
			}
		}

		protected virtual void OnAttached()
		{
		}

		protected virtual void OnDetaching()
		{
		}

		protected override Freezable CreateInstanceCore()
		{
			Type type = base.GetType();
			return (Freezable)Activator.CreateInstance(type);
		}

		DependencyObject IAttachedObject.AssociatedObject
		{
			get
			{
				return this.AssociatedObject;
			}
		}

		public void Attach(DependencyObject dependencyObject)
		{
			if (dependencyObject != this.AssociatedObject)
			{
				if (this.AssociatedObject != null)
				{
					throw new InvalidOperationException("CannotHostTriggerMultipleTimes");
				}
				if (dependencyObject != null && !this.AssociatedObjectTypeConstraint.IsAssignableFrom(dependencyObject.GetType()))
				{
					throw new InvalidOperationException("TypeConstraintViolated");
				}
				base.WritePreamble();
				this.associatedObject = dependencyObject;
				base.WritePostscript();
				this.Actions.Attach(dependencyObject);
				this.OnAttached();
			}
		}

		public void Detach()
		{
			this.OnDetaching();
			base.WritePreamble();
			this.associatedObject = null;
			base.WritePostscript();
			this.Actions.Detach();
		}

		private DependencyObject associatedObject;

		private Type associatedObjectTypeConstraint;

		private static readonly DependencyPropertyKey ActionsPropertyKey = DependencyProperty.RegisterReadOnly("Actions", typeof(TriggerActionCollection), typeof(TriggerBase), new FrameworkPropertyMetadata());

		public static readonly DependencyProperty ActionsProperty = TriggerBase.ActionsPropertyKey.DependencyProperty;
	}

	public abstract class TriggerBase<T> : TriggerBase where T : DependencyObject
	{
		protected TriggerBase() : base(typeof(T))
		{
		}

		protected new T AssociatedObject
		{
			get
			{
				return (T)((object)base.AssociatedObject);
			}
		}

		protected sealed override Type AssociatedObjectTypeConstraint
		{
			get
			{
				return base.AssociatedObjectTypeConstraint;
			}
		}
	}

	public sealed class TriggerCollection : AttachableCollection<TriggerBase>
	{
		internal TriggerCollection()
		{
		}

		protected override void OnAttached()
		{
			foreach (TriggerBase triggerBase in this)
			{
				triggerBase.Attach(base.AssociatedObject);
			}
		}

		protected override void OnDetaching()
		{
			foreach (TriggerBase triggerBase in this)
			{
				triggerBase.Detach();
			}
		}

		internal override void ItemAdded(TriggerBase item)
		{
			if (base.AssociatedObject != null)
			{
				item.Attach(base.AssociatedObject);
			}
		}

		internal override void ItemRemoved(TriggerBase item)
		{
			if (((IAttachedObject)item).AssociatedObject != null)
			{
				item.Detach();
			}
		}

		protected override Freezable CreateInstanceCore()
		{
			return new TriggerCollection();
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class TypeConstraintAttribute : Attribute
	{
		public Type Constraint { get; private set; }

		public TypeConstraintAttribute(Type constraint)
		{
			this.Constraint = constraint;
		}
	}
}
