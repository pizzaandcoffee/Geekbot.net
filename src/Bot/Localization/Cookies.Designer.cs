﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Geekbot.Bot.Localization {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Cookies {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Cookies() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Geekbot.Bot.Localization.Cookies", typeof(Cookies).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You ate {0} cookies, you&apos;ve only got {1} cookies left.
        /// </summary>
        internal static string AteCookies {
            get {
                return ResourceManager.GetString("AteCookies", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You got {0} cookies, there are now {1} cookies in you cookie jar.
        /// </summary>
        internal static string GetCookies {
            get {
                return ResourceManager.GetString("GetCookies", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You gave {0} cookies to {1}.
        /// </summary>
        internal static string Given {
            get {
                return ResourceManager.GetString("Given", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are {0} cookies in you cookie jar.
        /// </summary>
        internal static string InYourJar {
            get {
                return ResourceManager.GetString("InYourJar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your cookie jar looks almost empty, you should probably not eat a cookie.
        /// </summary>
        internal static string NotEnoughCookiesToEat {
            get {
                return ResourceManager.GetString("NotEnoughCookiesToEat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You don&apos;t have enough cookies.
        /// </summary>
        internal static string NotEnoughToGive {
            get {
                return ResourceManager.GetString("NotEnoughToGive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You already got cookies today, you can have more cookies in {0}.
        /// </summary>
        internal static string WaitForMoreCookies {
            get {
                return ResourceManager.GetString("WaitForMoreCookies", resourceCulture);
            }
        }
    }
}