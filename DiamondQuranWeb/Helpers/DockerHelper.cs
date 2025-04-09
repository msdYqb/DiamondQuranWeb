using Docker.DotNet;
using Docker.DotNet.Models;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DiamondQuranWeb.Helpers
{
    public class DockerHelper
    {
        public static bool InDocker { get { return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"; } }
        public static List<int> DockerPort { get; set; }

        public static async Task<ContainerListResponse?> GetExistingContainerByName(DockerClient client, string containerName)
        {
            try
            {
                var containers = await client.Containers.ListContainersAsync(
                    new ContainersListParameters()
                    {
                        All = true
                    });

                return containers.FirstOrDefault(c => c.Names.Any(n => n.Equals("/" + containerName)));
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        static string GetDockerId()
        {
            var containerId = string.Empty;
            string cgroupFile = "/proc/self/cgroup";

            if (File.Exists(cgroupFile))
            {
                string[] lines = File.ReadAllLines(cgroupFile);
                foreach (string line in lines)
                {
                    if (line.Contains("docker"))
                    {
                        string[] parts = line.Split('/');
                        containerId = parts[^1];
                    }
                }
            }
            return containerId;
        }
        public static async Task<List<int>> GetExposedPorts()
        {
            var portsList = new List<int>();
            var dockerClient = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock")).CreateClient();
            var dockerId = GetDockerId();
            var containerInspect = await dockerClient.Containers.InspectContainerAsync(dockerId);

            foreach (var port in containerInspect.NetworkSettings.Ports)
            {
                var portString = port.Key;

                if (portString.IndexOf('/') is int index && index > 0)
                    portString = portString.Substring(0, index);

                if (int.TryParse(portString, out int portNumber))
                    portsList.Add(portNumber);
            }

            return portsList;
        }
    }
}

