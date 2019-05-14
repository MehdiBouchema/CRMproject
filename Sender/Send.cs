using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text;
using System.Diagnostics;

namespace Sender
{
    class Send
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "10.3.56.27" };
            factory.UserName = "manager";
            factory.Password = "ehb";
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs", type: "fanout");
                String xmlStr;
                byte[] body;

                xmlStr = "<message><header><messageType>Visitor</messageType><description>Gwn een test visitor</description><sender>CRM</sender></header><datastructure><UUID>SESOSES</UUID><name><firstname>Pabli</firstname><lastname>dirkolo</lastname></name><email>FF9@saAt.be</email><GDPR>0</GDPR><timestamp>1522113440</timestamp><version>1</version><isActive>1</isActive><banned>0</banned><!-- Not required fields --><geboortedatum>1997-08-12</geboortedatum><btw>BE15656464654</btw><gsm>01164165468</gsm><extraField></extraField></datastructure></message>";
                //xmlStr = "<message><header><messageType>CreateEvent</messageType><description>Creation of an Event</description><sender>CRM</sender></header><datastructure><UUID>AZPPvbZA</UUID><name>FoShow</name><timestamp>1522113440</timestamp><version>2</version><isActive>1</isActive><extraField></extraField></datastructure></message>";
                //xmlStr = "<message><header><messageType>CreateSession</messageType><description>Creation of an session</description><sender>CRM</sender></header><datastructure><UUID>CDCCEARE234</UUID><event_UUID>23454RCV43DED</event_UUID><titel>GameSessionFree</titel><desc>BlaBliBlo description Bla Bla Bla</desc><lokaal>A205</lokaal><start>2014-08-10</start><end>2014-08-12</end><extraField></extraField></datastructure></message>";
                //xmlStr = "<message><header><messageType>GamingGroup</messageType><description>Creation of an Gaming group</description><sender>CRM</sender></header><datastructure><groupUUID>UUIDgroepUUID</groupUUID><groupName>CounterTeam</groupName><timestamp>1522113440</timestamp><version>1</version><isActive>1</isActive><gameUUID>FRTNITEUUID</gameUUID><GroupLeaderUUID>LEADERUUID</GroupLeaderUUID> <banned>0</banned><leden> <namen><naam>blo</naam><naam>blu</naam></namen></leden></datastructure></message>";

                body = Encoding.UTF8.GetBytes(xmlStr);
                channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: null, body: body);
                Console.WriteLine(" [x] Sent {0}", xmlStr);
                // Console.ReadKey();

            }
        }
    }
}
