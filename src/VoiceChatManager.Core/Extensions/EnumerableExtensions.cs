// -----------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="iopietro">
// Copyright (c) iopietro. All rights reserved.
// Licensed under the MIT license.
// </copyright>
// -----------------------------------------------------------------------

namespace VoiceChatManager.Core.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    /// <summary>
    /// Enumerable related extensions.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Performs parallel operations asynchronously on a specified method.
        /// </summary>
        /// <typeparam name="T">The type of the source.</typeparam>
        /// <param name="source">The source to perform actions in.</param>
        /// <param name="body">The body of the method that will be run in parallel.</param>
        /// <param name="maxDegreeOfParallelism">The max degree of parallelism.</param>
        /// <param name="scheduler">The task scheduler to be used to run the parallel foreach.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        /// <remarks>Source: https://stackoverflow.com/questions/14673728/run-async-method-8-times-in-parallel .</remarks>
        public static Task ParallelForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> body, int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded, TaskScheduler scheduler = null)
        {
            return source.ParallelForEachAsync(body, default, maxDegreeOfParallelism, scheduler);
        }

        /// <summary>
        /// Performs parallel operations asynchronously on a specified method.
        /// </summary>
        /// <typeparam name="T">The source generic type.</typeparam>
        /// <param name="source">The source to perform actions in.</param>
        /// <param name="body">The body of the method that will be run in parallel.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="maxDegreeOfParallelism">The max degree of parallelism.</param>
        /// <param name="scheduler">The task scheduler to be used to run the parallel foreach.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        /// <remarks>Source: https://stackoverflow.com/questions/14673728/run-async-method-8-times-in-parallel .</remarks>
        public static Task ParallelForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> body, CancellationToken cancellationToken, int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded, TaskScheduler scheduler = null)
        {
            var options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                CancellationToken = cancellationToken,
            };

            if (scheduler != null)
                options.TaskScheduler = scheduler;

            var block = new ActionBlock<T>(body, options);

            foreach (var item in source)
                block.Post(item);

            block.Complete();

            return block.Completion;
        }

        /// <summary>
        /// Clears a <see cref="ConcurrentQueue{T}"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="ConcurrentQueue{T}"/> generic type.</typeparam>
        /// <param name="queue">The <see cref="ConcurrentQueue{T}"/> to be cleared.</param>
        public static void Clear<T>(this ConcurrentQueue<T> queue)
        {
            while (!queue?.IsEmpty ?? false)
                queue.TryDequeue(out _);
        }
    }
}
