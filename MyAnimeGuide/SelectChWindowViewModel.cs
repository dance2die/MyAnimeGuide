﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml;

namespace MyAnimeGuide.ViewModel
{
    class SelectChWindowViewModel : INotifyPropertyChanged
    {
        readonly string CONFIG_PATH = @"config.xml";
        public ObservableCollection<string> MyChNameList { get; set; }
        public ObservableCollection<ChGroupPairData> AllChGroupPairs { get; set; }
        public Dictionary<string, ObservableCollection<ChData>> AllChGroupDict { get; set; }

        public SelectChWindowViewModel()
        {
            ChXmlData chXmlData = new ChXmlData();
            AllChGroupPairs = new ObservableCollection<ChGroupPairData>();

            //子ChDataリストを初期化済みのChGroupPairDataをChGroupの数だけ用意
            foreach (ChGroupData chGroupData in ChXmlData.AllChGroupData)
            {
                if (chGroupData.ChGroupName != "")  //ChGroupID:23が空のグループを作っているので除外
                {
                    AllChGroupPairs.Add(new ChGroupPairData(chGroupData));
                }
            }
            int countOfCh = 0;
            //ChDataをすべて該当のグループのChGroupPairDataの子リストに代入
            foreach (ChData chData in ChXmlData.AllChData)
            {
                foreach (ChGroupPairData pairData in AllChGroupPairs)
                {
                    if (chData.ChGroupID == pairData.ChGroupData.ChGroupID)
                    {
                        chData.ChGroupName = pairData.ChGroupData.ChGroupName;
                        pairData.ChildChDataList.Add(chData);
                    }
                }
            }

            MyChNameList = new ObservableCollection<string>();
            InitMyChList();
        }

        //override
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private void InitMyChList()
        {
            if (!File.Exists(CONFIG_PATH))
            {
                MessageBox.Show("設定ファイル(config.xml)が存在しません。新たに設定しなおしてください。", "MyAnimeGuide Information", MessageBoxButton.OK, MessageBoxImage.Information);
                MyChNameList = new ObservableCollection<string>();
            }
            else
            {
                XmlDocument configXmlDoc = new XmlDocument();
                configXmlDoc.Load(CONFIG_PATH);
                XmlNodeList allChNameXmlNodes = configXmlDoc.SelectNodes(@"//myChName");
                foreach (XmlNode chNameNode in allChNameXmlNodes)
                {
                    MyChNameList.Add(chNameNode.InnerText);
                }
            }
        }

        private void SaveConfigFile()
        {
            XmlDocument configXmlDoc = new XmlDocument();
            XmlDeclaration declaration = configXmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement configRoot = configXmlDoc.CreateElement("config");

            configXmlDoc.AppendChild(declaration);
            configXmlDoc.AppendChild(configRoot);

            XmlElement myChNamesElement = configXmlDoc.CreateElement("myChNames");
            configRoot.AppendChild(myChNamesElement);

            foreach (string chName in MyChNameList)
            {
                XmlElement myChNameElement = configXmlDoc.CreateElement("myChName");
                myChNameElement.InnerText = chName;
                myChNamesElement.AppendChild(myChNameElement);
            }
            configXmlDoc.Save(CONFIG_PATH);
        }

    }
}
