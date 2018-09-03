using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TrustPilotPony
{
    public sealed class WebHandler
    {
        private static WebHandler instance = null;
        private static readonly object padlock = new object();

        WebHandler(){}

        public static WebHandler Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new WebHandler();
                    }
                    return instance;
                }
            }
        }

        public string GetMazeId(string ponyName, int width, int height, int difficulty)
        {
            bool flag = false;
            if(!(width >= 15 && width <= 25)){
                Console.WriteLine("Incorrect Width");
                flag = true;
            }
            if (!(height >= 15 && height <= 25)) {
                Console.WriteLine("Incorrect Height");
                flag = true;
            }

            if (flag){return "Error";}

            string json = "{\r\n  \"maze-width\":"+width.ToString()+
                " ,\r\n  \"maze-height\":"+height.ToString()+
                ",\r\n  \"maze-player-name\": \""+ponyName+
                "\",\r\n  \"difficulty\": "+difficulty.ToString()+"\r\n}";

            string ponyResponse = PonyRequest("https://ponychallenge.trustpilot.com/pony-challenge/maze", json);

            JToken value;
            JObject jobj = JObject.Parse(ponyResponse);

            if(jobj.TryGetValue("maze_id", out value)) {
                return (string)value;
            }
            else {
                return "error";
            }
        }

        public string GetMaze(string mazeID)
        {
            string url = "https://ponychallenge.trustpilot.com/pony-challenge/maze/" + mazeID;
            string maze = PonyRequest(url, "", "GET");
            return maze;
        }

        public string GetMazeVisual(string mazeID)
        {
            string url = "https://ponychallenge.trustpilot.com/pony-challenge/maze/" + mazeID+"/print";
            string maze = PonyRequest(url, "", "GET");
            return maze;
        }

        public string MakeMove(string mazeID, string direction)
        {
            //Possible directions east, west, north, south, stay
            string json = "{\"direction\": \""+direction+"\"}";
            string url = "https://ponychallenge.trustpilot.com/pony-challenge/maze/"+mazeID;
            string response = PonyRequest(url, json);
            return response;
        }

        private string PonyRequest(string url, string json, string Method="POST")
        {
            try
            {
                string webAddr = url;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = Method;
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:61.0) Gecko/20100101 Firefox/61.0";
                if (httpWebRequest.Method == "POST")
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(json);
                        streamWriter.Flush();
                    }
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    return responseText;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "error";
        }
    }
}