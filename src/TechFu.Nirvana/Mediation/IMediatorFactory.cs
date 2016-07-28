﻿using System;
using TechFu.Nirvana.Configuration;
using TechFu.Nirvana.CQRS;
using TechFu.Nirvana.Util.Extensions;

namespace TechFu.Nirvana.Mediation
{
    public enum MediatorStrategy
    {
        ForwardToWeb,
        HandleInProc,
        ForwardToQueue,
    }

    public interface IMediatorFactory
    {
        IMediator GetMediator(Type messageType);
        CommandResponse<TResult> Command<TResult>(Command<TResult> command);
        QueryResponse<TResult> Query<TResult>(Query<TResult> query);
        UIEventResponse Notification<TResult>(UiEvent<TResult> uiNotification);
        InternalEventResponse InternalEvent(InternalEvent internalEvent);


    }

    public class MediatorFactory : IMediatorFactory
    {
        public IMediator GetMediator(Type messageType)
        {
            var mediatorStrategy = GetMediatorStrategy(messageType);
            return GetMediatorByStrategy(mediatorStrategy);
        }

        public CommandResponse<TResult> Command<TResult>(Command<TResult> command)
        {
            return GetMediator(command.GetType()).Command(command);
        }

        public QueryResponse<TResult> Query<TResult>(Query<TResult> query)
        {
            return GetMediator(query.GetType()).Query(query);
        }

        public UIEventResponse Notification<TResult>(UiEvent<TResult> uiNotification)
        {
            return GetMediator(uiNotification.GetType()).UiNotification(uiNotification);
        }
        public InternalEventResponse InternalEvent(InternalEvent internalEvent)
        {
            return GetMediator(internalEvent.GetType()).InternalEvent(internalEvent);
        }

        private IMediator GetMediatorByStrategy(MediatorStrategy mediatorStrategy)
        {
            if (mediatorStrategy == MediatorStrategy.ForwardToWeb)
            {
                return GetWebMediator();
            }
            if (mediatorStrategy == MediatorStrategy.ForwardToQueue)
            {
                return GetQueueMediator();
            }
            return GetInProcMediator();
        }

        private MediatorStrategy GetMediatorStrategy(Type messageType)
        {
            if (messageType.IsQuery() 
                || ( messageType.IsUiNotification() && NirvanaSetup.IsInProcess(TaskType.UiNotification))
                || ( messageType.IsCommand() && NirvanaSetup.IsInProcess(TaskType.UiNotification))
                || ( messageType.IsInternalEvent() && NirvanaSetup.IsInProcess(TaskType.InternalEvent))
                )
            {
                // Only commands can be offloaded currently
                return MediatorStrategy.HandleInProc;
            }

            if (
                 (messageType.IsUiNotification() && NirvanaSetup.ShouldForwardToWeb(TaskType.UiNotification))
                || (messageType.IsCommand() && NirvanaSetup.ShouldForwardToWeb(TaskType.UiNotification))
                || (messageType.IsInternalEvent() && NirvanaSetup.ShouldForwardToWeb(TaskType.InternalEvent))
               )
            {
                return MediatorStrategy.ForwardToWeb;
            }

            if (
                (messageType.IsUiNotification() && NirvanaSetup.ShouldForwardToQueue(TaskType.UiNotification))
                || (messageType.IsCommand() && NirvanaSetup.ShouldForwardToQueue(TaskType.UiNotification))
                || (messageType.IsInternalEvent() && NirvanaSetup.ShouldForwardToQueue(TaskType.InternalEvent))
                )
            {
                return MediatorStrategy.ForwardToQueue;
            }
            throw new NotImplementedException("Currently all children must be handed in proc in synchronously.");


        }



        private IMediator GetWebMediator()
        {
            return (IWebMediator) NirvanaSetup.GetService(typeof(IWebMediator));
        }

        private IMediator GetQueueMediator()
        {
            return (IQueueMediator) NirvanaSetup.GetService(typeof(IQueueMediator));
        }
        private static IMediator GetInProcMediator()
        {
            return (IMediator) NirvanaSetup.GetService(typeof(IMediator));
        }
    }
}