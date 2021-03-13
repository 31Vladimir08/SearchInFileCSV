namespace ResourceLibrary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static class Resource
    {
        public const char DELIMETER = ';';
        public const char DELIMETERCOLNAME = ' ';
        public const string CSV = ".csv";
        public const int LIMITETCOL = 1000000;
    }

    public static class Extensions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            if (chunksize < 1) throw new InvalidOperationException();

            var wrapper = new EnumeratorWrapper<T>(source);

            int currentPos = 0;
            T ignore;
            try
            {
                wrapper.AddRef();
                while (wrapper.Get(currentPos, out ignore))
                {
                    yield return new ChunkedEnumerable<T>(wrapper, chunksize, currentPos);
                    currentPos += chunksize;
                }
            }
            finally
            {
                wrapper.RemoveRef();
            }
        }

        class ChunkedEnumerable<T> : IEnumerable<T>
        {
            EnumeratorWrapper<T> wrapper;
            int chunkSize;
            int start;

            public ChunkedEnumerable(EnumeratorWrapper<T> wrapper, int chunkSize, int start)
            {
                this.wrapper = wrapper;
                this.chunkSize = chunkSize;
                this.start = start;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new ChildEnumerator(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            class ChildEnumerator : IEnumerator<T>
            {
                ChunkedEnumerable<T> parent;
                int position;
                bool done = false;
                T current;

                public ChildEnumerator(ChunkedEnumerable<T> parent)
                {
                    this.parent = parent;
                    position = -1;
                    parent.wrapper.AddRef();
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return Current;
                    }
                }

                public T Current
                {
                    get
                    {
                        if (position == -1 || done)
                        {
                            throw new InvalidOperationException();
                        }
                        return current;

                    }
                }

                public void Dispose()
                {
                    if (!done)
                    {
                        done = true;
                        parent.wrapper.RemoveRef();
                    }
                }

                public bool MoveNext()
                {
                    position++;

                    if (position + 1 > parent.chunkSize)
                    {
                        done = true;
                    }

                    if (!done)
                    {
                        done = !parent.wrapper.Get(position + parent.start, out current);
                    }

                    return !done;

                }

                public void Reset()
                {
                    throw new NotSupportedException();
                }
            }
        }

        class EnumeratorWrapper<T>
        {
            private Enumeration currentEnumeration;

            int refs = 0;

            public EnumeratorWrapper(IEnumerable<T> source)
            {
                SourceEumerable = source;
            }

            IEnumerable<T> SourceEumerable { get; set; }

            public bool Get(int pos, out T item)
            {

                if (currentEnumeration != null && currentEnumeration.Position > pos)
                {
                    currentEnumeration.Source.Dispose();
                    currentEnumeration = null;
                }

                if (currentEnumeration == null)
                {
                    currentEnumeration = new Enumeration { Position = -1, Source = SourceEumerable.GetEnumerator(), AtEnd = false };
                }

                item = default(T);
                if (currentEnumeration.AtEnd)
                {
                    return false;
                }

                while (currentEnumeration.Position < pos)
                {
                    currentEnumeration.AtEnd = !currentEnumeration.Source.MoveNext();
                    currentEnumeration.Position++;

                    if (currentEnumeration.AtEnd)
                    {
                        return false;
                    }

                }

                item = currentEnumeration.Source.Current;

                return true;
            }

            // needed for dispose semantics.
            public void AddRef()
            {
                refs++;
            }

            public void RemoveRef()
            {
                refs--;
                if (refs == 0 && currentEnumeration != null)
                {
                    var copy = currentEnumeration;
                    currentEnumeration = null;
                    copy.Source.Dispose();
                }
            }

            class Enumeration
            {
                public IEnumerator<T> Source { get; set; }
                public int Position { get; set; }
                public bool AtEnd { get; set; }
            }
        }
    }
}
