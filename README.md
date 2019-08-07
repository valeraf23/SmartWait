# SmartWait

**Each SmartWait  instance defines the maximum amount of time to wait for a condition, as well as the frequency with which to check the condition. Furthermore, the user may configure the wait to ignore specific types of exceptions whilst waiting** 

[![NuGet.org](https://img.shields.io/nuget/v/SmartWait.svg?style=flat-square&label=NuGet.org)](https://www.nuget.org/packages/SmartWait/)
[![Build status](https://ci.appveyor.com/api/projects/status/5p0bee7pvo6nn3tq/branch/master?svg=true)](https://ci.appveyor.com/project/valeraf23/smartwait/branch/master)
## Installation

#### Install with NuGet Package Manager Console
```
Install-Package SmartWait
```
#### Install with .NET CLI
```
dotnet add package SmartWait
```
## Example:

```csharp
          WaitFor.Condition(waitCondition,builder=>builder
          .SetMaxWaitTime(maxWaitTime)
          .SetExceptionHandling(exceptionHandling)
          .SetCallbackForSuccessful(callback)
          .SetNotIgnoredExceptionType(notIgnoredExceptionType)
          .Build()
          ,timeoutMessage);

            ```
