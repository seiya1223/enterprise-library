﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Practices.EnterpriseLibrary.Configuration.Console.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.Practices.EnterpriseLibrary.Configuration.Console.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to New {0} {1}.
        /// </summary>
        internal static string NewCollectionElementNameFormat {
            get {
                return ResourceManager.GetString("NewCollectionElementNameFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Configuration files (*.config)|*.config|All Files (*.*)|*.*.
        /// </summary>
        internal static string OpenConfigurationFileDialogFilter {
            get {
                return ResourceManager.GetString("OpenConfigurationFileDialogFilter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Open Configuration File.
        /// </summary>
        internal static string OpenConfigurationFileDialogTitle {
            get {
                return ResourceManager.GetString("OpenConfigurationFileDialogTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Continuing will discard changes made to the current configuration. Do you want save the current changes prior?.
        /// </summary>
        internal static string PromptContinueOperationDiscardChangesWarningMessage {
            get {
                return ResourceManager.GetString("PromptContinueOperationDiscardChangesWarningMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enterprise Library Configuration.
        /// </summary>
        internal static string PromptContinueOperationDiscardChangesWarningTitle {
            get {
                return ResourceManager.GetString("PromptContinueOperationDiscardChangesWarningTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File is Read Only, Would you like to Overwride the file?.
        /// </summary>
        internal static string PromptOverwriteReadonlyFileMessage {
            get {
                return ResourceManager.GetString("PromptOverwriteReadonlyFileMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Save Configuration File.
        /// </summary>
        internal static string PromptOverwriteReadonlyFiletitle {
            get {
                return ResourceManager.GetString("PromptOverwriteReadonlyFiletitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file &apos;{0}&apos; cannot be read from, it might not a valid configuration file or contain errors. Proceeding with save will overwrite the file..
        /// </summary>
        internal static string PromptSaveOverFileThatCannotBeReadFromWarningMessage {
            get {
                return ResourceManager.GetString("PromptSaveOverFileThatCannotBeReadFromWarningMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Save Configuration File.
        /// </summary>
        internal static string PromptSaveOverFileThatCannotBeReadFromWarningTitle {
            get {
                return ResourceManager.GetString("PromptSaveOverFileThatCannotBeReadFromWarningTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Configuration files (*.config)|*.config|All Files (*.*)|*.*.
        /// </summary>
        internal static string SaveConfigurationFileDialogFilter {
            get {
                return ResourceManager.GetString("SaveConfigurationFileDialogFilter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Save Configuration File.
        /// </summary>
        internal static string SaveConfigurationFileDialogTitle {
            get {
                return ResourceManager.GetString("SaveConfigurationFileDialogTitle", resourceCulture);
            }
        }
    }
}
