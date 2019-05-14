using sfservice;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CRMproject
{
    class SessionFlow :SFLogin
    {
        private void CreateSession(string nameSession, string uuidEvent, string uuidSession, string description, string lokaal, DateTime start, DateTime end)
        {

            SSession__c session1 = new SSession__c();

            session1.UUID_Event__c = uuidEvent;

            session1.UUID_Session__c = uuidSession;

            session1.Name = nameSession;
            session1.Description__c = description;
            session1.Lokaal__c = lokaal;

            session1.StartDTime__cSpecified = true;
            session1.EndDTime__cSpecified = true;

            session1.StartDTime__c = start;
            session1.EndDTime__c = end;



            var x = client.createAsync(header, null, null, null, null, null, null, null, null, null, null, null, new sObject[] { session1 });
            x.Wait();
            SaveResult[] createResult = x.Result.result;

            if (createResult[0].success)
            {
                string id = createResult[0].id;

                Console.WriteLine("Session: " + nameSession + " succesfully added ");

            }
            else
            {
                string resultaat = createResult[0].errors[0].message;
                Console.WriteLine("Error, session " + nameSession + " not added. " + Environment.NewLine + "ERROR> " + resultaat);
            }


        }
        private void UpdateRecordSession(string messageType, string idSession, string nameSession, string uuidEvent, string uuidSession, string description, string lokaal, DateTime start, DateTime end)
        {
            SSession__c[] updates = new SSession__c[1];

            SSession__c session1 = new SSession__c();

            session1.UUID_Event__c = uuidEvent;

            session1.UUID_Session__c = uuidSession;

            session1.Name = nameSession;
            session1.Description__c = description;
            session1.Lokaal__c = lokaal;

            session1.StartDTime__cSpecified = true;
            session1.EndDTime__cSpecified = true;

            session1.StartDTime__c = start;
            session1.EndDTime__c = end;



            updates[0] = session1;
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

        public void HandleMessageSession(XmlDocument doc)
        {
            XmlNodeList sessionName = doc.GetElementsByTagName("titel");
            XmlNodeList sessionUUID = doc.GetElementsByTagName("UUID");
            XmlNodeList UUIDofParentEvent = doc.GetElementsByTagName("event_UUID");
            XmlNodeList lokaal = doc.GetElementsByTagName("lokaal");
            XmlNodeList descr = doc.GetElementsByTagName("desc");
            XmlNodeList start = doc.GetElementsByTagName("start");
            XmlNodeList end = doc.GetElementsByTagName("end");

            XmlNodeList messageType = doc.GetElementsByTagName("messageType");
            XmlNodeList sender = doc.GetElementsByTagName("sender");
            XmlNodeList banned = doc.GetElementsByTagName("banned");


            if (sessionName[0].InnerText == "" || UUIDofParentEvent[0].InnerText == "" || sessionUUID[0].InnerText == "")
            {
                Console.WriteLine("Not every required field is set..." + Environment.NewLine);
                return;
            }
            else
            {
                //Session fromMessageSession = getSession(); -> via query ...
                if (/*check of event al bestaat... indien niet "create" anders ...*/0 < 1)
                {
                    Console.WriteLine("New data received from " + sender[0].InnerText + Environment.NewLine);
                    CreateSession(sessionName[0].InnerText, UUIDofParentEvent[0].InnerText, sessionUUID[0].InnerText, descr[0].InnerText, lokaal[0].InnerText, Convert.ToDateTime(start[0].InnerText), Convert.ToDateTime(end[0].InnerText));
                    Console.WriteLine(Environment.NewLine + "Message processed!" + Environment.NewLine);
                }
                else
                {
                    if (/*check versies als versie van message > versie op salesforce*/0 < 1)
                    {
                        UpdateRecordSession(messageType[0].InnerText,/*getIDSession via query ...*/"OOOOO", sessionName[0].InnerText, UUIDofParentEvent[0].InnerText, sessionUUID[0].InnerText, descr[0].InnerText, lokaal[0].InnerText, Convert.ToDateTime(start[0].InnerText), Convert.ToDateTime(end[0].InnerText));
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
