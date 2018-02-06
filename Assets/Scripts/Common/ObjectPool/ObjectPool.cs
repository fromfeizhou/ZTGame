/************************************************
 * 拓展方法；RecoveryAll()..回收所有产出对象
 ************************************************/

using System;
using System.Collections.Generic;

namespace com.game.client
{
	namespace utility
	{
		
		public class ObjectPool<T> where T: IPoolObject, new()
		{
			private readonly Stack<T> _objectStack;
            private readonly Action<T> _actionReset;
            private readonly Action<T> _actionInitObj;

			public ObjectPool(
				int initialBufferSize, 
				Action<T> resetAction = null, 
				Action<T> onetimeInitAction = null)
			{
			    _objectStack = new Stack<T>(initialBufferSize);
                _actionReset = resetAction;
                _actionInitObj = onetimeInitAction;

				for (int i = 0; i < initialBufferSize; i++)
				    _objectStack.Push(new T());
			}

			public int FreeObjCnt
			{
				get{
                    if (_objectStack == null)
						return 0;
                    return _objectStack.Count;
				}
			}

			public T Talk()
			{
                if (_objectStack.Count > 0)
				{
                    T obj = _objectStack.Pop();

                    if (_actionReset != null)
                        _actionReset(obj);

					return obj;
				}
				else
				{
					T obj = new T();
                    if (_actionInitObj != null)
                        _actionInitObj(obj);

					return obj;
				}
			}

			public void Recovery(T obj)
			{
				obj.Reset();
			    _objectStack.Push(obj);
			}
		}
	}
}