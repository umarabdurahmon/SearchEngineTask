using System;

namespace SearchEngineTask.CustomExceptions
{
    [Serializable]
    public class YandexCaptchaException : Exception
    {
        public YandexCaptchaModel YandexCapcha { get; private set; }

        public YandexCaptchaException(YandexCaptchaModel yandexCapcha)
        {
            YandexCapcha = yandexCapcha;
        }
    }

    public class YandexCaptchaModel
    {
        public string retkey { get; set; }
        public string key { get; set; }
        public string rep { get; set; }
        public string url_captcha { get; set; }
        public string searchText { get; set; }
    }
}