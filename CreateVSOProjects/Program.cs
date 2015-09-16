using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VSOCommon;
using System.Configuration;

namespace CreateVSOProjects
{
    class Program
    {
        private static VSOQuery vso;

        //Replace with your list of project names
        //This one is based on Microsoft project codenames https://en.wikipedia.org/wiki/List_of_Microsoft_codenames
        private static List<string> _projectNames = new List<string>()
            {"Apollo", "Avalon", "Bobcat", "Camano", "Cirrus", "Clarity", "Continuum", "Crescent", "Darwin", "Deco",
                "Diamond", "Eaglestone", "Everett", "Falcon", "Frosting", "Fusion", "HailStorm", "Indigo", "Jupiter",
                "Katmai", "Lonestar", "Mantis", "Metro", "Monad", "Neptune", "Odyssey", "Omega", "Paxos", "Photon",
                "Quattro", "RedDog", "Redstone", "Roslyn", "Snowball", "Sparta", "Symphony", "Thunder", "Vail", "Wolfpack",
                "Wolverine", "Millenium", "Razzle", "Freestyle", "Emerald", "Centro", "Birch", "Blue", "Pinball", "Viridian", "Juneau"};

        static void Main(string[] args)
        {

            //Using alternate authentication credential, see app.config.
            //Could also use OAuth https://www.visualstudio.com/integrate/get-started/auth/overview
            string _userName = ConfigurationManager.AppSettings["user"];
            string _password = ConfigurationManager.AppSettings["password"];
            string _url = ConfigurationManager.AppSettings["url"];
            vso = new VSOQuery(_url, _userName, _password);
            
            Console.WriteLine("Creating the projects");

            Task.Run(() => MainAsync()).Wait();

            Console.WriteLine("All done! Press any key to continue...");
            Console.ReadKey();
        }

        private static async Task MainAsync()
        {
            // Grab the process templates - this will choose Scrum
            var templates = await vso.GrabJSONDataWithPath("DefaultCollection/_apis/process/processes?api-version=1.0");
            string processTemplateId = "";
            foreach (var t in templates.value)
            {
                if (t.name == "Scrum")
                {
                    processTemplateId = t.id;
                    break;
                }
            }

            await Task.WhenAll(
                _projectNames.Select(async projectName =>
                                    {
                                        Console.WriteLine("Creating project " + projectName);
                                        await UseApis(projectName, processTemplateId); })
                );
        }

        private static async Task UseApis(string projectName, string processTemplateId)
        {
            var cookies = new CookieContainer();

            try
            {
                //Defaults source control to Git
                var literalData = "{ \"name\": \"" + projectName + "\", \"description\": \"Hackathon - team " + projectName + "\", \"capabilities\": { \"versioncontrol\": { \"sourceControlType\": \"Git\" }, \"processTemplate\": { \"templateTypeId\": \"" + processTemplateId + "\" }";
                
                // Call create project on public APIs
                var publicAdd = await vso.GrabJSONDataWithPathAndPost("DefaultCollection/_apis/projects?api-version=2.0-preview", new StringContent(literalData, Encoding.UTF8, "application/json"), cookies);

                // Wait for progress completion
                var publicJobId = publicAdd.id;

                // Wait for it to finish
                while (true)
                {
                    var waitResult = await vso.GrabJSONDataWithPath("_apis/operations/" + publicJobId);
                    var result = waitResult.status;
                    if (result == "succeeded")
                        break;
                    else if (result == "3")
                        throw new Exception("Project has been recently deleted");
                }
                Console.WriteLine("Created project " + projectName + " successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }
    }
}
