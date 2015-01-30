namespace Rust.FluentAssertion
{
    public class Signature
    {
        public Signature(SignatureType type, string name)
        {
            Name = name;
            SignatureType = type;
        }

        public SignatureType SignatureType { get; private set; }

        /// <summary>
        /// Property Name or Method Name
        /// </summary>
        public string Name { get; private set; }
    }
}