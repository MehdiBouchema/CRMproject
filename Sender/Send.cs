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

                //xmlStr = "<message><header><messageType>Visitor</messageType><description>Creation of a visitor</description><sender>front-end</sender><!-- crm --></header><datastructure><UUID>65dd5555</UUID><name><firstname>Lalalal</firstname><lastname>spliiit</lastname></name><email>lala@split.be</email><GDPR>1</GDPR><timestamp>1522113440</timestamp><version>1</version><isActive>1</isActive><banned>0</banned><!-- Not required fields --><geboortedatum>1997-08-12</geboortedatum><btw>BE15657464625</btw><gsm>01164165474</gsm><extraField></extraField></datastructure></message>";
                //xmlStr = "<message><header><messageType>CreateEvent</messageType><description>Creation of an Event</description><sender>front-end</sender><!-- crm --></header><datastructure><UUID>55edd85e</UUID><name>Tester2</name><timestamp>1522113440</timestamp><version>1</version><isActive>0</isActive><extraField></extraField></datastructure></message>";
                xmlStr = "<message><header><messageType>CreateSession</messageType><description>Creation of an session</description><sender>front-end</sender><!-- crm --></header><datastructure><UUID>tE5d4er</UUID><event_UUID>dehe55d</event_UUID><titel>GamingSessieHS</titel><desc>BlaBliBlo descriptionGaming Bla Bla Bla</desc><lokaal>A205</lokaal><start>1997-08-10</start><end>1997-08-12</end><extraField></extraField></datastructure></message>";
                //xmlStr = "<message><header><messageType>GameGroupTest</messageType><description>Creation of an Gaming group</description><sender>front-end</sender><!-- crm --></header><datastructure><groupUUID>GROUPUUID2</groupUUID><groupName>TheDESTROYERS</groupName><timestamp>1522113440</timestamp><version>1</version><isActive>1</isActive><gameUUID>GAMEUUID2</gameUUID><GroupLeaderUUID>LEADERUUID2</GroupLeaderUUID> <banned>0</banned><leden> <namen><naam>blo</naam><naam>blu</naam></namen></leden></datastructure></message>";
                
                body = Encoding.UTF8.GetBytes(xmlStr);
                channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: null, body: body);
                Console.WriteLine(" [x] Sent {0}", xmlStr);
                // Console.ReadKey();

            }
        }
    }
}
