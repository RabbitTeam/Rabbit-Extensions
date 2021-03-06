﻿namespace Rabbit.Extensions.DependencyInjection
{
    /// <summary>
    /// 表示实现者是一个基础依赖（当前容器区单例）。
    /// </summary>
    public interface IDependency
    {
    }

    /// <inheritdoc />
    /// <summary>
    /// 表示实现者是一个单列依赖。
    /// </summary>
    public interface ISingletonDependency : IDependency
    {
    }

    /// <inheritdoc />
    /// <summary>
    /// 表示实现者是一个瞬态依赖。
    /// </summary>
    public interface ITransientDependency : IDependency
    {
    }
}