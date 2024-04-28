using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FantaziaDesign.Core;

namespace FantaziaDesign.Wpf.Input
{
	public enum CommandComponentKind
	{
		CommandSource,
		Command,
		CommandParameter,
		CommandTarget
	}

	public interface IEnhancedCommandSource : ICommandSource
	{
		IInputElement ParentInputElement { get; set; }
		void SetCommand(ICommand command);
		void SetCommandParameter(object parameter);
		void SetCommandTarget(IInputElement target);
	}

	public interface IMultiCommandSource<TSourceKey, TEnhancedCommandSource> where TEnhancedCommandSource : IEnhancedCommandSource
	{
		IInputElement ParentInputElement { get; set; }

		IReadOnlyCollection<TEnhancedCommandSource> Sources { get; }

		TEnhancedCommandSource this[TSourceKey sourceKey, CommandComponentKind componentKind = CommandComponentKind.CommandSource] { get; }

		TEnhancedCommandSource RegisterCommandSource(TSourceKey sourceKey, CommandComponentKind componentKind, TEnhancedCommandSource commandSource, bool modifyIfDuplicated = false);

		bool UnregisterCommandSource(TSourceKey sourceKey, CommandComponentKind componentKind = CommandComponentKind.CommandSource);
	}

	public interface ICommandSourceExecutor<TKey>
	{
		bool CanExecuteCommandSource(TKey commandSourceName, CommandComponentKind componentKind = CommandComponentKind.CommandSource);
		bool ExecuteCommandSource(TKey commandSourceName, CommandComponentKind componentKind = CommandComponentKind.CommandSource);
	}

	public class CommonCommandSource : IEnhancedCommandSource, IDisposable
	{
		public static bool TryGetContractedName(DependencyProperty dependencyProperty, CommandComponentKind fromKind, CommandComponentKind toKind, out string result)
		{
			if (IsMatchComponentKind(dependencyProperty, fromKind))
			{
				return TryGetContractedName(dependencyProperty.Name, fromKind, toKind, out result);
			}
			result = null;
			return false;
		}

		private static bool IsMatchComponentKind(DependencyProperty dependencyProperty, CommandComponentKind componentKind)
		{
			if (dependencyProperty is null)
			{
				return false;
			}
			var propertyType = dependencyProperty.PropertyType;
			switch (componentKind)
			{
				case CommandComponentKind.CommandSource:
					return typeof(ICommandSource).IsAssignableFrom(propertyType);
				case CommandComponentKind.Command:
					return typeof(ICommand).IsAssignableFrom(propertyType);
				case CommandComponentKind.CommandParameter:
					return true;
				case CommandComponentKind.CommandTarget:
					return typeof(IInputElement).IsAssignableFrom(propertyType);
				default:
					return true;
			}
		}

		public static bool TryGetContractedName(string name, CommandComponentKind fromKind, CommandComponentKind toKind, out string result)
		{
			if (!string.IsNullOrWhiteSpace(name))
			{
				if (fromKind == toKind)
				{
					result = name;
					return true;
				}
				if (GetCommandName(name, fromKind, out var commandName))
				{
					return ConvertCommandNameToComponent(commandName, toKind, out result);
				}
			}
			result = null;
			return false;
		}

		private static bool ConvertCommandNameToComponent(string commandName, CommandComponentKind toKind, out string result)
		{
			switch (toKind)
			{
				case CommandComponentKind.CommandSource:
					result = commandName + "Source";
					break;
				case CommandComponentKind.Command:
					result = commandName;
					break;
				case CommandComponentKind.CommandParameter:
					result = commandName + "Parameter";
					break;
				case CommandComponentKind.CommandTarget:
					result = commandName + "Target";
					break;
				default:
					result = commandName;
					return false;
			}
			return true;
		}

