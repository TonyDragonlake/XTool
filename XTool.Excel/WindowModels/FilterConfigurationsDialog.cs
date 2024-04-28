using System;
using FantaziaDesign.Wpf.Message;

namespace XTool.Excel.WindowModels
{
	public sealed class FilterConfigurationsDialog : AcceptCancelDialog
	{
		private readonly FilterConfigurationsManager m_configsManager;

		public FilterConfigurationsDialog(FilterConfigurationsManager configsManager) : base()
		{
			m_configsManager = configsManager ?? throw new ArgumentNullException(nameof(configsManager));
			ContentTemplateNameKey = "FilterConfigurationsDialogTemplate";
		}

		public FilterConfigurationsManager ConfigurationsManager => m_configsManager;

		protected override void ExecuteAcceptCommand(object parameter)
		{
			base.ExecuteAcceptCommand(parameter);
			m_configsManager.ApplySelection();
		}
	}
}
