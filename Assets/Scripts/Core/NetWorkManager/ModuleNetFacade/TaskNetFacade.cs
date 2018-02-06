using gprotocol;
namespace com.game.client
{
	namespace network.facade
	{
		[NetFacadeAttribute(Module.task)]
		public class TaskNetFacade
		{
			[NetCommandAttribute(Command.task_update)]
			private void OnReceive_Task_Update(int code, task_update_s2c vo)
			{
				UnityEngine.Debug.Log ("[" + System.DateTime.Now + "]" + "[OnReceive_Task_Update]批量更新任务:" + vo.task_list.Count);

			}

		}
	}
}