		private static bool GetCommandName(string name, CommandComponentKind fromKind, out string commandName)
		{
			switch (fromKind)
			{
				case CommandComponentKind.CommandSource:
					{
						if (name.EndsWith("Source"))
						{
							commandName = name.Substring(0, name.Length - 6);
							return true;
						}
					}
					break;
				case CommandComponentKind.Command:
					commandName = name;
					return true;
				case CommandComponentKind.CommandParameter:
					if (name.EndsWith("Parameter"))
					{
						commandName = name.Substring(0, name.Length - 9);
						return true;
					}
					break;
				case CommandComponentKind.CommandTarget:
					if (name.EndsWith("Target"))
					{
						commandName = name.Substring(0, name.Length - 6);
						return true;
					}
					break;
				default:
					break;
			}
			commandName = null;
			return false;
		}

		protected IInputElement m_parent;

		protected ICommand m_cmd;

		protected object m_param;

		protected IInputElement m_target;

		public ICommand Command => m_cmd;

		public object CommandParameter => m_param;

		public IInputElement CommandTarget => m_target;

		public bool CanExecute { get; private set; }

		public IInputElement ParentInputElement { get => m_parent; set => m_parent = value; }

		public void SetCommand(ICommand command)
		{
			if (command != m_cmd)
			{
				var oldCommand = m_cmd;
				m_cmd = command;
				OnCommandChanged(oldCommand, command);
			}
		}

		private void OnCommandChanged(ICommand oldCommand, ICommand newCommand)
		{
			if (oldCommand != null)
			{
				UnhookCommand(oldCommand);
			}
			if (newCommand != null)
			{
				HookCommand(newCommand);
			}
		}

		private void UnhookCommand(ICommand command)
		{
			CanExecuteChangedEventManager.RemoveHandler(command, new EventHandler<EventArgs>(OnCanExecuteChanged));
			UpdateCanExecute();
		}

		private void HookCommand(ICommand command)
		{
			CanExecuteChangedEventManager.AddHandler(command, new EventHandler<EventArgs>(OnCanExecuteChanged));
			UpdateCanExecute();
		}

		private void OnCanExecuteChanged(object sender, EventArgs e)
		{
			UpdateCanExecute();
		}

		private void UpdateCanExecute()
		{
			if (Command != null)
			{
				CanExecute = CommandHelper.CanExecuteCommandSource(this);
				return;
			}
			CanExecute = true;
		}

		public virtual void SetCommandParameter(object parameter)
		{
			m_param = parameter;
		}

		public virtual void SetCommandTarget(IInputElement target)
		{
			m_target = target;
		}

		public virtual void Dispose()
		{
			SetCommand(null);
			m_parent = null;
			m_param = null;
			m_target = null;
		}
	}

