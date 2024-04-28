using System;
using System.Threading.Tasks;

namespace FantaziaDesign.Events
{
	public delegate Task<TResult> AsyncEventHandler<TResult>(object sender, EventArgs e);
}
