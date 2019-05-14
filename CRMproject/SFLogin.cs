using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using sfservice;
using System.ServiceModel;
using System.Reflection;
using System.Xml.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Xml;

namespace CRMproject
{
    class SFLogin
    {
        private static SoapClient loginClient; // for login endpoint
        protected static SoapClient client; // for API endpoint
        protected static SessionHeader header;
        private static EndpointAddress endpoint;

      static void Main(string[] args)
        {
            SFLogin sample = new SFLogin();
            sample.run();
        }

        public void run()

        {
            MethodInfo method = typeof(XmlSerializer).GetMethod("set_Mode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            method.Invoke(null, new object[] { 1 });

            try
            {
                // Make a login call 
                if (login())
                {

                    var factory = new ConnectionFactory() { HostName = "10.3.56.27", UserName = "manager", Password = "ehb", Port = 5672 };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                        channel.QueueBind(queue: "task_queue", exchange: "logs", routingKey: "task");
                        Console.WriteLine("[*] Waiting for Logs");

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine(" [x] Received {0}", message);

                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(message);
                            XmlNodeList xmlList = doc.GetElementsByTagName("messageType");
                            string messageType = xmlList[0].InnerText;
                       
                            switch (messageType)
                            {
                                case "Visitor":
                                    Visitor visitor = new Visitor();
                                    visitor.HandleMessageVisitor(doc);
                                    break;
                               case "CreateEvent":                                   
                                    EventFlow eventflow = new EventFlow();
                                    eventflow.HandleMessageEvent(doc);
                                    break;
                               case "GameGroup":
                                    GameGroup ggroup = new GameGroup();                              
                                    ggroup.HandleMessageGamingGroup(doc);
                                    break;
                               case "CreateSession":
                                    SessionFlow session = new SessionFlow();
                                    session.HandleMessageSession(doc);
                                    break;
                                default:
                                    Console.WriteLine("Invalid MessageType Received from the Sender.");
                                    break;
                            }
                        };
                        channel.BasicConsume(queue: "task_queue",
                                        autoAck: true,
                                        consumer: consumer);
                        Console.WriteLine(" Press [enter] to exit.");
                        Console.ReadLine();
                    }
                    //____________NOPE...Queries uittesten ...
                    //GetLeadID("ad4ce9e2-facb-4024-88ed-7f364731e184");
                    //QSDF->lead(Dom Toretto)

                    logout();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private bool login()
        {      
            string username = "anas.ahraoui@student.ehb.be";
            string password = "SVdbERAM1032JAopgk4StlivOMjARAz2koSCw";

            // Create a SoapClient specifically for logging in
            TimeSpan ts = new TimeSpan(50000000000);
            loginClient = new SoapClient("https://login.salesforce.com/services/Soap/c/45.0", ts, username, password);

            // (combine pw and token if necessary)
            LoginResult lr;
            try
            {
                Console.WriteLine("\nLogging in...\n");
                System.Threading.Tasks.Task<sfservice.loginResponse> t = loginClient.loginAsync(null, username, password);
                t.Wait();
                lr = t.Result.result;
            }
            catch (Exception e)
            {
                // Write the fault message to the console 
                Console.WriteLine("An unexpected error has occurred: " + e.Message);

                // Write the stack trace to the console 
                Console.WriteLine(e.StackTrace);
                return false;
            }

            // Check if the password has expired 
            if (lr.passwordExpired)
            {
                Console.WriteLine("An error has occurred. Your password has expired.");
                return false;
            }

            /** Once the client application has logged in successfully, it will use
             * the results of the login call to reset the endpoint of the service
             * to the virtual server instance that is servicing your organization
             */

            // On successful login, cache session info and API endpoint info
            endpoint = new EndpointAddress(lr.serverUrl);

            /** The sample client application now has a cached EndpointAddress
            * that is pointing to the correct endpoint. Next, the sample client
            * application sets a persistent SOAP header that contains the
            * valid sessionId for our login credentials. To do this, the sample
            * client application creates a new SessionHeader object. Add the session 
            * ID returned from the login to the session header
            */
            header = new SessionHeader();
            header.sessionId = lr.sessionId;

            // Create and cache an API endpoint client
            client = new SoapClient(lr.serverUrl, ts, username, password);
            printUserInfo(lr, lr.serverUrl);

            // Return true to indicate that we are logged in, pointed  
            // at the right URL and have our security token in place. 
            return true;
        }

        private void printUserInfo(LoginResult lr, String authEP)
        {
            try
            {
                GetUserInfoResult userInfo = lr.userInfo;

                Console.WriteLine("\nLogging in ...\n");
                Console.WriteLine("UserID: " + userInfo.userId);
                Console.WriteLine("User Full Name: " +
                    userInfo.userFullName);
                Console.WriteLine("User Email: " +
                    userInfo.userEmail);
                Console.WriteLine();
                Console.WriteLine("SessionID: " +
                    lr.sessionId);
                Console.WriteLine("Auth End Point: " +
                    authEP);
                Console.WriteLine("Service End Point: " +
                    lr.serverUrl);
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("An unexpected error has occurred: " + e.Message +
                    " Stack trace: " + e.StackTrace);
            }
        }

        private void logout()
        {
            try
            {
                client.logoutAsync(header).Wait();
                Console.WriteLine("Logged out.");
            }
            catch (Exception e)
            {
                // Write the fault message to the console 
                Console.WriteLine("An unexpected error has occurred: " + e.Message);

                // Write the stack trace to the console 
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}

