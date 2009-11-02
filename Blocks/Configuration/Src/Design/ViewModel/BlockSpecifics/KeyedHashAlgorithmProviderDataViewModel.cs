﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.Design;
using Microsoft.Practices.Unity;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography;
using System.Configuration;

namespace Console.Wpf.ViewModel.BlockSpecifics
{
    public class KeyedHashAlgorithmProviderDataViewModel : CollectionElementViewModel
    {
        public KeyedHashAlgorithmProviderDataViewModel(ElementCollectionViewModel containingCollection, ConfigurationElement thisElement)
            : base(containingCollection, thisElement)
        {
        }

        protected override IEnumerable<Property> GetAllProperties()
        {
            return base.GetAllProperties().Union(
                new Property[]{ 
                    ContainingSection.CreateProperty<ProtectedKeySettingsProperty>( 
                    new ParameterOverride("configuration", ConfigurationElement))
                });
        }


        private class ProtectedKeySettingsProperty : CustomPropertry<ProtectedKeySettings>, ICryptographicKeyProperty
        {
            KeyedHashAlgorithmProviderData configuration;
            private ProtectedKey key;

            public ProtectedKeySettingsProperty(IServiceProvider serviceProvider, KeyedHashAlgorithmProviderData configuration)
                :base(serviceProvider, "Key")
            {
                this.configuration = configuration;
            }

            public override object Value
            {
                get
                {
                    return new ProtectedKeySettings(configuration.ProtectedKeyFilename, configuration.ProtectedKeyProtectionScope)
                        {
                            ProtectedKey = key
                        };
                }
                set
                {
                    ProtectedKeySettings keySettings = (ProtectedKeySettings)value;
                    configuration.ProtectedKeyFilename = keySettings.Filename;
                    configuration.ProtectedKeyProtectionScope = keySettings.Scope;

                    key = keySettings.ProtectedKey;
                }
            }

            public override bool HasEditor
            {
                get
                {
                    return true;
                }
            }

            public override bool TextReadOnly
            {
                get
                {
                    return true;
                }
            }

            public override EditorBehavior EditorBehavior
            {
                get
                {
                    return EditorBehavior.ModalPopup;
                }
            }

            public override System.Drawing.Design.UITypeEditor PopupEditor
            {
                get
                {
                    return new KeyManagerEditor();
                }
            }

            public ProtectedKeySettings KeySettings
            {
                get { return TypedValue; }
            }

            public IKeyCreator KeyCreator
            {
                get { return new KeyedHashAlgorithmKeyCreator(configuration.AlgorithmType); }
            }
        }
    }
}
