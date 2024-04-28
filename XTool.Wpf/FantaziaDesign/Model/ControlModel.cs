using System.Collections.Generic;

namespace FantaziaDesign.Model
{
	public interface IContentTemplateHost
	{
		string ContentTemplateNameKey { get; set; }
	}

	public interface IContainerTemplateHost
	{
		string ContainerTemplateNameKey { get; set; }
	}

	public interface IStyleHost
	{
		string StyleNameKey { get; set; }
	}

	public class ControlModel : NotifiableModel, IContentTemplateHost, IContainerTemplateHost, IStyleHost
	{
		public ControlModel() : base()
		{
		}

		public string StyleNameKey { get; set; }

		public string ContentTemplateNameKey { get; set; }

		public string ContainerTemplateNameKey { get; set; }

	}

	public class CollectionControlModel<TNotifiableModel> : NotifiableModelList<TNotifiableModel>, IContentTemplateHost, IContainerTemplateHost, IStyleHost where TNotifiableModel : INotifiableModel
	{
		private int m_selectedIndex = -1;

		public CollectionControlModel() : base()
		{
		}

		public CollectionControlModel(int capacity) : base(capacity)
		{
		}

		public CollectionControlModel(IEnumerable<TNotifiableModel> collection) : base(collection)
		{
		}

		public CollectionControlModel(IList<TNotifiableModel> items) : base(items)
		{
		}

		public string StyleNameKey { get; set; }

		public string ContentTemplateNameKey { get; set; }

		public string ContainerTemplateNameKey { get; set; }

		public int SelectedIndex
		{
			get => m_selectedIndex;
			set
			{
				var oldIndex = m_selectedIndex;
				if (this.SetPropertyIfChanged(ref m_selectedIndex, value, nameof(SelectedIndex)))
				{
					OnSelectedIndexChanged(oldIndex, m_selectedIndex);
				}
			}
		}

		protected virtual void OnSelectedIndexChanged(int oldIndex, int newIndex)
		{

		}
	}


}
