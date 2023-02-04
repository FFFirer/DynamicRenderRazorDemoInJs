using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DynamicRazorRender.Server.Events
{
    public class Subscription
    {
        public Subscription(string eventName, string subscriptionId)
        {
            this.EventName = eventName;
            this.SubscriptionId = subscriptionId;
        }

        public string EventName { get; set; }
        public string SubscriptionId { get; set; }
    }

    public class EventBus<TEventArgs>
    {
        // private readonly ConcurrentDictionary<string, ConcurrentDictionary<TSubscriptorId, Action<TEventArgs>>> _subscriptions;

        private readonly ConcurrentDictionary<string, List<string>> _subscriptions;

        private readonly ConcurrentDictionary<string, Action<TEventArgs>> _subscriptionHandlers;

        private readonly ILogger<EventBus<TEventArgs>> _logger;

        public EventBus(ILogger<EventBus<TEventArgs>> logger)
        {
            // _subscriptions = new ConcurrentDictionary<string, ConcurrentDictionary<TSubscriptorId, Action<TEventArgs>>>();
            _subscriptions = new ConcurrentDictionary<string, List<string>>();
            _subscriptionHandlers = new ConcurrentDictionary<string, Action<TEventArgs>>();
            _logger = logger;
        }

        public void Emit(string eventName, TEventArgs eArgs)
        {
            if (_subscriptions.TryGetValue(eventName, out var subscribers))
            {
                foreach (var subscriber in subscribers)
                {
                    if (_subscriptionHandlers.TryGetValue(subscriber, out var eventHandler))
                    {
                        if (eventHandler != null)
                        {
                            _logger.LogInformation($"[Emit] {eventName} {subscriber}");
                            eventHandler.Invoke(eArgs);
                        }
                    }
                }
            }
        }

        public void EmitAsync(string eventName, TEventArgs eArgs)
        {
            Task.Factory.StartNew(() =>
            {
                this.Emit(eventName, eArgs);
            }).ConfigureAwait(false);
        }

        public Subscription Subscribe(string eventName, Action<TEventArgs> eventHandler)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentException("事件名不能为空", nameof(eventName));
            }

            var subscriptionId = $"{eventName}-{Guid.NewGuid()}";

            if (_subscriptionHandlers.TryGetValue(subscriptionId, out var v))
            {
                throw new InvalidOperationException($"重复的订阅Id");
            }


            if (!_subscriptionHandlers.TryAdd(subscriptionId, eventHandler))
            {
                throw new InvalidOperationException("注册事件失败");
            }

            var subscribers = _subscriptions.GetOrAdd(eventName, (arg) => new List<string>() { });

            subscribers.Add(subscriptionId);

            _logger.LogInformation($"[Subscribe] {eventName} {subscriptionId}");

            return new Subscription(eventName, subscriptionId);
        }

        public void UnSubscribe(Subscription subscription)
        {
            _subscriptionHandlers.Remove(subscription.SubscriptionId, out var _);

            if (_subscriptions.TryGetValue(subscription.EventName, out var subscribers))
            {
                subscribers.Remove(subscription.SubscriptionId);
            }

            _logger?.LogInformation($"[UnSubscribe] {subscription.EventName} {subscription.SubscriptionId}");
        }

        // public async Task PushAsync(string name, TEventArgs entity)
        // {


        //     await Task.CompletedTask;
        // }
    }
}