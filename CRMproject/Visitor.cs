using sfservice;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Xml;

namespace CRMproject
{
    class Visitor :SFLogin
    {
        private void CreateLead(string messageType, string uuid, string firstName, string lastName, string email, Int32 timestampLead, Int32 versionLead, bool isActive, bool isBanned, string gsm, DateTime gebDatum, string btwNr, bool gdpr)
        {
            Lead l1 = new Lead();

            l1.UUID__c = uuid;
            l1.FirstName = firstName;
            l1.LastName = lastName;
            l1.Email = email;

            l1.timestamp__cSpecified = true;
            l1.timestamp__c = timestampLead;

            l1.Version__cSpecified = true;
            l1.Version__c = versionLead;

            l1.IsActive__cSpecified = true;
            l1.IsActive__c = isActive;

            l1.IsBanned__cSpecified = true;

            l1.IsBanned__c = isBanned;

            l1.Company = messageType;

            l1.birthdate__cSpecified = true;
            l1.birthdate__c = gebDatum;


            l1.btw__c = btwNr;
            l1.Phone = gsm;

            l1.gdpr__cSpecified = true;
            l1.gdpr__c = gdpr;

            // Create the Result in Salesforce
            var x = client.createAsync(header, null, null, null, null, null, null, null, null, null, null, null, new sObject[] { l1 });
            x.Wait();
            //?zelfde data  doorsturen lukt niet.
            SaveResult[] createResult = x.Result.result;
            if (createResult[0].success)
            {
                string id = createResult[0].id;

                Console.WriteLine("Lead " + firstName + " " + lastName + " succesfully added " + Environment.NewLine + "UUID of the created Lead: " + uuid);
            }
            else
            {
                string resultaat = createResult[0].errors[0].message;
                Console.WriteLine("Error, Lead not added." + Environment.NewLine + "ERROR> " + resultaat);
            }

            //Console.WriteLine("\nPress ENTER to continue...");
            //Console.ReadLine();

        }
        private void UpdateRecordLead(string messageType, string idLead, string uuid, string firstName, string lastName, string email, Int32 timestampLead, double versionLead, bool isActive, bool isBanned, string gsm, DateTime gebDatum, string btwNr, bool gdpr)
        {
            Lead[] updates = new Lead[1];




            Lead l1 = new Lead();
            l1.Id = idLead;

            l1.UUID__c = uuid;
            l1.FirstName = firstName;
            l1.LastName = lastName;
            l1.Email = email;



            l1.timestamp__cSpecified = true;
            l1.timestamp__c = timestampLead;

            l1.Version__cSpecified = true;
            l1.Version__c = versionLead;

            l1.IsActive__cSpecified = true;
            l1.IsActive__c = isActive;

            l1.IsBanned__cSpecified = true;
            l1.IsBanned__c = isBanned;

            l1.Company = messageType;

            l1.birthdate__cSpecified = true;
            l1.birthdate__c = gebDatum;

            l1.btw__c = btwNr;
            l1.Phone = gsm;

            l1.gdpr__cSpecified = true;
            l1.gdpr__c = gdpr;
            updates[0] = l1;

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
        private static string GetLeadID(string uuid)
        {

            //endpoint & header
            QueryResult qResult = null;
            try
            {
                String soqlQuery = "SELECT Id FROM LEAD WHERE UUID__c='" + uuid + "'";

                //client.queryAsync(header, null, null, null, soqlQuery);
                //qResult = client.query(soqlQuery);
                //var x = client.queryAsync(header, null, null, null, soqlQuery);
                //var x = client.queryMoreAsync(header, null, soqlQuery);
                var x = client.queryAllAsync(header, null, soqlQuery);
                x.Wait();
                qResult = x.Result.result;
                Boolean done = false;
                if (qResult.size > 0)
                {
                    Console.WriteLine("Lead with UUID: " + uuid + " has been found!");
                    while (!done)
                    {
                        sObject[] records = qResult.records;
                        for (int i = 0; i < records.Length; i++)
                        {
                            Lead l1 = (Lead)records[i];
                            String id = l1.Id;
                            if (id != null)
                            {
                                Console.WriteLine("Lead with following UUID" + uuid + " has the following SalesForce-ID: " + id);
                                return id;
                            }
                            else
                            {
                                Console.WriteLine("Lead with following UUID" + uuid + " has no SalesForce-ID ..." + Environment.NewLine + "ERROR");
                                return "empty";
                            }

                        }
                        if (qResult.done)
                        {
                            done = true;
                        }

                    }
                }
                else
                {
                    Console.WriteLine("No Lead found.");
                    return "empty";

                }
                Console.WriteLine("Query succesfully executed." + Environment.NewLine + "No Lead with UUID: " + uuid + " founded in SalesForce...");
                return "empty";

            }
            catch (SmtpException e)
            {
                Console.WriteLine("An unexpected error has occurred: " +
                                           e.Message + "\n" + e.StackTrace);
                return "empty";

            }

        }

        public void HandleMessageVisitor(XmlDocument doc)
        {
            XmlNodeList uuidLead = doc.GetElementsByTagName("UUID");
            XmlNodeList fname = doc.GetElementsByTagName("firstname");
            XmlNodeList lname = doc.GetElementsByTagName("lastname");
            XmlNodeList email = doc.GetElementsByTagName("email");
            XmlNodeList gdpr = doc.GetElementsByTagName("GDPR");
            XmlNodeList timestamp = doc.GetElementsByTagName("timestamp");
            XmlNodeList version = doc.GetElementsByTagName("version");
            XmlNodeList isactive = doc.GetElementsByTagName("isActive");
            XmlNodeList banned = doc.GetElementsByTagName("banned");
            ///Not Required:
            XmlNodeList geboortedatum = doc.GetElementsByTagName("geboortedatum");
            XmlNodeList gsm = doc.GetElementsByTagName("gsm");
            XmlNodeList btw = doc.GetElementsByTagName("btw");
            XmlNodeList extra = doc.GetElementsByTagName("extraField");
            XmlNodeList messageType = doc.GetElementsByTagName("messageType");
            XmlNodeList sender = doc.GetElementsByTagName("sender");

            bool isactivebool = Convert.ToInt32(isactive[0].InnerText) != 0;
            bool gdprbool = Convert.ToInt32(gdpr[0].InnerText) != 0;
            bool bannedbool = Convert.ToInt32(banned[0].InnerText) != 0;

            if (uuidLead[0].InnerText == "" || fname[0].InnerText == "" || lname[0].InnerText == "" || email[0].InnerText == "" || timestamp[0].InnerText == "" || version[0].InnerText == "")
            {
                Console.WriteLine("Not every required field is set..." + Environment.NewLine);
                return;
            }
            else
            {
                //Lead fromMessage = getLead(); -> via query ...
                if (/*check of lead al bestaat... indien niet create anders ...*/0 < 1)
                {
                    Console.WriteLine("New data received from " + sender[0].InnerText + Environment.NewLine);
                    CreateLead(messageType[0].InnerText, uuidLead[0].InnerText, fname[0].InnerText, lname[0].InnerText, email[0].InnerText, Convert.ToInt32(timestamp[0].InnerText), Convert.ToInt32(version[0].InnerText), isactivebool, bannedbool, gsm[0].InnerText, Convert.ToDateTime(geboortedatum[0].InnerText), btw[0].InnerText, gdprbool);
                    Console.WriteLine(Environment.NewLine + "Message processed!" + Environment.NewLine);
                }
                else
                {
                    if (/*check versies als versie van message > versie op salesforce*/0 < 1)
                    {
                        UpdateRecordLead(messageType[0].InnerText,/*getIDLead via query ...*/"OOOOO", uuidLead[0].InnerText, fname[0].InnerText, lname[0].InnerText, email[0].InnerText, Convert.ToInt32(timestamp[0].InnerText), Convert.ToInt32(version[0].InnerText), isactivebool, bannedbool, gsm[0].InnerText, Convert.ToDateTime(geboortedatum[0].InnerText), btw[0].InnerText, gdprbool);
                        Console.WriteLine(Environment.NewLine + "Message processed!" + Environment.NewLine);
                    }
                    else
                    {
                        Console.WriteLine("Version lower or same as the already saved lead on SalesForce..." + Environment.NewLine);
                    }
                }


            }


            return;

        }

    }
}
