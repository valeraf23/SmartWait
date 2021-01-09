# SmartWait

**Each SmartWait  instance defines the maximum amount of time to wait for a condition, as well as the frequency with which to check the condition. Furthermore, the user may configure the wait to ignore specific types of exceptions whilst waiting** 

[![NuGet.org](https://img.shields.io/nuget/v/SmartWait.svg?style=flat-square&label=NuGet.org)](https://www.nuget.org/packages/SmartWait/)
![Nuget](https://img.shields.io/nuget/dt/SmartWait)
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
WaitFor.Condition(waitCondition, timeoutMessage);
                     
WaitFor.Condition(waitCondition, builder=>builder
                                   .SetMaxWaitTime(maxWaitTime)
                                   .SetCallbackForSuccessful(callback)
                                   .SetNotIgnoredExceptionType(notIgnoredExceptionType)
                                   .Build(), timeoutMessage);
```
#### In case when you use `WaitFor.Condition` if the given condition is not met will be rise exception  
![Screenshot](https://user-images.githubusercontent.com/6804802/103997429-bbf75600-51a3-11eb-8107-8177951e6e43.png)

#### In case when some exceptions happen and we got not expected value we can read information about a number of exceptions and where it happened
![Screenshot](https://user-images.githubusercontent.com/6804802/103993612-8bf98400-519e-11eb-9a95-5e93451b9cfe.png)

**In case when you use `WaitFor.For` this function wait until the specified condition is met and return the value that we expected
In case when we get no expected value we can set up cases for the value that we want to return**
```csharp
 var result = WaitFor.For(() => 0)
                .Become(a => a == 5)
                .OnFailure(_ => 1, fail => fail is NotExpectedValue<int>)
                .OnFailure(_ => -2);
  ```     
  ####  You can use the predefined algorithm like LogarithmStep and ParabolaStep which calculate delay steps
  ```csharp
 var res = WaitFor.For(() => actual,
                    w => w.SetLogarithmStep(Time.FromSeconds).Build())
                   .Become(a => a == 3)
                   .OnFailureThrowException();
  ```                
 #### Also, you can use your custom algorithm for delayed steps   
 
 ```csharp                
      var res = WaitFor.For(() => actual, b => b.SetTimeBetweenStep(retryAttempt =>
                                                 TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                                                 .Build())
                                                 .Become(a => a == 5);
```
#### For **additional information** look in [Tests Cases](https://github.com/valeraf23/SmartWait/blob/master/SmartWait.Tests/WaitForTest.cs)
