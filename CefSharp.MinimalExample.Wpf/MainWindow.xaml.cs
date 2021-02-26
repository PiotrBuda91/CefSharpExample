using CefSharp.Wpf;
using System;
using System.Diagnostics;
using System.Windows;

namespace CefSharp.MinimalExample.Wpf
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            CefSharpSettings.WcfEnabled = true;
            InitializeComponent();
            //InitBrowser();

            Browser.FrameLoadEnd += OnFrameLoadEnd;
            //Browser.ConsoleMessage += OnConsoleMessage;
            Browser.JavascriptMessageReceived += OnBrowserJavascriptMessageReceived;
            Browser.ConsoleMessage += OnConsoleMessage;
            Browser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
            Browser.JavascriptObjectRepository.Register("boundAsync", new AsyncBoundObject(), isAsync: true, options: BindingOptions.DefaultBinder);
        }

        public class AsyncBoundObject
        {
            //We expect an exception here, so tell VS to ignore
            [DebuggerHidden]
            public void Error()
            {
                throw new Exception("This is an exception coming from C#");
            }

            //We expect an exception here, so tell VS to ignore
            [DebuggerHidden]
            public string Space()
            {
                return "Message from CSharp!";
            }
        }


        //public void InitBrowser()
        //{
        //    Cef.Initialize(new CefSettings());
        //    CefSettings settings = new CefSettings();
        //    ChromiumWebBrowser browser = new ChromiumWebBrowser("http://127.0.0.1:5500/");
        //}

        public void OnFrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
     
        }

        public void OnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain)
            {
                //In the main frame we inject some javascript that's run on mouseUp
                //You can hook any javascript event you like.
                Browser.ExecuteScriptAsync(@"
                      document.body.onmouseup = function()
                      {
                        //CefSharp.PostMessage can be used to communicate between the browser
                        //and .Net, in this case we pass a simple string,
                        //complex objects are supported, passing a reference to Javascript methods
                        //is also supported.
                        //See https://github.com/cefsharp/CefSharp/issues/2775#issuecomment-498454221 for details
                        CefSharp.PostMessage(window.getSelection().toString());
                      }
                    ");
            }
        }
        public void OnConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            string msg = e.Message;
            if (msg!="Live reload enabled.")
            {
                //MessageBox.Show("Console message received: " + msg);
            }
            
        }

        private void OnBrowserJavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        {

            string messageFromJavaScript = (string)e.Message;
            if (!string.IsNullOrWhiteSpace(messageFromJavaScript))
            {
                MessageBox.Show(messageFromJavaScript + "blabla!");
            }
            //Browser.RegisterAsyncJsObject("boundAsync", "hi from csharp!");
            //Replaced with
            
            
            //MessageBox.Show("DDB JavaScript button clicked! Message: " + messageFromJavaScript);

            //Browser.ConsoleMessage += OnConsoleMessage;
            //DO SOMETHING WITH THIS MESSAGE
            //This event is called on a CEF Thread, to access your UI thread
            //use Control.BeginInvoke/Dispatcher.BeginInvoke
        }

        //browser.JavascriptMessageReceived += OnBrowserJavascriptMessageReceived;

        //private void OnBrowserJavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
        //{
        //    //Complext objects are initially expresses as IDicionary
        //    //You can use dynamic to access properties (the IDicionary is an ExpandoObject)
        //    //dynamic msg = e.Message;
        //    //Alternatively you can use the built in Model Binder to convert to a custom model
        //    var msg = e.ConvertMessageTo<PostMessageExample>();
        //    var callback = (IJavascriptCallback)msg.Callback;
        //    var type = msg.Type;
        //    var property = msg.Data.Property;

        //    //Call a javascript function with your response data
        //    callback.ExecuteAsync(type);
        //}
    }
}
