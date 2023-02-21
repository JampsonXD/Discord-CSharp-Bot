using System.Threading.Channels;
using Microsoft.Extensions.Hosting;

namespace DiscordBot.Core;

public interface IMessage
{
    
}

public interface IMessageSink
{
    ValueTask Consume(IMessage message);
}

public interface ISubscriptionService
{
    ISubscriptionHandle Subscribe<T>(Action<T> action) where T: IMessage;
    void Unsubscribe(ISubscriptionHandle handle);
}

public interface ISubscriptionHandle
{
    public bool IsValid();
}

public class MessageProcessor : BackgroundService, IMessageSink, ISubscriptionService
{
    private class MessageProcessorNewActionMessage : IMessage
    {
        public required Type MessageType { get; init; }
        public required Action<IMessage> Action { get; init; }
    
        public required ulong Id { get; init; }
    }
    
    private record SubscriptionHandle : ISubscriptionHandle
    {
        public required ulong Id { get; init; }
        
        public required Type Type { get; set; }
    
        public bool IsValid()
        {
            return Id > 0;
        }
    }

    private record SubscribedActionsEntry
    {
        public required Action<IMessage> Action { get; init; }
        public ulong Id { get; init; }
    }
    
    
    private readonly Channel<IMessage> _channel;
    private readonly Dictionary<Type, List<SubscribedActionsEntry>> _subscriptionEntries;

    // Id that is given to handles for subscribed actions
    private ulong _internalHandlerId;

    public MessageProcessor()
    {
        _subscriptionEntries = new Dictionary<Type, List<SubscribedActionsEntry>>();
        _channel = Channel.CreateUnbounded<IMessage>();
    }

    public ValueTask Consume(IMessage message) => _channel.Writer.WriteAsync(message);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = await _channel.Reader.ReadAsync(stoppingToken);
                HandleReadMessage(message);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private void HandleReadMessage(IMessage message)
    {
        if (message is MessageProcessorNewActionMessage processorActionMessage)
        {
            OnMessageProcessorAcquiredNewAction(processorActionMessage);
            return;
        }
        
        if (_subscriptionEntries.TryGetValue(message.GetType(), out var list))
        {
            foreach (var entry in list)
            {
                entry.Action.Invoke(message);
            }
        }
        else
        {
            Console.WriteLine(
                $"Nothing is subscribed to listen for messages of type {message.GetType()}. Message: {message}");
        }
    }
    
    private void OnMessageProcessorAcquiredNewAction(MessageProcessorNewActionMessage message)
    {
        if (_subscriptionEntries.TryGetValue(message.MessageType, out var entries))
        {
            entries.Add(new SubscribedActionsEntry()
            {
                Action = message.Action,
                Id = message.Id
            });
            return;
        }

        _subscriptionEntries.TryAdd(message.MessageType, new List<SubscribedActionsEntry>()
        {
            new SubscribedActionsEntry()
            {
                Action = message.Action,
                Id = message.Id
            }
        });
    }

    private SubscriptionHandle CreateHandle(Type type)
    {
        return new SubscriptionHandle()
        {
            Id = ++_internalHandlerId,
            Type = type
        };
    }

    public ISubscriptionHandle Subscribe<T>(Action<T> action) where T: IMessage
    {
        // Create a new action of IMessage that wraps our passed in action
        var result = new Action<IMessage>(message => action.Invoke((T) message));

        var handle = CreateHandle(typeof(T));
        
        // Add our action to the message queue and let us handle it after the current messages are handled
        Consume(new MessageProcessorNewActionMessage()
        {
            MessageType = typeof(T),
            Action = result,
            Id = handle.Id
        });

        return handle;
    }

    public void Unsubscribe(ISubscriptionHandle handle)
    {
        if (!handle.IsValid() || handle is not SubscriptionHandle subscriptionHandle)
        {
            return;
        }

        if(_subscriptionEntries.TryGetValue(subscriptionHandle.Type, out var list))
        {
            bool shouldRemove = false;
            foreach (var entry in list)
            {
                if (entry.Id == subscriptionHandle.Id)
                {
                    list.Remove(entry);
                    if (list.Count == 0)
                    {
                        shouldRemove = true;
                    }
                    break;
                }
            }

            if (shouldRemove)
            {
                _subscriptionEntries.Remove(subscriptionHandle.Type);
            }
        }
    }
}