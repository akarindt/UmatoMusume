using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UmatoMusume.Models
{
    public class OcrResult
    {
        public OcrResult(int _code, object _data)
        {
            Code = _code;
            Data = _data;
        }

        public OcrResult() { }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonIgnore] public bool Succeed => Code == 100;

        [JsonIgnore]
        public OcrData[] OcrData => Succeed ? JsonConvert.DeserializeObject<OcrData[]>(Data.ToString()) : null;
    }

    public class OcrData
    {
        public OcrData(int[][] _box, double _score, string _text)
        {
            Box = _box;
            Score = _score;
            Text = _text;
        }

        public OcrData() { }

        [JsonProperty("box")]
        public int[][] Box { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
