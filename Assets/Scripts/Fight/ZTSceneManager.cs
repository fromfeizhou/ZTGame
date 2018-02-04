using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZTSceneManager : Singleton<ZTSceneManager>
{

    private Dictionary<int,CharaActorInfo> _charaDic = null;
    private List<CharaActorInfo> _charaList = null;
    private Dictionary<int, GameObject> _charaViewDic = null;

    public PlayerBattleInfo MyPlayer = null;

    private GameObject _playerPrefab = null;
    public int SceneFrame = 0;
    //操作指令集合
    public Dictionary<int, List<FightCommandBase>> CommandDic;

    //初始化
    public override void Init()
    {
        _charaDic = new Dictionary<int, CharaActorInfo>();
        _charaViewDic = new Dictionary<int, GameObject>();
        _charaList = new List<CharaActorInfo>();
        SceneFrame = 0;

        //操作集合初始化
        CommandDic = new Dictionary<int, List<FightCommandBase>>();

        OwnerControl.GetInstance().Init();
        //地图初始化
        MapManager.GetInstance().InitMap();
        //技能解析管理器初始化
        SkillActionManager.GetInstance().Init();
        //特效管理器
        EffectManager.GetInstance().Init();

        InitEvent();
    }

    public override void Destroy()
    {
        RemoveEvent();
        ClearChara();

        OwnerControl.GetInstance().Destroy();
        //地图移除
        MapManager.GetInstance().Destroy();
        //技能解析管理器移除
        SkillActionManager.GetInstance().Destroy();
        //特效管理器
        EffectManager.GetInstance().Destroy();
    }

    private void ClearChara()
    {
        //显示清除
        if (null != _charaViewDic)
        {
            foreach(int key in _charaViewDic.Keys)
            {
                GameObject.Destroy(_charaViewDic[key]);
            }
            _charaViewDic.Clear();
            _charaViewDic = null;
        }
        if (_charaDic != null)
        {
            _charaDic.Clear();
            _charaDic = null;
        }
        //数据清除
        if (_charaList != null)
        {
            for (int i = 0; i < _charaList.Count; i++)
            {
                CharaActorInfo actorInfo = _charaList[i] as CharaActorInfo;
                if (null != actorInfo)
                {
                    actorInfo.Destroy();
                }
            }
            _charaList.Clear();
            _charaList = null;
        }
    }

    //刷新对象( 场景相关计算 唯一更新接口)
    public void Update()
    {
        if (SceneFrame > FightDefine.MaxFrame)
        {
            SceneFrame = 0;
        }
        SceneFrame++;
        //刷新玩家虚拟摇杆
        OwnerControl.GetInstance().Update();
        UpdateCommand();
        UpdatePlayer();

        SkillActionManager.GetInstance().Update();
    }

    //命令刷新
    private void UpdateCommand()
    {
        foreach(int frame in CommandDic.Keys){
            if (FightDefine.CompareFrame(frame))
            {
                List<FightCommandBase> list = CommandDic[frame];
                for (int i = 0; i < list.Count; i++)
                {
                    DoCommand(list[i]);
                }
                list.Clear();
            }
        }
    }

    private void DoCommand(FightCommandBase command)
    {
        int battleId = command.BattleId;
        if (_charaDic.ContainsKey(battleId))
        {
            ICharaBattle info = GetCharaById(battleId) as ICharaBattle;
            if (null == info) return;

            switch (command.CommandType)
            {
                case COMMAND_TYPE.MOVE:
                    info.MoveCommand(command as MoveCommand);
                    break;
                case COMMAND_TYPE.SKILL:
                    info.SkillCommand(command as SkillCommand);
                    break;
            }
        }
    }

    private void InitEvent()
    {
        SceneEvent.GetInstance().addEventListener(SCENE_EVENT.ADD_PLAYER, OnAddPlayer);
        SceneEvent.GetInstance().addEventListener(SCENE_EVENT.ADD_COMMAND, OnAddCommand);
    }

    private void RemoveEvent()
    {
        SceneEvent.GetInstance().removeEventListener(SCENE_EVENT.ADD_PLAYER, OnAddPlayer);
        SceneEvent.GetInstance().removeEventListener(SCENE_EVENT.ADD_COMMAND, OnAddCommand);
    }

    private void OnAddCommand(Notification data)
    {
        FightCommandBase command = data.param as FightCommandBase;
        int frame = command.Frame;
        if (!CommandDic.ContainsKey(frame)) CommandDic.Add(frame, new List<FightCommandBase>());
        CommandDic[frame].Add(command);
    }

    private void OnAddPlayer(Notification note)
    {
        int playerId = (int)note.param;
        CreatePlayer(playerId);
    }

    //创建玩家s
    private void CreatePlayer(int battleId)
    {
        //test
        if (!_charaDic.ContainsKey(battleId))
        {
            PlayerBattleInfo playerInfo = new PlayerBattleInfo(1, CHARA_TYPE.PLAYER);
            playerInfo.SetFightInfo(100);
            playerInfo.SetPlayerInfo();
            playerInfo.SetBattleInfo(battleId, battleId, new Vector3(_charaList.Count * 10 + 200f, 0, _charaList.Count * 10 + 200f));
            //test
            _charaDic.Add(battleId, playerInfo);
            _charaList.Add(playerInfo);
        }
      

        if (null != _playerPrefab)
        {
            PlayerBattleInfo info = GetCharaById(battleId) as PlayerBattleInfo;

            if (null == info) return;

            GameObject gameObject = GameObject.Instantiate(_playerPrefab);
            gameObject.transform.localPosition = info.MovePos;
            gameObject.transform.parent = GameObject.Find("PlayerLayer").transform;
            _charaViewDic.Add(battleId, gameObject);
            gameObject.AddComponent<PlayerBattleActor>();
            gameObject.GetComponent<PlayerBattleActor>().SetInfo(info);

            if (battleId == 1)
            {
                MyPlayer = info;
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
        foreach (int key in _charaDic.Keys)
        {
            if (!_charaViewDic.ContainsKey(key))
            {
                CreatePlayer(key);
            }
        }
    }

    //移除玩家
    private void RemovePlayerById(int battleId)
    {
        int index = _charaList.FindIndex(delegate(CharaActorInfo player)
        {
            ICharaBattle info = player as ICharaBattle;
            if (null != info)
            {
                return info.BattleId == battleId;
            }
            return false;
        });

        //移除数据对象
        if (index != -1)
        {
            _charaList.RemoveAt(index);
        }
        if (_charaDic.ContainsKey(battleId))
        {
            _charaDic.Remove(battleId);
        }
        //移除场景对象
        if (_charaDic.ContainsKey(battleId))
        {
            GameObject.Destroy(_charaViewDic[battleId]);
            _charaViewDic.Remove(battleId);
        }
    }

    public CharaActorInfo GetCharaById(int battleId)
    {
        if (_charaDic.ContainsKey(battleId)) return _charaDic[battleId];
        return null;
    }

    public List<CharaActorInfo> GetCharaList()
    {
        return _charaList;
    }

    //刷新玩家
    private void UpdatePlayer()
    {
        //所有对象刷新
        for (int i = 0; i < _charaList.Count; i++)
        {
            _charaList[i].UpdateFrame();
        }
    }

}
