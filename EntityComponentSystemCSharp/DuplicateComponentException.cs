namespace CommerceSim
{
    [System.Serializable]
        public class DuplicateComponentException : System.Exception
        {
            public DuplicateComponentException() { }
            public DuplicateComponentException(string message) : base(message) { }
            public DuplicateComponentException(string message, System.Exception inner) : base(message, inner) { }
            protected DuplicateComponentException(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

}