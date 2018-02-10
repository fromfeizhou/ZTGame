using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

namespace com.game.client
{
    namespace network
    {
        public partial class NetWorkManager
        {
			private static string FacadeNameSpace = "com.game.client.network.facade";

			private Dictionary<byte, ModuleNetFacadeParam> _moduleNetFacadeDic;

            /** 门面参数 */
			public class ModuleNetFacadeParam
            {
                public object moduleNetFacadeObj;
				public Dictionary<byte, MethodInfo> methodInfoDic = new Dictionary<byte, MethodInfo>();
            }

			#if UNITY_EDITOR
			public Dictionary<byte, ModuleNetFacadeParam> ModuleNetFacadeDic{
				get{ 
					return _moduleNetFacadeDic;
				}
			}


			#endif

            /** 注册网络门面 */
            public void RegisterFacades()
            {
				_moduleNetFacadeDic = new Dictionary<byte, ModuleNetFacadeParam>();
                List<string> allFacades = getAllFacades();
                foreach (string typeName in allFacades)
                {
                    Type t = Type.GetType(typeName);
                    NetFacadeAttribute netFacade = t.GetCustomAttributes(typeof(NetFacadeAttribute), false)[0] as NetFacadeAttribute;
                    if (_moduleNetFacadeDic.ContainsKey(netFacade.moduleId))
                    {
                        Debug.LogError("重复注册Facade. facadeId:" + netFacade.moduleId);
                        continue;
                    }
                    ModuleNetFacadeParam facadeParam = new ModuleNetFacadeParam();
                    facadeParam.moduleNetFacadeObj = Activator.CreateInstance(t);
					facadeParam.methodInfoDic = new Dictionary<byte, MethodInfo>();
                    _moduleNetFacadeDic[netFacade.moduleId] = facadeParam;

                    MethodInfo[] mis = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance |
                                                    BindingFlags.DeclaredOnly);
                    for (int i = 0; i < mis.Length; i++)
                    {
                        NetCommandAttribute netCommand =
                            mis[i].GetCustomAttributes(typeof(NetCommandAttribute), false)[0] as NetCommandAttribute;
                        if (_moduleNetFacadeDic[netFacade.moduleId].methodInfoDic.ContainsKey(netCommand.command))
                        {
                            Debug.LogError("重复注册Command. Command:" + netCommand.command);
                            continue;
                        }
                        //Debug.Log("成功注册：Module:" + netFacade.moduleId + ", Command:" + netCommand.command);
                        facadeParam.methodInfoDic[netCommand.command] = mis[i];
                    }
                    _moduleNetFacadeDic[netFacade.moduleId] = facadeParam;
                }
            }

            /** 调用网络门面 */
			private void FacadeInvoking(Message message)
            {
				if (_moduleNetFacadeDic.ContainsKey (message.module)) {
					ModuleNetFacadeParam facadeParam = _moduleNetFacadeDic [message.module];
					if (facadeParam.methodInfoDic.ContainsKey (message.command)) {
						MethodInfo methodInfo = facadeParam.methodInfoDic [message.command];
						ParameterInfo paramInfo = methodInfo.GetParameters () [1];
						object[] param = new object[2];
						param [0] = 0;//预留，错误码
						using (System.IO.MemoryStream m = new System.IO.MemoryStream (message.voData)) {
							param [1] = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize (m, null, paramInfo.ParameterType);
						}
						methodInfo.Invoke (facadeParam.moduleNetFacadeObj, param);	
						return;
					}
				}
				   
				if (message.module == Module.login )
				{
					switch (message.command) {
					case Command.login_heart:
						{
							using (System.IO.MemoryStream m = new System.IO.MemoryStream(message.voData))
							{
								gprotocol.login_heart_s2c hearVO = ProtoBuf.Serializer.Deserialize<gprotocol.login_heart_s2c>(m);
								OnReceive_Heart (hearVO);
							}
							return;
						}
					case Command.login_auth_key:
						{
							LoginModule.GetInstance().LoginPanel.LockPanel.Hide ();
							using (System.IO.MemoryStream m = new System.IO.MemoryStream(message.voData))
							{
								authData = ProtoBuf.Serializer.Deserialize<gprotocol.login_auth_key_s2c>(m);
								_curMsgSeq = (Int16)authData.unique_id;
								Debug.Log ("设置网络参数：" + "Start:" + authData.unique_id + ", Add:" + authData.unique_add + ", Max:" + authData.max_unique_id + ", ReLoginKey:" + authData.relogin_key);
							}
							return;
						}
					case Command.login_relogin:
						{
							using (System.IO.MemoryStream m = new System.IO.MemoryStream(message.voData))
							{
								Debug.Log ("断线重连返回");
								gprotocol.login_relogin_s2c vo = ProtoBuf.Serializer.Deserialize<gprotocol.login_relogin_s2c>(m);
								CheckErrCode (vo.code);
								HeartSwitch (true);
								Debug.Log ("断线重连结果");
							}
							return;
						}
					}
				}

				Debug.LogError("没有找到模块:" + message.module + ", 指令:" + message.command);
            }



            /** 获取命名空间内的所有网络门面类 */
            private List<string> getAllFacades()
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                List<string> namespacelist = new List<string>();
                List<string> classlist = new List<string>();
                for (int i = 0; i < asm.GetTypes().Length; i++)
                {
                    Type type = asm.GetTypes()[i];
                    if (type.Namespace == FacadeNameSpace)
                    {
                        namespacelist.Add(type.FullName);
                    }
                }
                for (int i = 0; i < namespacelist.Count; i++)
                {
                    string classname = namespacelist[i];
                    classlist.Add(classname);
                }
                return classlist;
            }
        }
    }
}