#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace DLQ
{
    #region using

    #endregion

    internal interface DeadLetterQueueManager
    {
        DeadLetterQueue<T> CreateQueue<T>(string name);
        void DeleteQueue(string name);
    }

    internal interface DeadLetterQueue<T>
    {
        void PutTask(T task);
    }

    internal interface EventHandler<T>
        where T : Event
    {
        void Handle(T @event);
    }

    internal interface EventSource<T>
        where T : Event
    {
        IList<T> Events(string lastEventId);
    }

    internal interface StateStorage
    {
        void SaveState(string lastEventId);

        string GetState();
    }

    internal class Event
    {
        public string Id { get; set; }
        public string EntityId { get; set; }
    }

    internal class Daemon<T>
        where T : Event
    {
        private const int MaxQueueCount = 10;

        private readonly Dictionary<string, DeadLetterQueue<T>> deadLetterQueues =
            new Dictionary<string, DeadLetterQueue<T>>();

        private readonly object dlqLock = new object();
        private readonly DeadLetterQueueManager dlqManager;
        private readonly EventHandler<T> eventHandler;
        private readonly EventSource<T> eventSource;
        private readonly StateStorage stateStorage;

        public Daemon(EventHandler<T> eventHandler, EventSource<T> eventSource,
            StateStorage stateStorage, DeadLetterQueueManager dlqManager)
        {
            this.eventHandler = eventHandler;
            this.eventSource = eventSource;
            this.stateStorage = stateStorage;
            this.dlqManager = dlqManager;
        }

        private bool IsRunBlock
        {
            get
            {
                lock (dlqLock)
                {
                    return deadLetterQueues.Count > MaxQueueCount;
                }
            }
        }


        public void Run()
        {
            Task.Run(() =>
            {
                while (Thread.CurrentThread.IsAlive)
                {
                    if (IsRunBlock)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        continue;
                    }

                    var @event = GetNextEvent(stateStorage.GetState());
                    if (@event != null)
                        TryHandleEvent(@event);
                }
            });
        }

        private void TryHandleEvent(T @event)
        {
            var dlqName = GetQueueName(@event);
            if (!CheckEntityBlock(@event, dlqName))
                try
                {
                    eventHandler.Handle(@event);
                }
                catch
                {
                    CreateQueueAndPutTask(@event, dlqName);
                }

            stateStorage.SaveState(@event.Id);
        }

        private bool CheckEntityBlock(T @event, string dlqName)
        {
            lock (dlqLock)
            {
                if (!deadLetterQueues.ContainsKey(dlqName))
                    return false;

                deadLetterQueues[dlqName].PutTask(@event);
                return true;
            }
        }

        private string GetQueueName(T @event)
        {
            return $"DLQ.{@event.EntityId}";
        }

        private void CreateQueueAndPutTask(T @event, string dlqName)
        {
            lock (dlqLock)
            {
                deadLetterQueues[dlqName] = dlqManager.CreateQueue<T>(dlqName);
                deadLetterQueues[dlqName].PutTask(@event);
            }
        }

        private T GetNextEvent(string lastState)
        {
            var events = eventSource.Events(lastState);
            return events.FirstOrDefault();
        }

        public void DlqWasCleared(string dlqName)
        {
            lock (dlqLock)
            {
                deadLetterQueues.Remove(dlqName);
                dlqManager.DeleteQueue(dlqName);
            }
        }
    }
}