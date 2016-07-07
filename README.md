# Nirvana

##Glue for .NET and UI applications

**Nirvana n. nir·va·na  (nîr-vä′nə, nər-)**

###1. In Buddhism, a state in which the mind, enlightened as to the illusory nature of the self, transcends all suffering and attains peace.

###2 In Hinduism - a state in which the soul, having relinquished individual attachments and recognized its identity with Brahman, escapes samsara.

###3 An ideal condition of rest, harmony, stability, or joy.



I am tired of writing the same boilerplate code over and over for CQRS - my latest side project is an Angular 2 app using WebAPI, and I've decided to open source and create a nuget package for everything I'm working on.  I had the idea for 6ish months to use ROslyn to create my endpoints, and when it all came together it was magical!

Why Nirvana?  I thought it was fitting becuase of a few things - the harmony between UI and middle .NET layer ( and separation of concerns), the simplicity and speed for adding business features, and the happiness that I get when working with it vs. everything else I've ever used.  

I am open to feedback, and will accept pull requests, but until I have a better set of tests to validate changes I won't be rolling any suggestions into the system.


Features coming:
- SignalR integration and wireup to UI ( pending on Angular 2 RTM and signlar support)
- Azure Queue  ( or any queue you want, but it's my preference) support for offloading and durability of work in commands
- Plugin for EventStore
- Ability to configure which controllers to use when standing up an endpoint.
- Ability to stand up a test endpoint ( with examples for test data)
- Ability to have an endpoint that only routes to a separate layer for processiing ( useful for external API and multi layered setups)
- Sample repository code moved into the base framework with code first integration
- Object DB support for readonly repository (used in queries)
- A seprate Angular 2 seed project that uses the output of the webapi
- A working WebAPI seed app that can be downloaded and run.


Questions? Comments? Concerns?  Feel free to shoot me a note.


 

