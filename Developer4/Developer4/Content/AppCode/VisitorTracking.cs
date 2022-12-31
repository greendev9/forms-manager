using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Content.AppCode
{
    public class VisitorTracking
    {
        #region GeneralFunctions
        public static string VisitorIP
        {
            get
            {
                string ip = VisitorIPMasked(null);
                return ip;
            }
        }

        public static string BrowserInfo
        {
            get
            {
                string browserInfo = HttpContext.Current.Request.UserAgent;

                return browserInfo;
            }
        }

        public static string Referer
        {
            get
            {
                Uri referer = HttpContext.Current.Request.UrlReferrer;
                string actual = referer != null ? referer.ToString() : String.Empty;

                return actual;
            }
        }

        public static bool MobileDevice
        {
            get
            {
                bool mobileDevice = false;
                string browserInfo = BrowserInfo.ToLower();

                if (browserInfo.Contains("mobile") && browserInfo.Contains("ipad") == false)
                {
                    mobileDevice = true;
                }

                return mobileDevice;
                //return true;    // To test mobile version on the desktop
            }
        }

        public static bool IsIPad
        {
            get
            {
                bool isIPad = false;
                string browserInfo = BrowserInfo.ToLower();

                if (browserInfo.Contains("ipad"))
                {
                    isIPad = true;
                }

                return isIPad;
            }
        }

        public static string PathInfo
        {
            get
            {
                string pathInfo = HttpContext.Current.Request.FilePath;

                return pathInfo;
            }
        }

        public static string QueryStrings
        {
            get
            {
                string result = String.Empty;
                foreach (String key in HttpContext.Current.Request.QueryString.AllKeys)
                {
                    if (!String.IsNullOrEmpty(result))
                    {
                        result += "&";
                    }
                    result += key + "=" + HttpContext.Current.Request.QueryString[key];
                }

                result = !String.IsNullOrEmpty(result) ? ("?" + result) : result;

                return result;
            }
        }

        public static string PathInfoFull
        {
            get
            {
                string origPage = PathInfo;
                string qStrings = QueryStrings;
                origPage += !String.IsNullOrEmpty(origPage) ? (!String.IsNullOrEmpty(qStrings) ? qStrings : String.Empty) : String.Empty;

                return origPage;
            }
        }

        public static string TestEmpty(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                str = "No Data/Unknown";
            }
            return str;
        }

        public static bool IsLocalHost
        {
            get
            {
                bool isLocal = HttpContext.Current.Request.Url.Authority.Contains("localhost");
                return isLocal;
            }
        }

        public static string VisitorIPMasked(string mIP)
        {
            string VisitorsIPAddr = String.Empty;
            if (!String.IsNullOrEmpty(mIP))
            {
                VisitorsIPAddr = mIP;
            }
            else
            {
                VisitorsIPAddr = IsLocalHost ? "localhost" : HttpContext.Current.Request.UserHostAddress;
            }

            return VisitorsIPAddr;
        }

        // The referer is localhost or a formilae.com web page
        public static bool FormilaeSiteReferer
        {
            get
            {
                var referrer = Referer;
                var formilaeReferer = IsLocalHost;
                if (!formilaeReferer && !String.IsNullOrEmpty(referrer))
                {
                    if (referrer.Length >= 24)
                    {
                        if (referrer.Substring(0, 24) == "https://www.formilae.com")
                        {
                            formilaeReferer = true;
                        }
                    }
                    if (referrer.Length >= 20)
                    {
                        if (referrer.Substring(0, 20) == "https://formilae.com")
                        {
                            formilaeReferer = true;
                        }
                    }
                    if (referrer.Length >= 19)
                    {
                        if (referrer.Substring(0, 19) == "http://formilae.com")
                        {
                            formilaeReferer = true;
                        }
                    }
                    if (referrer.Length >= 23)
                    {
                        if (referrer.Substring(0, 23) == "http://www.formilae.com")
                        {
                            formilaeReferer = true;
                        }
                    }
                }

                return formilaeReferer;
            }
        }
        #endregion
    }
}