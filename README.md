
[![CI Pipeline](https://github.com/JimGeersinga/MediatR.Chained/actions/workflows/ci-pipeline.yml/badge.svg)](https://github.com/JimGeersinga/MediatR.ChainedT/actions/workflows/ci-pipeline.yml)



# MediatR.Chained

[![NuGet](https://img.shields.io/nuget/dt/MediatR.Chained.svg)](https://www.nuget.org/packages/MediatR.Chained) 
[![NuGet](https://img.shields.io/nuget/vpre/MediatR.Chained.svg)](https://www.nuget.org/packagesMediatR.Chained)

## Overview
The `MediatorChain` class is a part of the MediatR.Chained library and is used to create and execute a chain of commands or requests sequentially. It provides a fluent interface for adding requests to the chain and executing them asynchronously.

## Usage
To use the `MediatorChain` class, follow these steps:

1. Create an instance of the `MediatorChain` class by passing an instance of the `IMediator` interface and a list of `MediatorChainStep` objects representing the steps in the chain.

2. Use the `Add` method to add requests to the chain. There are two overloads of the `Add` method:
   - The first overload takes a request object of type `IRequest<TNext>` and adds it to the chain. It returns the next mediator chain with the added request.
   - The second overload takes a function that creates a request based on the previous result. It returns the next mediator chain with the added request.

3. Use the `FailWhen` method to add a condition to the chain that fails when the specified predicate returns true. This method is only available when using a mediator chain with a previous result.

4. Use the `SendAsync` method to execute the chain of requests asynchronously and retrieve the response. There are two overloads of the `SendAsync` method:
   - The first overload returns the response of type `TResponse`.
   - The second overload returns the response as an object.

Here's an example of how to use the `MediatorChain` class:

## Installation
You can install the MediatR.Chained library via NuGet. Run the following command in the NuGet Package Manager Console:
```bash 
dotnet add package MediatR.Chained
```



# MediatR.Chained.EntityFrameworkCore

[![NuGet](https://img.shields.io/nuget/dt/MediatR.Chained.EntityFrameworkCore.svg)](https://www.nuget.org/packages/MediatR.Chained.EntityFrameworkCore) 
[![NuGet](https://img.shields.io/nuget/vpre/MediatR.Chained.EntityFrameworkCore.svg)](https://www.nuget.org/packagesMediatR.Chained.EntityFrameworkCore)

## Installation
You can install the MediatR.Chained.EntityFrameworkCore library via NuGet. Run the following command in the NuGet Package Manager Console:
```bash 
dotnet add package MediatR.Chained
```


# License
This library is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.