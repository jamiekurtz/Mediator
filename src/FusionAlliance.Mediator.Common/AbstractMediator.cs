﻿using System;
using System.Threading.Tasks;

namespace FusionAlliance.Mediator.Common
{
    public abstract class AbstractMediator : IMediator
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            Dispose(true);
        }

        public TReply Request<TReply>(IRequest<TReply> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            Type requestType = request.GetType();
            Type replyType = typeof(TReply);
            try
            {
                RequestHandlerInfo handler = GetRequestHandler(typeof(IRequestHandler<,>), requestType, replyType);
                object instance = GetInstanceOfHandler(handler.GenericType);
                return InvokeHandlerMethod<TReply>(handler, instance, request);
            }
            catch (Exception e)
            {
                var message = string.Format("Unable to resolve request.\nRequested type: {0}\nRequest: {1}", typeof(IRequest<TReply>), request);
                throw new UnableToResolveRequestException(message, e);
            }
        }

        public Task<TReply> RequestAsync<TReply>(IRequest<TReply> request)
        {
            return Task.FromResult(Request(request));// new Task<TReply>(() =>
        }

        protected virtual RequestHandlerInfo GetRequestHandler(Type requestHandlerType, Type requestType, Type replyType, string methodName = "Handle")
        {
            var genericType = requestHandlerType.MakeGenericType(requestType, replyType);
            var method = genericType.GetMethod(methodName);
            return new RequestHandlerInfo
            {
                GenericType = genericType,
                Method = method
            };
        }

        protected abstract object GetInstanceOfHandler(Type type);

        protected virtual TReply InvokeHandlerMethod<TReply>(RequestHandlerInfo handlerInfo, object instance, params object[] parameters)
        {
            var result = handlerInfo.Method.Invoke(instance, parameters);
            return (TReply)result;
        }

        protected virtual void OnDisposing()
        {
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                OnDisposing();
                IsDisposed = true;
            }
        }
    }
}
