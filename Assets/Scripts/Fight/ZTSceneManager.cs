using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZTSceneManager{
    private static ZTSceneManager _instance = null;

    public  Dictionary<int,PlayerBase> PlayerDic = null;
    public Dictionary<int,GameObject> PlayerViewDic = null;

    public List<PlayerBase> PlayerList = null;
    public PlayerBase MyPlayer = null;

    private GameObject _playerPrefab = null;
    public int SceneFrame = 0;
    //Single 
    public static ZTSceneManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new ZTSceneManager();
            return _instance;
        }
        return _instance;
    }

    //初始化
    public void Init()
    {
        PlayerDic = new Dictionary<int, PlayerBase>();
        PlayerViewDic = new Dictionary<int, GameObject>();
        PlayerList = new List<PlayerBase>();
        SceneFrame = 0;

        OwnerControl.GetInstance().Init();
        InitEvent();
    }

    //刷新对象
    public void Update()
    {
        if (SceneFrame > int.MaxValue)
        {
            SceneFrame = 0;
        }
        SceneFrame++;
        //刷新玩家虚拟摇杆
        OwnerControl.GetInstance().Update();

        UpdatePlayer();
    }


    public void Destroy()
    {
        OwnerControl.GetInstance().Destroy();
        RemoveEvent();
    }

    //使用技能
    public void PlayerUseSkill(int playerId = -1, SkillOpera opera = null)
    {
        PlayerBase player = null;
        if (PlayerDic.ContainsKey(playerId))
        {
            player = PlayerDic[playerId];
        }else{
            player = MyPlayer;
        }

        player.dispatchEvent(FightDefine.FightOperaEvents.PLAY_SKILL, new Notification(opera));
    }

    private void InitEvent()
    {
        SceneEvent.GetInstance().addEventListener(ScenePlayerEvents.ADD_PLAYER, OnAddPlayer);
    }

    private void RemoveEvent()
    {
        SceneEvent.GetInstance().removeEventListener(ScenePlayerEvents.ADD_PLAYER, OnAddPlayer);
    }

    private void OnAddPlayer(Notification note)
    {
        int playerId = (int)note.param;
        CreatePlayer(playerId);
    }

    //创建玩家s
    private void CreatePlayer(int playerId)
    {
        //test
        Dictionary<int, PlayerBaseData> dataDic = new Dictionary<int, PlayerBaseData>();
        dataDic.Add(1, new PlayerBaseData(1,1,new Vector3(0,0,0)));
        dataDic.Add(2, new PlayerBaseData(2, 2, new Vector3(5, 0, 5)));

        if (!PlayerDic.ContainsKey(playerId))
        {
            PlayerBase player = new PlayerBase(playerId);
            //test
            player.SetPlayerBaseData(dataDic[playerId]);
            PlayerDic.Add(playerId, player);
            PlayerList.Add(player);
        }
      

        if (null != _playerPrefab)
        {
            GameObject gameObject = GameObject.Instantiate(_playerPrefab);
            gameObject.transform.localPosition = PlayerDic[playerId].PlayerPos;
            gameObject.transform.parent = GameObject.Find("PlayerLayer").transform;
            PlayerViewDic.Add(playerId, gameObject);

            gameObject.GetComponent<PlayerControl>().SetPlayerData(PlayerDic[playerId]);

            if (playerId == 1)
            {
                MyPlayer = PlayerDic[playerId];
                GameObject.Find("Main Camera").GetComponent<CameraFollow>().target = gameObject.transform;
                GameObject.Find("SkillJoystick").GetComponent<SkillArea>().player = gameObject;
            }
        }
        else
        {
            AssetManager.LoadAsset("Assets/Models/TmpCharacter/@fashi_1.prefab", LoadPrefabCom);
        }
    }

    private void LoadPrefabCom(Object target, string path)
    {
        _playerPrefab = target as GameObject;
        LaterCreatePlayer();
    }

    //延后创建玩家
    private void LaterCreatePlayer(){
        foreach (int key in PlayerDic.Keys)
        {
            if (!PlayerViewDic.ContainsKey(key))
            {
                CreatePlayer(key);
            }
        }
    }

    //移除玩家
    private void RemovePlayerById(int playerId)
    {
        int index = PlayerList.FindIndex(delegate(PlayerBase player)
        {
            return player.Id == playerId;
        });

        //移除数据对象
        if (index != -1)
        {
            PlayerList.RemoveAt(index);
        }
        if (PlayerDic.ContainsKey(playerId))
        {
            PlayerDic.Remove(playerId);
        }
        //移除场景对象
        if (PlayerDic.ContainsKey(playerId))
        {
            GameObject.Destroy(PlayerViewDic[playerId]);
            PlayerViewDic.Remove(playerId);
        }
    }

    //刷新玩家
    private void UpdatePlayer()
    {
        //所有对象刷新
        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerList[i].Update();
        }
    }

}
