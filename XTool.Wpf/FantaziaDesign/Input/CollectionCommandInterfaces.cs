using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace FantaziaDesign.Input
{
	public interface IAddItemCommandHost
	{
		ICommand AddItemCommand { get; }
	}

	public interface IRemoveItemCommandHost
	{
		ICommand RemoveItemCommand { get; }
	}

	public interface IClearItemsCommandHost
	{
		ICommand ClearItemsCommand { get; }
	}

	public interface IAddSelfCommandHost
	{
		ICommand AddSelfCommand { get; }
	}

	public interface IRemoveSelfCommandHost
	{
		ICommand RemoveSelfCommand { get; }
	}
}
