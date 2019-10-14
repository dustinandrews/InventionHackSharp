namespace CommerceSim.Systems
{
	[System.Serializable]
public class MissingComponentException : System.Exception
{
	public MissingComponentException() { }
	public MissingComponentException(string message) : base(message) { }
	public MissingComponentException(string message, System.Exception inner) : base(message, inner) { }
	protected MissingComponentException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
}