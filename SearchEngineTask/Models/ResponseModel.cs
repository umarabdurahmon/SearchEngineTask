using SearchEngineTask.CustomExceptions;
using System;
using System.Collections.Generic;

namespace SearchEngineTask.Models
{
    public class ResponseModel
    {
        public ResponseModel()
        {
            Responses = new List<ResultModel>();
        }

        public TimeSpan ResponseDuration { get; set; }

        public string SearchEngineName { get; set; }

        public List<ResultModel> Responses { get; set; }

        public YandexCaptchaModel Captcha { get; set; }
    }
}