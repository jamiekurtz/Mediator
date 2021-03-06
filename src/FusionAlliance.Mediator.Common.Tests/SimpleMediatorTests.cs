﻿using System;
using NUnit.Framework;

namespace FusionAlliance.Mediator.Common.Tests
{
    [TestFixture]
    public class SimpleMediatorTests : IDisposable
    {
        private SimpleMediator _mediator;
        private static readonly TimeSpan _waitDuration = TimeSpan.FromSeconds(5);

        [SetUp]
        public void BeforeEachTest()
        {
            _mediator = new SimpleMediator();
            _mediator.Bind(typeof(IRequestHandler<DoubleInteger, DoubleIntegerReply>), typeof(DoubleIntegerHandler));
            _mediator.Bind(typeof(IRequestHandler<SayHello, SayHelloReply>), typeof(SayHelloHandler));
        }

        [Test]
        public void Async_requests_allow_the_thread_to_continue()
        {
            var request = new SayHello("World");
            var task = _mediator.RequestAsync(request);
            Assert.IsFalse(task.IsCompleted);
            task.Wait(_waitDuration);
            Assert.AreEqual("Hello, World!", task.Result.Hello);
        }

        [Test]
        public void IsDisposed_is_false_by_default()
        {
            Assert.IsFalse(_mediator.IsDisposed);
        }

        [Test]
        public void IsDisposed_is_true_after_disposing()
        {
            _mediator.Dispose();
            Assert.IsTrue(_mediator.IsDisposed);
        }

        [Test]
        public void It_can_resolve_request()
        {
            var doubleInteger = new DoubleInteger(5);
            var reply = _mediator.Request(doubleInteger);
            Assert.AreEqual(10, reply.DoubledValue);
        }

        [Test]
        public void It_throws_an_exception_when_the_request_is_null()
        {
            DoubleInteger request = null;
            Assert.Throws<ArgumentNullException>(() => _mediator.Request(request));
        }

        [Test]
        public void It_throws_an_exception_when_no_handler_can_be_found()
        {
            var squareInteger = new SquareInteger(5);
            Assert.Throws<UnableToResolveRequestException>(() => _mediator.Request(squareInteger));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _mediator.Dispose();
            }
        }
    }
}
