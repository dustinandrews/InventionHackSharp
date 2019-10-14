namespace CommerceSim
{
    [System.Serializable]
    public class NotComponentException : System.Exception
    {
        public NotComponentException
    () { }
        public NotComponentException
    (string message) : base(message) { }
        public NotComponentException
    (string message, System.Exception inner) : base(message, inner) { }
        protected NotComponentException
    (
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}