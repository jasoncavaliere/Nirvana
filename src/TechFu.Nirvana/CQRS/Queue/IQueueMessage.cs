﻿using System;
using TechFu.Nirvana.Configuration;

namespace TechFu.Nirvana.CQRS.Queue
{
    public interface IQueueMessage : IMessage
    {
        string Id { get; }
        int DequeueCount { get; }
        string Text { get; }
        DateTime? NextVisibleTime { get; }
        DateTime? InsertionTime { get; }
        DateTime? ExpirationTime { get; }
        NirvanaTypeDefinition MessageType { get; }
    }


    public class QueueMessage : IQueueMessage
    {
        public QueueMessage(NirvanaTypeDefinition messageType)
        {
            MessageType = messageType;
        }

        public DateTime? Created { get; protected set; }
        public string CreatedBy { get; protected set; }
        public object Body { get; protected set; }
        public string Id { get; protected set; }
        public int DequeueCount { get; protected set; }
        public string Text { get; protected set; }
        public DateTime? NextVisibleTime { get; protected set; }
        public DateTime? InsertionTime { get; protected set; }
        public DateTime? ExpirationTime { get; protected set; }
        public NirvanaTypeDefinition MessageType { get; protected set; }

        public QueueMessage SetText(string text)
        {
            Text = text;
            return this;
        }
    }
}