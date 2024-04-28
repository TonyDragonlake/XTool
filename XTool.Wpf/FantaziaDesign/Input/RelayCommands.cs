using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

// ICommand interface is framework independence for using Attribute in System.dll : 
// [TypeForwardedFrom("PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]

namespace FantaziaDesign.Input
{
	public interface IAsyncCommand : ICommand
	{
		Task ExecuteAsync(object parameter);
	}

	public class RelayCommand : ICommand
	{
		protected SynchronizationContext m_synchronizationContext;
		protected Delegate m_canExecuteDelegate;
		protected Delegate m_executeDelegate;
		protected object[] m_additionalParams;
		protected int m_paramsCount;
		protected bool m_alwaysCanExecute;

		protected RelayCommand()
		{
			m_synchronizationContext = SynchronizationContext.Current;
		}

		public SynchronizationContext SynchronizationContext => m_synchronizationContext;

		public object[] AdditionalParams { get => m_additionalParams; set => m_additionalParams = value; }

		public RelayCommand(Func<object, object[], bool> canExecute, Action<object, object[]> execute) : this()
		{
			m_canExecuteDelegate = canExecute;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
			m_paramsCount = 2;
		}

		public RelayCommand(Func<object, bool> canExecute, Action<object> execute) : this()
		{
			m_canExecuteDelegate = canExecute;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
			m_paramsCount = 1;
		}

		public RelayCommand(Func<bool> canExecute, Action execute) : this()
		{
			m_canExecuteDelegate = canExecute;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
		}

		public RelayCommand(Action<object, object[]> execute) : this()
		{
			m_alwaysCanExecute = true;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
			m_paramsCount = 2;
		}

		public RelayCommand(Action<object> execute) : this()
		{
			m_alwaysCanExecute = true;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
			m_paramsCount = 1;
		}

		public RelayCommand(Action execute) : this()
		{
			m_alwaysCanExecute = true;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
		}

