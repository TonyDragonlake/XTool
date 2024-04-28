using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FantaziaDesign.Input;

namespace FantaziaDesign.Wpf.Input
{
	public static class CommandHelper
	{
		public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler)
		{
			RegisterCommandHandlerCore(
				controlType, command, executedRoutedEventHandler, null, null);
		}

		public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, InputGesture inputGesture)
		{
			RegisterCommandHandlerCore(
				controlType, command, executedRoutedEventHandler, null, inputGesture);
		}

		public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, Key key)
		{
			RegisterCommandHandlerCore(
				controlType, command, executedRoutedEventHandler, null, new KeyGesture(key));
		}

		public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, InputGesture inputGesture, InputGesture inputGesture2)
		{
			RegisterCommandHandlerCore(
				controlType, command, executedRoutedEventHandler, null, inputGesture,inputGesture2);
		}

		public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler)
		{
			RegisterCommandHandlerCore(
				controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, null);
		}

		public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler, InputGesture inputGesture)
		{
			RegisterCommandHandlerCore(
				controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, inputGesture);
		}

		public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler, Key key)
		{
			RegisterCommandHandlerCore(
				controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, new KeyGesture(key));
		}

		public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler, InputGesture inputGesture, InputGesture inputGesture2)
		{
			RegisterCommandHandlerCore(
				controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, inputGesture, inputGesture2);
		}

		public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler, InputGesture inputGesture, InputGesture inputGesture2, InputGesture inputGesture3, InputGesture inputGesture4)
		{
			RegisterCommandHandlerCore(
				controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, inputGesture, inputGesture2, inputGesture3, inputGesture4);
		}

		public static void RegisterCommandHandler(Type controlType, RoutedCommand command, Key key, ModifierKeys modifierKeys, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler)
		{
			RegisterCommandHandlerCore(
				controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, new KeyGesture(key, modifierKeys));
		}

		public static void RegisterCommandHandlerCore(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler, params InputGesture[] inputGestures)
		{
			CommandManager.RegisterClassCommandBinding(controlType, new CommandBinding(command, executedRoutedEventHandler, canExecuteRoutedEventHandler));
			if (inputGestures != null)
			{
				for (int i = 0; i < inputGestures.Length; i++)
				{
					CommandManager.RegisterClassInputBinding(controlType, new InputBinding(command, inputGestures[i]));
				}
			}
		}

		public static bool CanExecuteCommandSource(ICommandSource commandSource)
		{
			ICommand command = commandSource.Command;
			//System.Diagnostics.Debug.WriteLine($"CanExecuteCommandSource, ICommandSource.Command = {command}");
			if (command is null)
			{
				return false;
			}
			object commandParameter = commandSource.CommandParameter;
			IInputElement inputElement = commandSource.CommandTarget;
			RoutedCommand routedCommand = command as RoutedCommand;
			if (routedCommand != null)
			{
				if (inputElement is null)
				{
					inputElement = commandSource as IInputElement;
				}
				if (inputElement is null)
				{
					var enhancedSource = commandSource as IEnhancedCommandSource;
					inputElement = enhancedSource?.ParentInputElement;
				}
				return routedCommand.CanExecute(commandParameter, inputElement);
			}
			return command.CanExecute(commandParameter);
		}

		public static bool ExecuteCommandSource(ICommandSource commandSource)
		{
			return CriticalExecuteCommandSource(commandSource);
		}

		public static Task<bool> ExecuteCommandSourceAsync(ICommandSource commandSource)
		{
			return CriticalExecuteCommandSourceAsync(commandSource);
		}

		internal static bool CriticalExecuteCommandSource(ICommandSource commandSource)
		{
			ICommand command = commandSource.Command;
			if (command != null)
			{
				object commandParameter = commandSource.CommandParameter;
				IInputElement inputElement = commandSource.CommandTarget;
				RoutedCommand routedCommand = command as RoutedCommand;
				if (routedCommand != null)
				{
					if (inputElement is null)
					{
						inputElement = commandSource as IInputElement;
					}
					if (inputElement is null)
					{
						var enhancedSource = commandSource as IEnhancedCommandSource;
						inputElement = enhancedSource?.ParentInputElement;
					}
					if (routedCommand.CanExecute(commandParameter, inputElement))
					{
						routedCommand.Execute(commandParameter, inputElement);
						return true;
					}
				}
				else if (command.CanExecute(commandParameter))
				{
					command.Execute(commandParameter);
					return true;
				}
			}
			return false;
		}

		internal static Task<bool> CriticalExecuteCommandSourceAsync(ICommandSource commandSource)
		{
			ICommand command = commandSource.Command;
			if (command != null)
			{
				object commandParameter = commandSource.CommandParameter;
				IInputElement inputElement = commandSource.CommandTarget;
				RoutedCommand routedCommand = command as RoutedCommand;
				if (routedCommand != null)
				{
					if (inputElement is null)
					{
						inputElement = commandSource as IInputElement;
					}
					if (inputElement is null)
					{
						var enhancedSource = commandSource as IEnhancedCommandSource;
						inputElement = enhancedSource?.ParentInputElement;
					}
					if (routedCommand.CanExecute(commandParameter, inputElement))
					{
						return Task.Run(() =>
						{
							routedCommand.Execute(commandParameter, inputElement);
							return true;
						});
					}
				}
				else if (command is IAsyncCommand asyncCommand)
				{
					if (asyncCommand.CanExecute(commandParameter))
					{
						var task = asyncCommand.ExecuteAsync(commandParameter);
						var resultTask = task as Task<bool>;
						if (resultTask is null)
						{
							return task.ContinueWith(
							(t) =>
							{
								return true;
							}
							);
						}
						else
						{
							return resultTask;
						}
					}
				}
				else if (command.CanExecute(commandParameter))
				{
					return Task.Run(() =>
					{
						command.Execute(commandParameter);
						return true;
					});
				}
			}
			return Task.FromResult(false);
		}

		public static bool ExecuteCommand(ICommand command, object parameter, IInputElement target)
		{
			RoutedCommand routedCommand = command as RoutedCommand;
			if (routedCommand != null)
			{
				if (routedCommand.CanExecute(parameter, target))
				{
					routedCommand.Execute(parameter, target);
					return true;
				}
			}
			else if (command.CanExecute(parameter))
			{
				command.Execute(parameter);
				return true;
			}
			return false;
		}

		public static async Task<bool> ExecuteCommandAsync(ICommand command, object parameter, IInputElement target)
		{
			RoutedCommand routedCommand = command as RoutedCommand;
			if (routedCommand != null)
			{
				if (routedCommand.CanExecute(parameter, target))
				{
					return await Task.Run(() =>
					{
						routedCommand.Execute(parameter, target);
						return true;
					});
				}
			}
			else if (command is IAsyncCommand asyncCommand)
			{
				if (asyncCommand.CanExecute(parameter))
				{
					await asyncCommand.ExecuteAsync(parameter).ContinueWith((t) => { return true; });
				}
			}
			else if (command.CanExecute(parameter))
			{
				return await Task.Run(() =>
				{
					command.Execute(parameter);
					return true;
				});
			}
			return await Task.FromResult(false);
		}
	}
}
