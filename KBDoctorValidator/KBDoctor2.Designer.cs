﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Concepto.Packages.KBDoctor {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    public sealed partial class KBDoctor2 : global::System.Configuration.ApplicationSettingsBase {
        
        private static KBDoctor2 defaultInstance = ((KBDoctor2)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new KBDoctor2())));
        
        public static KBDoctor2 Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ValidateINOUTParm {
            get {
                return ((bool)(this["ValidateINOUTParm"]));
            }
            set {
                this["ValidateINOUTParm"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int MaxNestLevel {
            get {
                return ((int)(this["MaxNestLevel"]));
            }
            set {
                this["MaxNestLevel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public int ComplexityLevel {
            get {
                return ((int)(this["ComplexityLevel"]));
            }
            set {
                this["ComplexityLevel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("509")]
        public int MaxCodeBlcok {
            get {
                return ((int)(this["MaxCodeBlcok"]));
            }
            set {
                this["MaxCodeBlcok"] = value;
            }
        }
    }
}
