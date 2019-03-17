# UniRxEventAggregator

An event aggregator for Unity that uses UniRx observables. The base class to inherit from is `PubSubMonoBehaviour`.

## Events

An event is a normal class:

    Event
    {
      int Property { get; private set }
      
      Event(int property)
      {
        Property = property;
      }
    }
    
To notify subscribers of an event:

    PubSub.Publish(new Event(100));

To subscribe to an event and call a method in response:

    Subscribe<Event>(Handler);
    ...
    Handler(Event event)
    {
      // Do something
    }
    
To subscribe to an event with a filter:

    PubSub.GetEvent<Event>().Where(e => e.Property > 50).Subscribe(Handler);
    
## BehaviourSubjects

When subscribing to a `BehaviorSubject` the latest or default value is returned. To create and register a `BehaviorSubject` observable with default values:

    Subject
    {
      int Property { get; private set; }
        
      Subject(int property)
      {
        Property = property;
      }
    }

    SomeClass
    {
      BehaviorSubject<Subject> Subject { get; private set; }
      
      Subject = new BehaviorSubject<Subject>(new Subject(100));
      Register(Subject);
    }
    
To notify subscribers that a `BehaviorSubject` has changed:

    Subject.OnNext(new Subject(50));
    
To subscribe to a `BehaviorSubject` and call a method in response to changes:

    Subscribe<Subject>(Handler);
    ...
    Handler(Subject subject)
    {
      // Do something
    }
    
