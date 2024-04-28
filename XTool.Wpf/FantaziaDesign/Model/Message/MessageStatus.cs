namespace FantaziaDesign.Model.Message
{
	public enum MessageStatus : byte
	{
		Unknown,
		RequestOpen,
		RequestCancelOpen,
		Opened,
		RequestClose,
		RequestCancelClose,
		Closing,
		Closed
	}

}
