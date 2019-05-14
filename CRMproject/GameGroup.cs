using sfservice;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CRMproject
{
    class GameGroup :SFLogin
    {
        private void CreateGamingGroup(string name, string uuidGroup, string uuidLeader, string uuidGame, Int32 timestamp, Int32 version, bool isActive, bool banned)
        {

            GameGroup__c gameGroup = new GameGroup__c();

            gameGroup.Name = name;
            gameGroup.UUID_Group__c = uuidGroup;

            gameGroup.timestamp__cSpecified = true;
            gameGroup.timestamp__c = timestamp;

            gameGroup.Version__cSpecified = true;
            gameGroup.Version__c = version;

            gameGroup.UUID_Game__c = uuidGame;
            gameGroup.GroupLeader_UUID__c = uuidLeader;

            gameGroup.IsActive__cSpecified = true;
            gameGroup.Banned__cSpecified = true;

            gameGroup.IsActive__c = isActive;
            gameGroup.Banned__c = banned;


            var x = client.createAsync(header, null, null, null, null, null, null, null, null, null, null, null, new sObject[] { gameGroup });
            x.Wait();
            SaveResult[] createResult = x.Result.result;

            if (createResult[0].success)
            {
                string id = createResult[0].id;

                Console.WriteLine("Gaming-group:" + name + " succesfully added ");

            }
            else
            {
                string resultaat = createResult[0].errors[0].message;
                Console.WriteLine("Error, Gaming-group " + name + " not added. " + Environment.NewLine + "ERROR> " + resultaat);
            }


        }
        private void UpdateRecordGamingGroup(string messageType, string idGameGroup, string name, string uuidGroup, string uuidLeader, string uuidGame, Int32 timestamp, Int32 version, bool isActive, bool banned)
        {
            GameGroup__c[] updates = new GameGroup__c[1];


            GameGroup__c gameGroup = new GameGroup__c();
            gameGroup.Id = idGameGroup;

            gameGroup.Name = name;
            gameGroup.UUID_Group__c = uuidGroup;

            gameGroup.timestamp__cSpecified = true;
            gameGroup.timestamp__c = timestamp;

            gameGroup.Version__cSpecified = true;
            gameGroup.Version__c = version;

            gameGroup.UUID_Game__c = uuidGame;
            gameGroup.GroupLeader_UUID__c = uuidLeader;

            gameGroup.IsActive__cSpecified = true;
            gameGroup.Banned__cSpecified = true;

            gameGroup.IsActive__c = isActive;
            gameGroup.Banned__c = banned;

            updates[0] = gameGroup;

            var x = client.updateAsync(header, null, null, null, null, null, null, null, null, null, null, null, null, updates);
            x.Wait();
            //?zelfde data  doorsturen lukt niet.
            SaveResult[] createResult = x.Result.result;
            if (createResult[0].success)
            {
                string id = createResult[0].id;

                Console.WriteLine("Updated!");
            }
            else
            {
                string resultaat = createResult[0].errors[0].message;
                Console.WriteLine("ERROR> " + resultaat);
            }


        }
        public void HandleMessageGamingGroup(XmlDocument doc)
        {
            XmlNodeList uuidGroup = doc.GetElementsByTagName("groupUUID");
            XmlNodeList uuidGame = doc.GetElementsByTagName("gameUUID");
            XmlNodeList uuidLeader = doc.GetElementsByTagName("GroupLeaderUUID");
            XmlNodeList gamegroupname = doc.GetElementsByTagName("groupName");
            XmlNodeList timestamp = doc.GetElementsByTagName("timestamp");
            XmlNodeList version = doc.GetElementsByTagName("version");
            XmlNodeList isactive = doc.GetElementsByTagName("isActive");
            XmlNodeList extra = doc.GetElementsByTagName("extraField");
            XmlNodeList messageType = doc.GetElementsByTagName("messageType");
            XmlNodeList sender = doc.GetElementsByTagName("sender");
            XmlNodeList banned = doc.GetElementsByTagName("banned");

            bool bannedbool = Convert.ToInt32(banned[0].InnerText) != 0;
            bool isactivebool = Convert.ToInt32(isactive[0].InnerText) != 0;

            if (uuidGroup[0].InnerText == "" || uuidLeader[0].InnerText == "" || uuidGame[0].InnerText == "")
            {
                Console.WriteLine("Not every required field is set..." + Environment.NewLine);
                return;
            }
            else
            {
                //GAmingGroup fromMessageGaminGroup = getGamingGroup(); -> via query ...
                if (/*check of event al bestaat... indien niet "create" anders ...*/0 < 1)
                {
                    Console.WriteLine("New data received from " + sender[0].InnerText + Environment.NewLine);
                    CreateGamingGroup(gamegroupname[0].InnerText, uuidGroup[0].InnerText, uuidLeader[0].InnerText, uuidGame[0].InnerText, Convert.ToInt32(timestamp[0].InnerText), Convert.ToInt32(version[0].InnerText), isactivebool, bannedbool);
                    Console.WriteLine(Environment.NewLine + "Message processed!" + Environment.NewLine);
                }
                else
                {
                    if (/*check versies als versie van message > versie op salesforce*/0 < 1)
                    {
                        UpdateRecordGamingGroup(messageType[0].InnerText,/*getIDGroup via query ...*/"OOOOO", gamegroupname[0].InnerText, uuidGroup[0].InnerText, uuidLeader[0].InnerText, uuidGame[0].InnerText, Convert.ToInt32(timestamp[0].InnerText), Convert.ToInt32(version[0].InnerText), isactivebool, bannedbool);
                        Console.WriteLine(Environment.NewLine + "Message processed!" + Environment.NewLine);
                    }
                    else
                    {
                        Console.WriteLine("Version lower or same as the already saved Event on SalesForce..." + Environment.NewLine);
                    }
                }


            }
            return;

        }
    }
}
