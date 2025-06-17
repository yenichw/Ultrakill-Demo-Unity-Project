using System.Collections;
using System.Collections.Generic;

namespace cakeslice
{
	public class LinkedSet<T> : IEnumerable<T>, IEnumerable
	{
		public enum AddType
		{
			NO_CHANGE = 0,
			ADDED = 1,
			MOVED = 2
		}

		private LinkedList<T> list;

		private Dictionary<T, LinkedListNode<T>> dictionary;

		public LinkedSet()
		{
			list = new LinkedList<T>();
			dictionary = new Dictionary<T, LinkedListNode<T>>();
		}

		public LinkedSet(IEqualityComparer<T> comparer)
		{
			list = new LinkedList<T>();
			dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
		}

		public bool Contains(T t)
		{
			return dictionary.ContainsKey(t);
		}

		public bool Add(T t)
		{
			if (dictionary.ContainsKey(t))
			{
				return false;
			}
			LinkedListNode<T> value = list.AddLast(t);
			dictionary.Add(t, value);
			return true;
		}

		public void Clear()
		{
			list.Clear();
			dictionary.Clear();
		}

		public AddType AddOrMoveToEnd(T t)
		{
			if (dictionary.Comparer.Equals(t, list.Last.Value))
			{
				return AddType.NO_CHANGE;
			}
			if (dictionary.TryGetValue(t, out var value))
			{
				list.Remove(value);
				value = list.AddLast(t);
				dictionary[t] = value;
				return AddType.MOVED;
			}
			value = list.AddLast(t);
			dictionary[t] = value;
			return AddType.ADDED;
		}

		public bool Remove(T t)
		{
			if (dictionary.TryGetValue(t, out var value) && dictionary.Remove(t))
			{
				list.Remove(value);
				return true;
			}
			return false;
		}

		public void ExceptWith(IEnumerable<T> enumerable)
		{
			foreach (T item in enumerable)
			{
				Remove(item);
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}
	}
}
