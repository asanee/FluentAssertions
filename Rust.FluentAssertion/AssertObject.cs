namespace Rust.FluentAssertion
{
    using System;

    public class AssertObject<T, TProperty>
    {
        public AssertObject(AssertScope<T, TProperty> scope)
        {
            Scope = scope;
        }

        public virtual ValidationScope<T, TProperty> IsTrue(Func<TProperty, bool> predicate, string assertName)
        {
            Scope.IsTrue(predicate, assertName);

            return GetValidationScope();
        }

        public virtual ValidationScope<T, TProperty> AreEqual(TProperty expected)
        {
            Scope.AreEqual(expected);

            return GetValidationScope();
        }

        public virtual ValidationScope<T, TProperty> AreNotEqual(TProperty unexpected)
        {
            Scope.AreNotEqual(unexpected);

            return GetValidationScope();
        }

        private ValidationScope<T, TProperty> GetValidationScope()
        {
            return new ValidationScope<T, TProperty>(this);
        }

        internal AssertScope<T, TProperty> Scope { get; private set; }

        public AssertObject<T, TProperty> Not
        {
            get
            {
                return new NotAssertObject(this);
            }
        }

        class NotAssertObject : AssertObject<T, TProperty>
        {
            public NotAssertObject(AssertObject<T, TProperty> baseAssertObject)
                : base(baseAssertObject.Scope)
            {
                BaseAssertObject = baseAssertObject;
            }

            public override ValidationScope<T, TProperty> IsTrue(
                Func<TProperty, bool> predicate,
                string assertName)
            {
                return base.IsTrue(x => !predicate(x), "Not" + assertName);
            }

            public override ValidationScope<T, TProperty> AreEqual(TProperty expected)
            {
                return base.AreNotEqual(expected);
            }

            public override ValidationScope<T, TProperty> AreNotEqual(TProperty unexpected)
            {
                return base.AreEqual(unexpected);
            }

            private AssertObject<T, TProperty> BaseAssertObject { get; set; }
        }
    }
}