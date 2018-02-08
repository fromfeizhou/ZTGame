using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using com.game.client.network;

public class GameManager : MonoSingleton<GameManager>
{
    public static bool GameInit = false;

    private int _loadIndex = 0;
    private List<string> _stateList;
    private List<UnityAction> _loadFuncList;
    // Use this for initialization

	public override void Init ()
	{
        Application.targetFrameRate = 45;
        Debug.Log("GameManager LoadStart");
        _loadIndex = 0;

        _stateList = new List<string>();
        _loadFuncList = new List<UnityAction>();

        //加载地址文件
        _stateList.Add(GAME_LOAD_SETP_EVENT.LOAD_PATH);
        _loadFuncList.Add(PathManager.ParsePath);
        //加载本地化文本
        _stateList.Add(GAME_LOAD_SETP_EVENT.LOAD_WORD);
        _loadFuncList.Add(LocalString.ParseWord);

        //加载表情资源
        //_stateList.Add(GAME_LOAD_SETP_EVENT.LOAD_FACE_ASSET);
        //_loadFuncList.Add(SpriteFaceCache.ParseAsset);

        LoadDataIndex();

        if (NetWorkConst.IsOpenNetWork)
        {
            NetWorkManager.Instace.Init(null);
            NetWorkManager.Instace.Connect();
        }
    }

    private void LoadDataIndex()
    {
        Debug.Log("GameManager LoadDataIndex: " + _loadIndex);
        GameStartEvent.GetInstance().addEventListener(_stateList[_loadIndex].ToString(), LoadDataCom);
        _loadFuncList[_loadIndex]();
    }
    private void LoadDataCom(Notification note)
    {
        GameStartEvent.GetInstance().removeEventListener(_stateList[_loadIndex].ToString(), LoadDataCom);
        _loadIndex++;
        if (_loadIndex >= _stateList.Count)
        {
            //GameStart();
			Debug.Log("GameManager Init Finish");
            return;
        }

        LoadDataIndex();
    }
   
	public void GameStart()
    {
        GameManager.GameInit = true;
        GameStartEvent.GetInstance().dispatchEvent(GAME_LOAD_SETP_EVENT.LOAD_COM);

        ZTSceneManager.GetInstance().Init();
        //ZTXLuaEnv.GetInstance().Init();
        //测试
        Test();
    }

    // Update is called once per frame
    void Update()
    {
		NetWorkManager.Instace.Update ();

        if (GameManager.GameInit == false)
        {
            return;
        }

        //战斗刷新 场景刷新 统一通过ZTSceneManager update内部调用
        ZTSceneManager.GetInstance().Update();
    }

	public override void Destroy()
    {
        LocalString.Destroy();
        PathManager.Destroy();
        ZTSceneManager.GetInstance().Destroy();
        //SpriteFaceCache.Destory();
        //AssetManager.Destroy();
        //ZTXLuaEnv.GetInstance().Destroy();
        GameManager.GameInit = false;
    }

    private void Test()
    {
        //SceneEvent.GetInstance().dispatchEvent(SCENE_EVENT.ADD_PLAYER, new Notification(1, this.gameObject));
        //SceneEvent.GetInstance().dispatchEvent(SCENE_EVENT.ADD_PLAYER, new Notification(2, this.gameObject));

        BattleProtocol.GetInstance().Init();
        BattleProtocol.GetInstance().SendEnterBattle(PlayerModule.GetInstance().RoleID, new Vector3(400,0,400));
    }

}
