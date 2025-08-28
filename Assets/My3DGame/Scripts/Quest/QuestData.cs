using My3DGame.Manager;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using System.Xml;
using My3DGame.Common;

namespace My3DGame
{
    /// <summary>
    /// 퀘스트 데이터 파일 읽어 오기, 읽기전용
    /// </summary>
    public class QuestData : ScriptableObject
    {
        #region Variables
        public Quests quests;       //퀘스트 데이터 베이스
        private string dataPath = "Data/QuestData";
        #endregion

        //생성자
        public QuestData() { }

        //퀘스트 데이터 읽어오기
        public void LoadData()
        {
            TextAsset asset = (TextAsset)ResourcesManager.Load(dataPath);
            //asset 체크
            if (asset == null || asset.text == null)
            {
                return;
            }

            using (XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
            {
                var xs = new XmlSerializer(typeof(Quests));
                quests = (Quests)xs.Deserialize(reader);
            }
        }
    }
}