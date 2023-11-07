> [!NOTE]
> This project is no longer maintained

# Manhole

The Manhole package provides a simple REPL into a running ASP.NET application.

The parsing, compilation and execution of statements is provided by the [CSharp assembly from the Mono-project](http://www.mono-project.com/CSharp_Compiler), the client side magic is from the [jQuery Terminal](http://terminal.jcubic.pl/) project.

## Installation
Begin by installing the Nuget package

```
PM> install-package manhole.web
```

This will bring in the base Manhole package add then necessary assemblies to your project. It will also create a `App_Start/ManholeStart.cs` file in your project which can be used to configure the manhole.

Compile and run the project, and then simply go to `http://yourapp.com/manhole/` and go nuts.

## Configuration
There are currently only two proper configuration options available

### Route name
You can change the route of the handler by adjusting line 7 in `App_Start/ManholeStart.cs`, replace manhole with whatever you need.

### Authorization
By default the manhole console is only available on local requests. You can add custom authorization by replacing the authorization handler on line 15 of `App_Start/ManholeStart.cs`.

*Note!* The user of the console is allowed to perform any action that the account running the application pool has access to, this includes creating and modifying files, accessing databases, and other local and remote resources.

You should think long and hard about how you formulate your authorization rules.
