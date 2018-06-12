using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DarlMLRestExample
{
    class Program
    {

        static string destEmail = "support@darl.ai"; //put your email address here!
        static void Main(string[] args)
        {
            DarlML("yingyang").Wait();
            DarlML("iris").Wait();
            DarlML("cleveheart").Wait();
        }

        static async Task DarlML(string examplename)
        {
            var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream($"DarlMLRestExample.{examplename}.darl"));
            var source = reader.ReadToEnd();
            reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream($"DarlMLRestExample.{examplename}.xml"));
            var data = reader.ReadToEnd();
            var spec = new DarlMLData { code = source, data = data, email = destEmail, percentTrain = 100, sets = 5, jobName = examplename};//use your own choice of training percent (1-100) and sets, (3,5,7,9)
            var valueString = JsonConvert.SerializeObject(spec);
            var client = new HttpClient();
            var response = await client.PostAsync("https://darl.ai/api/Linter/DarlML", new StringContent(valueString, Encoding.UTF8, "application/json"));
            //check for errors here...
        }
    }
}