		protected virtual void OnCanExecuteChanged()
		{
			var handler = CanExecuteChanged;
			if (handler != null)
			{
				if (m_synchronizationContext != null && m_synchronizationContext != SynchronizationContext.Current)
				{
					m_synchronizationContext.Post((o) => handler.Invoke(this, EventArgs.Empty), null);
				}
				else
				{
					handler.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public virtual event EventHandler CanExecuteChanged;

		public virtual bool CanExecute(object parameter)
		{
			if (m_alwaysCanExecute)
			{
				return true;
			}

			switch (m_paramsCount)
			{
				case 1:
					return CanExecuteSingleParams(parameter);
				case 2:
					return CanExecuteMultipleParams(parameter);
				default:
					return CanExecuteEmptyParams();
			}
		}

		private bool CanExecuteEmptyParams()
		{
			var canExecute = m_canExecuteDelegate as Func<bool>;
			return (canExecute?.Invoke()).GetValueOrDefault();
		}

		private bool CanExecuteSingleParams(object parameter)
		{
			var canExecute1 = m_canExecuteDelegate as Func<object, bool>;
			return (canExecute1?.Invoke(parameter)).GetValueOrDefault();
		}

		private bool CanExecuteMultipleParams(object parameter)
		{
			var canExecute2 = m_canExecuteDelegate as Func<object, object[], bool>;
			return (canExecute2?.Invoke(parameter, m_additionalParams)).GetValueOrDefault();
		}

		public virtual void Execute(object parameter)
		{
			switch (m_paramsCount)
			{
				case 1:
					ExecuteSingleParams(parameter);
					break;
				case 2:
					ExecuteMultipleParams(parameter);
					break;
				default:
					ExecuteEmptyParams();
					break;
			}
		}

		private void ExecuteEmptyParams()
		{
			var execute = m_executeDelegate as Action;
			execute?.Invoke();
		}

		private void ExecuteSingleParams(object parameter)
		{
			var execute = m_executeDelegate as Action<object>;
			execute?.Invoke(parameter);
		}

		private void ExecuteMultipleParams(object parameter)
		{
			var execute = m_executeDelegate as Action<object, object[]>;
			execute?.Invoke(parameter, m_additionalParams);
		}

	}

	public class RelayAsyncCommand : RelayCommand, IAsyncCommand
	{
		protected CancellationTokenSource m_tokenSource;

		protected bool m_isAsyncExecuteDelegate;

		protected Func<object, Task> m_asyncExecuteDelegate;

		public RelayAsyncCommand(Func<object, object[], bool> canExecute, Action<CancellationToken, object, object[]> execute) : base()
		{
			m_canExecuteDelegate = canExecute;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
			m_paramsCount = 2;
		}

		public RelayAsyncCommand(Func<object, bool> canExecute, Action<CancellationToken, object> execute) : base()
		{
			m_canExecuteDelegate = canExecute;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
			m_paramsCount = 1;
		}

		public RelayAsyncCommand(Func<bool> canExecute, Action<CancellationToken> execute) : base()
		{
			m_canExecuteDelegate = canExecute;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
		}

		public RelayAsyncCommand(Func<object, object[], bool> canExecute, Func<CancellationToken, object, object[], Task> execute) : base()
		{
			m_canExecuteDelegate = canExecute;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
			m_paramsCount = 2;
			m_isAsyncExecuteDelegate = true;
		}

		public RelayAsyncCommand(Func<object, bool> canExecute, Func<CancellationToken, object, Task> execute) : base()
		{
			m_canExecuteDelegate = canExecute;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
			m_paramsCount = 1;
			m_isAsyncExecuteDelegate = true;
		}

		public RelayAsyncCommand(Func<bool> canExecute, Func<CancellationToken, Task> execute) : base()
		{
			m_canExecuteDelegate = canExecute;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
			m_isAsyncExecuteDelegate = true;
		}

		public RelayAsyncCommand(Action<CancellationToken, object, object[]> execute) : base()
		{
			m_alwaysCanExecute = true;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
			m_paramsCount = 2;
		}

		public RelayAsyncCommand(Action<CancellationToken, object> execute) : base()
		{
			m_alwaysCanExecute = true;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
			m_paramsCount = 1;
		}

		public RelayAsyncCommand(Action<CancellationToken> execute) : base()
		{
			m_alwaysCanExecute = true;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
		}

		public RelayAsyncCommand(Func<CancellationToken, object, object[], Task> execute) : base()
		{
			m_alwaysCanExecute = true;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
			m_paramsCount = 2;
			m_isAsyncExecuteDelegate = true;
		}

		public RelayAsyncCommand(Func<CancellationToken, object, Task> execute) : base()
		{
			m_alwaysCanExecute = true;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
			m_paramsCount = 1;
			m_isAsyncExecuteDelegate = true;
		}

		public RelayAsyncCommand(Func<CancellationToken, Task> execute) : base()
		{
			m_alwaysCanExecute = true;
			if (execute is null)
			{
				return;
			}
			m_executeDelegate = execute;
			m_isAsyncExecuteDelegate = true;
		}

		public CancellationTokenSource CancellationTokenSource => m_tokenSource;

		public override bool CanExecute(object parameter)
		{
			ResetToken();
			if (base.CanExecute(parameter))
			{
				m_tokenSource = new CancellationTokenSource();
				return true;
			}
			return false;
		}

		public override async void Execute(object parameter)
		{
			await ExecuteAsync(parameter);
		}

		public Task ExecuteAsync(object parameter)
		{
			if (m_tokenSource == null)
			{
				return null;
			}
			if (m_isAsyncExecuteDelegate)
			{
				switch (m_paramsCount)
				{
					case 1:
						return ExecuteSingleParamsAsync(parameter);
					case 2:
						return ExecuteMultipleParamsAsync(parameter);
					default:
						return ExecuteEmptyParamsAsync();
				}
			}
			else
			{
				switch (m_paramsCount)
				{
					case 1:
						return Task.Factory.StartNew(ExecuteSingleParams, parameter);
					case 2:
						return Task.Factory.StartNew(ExecuteMultipleParams, parameter);
					default:
						return Task.Factory.StartNew(ExecuteEmptyParams);
				}
			}
		}

		private void ExecuteEmptyParams()
		{
			var execute = m_executeDelegate as Action<CancellationToken>;
			execute?.Invoke(m_tokenSource.Token);
		}

		private void ExecuteSingleParams(object parameter)
		{
			var execute = m_executeDelegate as Action<CancellationToken, object>;
			execute?.Invoke(m_tokenSource.Token, parameter);
		}

		private void ExecuteMultipleParams(object parameter)
		{
			var execute = m_executeDelegate as Action<CancellationToken, object, object[]>;
			execute?.Invoke(m_tokenSource.Token, parameter, m_additionalParams);
		}

		private Task ExecuteEmptyParamsAsync()
		{
			var execute = m_executeDelegate as Func<CancellationToken, Task>;
			return execute?.Invoke(m_tokenSource.Token);
		}

		private Task ExecuteSingleParamsAsync(object parameter)
		{
			var execute = m_executeDelegate as Func<CancellationToken, object, Task>;
			return execute?.Invoke(m_tokenSource.Token, parameter);
		}

		private Task ExecuteMultipleParamsAsync(object parameter)
		{
			var execute = m_executeDelegate as Func<CancellationToken, object, object[], Task>;
			return execute?.Invoke(m_tokenSource.Token, parameter, m_additionalParams);
		}

		public void ResetToken()
		{
			if (m_tokenSource != null)
			{
				m_tokenSource.Dispose();
				m_tokenSource = null;
			}
		}

		public void Cancel()
		{
			m_tokenSource?.Cancel();
		}
	}

}
