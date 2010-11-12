using System;
using System.Collections.Generic;


public class Builder<T>
{
	List<T> _contents = new List<T>();

	public void Append(T param)
	{
		_contents.Add(param);
	}
	public void Append(T[] param)
	{
		foreach (var item in param)
		{
			_contents.Add(item);
		}
	}

	public T[] ToArray()
	{
		T[] returnValue = new T[_contents.Count];
		for (int i = 0; i < _contents.Count; i++)
		{
			returnValue[i] = _contents[i];
		}
		return returnValue;
	}
}
