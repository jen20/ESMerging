using System;
using System.Collections.Generic;
using Domain;
using Events;
using NUnit.Framework;

namespace Test.Domain
{
    [TestFixture]
    public abstract class AggregateRootTestFixture<TAggregateRoot> where TAggregateRoot : AggregateRootBase
    {
        protected TAggregateRoot SubjectUnderTest;
        protected Exception CaughtException;
        protected IEnumerable<Event> ProducedEvents;

        protected virtual IEnumerable<Event> Given()
        {
            return new List<Event>();
        }

        protected virtual void TestInitialization()
        {

        }

        protected abstract void When();

        protected virtual void Finally() { }

        [SetUp]
        public void Setup()
        {
            CaughtException = new ThereWasNoExceptionButOneWasExpectedException();

            TestInitialization();

            SubjectUnderTest = (TAggregateRoot)Activator.CreateInstance(typeof(TAggregateRoot), true);
            SubjectUnderTest.LoadFromEventStream(Given());

            try
            {
                When();
                ProducedEvents = SubjectUnderTest.GetUncommittedChanges();
            }
            catch (Exception e)
            {
                CaughtException = e;
                ProducedEvents = SubjectUnderTest.GetUncommittedChanges();
            }
            finally
            {
                Finally();
            }
        }
    }
}
