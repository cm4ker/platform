﻿namespace WebAssembly.Browser.DOM
{
    public class Web
    {
        // public static BrowserInformation BrowserInformation
        // {
        //     get {
        //         return new BrowserInformation();
        //     }
        // }

        public static Window Window
        {
            get {
                return new Window();
            }
        }

        public static Document Document
        {
            get {
                return new Document();
            }
        }
    }
}