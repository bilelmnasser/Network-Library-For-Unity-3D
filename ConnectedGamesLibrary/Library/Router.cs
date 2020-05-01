using ConnectedGames.Library;
using Mono.Nat;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectedGamesLibrary.Library
{
    public class Router
    {

        public NetworkInformation obj;




        public INatDevice device;





        public Router(NetworkInformation o)
        {
            obj = o;
            NatUtility.DeviceFound += DeviceFound;
            NatUtility.DeviceLost += DeviceLost;
            NatUtility.StartDiscovery();

        }



        private void DeviceFound(object sender, DeviceEventArgs args)
        {
             device = args.Device;

            // on device found code
            Console.WriteLine(device.GetExternalIP().ToString());


            device.CreatePortMap(new Mapping(Protocol.Tcp, obj.Client_Port, obj.Client_Port,100000));
            device.CreatePortMap(new Mapping(Protocol.Udp, obj.Client_Port, obj.Client_Port,100000));


            
            obj.Public_IP = device.GetExternalIP();
            Console.WriteLine(device.GetExternalIP().ToString());


            foreach (Mapping portMap in device.GetAllMappings())
            {
                Console.WriteLine(portMap.ToString());
            }

        }

        private void DeviceLost(object sender, DeviceEventArgs args)
        {
             device = args.Device;

            // on device disconnect code
        }
        /*  public async Task<IPAddress> Externel_IPAsync()
          {

              var discoverer = new NatDiscoverer();
              var device = await discoverer.DiscoverDeviceAsync();
              var ip = await device.GetExternalIPAsync();
              Console.WriteLine("The external IP Address is: {0} ", ip);
              return ip;

          }

          public async Task Create_New_MapAsync(int privatePort, int publicPort, string Mappingname)
          {

              var discoverer = new NatDiscoverer();
              var cts = new CancellationTokenSource(10000);
              var device = await discoverer.DiscoverDeviceAsync(PortMapper.Upnp, cts);
              /* var mappings = device.GetAllMappingsAsync();


              await mappings;

              Console.WriteLine(mappings.Result.Count()+" Found Mappings.");

              foreach (Mapping m in mappings.Result)
                  {

                      Console.WriteLine("Description : " + m.Description + " Private IP:Port : " + m.PrivateIP + ":" + m.PrivatePort + " Public IP:Port : " + m.PublicIP + ":" + m.PrivatePort);

                  }
              await device.CreatePortMapAsync(new Mapping(Protocol.Tcp, privatePort, publicPort, Mappingname));
              // await device.CreatePortMapAsync(new Mapping(Protocol.Udp, privatePort, publicPort, Mappingname));

          }

      */

    }
}
