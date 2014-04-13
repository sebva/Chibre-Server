using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace Chibre_Server.Game
{
    class Protocol
    {
        public static void Hello(JsonObject data, Player player)
        {
            string uuid = data.GetNamedString("uuid");
            Guid guid;
            bool ok = Guid.TryParse(uuid, out guid);
            if (ok)
                ConnectionManager.Instance.OnHelloReceived(guid);
            else
                Debug.WriteLine("Invalid GUID");
        }
    }
}