	public class CommonCommandSourceCollection
		: IMultiCommandSource<string, CommonCommandSource>,
		  ICommandSourceExecutor<string>,
		  IMultiCommandSource<DependencyProperty, CommonCommandSource>,
		  ICommandSourceExecutor<DependencyProperty>,
		  IReadOnlyDictionary<string, CommonCommandSource>,
		  IEnumerable<CommonCommandSource>,
		  IDisposable
	{
		private IInputElement m_parent;
		private bool isDisposed;
		private IReadOnlyCollection<CommonCommandSource> m_sourceWrapper;
		private Dictionary<string, CommonCommandSource> m_commandStore = new Dictionary<string, CommonCommandSource>();

		private void ThrowIfDisposed()
		{
			if (isDisposed)
			{
				throw new ObjectDisposedException(nameof(CommonCommandSourceCollection));
			}
		}

		public CommonCommandSource this[string sourceName, CommandComponentKind componentKind = CommandComponentKind.CommandSource]
		{
			get
			{
				ThrowIfDisposed();
				if (!CommonCommandSource.TryGetContractedName(sourceName, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
				{
					throw new InvalidOperationException("Cannot get command source name from DependencyProperty");
				}
				return m_commandStore[sourceName];
			}
		}

		public CommonCommandSource this[DependencyProperty dependencyProperty, CommandComponentKind componentKind = CommandComponentKind.CommandSource]
		{
			get
			{
				ThrowIfDisposed();
				if (!CommonCommandSource.TryGetContractedName(dependencyProperty, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
				{
					throw new InvalidOperationException("Cannot get command source name from DependencyProperty");
				}
				return m_commandStore[cmdSrcName];
			}
		}

		public int Count
		{
			get
			{
				ThrowIfDisposed();
				return m_commandStore.Count;
			}
		}

		public IInputElement ParentInputElement
		{
			get
			{
				ThrowIfDisposed();
				return m_parent;
			}

			set
			{
				ThrowIfDisposed();
				m_parent = value;
			}
		}

		public IReadOnlyCollection<CommonCommandSource> Sources
		{
			get
			{
				ThrowIfDisposed();
				if (m_sourceWrapper is null)
				{
					m_sourceWrapper = m_commandStore.Values.AsReadOnlyCollection();
				}
				return m_sourceWrapper;
			}
		}

		public bool ContainsKey(string commandName)
		{
			ThrowIfDisposed();
			return m_commandStore.ContainsKey(commandName);
		}

		public bool TryGetValue(string commandName, out CommonCommandSource value)
		{
			ThrowIfDisposed();
			return m_commandStore.TryGetValue(commandName, out value);
		}

		public IEnumerator<CommonCommandSource> GetEnumerator()
		{
			ThrowIfDisposed();
			return m_commandStore.Values.GetEnumerator();
		}

		IEnumerable<string> IReadOnlyDictionary<string, CommonCommandSource>.Keys
		{
			get
			{
				ThrowIfDisposed();
				return m_commandStore.Keys;
			}
		}

		IEnumerable<CommonCommandSource> IReadOnlyDictionary<string, CommonCommandSource>.Values
		{
			get
			{
				ThrowIfDisposed();
				return m_commandStore.Values;
			}
		}

		CommonCommandSource IReadOnlyDictionary<string, CommonCommandSource>.this[string key] => this[key, CommandComponentKind.CommandSource];

		IEnumerator<KeyValuePair<string, CommonCommandSource>> IEnumerable<KeyValuePair<string, CommonCommandSource>>.GetEnumerator()
		{
			ThrowIfDisposed();
			return m_commandStore.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			ThrowIfDisposed();
			return m_commandStore.GetEnumerator();
		}

		private CommonCommandSource RegisterCommandSourceInternal(string sourceKey, CommonCommandSource commandSource, ref bool modifyIfDuplicated)
		{
			if (m_commandStore.TryGetValue(sourceKey, out CommonCommandSource oldSource))
			{
				if (modifyIfDuplicated)
				{
					m_commandStore[sourceKey] = commandSource;
					oldSource.ParentInputElement = null;
				}
			}
			else
			{
				m_commandStore.Add(sourceKey, commandSource);
			}
			commandSource.ParentInputElement = ParentInputElement;
			return commandSource;
		}

		public CommonCommandSource RegisterCommandSource(string sourceKey, CommandComponentKind componentKind, CommonCommandSource commandSource, bool modifyIfDuplicated = false)
		{
			ThrowIfDisposed();
			if (!CommonCommandSource.TryGetContractedName(sourceKey, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				throw new InvalidOperationException("Cannot get command source name from DependencyProperty");
			}
			if (commandSource is null)
			{
				throw new ArgumentNullException(nameof(commandSource));
			}
			return RegisterCommandSourceInternal(cmdSrcName, commandSource, ref modifyIfDuplicated);
		}

		public CommonCommandSource RegisterCommandSource(string sourceKey, CommandComponentKind componentKind, bool modifyIfDuplicated = false)
		{
			ThrowIfDisposed();
			if (!CommonCommandSource.TryGetContractedName(sourceKey, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				throw new InvalidOperationException("Cannot get command source name from DependencyProperty");
			}
			var commandSource = new CommonCommandSource();
			return RegisterCommandSourceInternal(cmdSrcName, commandSource, ref modifyIfDuplicated);
		}

		public CommonCommandSource RegisterCommandSource(DependencyProperty dependencyProperty, CommandComponentKind componentKind, bool modifyIfDuplicated = false)
		{
			ThrowIfDisposed();
			if (!CommonCommandSource.TryGetContractedName(dependencyProperty, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				throw new InvalidOperationException("Cannot get command source name from DependencyProperty");
			}
			var commandSource = new CommonCommandSource();
			return RegisterCommandSourceInternal(cmdSrcName, commandSource, ref modifyIfDuplicated);
		}

		public CommonCommandSource RegisterCommandSource(DependencyProperty dependencyProperty, CommandComponentKind componentKind, CommonCommandSource commandSource, bool modifyIfDuplicated = false)
		{
			ThrowIfDisposed();
			if (!CommonCommandSource.TryGetContractedName(dependencyProperty, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				throw new InvalidOperationException("Cannot get command source name from DependencyProperty");
			}
			return RegisterCommandSourceInternal(cmdSrcName, commandSource, ref modifyIfDuplicated);
		}

		public bool UnregisterCommandSource(string sourceKey, CommandComponentKind componentKind)
		{
			ThrowIfDisposed();
			if (CommonCommandSource.TryGetContractedName(sourceKey, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				if (m_commandStore.TryGetValue(sourceKey, out CommonCommandSource oldSource))
				{
					oldSource.ParentInputElement = null;
					return m_commandStore.Remove(sourceKey);
				}
			}
			return false;
		}

		public bool UnregisterCommandSource(DependencyProperty dependencyProperty, CommandComponentKind componentKind)
		{
			ThrowIfDisposed();
			if (CommonCommandSource.TryGetContractedName(dependencyProperty, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				if (m_commandStore.TryGetValue(cmdSrcName, out CommonCommandSource oldSource))
				{
					oldSource.ParentInputElement = null;
					return m_commandStore.Remove(cmdSrcName);
				}
			}
			return false;
		}

		public void Dispose()
		{
			if (isDisposed)
			{
				return;
			}
			m_commandStore.Clear();
			m_parent = null;
			m_sourceWrapper = null;
			m_commandStore = null;
			isDisposed = true;
		}

		public bool CanExecuteCommandSource(string name, CommandComponentKind componentKind = CommandComponentKind.CommandSource)
		{
			ThrowIfDisposed();
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"{nameof(name)} cannot be null or whitespace", nameof(name));
			}
			if (CommonCommandSource.TryGetContractedName(name, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				if (m_commandStore.TryGetValue(cmdSrcName, out CommonCommandSource commandSource))
				{
					return CommandHelper.CanExecuteCommandSource(commandSource);
				}
			}
			return false;
		}

		public bool ExecuteCommandSource(string name, CommandComponentKind componentKind = CommandComponentKind.CommandSource)
		{
			ThrowIfDisposed();
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"{nameof(name)} cannot be null or whitespace", nameof(name));
			}
			if (CommonCommandSource.TryGetContractedName(name, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				if (m_commandStore.TryGetValue(cmdSrcName, out CommonCommandSource commandSource))
				{
					return CommandHelper.CriticalExecuteCommandSource(commandSource);
				}
			}
			return false;
		}

		public Task<bool> ExecuteCommandSourceAsync(string name, CommandComponentKind componentKind = CommandComponentKind.CommandSource)
		{
			ThrowIfDisposed();
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException($"{nameof(name)} cannot be null or whitespace", nameof(name));
			}
			if (CommonCommandSource.TryGetContractedName(name, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				if (m_commandStore.TryGetValue(cmdSrcName, out CommonCommandSource commandSource))
				{
					return CommandHelper.CriticalExecuteCommandSourceAsync(commandSource);
				}
			}
			return Task.FromResult(false);
		}

		public bool CanExecuteCommandSource(DependencyProperty dependencyProperty, CommandComponentKind componentKind = CommandComponentKind.CommandSource)
		{
			ThrowIfDisposed();
			if (CommonCommandSource.TryGetContractedName(dependencyProperty, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				if (m_commandStore.TryGetValue(cmdSrcName, out CommonCommandSource commandSource))
				{
					return CommandHelper.CanExecuteCommandSource(commandSource);
				}
			}
			return false;
		}

		public bool ExecuteCommandSource(DependencyProperty dependencyProperty, CommandComponentKind componentKind = CommandComponentKind.CommandSource)
		{
			ThrowIfDisposed();
			if (CommonCommandSource.TryGetContractedName(dependencyProperty, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				if (m_commandStore.TryGetValue(cmdSrcName, out CommonCommandSource commandSource))
				{
					return CommandHelper.CriticalExecuteCommandSource(commandSource);
				}
			}
			return false;
		}

		public Task<bool> ExecuteCommandSourceAsync(DependencyProperty dependencyProperty, CommandComponentKind componentKind = CommandComponentKind.CommandSource)
		{
			ThrowIfDisposed();
			if (CommonCommandSource.TryGetContractedName(dependencyProperty, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				if (m_commandStore.TryGetValue(cmdSrcName, out CommonCommandSource commandSource))
				{
					return CommandHelper.CriticalExecuteCommandSourceAsync(commandSource);
				}
			}
			return Task.FromResult(false);
		}

		public bool TrySetCommand(string name, ICommand command, CommandComponentKind componentKind = CommandComponentKind.Command)
		{
			ThrowIfDisposed();
			if (CommonCommandSource.TryGetContractedName(name, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				if (m_commandStore.TryGetValue(cmdSrcName, out CommonCommandSource commandSource))
				{
					commandSource.SetCommand(command);
					return true;
				}
			}
			return false;
		}

		public bool TrySetCommandParameter(string name, object commandParameter, CommandComponentKind componentKind = CommandComponentKind.CommandParameter)
		{
			ThrowIfDisposed();
			if (CommonCommandSource.TryGetContractedName(name, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				if (m_commandStore.TryGetValue(cmdSrcName, out CommonCommandSource commandSource))
				{
					commandSource.SetCommandParameter(commandParameter);
					return true;
				}
			}
			return false;
		}

		public bool TrySetCommandTarget(string name, IInputElement commandTarget, CommandComponentKind componentKind = CommandComponentKind.CommandTarget)
		{
			ThrowIfDisposed();
			if (CommonCommandSource.TryGetContractedName(name, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				if (m_commandStore.TryGetValue(cmdSrcName, out CommonCommandSource commandSource))
				{
					commandSource.SetCommandTarget(commandTarget);
					return true;
				}
			}
			return false;
		}

		public bool TrySetCommand(DependencyProperty dependencyProperty, ICommand command, CommandComponentKind componentKind = CommandComponentKind.Command)
		{
			ThrowIfDisposed();
			if (CommonCommandSource.TryGetContractedName(dependencyProperty, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				if (m_commandStore.TryGetValue(cmdSrcName, out CommonCommandSource commandSource))
				{
					commandSource.SetCommand(command);
					return true;
				}
			}
			return false;
		}

		public bool TrySetCommandParameter(DependencyProperty dependencyProperty, object commandParameter, CommandComponentKind componentKind = CommandComponentKind.CommandParameter)
		{
			ThrowIfDisposed();
			if (CommonCommandSource.TryGetContractedName(dependencyProperty, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				if (m_commandStore.TryGetValue(cmdSrcName, out CommonCommandSource commandSource))
				{
					commandSource.SetCommandParameter(commandParameter);
					return true;
				}
			}
			return false;
		}

		public bool TrySetCommandTarget(DependencyProperty dependencyProperty, IInputElement commandTarget, CommandComponentKind componentKind = CommandComponentKind.CommandTarget)
		{
			ThrowIfDisposed();
			if (CommonCommandSource.TryGetContractedName(dependencyProperty, componentKind, CommandComponentKind.CommandSource, out var cmdSrcName))
			{
				if (m_commandStore.TryGetValue(cmdSrcName, out CommonCommandSource commandSource))
				{
					commandSource.SetCommandTarget(commandTarget);
					return true;
				}
			}
			return false;
		}


	}

}
