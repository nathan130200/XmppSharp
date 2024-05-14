using XmppSharp.Expat.Native;

namespace XmppSharp.Expat;

public readonly struct ExpatFeatureInfo
{
	public FeatureType Type { get; init; }
	public string Name { get; init; }
	public uint Value { get; init; }

	public static IReadOnlyList<ExpatFeatureInfo> GetFeatures()
		=> PInvoke.GetFeatureList();
}