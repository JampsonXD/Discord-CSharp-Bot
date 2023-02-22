using System.Threading.Channels;
using Microsoft.Extensions.Hosting;

namespace DiscordBot.Core;

/// <summary>

/// Represents a message with different types of data sent to a message sink

/// </summary>

public interface IMessage
{
    
}

/// <summary>

/// Represents an object that collects messages

/// </summary>

public interface IMessageSink
{
    /// <summary>
    /// Processes passed in messages
    /// </summary>
    /// <param name="message">The message</param>
    /// <returns>An awaitable result for an async operation</returns>
    ValueTask Process(IMessage message);
}

/// <summary>

/// Represents a subscription service that can register and unregister events for different message types

/// </summary>

public interface ISubscriptionService
{
    /// <summary>
    /// Subscribes a callback action to be called when a message of the same type is broadcast
    /// </summary>
    /// <typeparam name="TMessage">The Type of message and callback action</typeparam>
    /// <param name="action">The callback action</param>
    /// <returns>A <see cref="ISubscriptionHandle"/> that holds a reference to the subscription entry that can be used to <see cref="Unsubscribe"/></returns>
    ISubscriptionHandle Subscribe<TMessage>(Action<TMessage> action) where TMessage: IMessage;
    /// <summary>
    /// De-registers a subscribed action
    /// </summary>
    /// <param name="handle">The handle holding a reference to the subscribed action</param>
    void Unsubscribe(ISubscriptionHandle handle);
}

/// <summary>

/// Represents a handle to a subscribed action for a <see cref="ISubscriptionService"/>

/// </summary>

public interface ISubscriptionHandle
{
    /// <summary>
    /// Describes whether this handle points to a valid subscription action
    /// </summary>
    /// <returns>Whether the handle is valid or invalid</returns>
    public bool IsValid();
}

/// <summary>

/// Represents a Message Processing Service that allows providers to provide <see cref="IMessage"/> and consumers to consume <see cref="IMessage"/>

/// </summary>

/// <seealso cref="BackgroundService"/>

/// <seealso cref="IMessageSink"/>

/// <seealso cref="ISubscriptionService"/>

public class MessageProcessor : BackgroundService, IMessageSink, ISubscriptionService
{
    /// <summary>
    /// Represents a message that contains information on a newly requested subscription entry.
    /// </summary>
    /// <seealso cref="IMessage"/>
    private class MessageProcessorNewActionMessage : IMessage
    {
        /// <summary>
        /// The type of message to be subscribed to.
        /// </summary>
        public required Type MessageType { get; init; }
        /// <summary>
        /// An action to call of the generic <see cref="IMessage"/> type.
        /// </summary>
        public required Action<IMessage> Action { get; init; }
    
        /// <summary>
        /// The ID to use for the subscription entry that correlates to the <see cref="SubscriptionHandle"/> ID.
        /// </summary>
        public required ulong Id { get; init; }
    }
    
    /// <summary>
    /// Concrete Type for a <see cref="MessageProcessor"/> subscription handle.
    /// </summary>
    private record SubscriptionHandle : ISubscriptionHandle
    {
        /// <summary>
        /// The ID of the handle.
        /// </summary>
        public required ulong Id { get; init; }
        
        /// <summary>
        /// The type of action the subscription entry was subscribed to.
        /// </summary>
        public required Type Type { get; set; }
        
        public bool IsValid()
        {
            return Id > 0;
        }
    }

    /// <summary>
    /// Represents a subscription information entry in a list.
    /// </summary>
    private record SubscribedActionsEntry
    {
        /// <summary>
        /// An action to be called.
        /// </summary>
        public required Action<IMessage> Action { get; init; }
        /// <summary>
        /// The ID of the entry.
        /// </summary>
        public required ulong Id { get; init; }
    }
    
    
    /// <summary>
    /// Channel that is used for awaiting new message requests and writing requests.
    /// </summary>
    private readonly Channel<IMessage> _channel;
    /// <summary>
    /// Dictionary containing subscription entries subscribed to the key type
    /// </summary>
    private readonly Dictionary<Type, List<SubscribedActionsEntry>> _subscriptionEntries;
    
    /// <summary>
    /// An incrementing counter for new subscription handle ids to be created.
    /// </summary>
    private ulong _internalHandlerId;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
    /// </summary>
    public MessageProcessor()
    {
        _subscriptionEntries = new Dictionary<Type, List<SubscribedActionsEntry>>();
        _channel = Channel.CreateUnbounded<IMessage>();
    }

    /// <summary>
    /// Writes <see cref="IMessage"/> to the channel to be processed.
    /// </summary>
    /// <param name="message">The message to be processed</param>
    /// <returns>An awaitable result for an async operation</returns>
    public ValueTask Process(IMessage message) => _channel.Writer.WriteAsync(message);

    /// <summary>
    /// Service loop that awaits incoming <see cref="Process"/> calls and handles calling subscribed actions.
    /// </summary>
    /// <param name="stoppingToken">The stopping token</param>
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

    /// <summary>
    /// Handles passed in <see cref="IMessage"/> and calls needed subscription actions.
    /// </summary>
    /// <param name="message">The inbound message to be processed.</param>
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
    
    /// <summary>
    /// Handles creating new subscription entries when <see cref="OnMessageProcessorAcquiredNewAction"/> messages are consumed.
    /// </summary>
    /// <param name="message">The message</param>
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

    /// <summary>
    /// Creates a subscription handle pertaining to a <see cref="SubscribedActionsEntry"/>.
    /// </summary>
    /// <param name="type">The type of subscription entry.</param>
    /// <returns>A subscription handle to the subscription entry.</returns>
    private SubscriptionHandle CreateHandle(Type type)
    {
        return new SubscriptionHandle()
        {
            Id = ++_internalHandlerId,
            Type = type
        };
    }

    /// <summary>
    /// Subscribes actions to the <see cref="MessageProcessor"/> to call when messages of the same type are processed.
    /// </summary>
    /// <typeparam name="TMessage">The message and action type.</typeparam>
    /// <param name="action">The action to be called.</param>
    /// <returns>A handle to the subscription entry.</returns>
    public ISubscriptionHandle Subscribe<TMessage>(Action<TMessage> action) where TMessage: IMessage
    {
        // Create a new action of IMessage that wraps our passed in action
        var result = new Action<IMessage>(message => action.Invoke((TMessage) message));

        var handle = CreateHandle(typeof(TMessage));
        
        // Add our action to the message queue and let us handle it after the current messages are handled
        Process(new MessageProcessorNewActionMessage()
        {
            MessageType = typeof(TMessage),
            Action = result,
            Id = handle.Id
        });

        return handle;
    }

    /// <summary>
    /// Unsubscribes any actions related to the <see cref="SubscriptionHandle"/> that is passed is.
    /// </summary>
    /// <param name="handle">The handle</param>
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