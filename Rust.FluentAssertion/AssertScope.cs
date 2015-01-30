namespace Rust.FluentAssertion
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class AssertScope<T, TProperty>
    {
        public AssertScope(T subject, Expression<Func<T, TProperty>> expression, string variableName, Signature signature)
        {
            Subject = subject;
            Expression = expression;
            VariableName = variableName;
            Signature = signature;

            if (!string.IsNullOrWhiteSpace(variableName))
            {
                AttrachedVariableName = " On " + variableName + " ";
            }
            else
            {
                AttrachedVariableName = string.Empty;
            }
        }

        public virtual void IsTrue(Func<TProperty, bool> predicate, string assertName)
        {
            var actual = GetActual();

            IsTrue(predicate(actual), "Assert.{1} failed{2}. {3} {0}", Signature.Name, assertName, AttrachedVariableName, Signature.SignatureType);

            Trace.WriteLine(
                string.Format("Assert.{1} succeeded{3}. Type:{2}, {4}:{0}", Signature.Name, assertName, typeof(T).Name, AttrachedVariableName, Signature.SignatureType));
        }

        public virtual void AreNotEqual(TProperty notExpected)
        {
            var actual = GetActual();

            AreNotEqual(
                notExpected,
                actual,
                "Assert.AreNotEqual failed{3}. {4} {0} NotExpected:<{1}> . Actual:<{2}>.",
                Signature.Name,
                notExpected,
                actual,
                AttrachedVariableName,
                Signature.SignatureType);

            Trace.WriteLine(string.Format(
                "Assert.AreNotEqual succeeded{3}. {4} {0} NotExpected:<{1}> . Actual:<{2}>.",
                Signature.Name,
                notExpected,
                actual,
                AttrachedVariableName,
                Signature.SignatureType));
        }

        public virtual void AreEqual(TProperty expected)
        {
            var actual = GetActual();

            AreEqual(
                expected,
                actual,
                "Assert.AreEqual failed{3}. {4} {0} Expected:<{1}> . Actual:<{2}>.",
                Signature.Name,
                expected,
                actual,
                AttrachedVariableName,
                Signature.SignatureType);

            Trace.WriteLine(string.Format(
                "Assert.AreEqual succeeded{3}. {4} {0} Expected:<{1}> . Actual:<{2}>.",
                Signature.Name,
                expected,
                actual,
                AttrachedVariableName,
                Signature.SignatureType));
        }

        protected void IsTrue(bool isTrue, string message, params object[] parameters)
        {
            if (isTrue)
            {
                return;
            }

            throw new AssertFailedException(string.Format(message, parameters));
        }

        protected void AreNotEqual(TProperty expected, TProperty actual, string message, params object[] parameters)
        {
            if (!Equals(expected, actual))
            {
                return;
            }

            throw new AssertFailedException(string.Format(message, parameters));
        }

        protected void AreEqual(TProperty expected, TProperty actual, string message, params object[] parameters)
        {
            if (Equals(expected, actual))
            {
                return;
            }

            throw new AssertFailedException(string.Format(message, parameters));
        }

        public AssertObject<T, TProperty> Should()
        {
            return new AssertObject<T, TProperty>(this);
        }

        public T Subject { get; private set; }

        public Expression<Func<T, TProperty>> Expression { get; private set; }

        protected internal TProperty GetActual()
        {
            if (_isProceeded)
            {
                return _actual;
            }

            try
            {
                _actual = Expression.Compile()(Subject);
                _isProceeded = true;
            }

            catch
            {
                Trace.WriteLine(string.Format("Failed invoke {0} {1}.", Signature.SignatureType, ExpressionName));

                throw;
            }

            return _actual;
        }

        private bool _isProceeded;

        private TProperty _actual;

        protected internal string ExpressionName
        {
            get
            {
                return Signature.Name + AttrachedVariableName;
            }
        }

        public string VariableName { get; private set; }

        public string AttrachedVariableName { get; private set; }

        public Signature Signature { get; private set; }
    }
}