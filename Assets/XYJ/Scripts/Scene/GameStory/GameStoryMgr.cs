using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public class GameStoryMgr
    {
        static Dictionary<string, StoryPlayerObject> _storyPlayers = new Dictionary<string, StoryPlayerObject>();

        public static void OnLoadSceneStart()
        {

        }

        public static void OnLoadSceneEnd ()
        {

        }

        public static void Update()
        {

        }

        public static void OnDestory()
        {

        }

        public static void StopStory(string storyId)
        {

        }


        public static StoryPlayer BeginStory(string storyId)
        {
            StoryPlayer player = null;

            return player;
        }

        public static StoryPlayer BeginStory(StoryConfig configData)
        {
            StoryPlayerObject playerObject = null;
            if (_storyPlayers.ContainsKey(configData.storyID))
                playerObject = _storyPlayers[configData.storyID];
            else
            {
                playerObject = StoryPlayerObject.Create(configData);
                _storyPlayers.Add(configData.storyID, playerObject);
            }

            playerObject.Begin();
            return playerObject.Player;
        }

    }
}
