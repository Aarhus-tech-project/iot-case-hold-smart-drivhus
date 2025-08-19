﻿using Application.Common.Interfaces.Factories;
using Application.Common.Models;

namespace Infrastructure.InterfaceImplementations.Factories.Result;

public class ResultFactory : IResultFactory
{
    public Result<T> Ok<T>(T value)
    {
        return new Result<T> { StatusCode = 200, Value = value };
    }

    public Result<T> NotFound<T>()
    {
        return new Result<T> { StatusCode = 404 };
    }

    public Result<T> NoContent<T>()
    {
        return new Result<T> { StatusCode = 204 };
    }

    public Result<T> Created<T>(T value)
    {
        return new Result<T> { StatusCode = 201, Value = value };
    }

    public Result<T> BadRequest<T>()
    {
        return new Result<T> { StatusCode = 400 };
    }
}