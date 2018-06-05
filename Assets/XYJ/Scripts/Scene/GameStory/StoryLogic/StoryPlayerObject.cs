using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public class StoryPlayerObject : MonoBehaviour
    {

        public StoryPlayer Player;

        #region GameStoryObject

        // StoryPlayer根对象
        static Transform _gameStoryObject;
        public static Transform GameStoryObject
        {
            get
            {
                if (_gameStoryObject==null)
                {
                    GameObject go = GameObject.Find("[GameStory]");
                    if (go == null)
                        go = new GameObject("[GameStory]");
                    _gameStoryObject = go.transform;
                }
                return _gameStoryObject;
            }
        }

        /// <summary>
        /// 创建StoryPlayer对象
        /// </summary>
        /// <param name="configData"></param>
        /// <returns></returns>
        public static StoryPlayerObject Create(StoryConfig configData)
        {
            GameObject go = new GameObject(string.Format("StoryPlayer({0})", configData.storyID));
            go.transform.parent = GameStoryObject;

            StoryPlayerObject playerObj = go.AddComponent<StoryPlayerObject>();
            playerObj.Player = new StoryPlayer(configData);

            return playerObj;
        }

        public static StoryPlayerObject Create(StoryPlayer player)
        {
            GameObject go = new GameObject(string.Format("StoryPlayer({0})", player.storyID));
            go.transform.parent = GameStoryObject;

            StoryPlayerObject playerObj = go.AddComponent<StoryPlayerObject>();
            playerObj.Player = player;

            return playerObj;
        }

        #endregion

        /// <summary>
        /// 开始剧情
        /// </summary>
        public void Begin()
        {
            if (Player.IsRunning)
                Player.Stop();

            Player.Play();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Player!=null && Player.IsRunning)
            {
                Player.Update();
            }
        }
    }
}
