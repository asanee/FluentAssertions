namespace Rust.FluentAssertion
{
    public class ValidationScope<T, TProperty>
    {
        public ValidationScope(AssertObject<T, TProperty> assertObject)
        {
            AssertObject = assertObject;
        }

        internal AssertObject<T, TProperty> AssertObject { get; private set; }

        public AssertScope<T, TProperty> And
        {
            get
            {
                return AssertObject.Scope;
            }
        }
    }
}