using System.Collections.Generic;

namespace com.game.client
{
	namespace network.utility
	{
		public class DoubleBuff<T>
		{
			private bool _useOne;
			private readonly Queue<T> _one;   
            private readonly Queue<T> _two;

			public DoubleBuff()
			{
				_useOne = true;
				_one = new Queue<T> ();
				_two = new Queue<T> ();
			}

			private Queue<T> PopQue
			{
				get{
					return _useOne ? _one : _two;
				}
			}

			private Queue<T> PushQue
			{
				get{
					return _useOne ? _two : _one;
				} 
			}


			public int Count{
				get{
					int cnt = 0;
					if (_one != null)
						cnt += _one.Count;
					if (_two != null)
						cnt += _two.Count;
					return cnt;
				}
			}

			public T Pop(){
				
				if (Count == 0) {
					return default (T);
				}

                if (PopQue.Count == 0)
					_useOne = !_useOne;

			    return PopQue.Dequeue();
			}

			public void Push(T t)
			{
			    PushQue.Enqueue(t);
			}

			public void Clear()
			{
				_one.Clear ();
				_two.Clear ();
			}
		}
	}
}