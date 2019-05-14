using sfservice;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CRMproject
{
    class EventFlow :SFLogin
    {
        private void CreateEvent(string nameEvent, string uuid, Int32 timestamp, Int32 version, bool isActive)
        {

            EEvent__c event1 = new EEvent__c();

            event1.UUID__c = uuid;
            event1.Name = nameEvent;

            event1.IsActive__cSpecified = true;
            event1.timestamp__cSpecified = true;
            event1.Version__cSpecified = true;
            event1.timestamp__c = timestamp;
            event1.Version__c = version;
            event1.IsActive__c = isActive;

            var x = client.createAsync(header, null, null, null, null, null, null, null, null, null, null, null, new sObject[] { event1 });
            x.Wait();
            SaveResult[] createResult = x.Result.result;

            if (createResult[0].success)
            {
                string id = createResult[0].id;

                Console.WriteLine("Event:" + nameEvent + " succesfully added ");

            }
            else
            {
                string resultaat = createResult[0].errors[0].message;
                Console.WriteLine("Error, event " + nameEvent + " not added. " + Environment.NewLine + "ERROR> " + resultaat);
            }


        }

        private void UpdateRecordEvent(string messageType, string idEvent, string nameEvent, string uuid, Int32 timestamp, Int32 version, bool isActive)
        {
            EEvent__c[] updates = new EEvent__c[1];

            EEvent__c event1 = new EEvent__c();
            event1.Id = idEvent;


            event1.UUID__c = uuid;
            event1.Name = nameEvent;

            event1.IsActive__cSpecified = true;
            event1.timestamp__cSpecified = true;
            event1.timestamp__c = timestamp;
            event1.Version__c = version;
            event1.IsActive__c = isActive;

            updates[0] = event1;
            //event1.Id, new { event1 }
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
        public void HandleMessageEvent(XmlDocument doc)
        {
            XmlNodeList eventUUID = doc.GetElementsByTagName("UUID");
            XmlNodeList eventName = doc.GetElementsByTagName("name");
            XmlNodeList timestamp = doc.GetElementsByTagName("timestamp");
            XmlNodeList version = doc.GetElementsByTagName("version");
            XmlNodeList isactive = doc.GetElementsByTagName("isActive");
            XmlNodeList extra = doc.GetElementsByTagName("extraField");
            XmlNodeList messageType = doc.GetElementsByTagName("messageType");
            XmlNodeList sender = doc.GetElementsByTagName("sender");

            bool isactivebool = Convert.ToInt32(isactive[0].InnerText) != 0;

            if (eventUUID[0].InnerText == "" || eventName[0].InnerText == "")
            {
                Console.WriteLine("Not every required field is set..." + Environment.NewLine);
                return;
            }
            else
            {
                //Event fromMessageEvent = getEvent(); -> via query ...
                if (/*check of event al bestaat... indien niet "create" anders ...*/0 < 1)
                {
                    Console.WriteLine("New data received from " + sender[0].InnerText + Environment.NewLine);
                    CreateEvent(eventName[0].InnerText, eventUUID[0].InnerText, Convert.ToInt32(timestamp[0].InnerText), Convert.ToInt32(version[0].InnerText), isactivebool);
                    Console.WriteLine(Environment.NewLine + "Message processed!" + Environment.NewLine);
                }
                else
                {
                    if (/*check versies als versie van message > versie op salesforce*/0 < 1)
                    {
                        UpdateRecordEvent(messageType[0].InnerText,/*getIDLead via query ...*/"OOOOO", eventName[0].InnerText, eventUUID[0].InnerText, Convert.ToInt32(timestamp[0].InnerText), Convert.ToInt32(version[0].InnerText), isactivebool);
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
