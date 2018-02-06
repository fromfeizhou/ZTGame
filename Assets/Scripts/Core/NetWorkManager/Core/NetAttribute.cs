using System;

namespace com.game.client
{
	namespace network
	{

		[AttributeUsage (AttributeTargets.Class)]
		public sealed class NetFacadeAttribute : Attribute
		{
			public byte moduleId;

			public NetFacadeAttribute (byte moduleId)
			{
				this.moduleId = moduleId;
			}
		}

		[AttributeUsage (AttributeTargets.Method)]
		public sealed class NetCommandAttribute : Attribute
		{
			public byte command;

			public NetCommandAttribute (byte command)
			{
				this.command = command;
			}
		}

	}
}