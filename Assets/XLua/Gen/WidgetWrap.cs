#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class WidgetWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Widget);
			Utils.BeginObjectRegister(type, L, translator, 0, 4, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "BandingToggle_OnValueChange", _m_BandingToggle_OnValueChange);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "BandingBtn_OnClick", _m_BandingBtn_OnClick);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "BandingInputField_OnEndEdit", _m_BandingInputField_OnEndEdit);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "BandingInputField_OnValueChange", _m_BandingInputField_OnValueChange);
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 2, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "Create", _m_Create_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "Widget does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Create_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject panel = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    
                        Widget __cl_gen_ret = Widget.Create( panel );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_BandingToggle_OnValueChange(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Widget __cl_gen_to_be_invoked = (Widget)translator.FastGetCSObj(L, 1);
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 3&& translator.Assignable<UnityEngine.Events.UnityAction<bool>>(L, 2)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)) 
                {
                    UnityEngine.Events.UnityAction<bool> degAction = translator.GetDelegate<UnityEngine.Events.UnityAction<bool>>(L, 2);
                    string path = LuaAPI.lua_tostring(L, 3);
                    
                    __cl_gen_to_be_invoked.BandingToggle_OnValueChange( degAction, path );
                    
                    
                    
                    return 0;
                }
                if(__gen_param_count == 2&& translator.Assignable<UnityEngine.Events.UnityAction<bool>>(L, 2)) 
                {
                    UnityEngine.Events.UnityAction<bool> degAction = translator.GetDelegate<UnityEngine.Events.UnityAction<bool>>(L, 2);
                    
                    __cl_gen_to_be_invoked.BandingToggle_OnValueChange( degAction );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Widget.BandingToggle_OnValueChange!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_BandingBtn_OnClick(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Widget __cl_gen_to_be_invoked = (Widget)translator.FastGetCSObj(L, 1);
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 3&& translator.Assignable<UnityEngine.Events.UnityAction>(L, 2)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)) 
                {
                    UnityEngine.Events.UnityAction degAction = translator.GetDelegate<UnityEngine.Events.UnityAction>(L, 2);
                    string path = LuaAPI.lua_tostring(L, 3);
                    
                    __cl_gen_to_be_invoked.BandingBtn_OnClick( degAction, path );
                    
                    
                    
                    return 0;
                }
                if(__gen_param_count == 2&& translator.Assignable<UnityEngine.Events.UnityAction>(L, 2)) 
                {
                    UnityEngine.Events.UnityAction degAction = translator.GetDelegate<UnityEngine.Events.UnityAction>(L, 2);
                    
                    __cl_gen_to_be_invoked.BandingBtn_OnClick( degAction );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Widget.BandingBtn_OnClick!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_BandingInputField_OnEndEdit(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Widget __cl_gen_to_be_invoked = (Widget)translator.FastGetCSObj(L, 1);
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 3&& translator.Assignable<UnityEngine.Events.UnityAction<string>>(L, 2)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)) 
                {
                    UnityEngine.Events.UnityAction<string> degAction = translator.GetDelegate<UnityEngine.Events.UnityAction<string>>(L, 2);
                    string path = LuaAPI.lua_tostring(L, 3);
                    
                    __cl_gen_to_be_invoked.BandingInputField_OnEndEdit( degAction, path );
                    
                    
                    
                    return 0;
                }
                if(__gen_param_count == 2&& translator.Assignable<UnityEngine.Events.UnityAction<string>>(L, 2)) 
                {
                    UnityEngine.Events.UnityAction<string> degAction = translator.GetDelegate<UnityEngine.Events.UnityAction<string>>(L, 2);
                    
                    __cl_gen_to_be_invoked.BandingInputField_OnEndEdit( degAction );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Widget.BandingInputField_OnEndEdit!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_BandingInputField_OnValueChange(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Widget __cl_gen_to_be_invoked = (Widget)translator.FastGetCSObj(L, 1);
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 3&& translator.Assignable<UnityEngine.Events.UnityAction<string>>(L, 2)&& (LuaAPI.lua_isnil(L, 3) || LuaAPI.lua_type(L, 3) == LuaTypes.LUA_TSTRING)) 
                {
                    UnityEngine.Events.UnityAction<string> degAction = translator.GetDelegate<UnityEngine.Events.UnityAction<string>>(L, 2);
                    string path = LuaAPI.lua_tostring(L, 3);
                    
                    __cl_gen_to_be_invoked.BandingInputField_OnValueChange( degAction, path );
                    
                    
                    
                    return 0;
                }
                if(__gen_param_count == 2&& translator.Assignable<UnityEngine.Events.UnityAction<string>>(L, 2)) 
                {
                    UnityEngine.Events.UnityAction<string> degAction = translator.GetDelegate<UnityEngine.Events.UnityAction<string>>(L, 2);
                    
                    __cl_gen_to_be_invoked.BandingInputField_OnValueChange( degAction );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Widget.BandingInputField_OnValueChange!");
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
