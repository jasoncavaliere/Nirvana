﻿using System;
using Microsoft.AspNet.SignalR;
using TechFu.Nirvana.Configuration;
using TechFu.Nirvana.CQRS;
using TechFu.Nirvana.CQRS.UiNotifications;
using TechFu.Nirvana.WebApi.Controllers;

namespace TechFu.Nirvana.SignalRNotifications
{
    public abstract class ApiControllerWithHub<THub> : CommandQueryApiControllerBase
        where THub : UiNotificationHub
    {
        public readonly Lazy<IHubContext> _hub =
            new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<THub>());

        protected IHubContext Hub => _hub.Value;

        protected string GroupName(string instance)
        {
            return  typeof(THub).Name + ":" + instance;
        }

        protected void PublishEvent(string eventName, UiEvent task,string channelName)
        {
            var channelEvent = new ChannelEvent()
            {
                ChannelName = Constants.TaskChannel,
                Name = eventName,
                Data = task,
                AggregateRoot = task.AggregateRoot,
                
            };
            Hub.Clients.Group(channelName).OnEvent(Constants.TaskChannel, channelEvent);
        }

    }
}