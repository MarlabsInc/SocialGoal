SocialGoal v1.0.0
================
[![alt text](http://www.marlabs.com/sites/default/files/logo.png "Marlabs")](http://www.marlabs.com)

SocialGoal is a social networking web app for socializing your goals and strategies with people. The primary objective of the SocialGoal app is to provide a learning app for building real-world web apps with ASP.NET MVC 5 and EF 6 Code First. The application architecture is inspired from [EFMVC](http://efmvc.codeplex.com/). SocialGoal is developed by [Marlabs](http://www.marlabs.com).


Technologies
------------
* ASP.NET MVC 5
* EF 6 Code First 
* AutoMapper
* Autofac
* Twitter Bootstrap
* NUnit
* Moq

Patterns & Practices
---------------------
* Domain Driven Design (DDD)
* Test-Driven Development (TDD)
* Repository Pattern & Generic Repository
* Unit of Work 
* Dependency Injection

Running the Application
-----------------------

1. Open the solution in Visual Studio 2013. Build the solution to install Nuget packages.(This will automatically restore Nuget packages. Please ensure you have Nuget version 2.7 or higher)
2. Open the web.config and change the connecting string "SocialGoalEntities" for working with your system.
3. Run the application and  register a new User. (Please note that currently the applictaion does not provided any pre-defined user. Earlier there was a pre-defined user named "Admin")

Please note that we have tested the app in Chrome browser. We have observed that a JavaScript Chart compoment is having browser compatibility issues with some versions of IE. 

Goals and Roadmap
-----------------

### Overall Project Goals

* Web app for Social Networking for soclialize your goals and strategies.
* A reference web app for ASP.NET MVC 5 and EF 6 Code First.
* Improve developer productivity for building web apps on the Microsoft Web stack.
* Reference app for building Test-Driven Development (TDD) and Domain-Driven Design (DDD).
* Mobilize an existing app for solving the mobility challenges.

### Roadmap Targets

* Build a full-fledged social networking app with enhanced UI and new features.
* Mobilize the existing app
 * Provide an API for Mobility, by using ASP.NET Web API 2.  
 * Build cross platform, minimalist mobile apps by using HTML5/JavaScript platform.
 * Build Mobile Backend as a Service (MBaaS) solution on the Windows Azure for the mobile apps. 
 
## Team

* [Shiju Varghese](http://weblogs.asp.net/shijuvarghese/) - Architect, Core Committer
* [Sharon Sudhan](https://github.com/Sharonsudhan) - Lead Developer 
* [Adarsh V.S.](https://github.com/adarsh-vs) - Lead Developer
* [Peter Kneale](https://github.com/PeterKneale) - Contributor
* [offi] (https://github.com/offi) - Contributor
 



 

