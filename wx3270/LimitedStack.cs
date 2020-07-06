// <copyright file="LimitedStack.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A stack with a size limit.
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    public class LimitedStack<T> : Stack<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LimitedStack{T}"/> class.
        /// </summary>
        /// <param name="limit">Size limit</param>
        public LimitedStack(int limit)
            : base(limit)
        {
            if (limit <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(limit), "Must be greater than zero");
            }

            this.Limit = limit;
        }

        /// <summary>
        /// Gets the size limit.
        /// </summary>
        public int Limit { get; private set; }

        /// <summary>
        /// Pushes an element onto the stack.
        /// </summary>
        /// <param name="element">Element to push</param>
        public new void Push(T element)
        {
            if (this.Count >= this.Limit)
            {
                // Save everything but the top item.
                var allBut = this.Reverse().Skip(1).ToList();
                this.Clear();
                allBut.ForEach(e => { this.Push(e); });
            }

            base.Push(element);
        }
    }
}
