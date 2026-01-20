namespace XmppSharp.Protocol.Extensions.XEP0030;

public static partial class Identities
{
	/// <summary>
	/// The "client" category consists of different types of clients, mostly for instant messaging.
	/// </summary>
	public static class Client
	{
		const string Name = "client";

		/// <summary>
		/// An automated client that is not controlled by a human user.
		/// </summary>
		public static Identity Bot => new(Name, "bot");

		/// <summary>
		/// Minimal non-GUI client used on dumb terminals or text-only screens.
		/// </summary>
		public static Identity Console => new(Name, "console");

		/// <summary>
		/// A client running on a gaming console.
		/// </summary>
		public static Identity Game => new(Name, "game");

		/// <summary>
		/// A client running on a PDA, RIM device, or other handheld.
		/// </summary>
		public static Identity Handheld => new(Name, "handheld");

		/// <summary>
		/// Standard full-GUI client used on desktops and laptops.
		/// </summary>
		public static Identity PC => new(Name, "pc");

		/// <summary>
		/// A client running on a mobile phone or other telephony device.
		/// </summary>
		public static Identity Phone => new(Name, "phone");

		/// <summary>
		/// A client that is not actually using an instant messaging client; however, messages sent to this contact will be delivered as Short Message Service (SMS) messages.
		/// </summary>
		public static Identity Sms => new(Name, "sms");

		/// <summary>
		/// A client running on a touchscreen device larger than a smartphone and without a physical keyboard permanently attached to it.
		/// </summary>
		public static Identity Tablet => new(Name, "tablet");

		/// <summary>
		/// A client operated from within a web browser.
		/// </summary>
		public static Identity Web => new(Name, "web");
	}
}