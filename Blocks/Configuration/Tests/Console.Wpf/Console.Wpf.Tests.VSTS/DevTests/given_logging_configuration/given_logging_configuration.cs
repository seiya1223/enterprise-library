﻿//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Console.Wpf.Tests.VSTS.TestSupport;
using Microsoft.Practices.EnterpriseLibrary.Common.TestSupport.ContextBase;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Console.Wpf.ViewModel;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Console.Wpf.ViewModel.Services;
using Microsoft.Practices.Unity;

namespace Console.Wpf.Tests.VSTS.DevTests.given_logging_configuration
{
    public abstract class given_logging_configuration : Contexts.ContainerContext
    {
        protected LoggingSettings LoggingSection;

        protected override void Arrange()
        {
            base.Arrange();

            IConfigurationSource source = new DictionaryConfigurationSource();
            ConfigurationSourceBuilder sourceBuiler = new ConfigurationSourceBuilder();

            sourceBuiler.ConfigureLogging()
                .WithOptions.DisableTracing()
                            .DoNotRevertImpersonation()
                .FilterOnPriority("prio filter").StartingWithPriority(10)
                .LogToCategoryNamed("General")
                        .SendTo.EventLog("Event Log Listener")
                                    .FormatWith(new FormatterBuilder().TextFormatterNamed("Default"))
                .LogToCategoryNamed("Critial")
                        .SendTo.SharedListenerNamed("Event Log Listener")
                        .SendTo.Custom<MyCustomListener>("Custom Listener")
                        .SendTo.Email("Email Listener");

            sourceBuiler.UpdateConfigurationWithReplace(source);
            LoggingSection = (LoggingSettings)source.GetSection(LoggingSettings.SectionName);
        }

        private class MyCustomListener : CustomTraceListener
        {
            public override void Write(string message)
            {
                throw new NotImplementedException();
            }

            public override void WriteLine(string message)
            {
                throw new NotImplementedException();
            }
        }
    }

    [TestClass]
    public class when_creating_logging_view_model : given_logging_configuration
    {
        SectionViewModel loggingViewModel;

        protected override void Arrange()
        {
            base.Arrange();

            var elementLookup = Container.Resolve<ElementLookup>();
            elementLookup.AddCustomElement(new CustomAttributesPropertyExtender(base.Container.Resolve<IServiceProvider>()));
        }

        protected override void Act()
        {
            loggingViewModel = SectionViewModel.CreateSection(base.Container, LoggingSettings.SectionName, LoggingSection);
            loggingViewModel.UpdateLayout();
        }

        [TestMethod]
        public void then_trace_listener_collection_has_custom_trace_listener_types()
        {
            var tracelistenerCollection =  (ElementCollectionViewModel) loggingViewModel.DescendentElements().Where(x => typeof(TraceListenerDataCollection) == x.ConfigurationType).First();
            Assert.IsTrue(tracelistenerCollection.PolymorphicCollectionElementTypes.Contains(typeof(SystemDiagnosticsTraceListenerData)));
            Assert.IsTrue(tracelistenerCollection.PolymorphicCollectionElementTypes.Contains(typeof(CustomTraceListenerData)));
        }

        [TestMethod]
        public void then_view_model_has_category_with_name_AllEvents()
        {
            Assert.IsTrue(loggingViewModel.DescendentElements().Where(x => x.Name == "All Events").Any());
        }

        [TestMethod]
        public void then_view_model_has_category_with_name_ErrorsAndWarnings()
        {
            Assert.IsTrue(loggingViewModel.DescendentElements().Where(x => x.Name == "Logging Errors & Warnings").Any());
        }

        [TestMethod]
        public void then_view_model_has_category_with_name_UnProcessed()
        {
            Assert.IsTrue(loggingViewModel.DescendentElements().Where(x => x.Name == "Unprocessed Category").Any());
        }

        [TestMethod]
        public void then_trace_sources_are_contained_in_first_column()
        {
            var traceSources = loggingViewModel.DescendentElements().Where(x => typeof(TraceSourceData).IsAssignableFrom(x.ConfigurationType));

            Assert.AreNotEqual(0, traceSources.Count());
            Assert.IsFalse(traceSources.Where(x => x.Column != 0).Any());
        }

        [TestMethod]
        public void then_trace_sources_header_is_at_row_0_col_0()
        {
            var trListenersHeader = loggingViewModel.GetGridVisuals()
                                                    .Where(x => x.Column == 0 && x.Row == 0)
                                                    .OfType<ElementHeaderViewModel>()
                                                    .FirstOrDefault();

            Assert.IsNotNull(trListenersHeader);
            Assert.AreEqual("Category Filters", trListenersHeader.Name);
        }

        [TestMethod]
        public void then_custom_trace_listener_has_attributes_property()
        {
            var customListener = loggingViewModel.DescendentElements(x => x.Name == "Custom Listener").First();
            Assert.IsNotNull(customListener.Property("Attributes"));
        }

        [TestMethod]
        public void then_trace_source_rows_increment_from_row_1()
        {
            var traceSources = loggingViewModel.DescendentElements().Where(x => typeof(TraceSourceData).IsAssignableFrom(x.ConfigurationType));
            
            int numberOfHeaders = 1;
            int numberOfTracesourcesThatDoesntMatch = 0;

            Assert.AreNotEqual(0, traceSources.Count());
            for (int i = 1; i <= traceSources.Count() + numberOfHeaders; i++)
            {
                if (!traceSources.Where(x => x.Row == i).Any()) numberOfTracesourcesThatDoesntMatch++;
            }

            Assert.AreEqual(numberOfHeaders, numberOfTracesourcesThatDoesntMatch);
        }
        
        [TestMethod]
        public void then_filters_collection_is_positioned_after_last_trace_source()
        {
            var traceSources = loggingViewModel.DescendentElements().Where(x => typeof(TraceSourceData).IsAssignableFrom(x.ConfigurationType));
            var filtersCollection = loggingViewModel.GetGridVisuals().OfType<HeaderViewModel>().Where(x => x.Name == "Logging Filters").Single();

            Assert.AreEqual(traceSources.Max(x => x.Row + x.RowSpan) + 3, filtersCollection.Row);
            Assert.AreEqual(0, filtersCollection.Column);
        }

        [TestMethod]
        public void then_filters_are_positioned_after_filters_collection()
        {
            var filters = loggingViewModel.DescendentElements().Where(x => typeof(LogFilterData).IsAssignableFrom(x.ConfigurationType));
            var filtersCollection = loggingViewModel.GetGridVisuals().OfType<HeaderViewModel>().Where(x => x.Name == "Logging Filters").Single();

            Assert.IsTrue(filters.Any());
            Assert.IsFalse(filters.Where(x=>x.Column != 0).Any());
            Assert.IsFalse(filters.Where(x => x.Row <= filtersCollection.Row).Any());
        }


        [TestMethod]
        public void then_trace_listeners_are_contained_in_second_column()
        {
            var traceListeners = loggingViewModel.DescendentElements().Where(x => typeof(TraceListenerData).IsAssignableFrom(x.ConfigurationType));

            Assert.AreNotEqual(0, traceListeners.Count());
            Assert.IsFalse(traceListeners.Where(x => x.Column != 1).Any());
        }

        [TestMethod]
        public void then_tracelisteners_header_is_at_row_0_col_1()
        {
            var trListenersHeader = loggingViewModel.GetGridVisuals()
                                                    .Where(x => x.Column == 1 && x.Row == 0)
                                                    .OfType<ElementHeaderViewModel>()
                                                    .FirstOrDefault();

            Assert.IsNotNull(trListenersHeader);
            Assert.AreEqual("Logging Target Listeners", trListenersHeader.Name);
        }

        [TestMethod]
        public void then_trace_listeners_increment_from_row_1()
        {
            var traceListeners = loggingViewModel.DescendentElements().Where(x => typeof(TraceListenerData).IsAssignableFrom(x.ConfigurationType));

            Assert.AreNotEqual(0, traceListeners.Count());
            for (int i = 1; i <= traceListeners.Count(); i++)
            {
                Assert.IsTrue(traceListeners.Where(x => x.Row == i).Any());
            }
        }

        [TestMethod]
        public void then_formatters_are_contained_in_third_column()
        {
            var formatters = loggingViewModel.DescendentElements().Where(x => typeof(FormatterData).IsAssignableFrom(x.ConfigurationType));

            Assert.AreNotEqual(0, formatters.Count());
            Assert.IsFalse(formatters.Where(x => x.Column != 2).Any());
        }


        [TestMethod]
        public void then_formatters_header_is_at_row_0_col_2()
        {
            var formatterHeader = loggingViewModel.GetGridVisuals()
                                                    .Where(x => x.Column == 2 && x.Row == 0)
                                                    .OfType<ElementHeaderViewModel>()
                                                    .FirstOrDefault();

            Assert.IsNotNull(formatterHeader);
            Assert.AreEqual("Log Message Formatters", formatterHeader.Name);
        }

        [TestMethod]
        public void then_formatters_increment_from_row_1()
        {
            var formatters = loggingViewModel.DescendentElements().Where(x => typeof(FormatterData).IsAssignableFrom(x.ConfigurationType));

            Assert.AreNotEqual(0, formatters.Count());
            for (int i = 1; i <= formatters.Count(); i++)
            {
                Assert.IsTrue(formatters.Where(x => x.Row == i).Any());
            }
        }

        [TestMethod]
        public void then_trace_sources_have_listeners_as_related_elements()
        {
            var traceSources = loggingViewModel.GetDescendentsOfType<TraceSourceData>();
            foreach (var source in traceSources)
            {
                foreach (var listener in source.GetDescendentsOfType<TraceListenerReferenceData>())
                {
                    var thisListener = listener;

                    Assert.IsTrue(loggingViewModel.GetRelatedElements(source)
                                                    .Where(x=>x.Name == listener.Name && typeof(TraceListenerData).IsAssignableFrom( x.ConfigurationType))
                                                    .Any());
                }
            }
        }

        [TestMethod]
        public void then_listeners_have_sources_as_related_elements()
        {
            var traceSources = loggingViewModel.GetDescendentsOfType<TraceSourceData>();
            foreach (var source in traceSources)
            {
                foreach (var listenerRef in source.GetDescendentsOfType<TraceListenerReferenceData>())
                {
                    var listener = loggingViewModel.GetDescendentsOfType<TraceListenerData>().Where(x => x.Name == listenerRef.Name).FirstOrDefault();

                    Assert.IsTrue(loggingViewModel.GetRelatedElements(listener)
                                                    .Where(x => x == source)
                                                    .Any());
                }
            }
        }

        [TestMethod]
        public void then_category_source_has_property_for_each_trace_listener()
        {
            var traceSources = loggingViewModel.GetDescendentsOfType<TraceSourceData>();
            foreach (var source in traceSources)
            {
                foreach (var listener in source.GetDescendentsOfType<TraceListenerReferenceData>())
                {
                    var thisListener = listener;
                    Assert.IsTrue(source.Properties
                        .Where(p => p.PropertyName.StartsWith("SendTo") && ((string)p.Value) == thisListener.Name).Any());
                }
            }
        }

        [TestMethod]
        public void then_trace_listener_element_reference_property_value_returns_listener_name()
        {
            var traceSources = loggingViewModel.GetDescendentsOfType<TraceSourceData>();
            foreach (var source in traceSources)
            {
                foreach (var listener in source.GetDescendentsOfType<TraceListenerReferenceData>())
                {
                    var thisListener = listener;
                    Assert.IsTrue(source.Properties
                        .Where(p => p.PropertyName.StartsWith("SendTo")
                                    && ((string)p.Value) == thisListener.Name
                                    && p.Value.ToString() == thisListener.Name).Any());
                }
            }
        }

        [TestMethod]
        public void then_sendto_extended_properties_report_has_suggested_values()
        {
            var traceSources = loggingViewModel.GetDescendentsOfType<TraceSourceData>();
            foreach (var source in traceSources)
            {
                Assert.IsTrue(source.Properties.Where(p => p.PropertyName.StartsWith("SendTo")).All(p => p.HasSuggestedValues));
            }
        }

        [TestMethod]
        public void then_sendto_suggested_values_containe_all_listener_names_and_blank()
        {
            var traceSources = loggingViewModel.GetDescendentsOfType<TraceSourceData>();
            var expectedSuggestedValues =
                new [] { string.Empty }
                .Concat(
                    loggingViewModel.GetDescendentsOfType<TraceListenerData>().Select(l => l.Name)
                    ).ToArray();

            foreach (var source in traceSources)
            {
                foreach(var prop in source.Properties.Where(p => p.PropertyName.StartsWith("SendTo")))
                {
                    CollectionAssert.AreEquivalent(expectedSuggestedValues, prop.SuggestedValues.ToArray());
                }
            }
        }

        [TestMethod]
        public void then_one_new_trace_listener_property_offered_on_source()
        {
            var traceSources = loggingViewModel.GetDescendentsOfType<TraceSourceData>();
            Assert.IsTrue(traceSources.All(
                    s => s.Properties
                        .Count(p => p.PropertyName == "NewTraceListener") == 1
                    )
                );
        }

        [TestMethod]
        public void then_new_trace_listener_property_display_name_set()
        {
            var traceSources = loggingViewModel.GetDescendentsOfType<TraceSourceData>();
            Assert.IsTrue(traceSources.All(
                    s => s.Properties.Where(p => p.PropertyName == "NewTraceListener")
                            .All(p => p.DisplayName == "Connect to Listener")
                    )
                );
        }

        [TestMethod]
        public void then_has_suggested_values_is_true()
        {
            var traceSources = loggingViewModel.GetDescendentsOfType<TraceSourceData>();
            Assert.IsTrue(traceSources.All(
                    s => s.Properties
                                .Where(p => p.PropertyName == "NewTraceListener")
                                .All(p => p.HasSuggestedValues)
                    )
                );
        }

        [TestMethod]
        public void then_new_trace_listener_suggested_values_contains_unreferenced_trace_listener_list()
        {
            var traceSources = loggingViewModel.GetDescendentsOfType<TraceSourceData>();
            foreach(var source in traceSources)
            {
                var prop = source.Properties.Where(p => p.PropertyName == "NewTraceListener").Single();
                var expectedList = new[] {"[Select Listener]"}
                    .Concat(
                        loggingViewModel.GetDescendentsOfType<TraceListenerData>().Select(l => l.Name)
                        .Except(source.GetDescendentsOfType<TraceListenerReferenceData>().Select(r => r.Name)))
                    .ToArray();

                CollectionAssert.AreEquivalent(expectedList, prop.SuggestedValues.ToArray());
            }
        }
    }

    [TestClass]
    public class when_clearing_listener_reference_extension_property : given_logging_configuration
    {
        private ElementViewModel traceSourceData;
        private string referencedTraceListenerName;

        protected override void Act()
        {
            var loggingViewModel = SectionViewModel.CreateSection(Container, LoggingSettings.SectionName, LoggingSection);
            traceSourceData = loggingViewModel.GetDescendentsOfType<TraceSourceData>()
                    .Where(
                        e => e.Properties.Any(
                            p => p.PropertyName.StartsWith("SendTo"))
                            ).Last();

            var referenceProperty = traceSourceData.Properties.Where(p => p.PropertyName.StartsWith("SendTo")).Last();
            referencedTraceListenerName = referenceProperty.Value.ToString();
            referenceProperty.Value = string.Empty;
        }

        [TestMethod]
        public void then_reference_configuration_element_removed_from_source()
        {
            var traceSourceConfig = (TraceSourceData) traceSourceData.ConfigurationElement;
            Assert.IsTrue(traceSourceConfig.TraceListeners.All(l => l.Name != referencedTraceListenerName));
        }

        [TestMethod]
        public void then_extension_property_is_removed()
        {
            Assert.IsFalse(
                traceSourceData.Properties.Any(
                    x => x.PropertyName.StartsWith("SendTo") && x.DisplayName.Contains(referencedTraceListenerName)));

        }
    }

    [TestClass]
    public class when_referenced_trace_listener_name_changes : given_logging_configuration
    {
        private ElementViewModel traceListener;
        private readonly string NewNameValue = "SomeUnexpectedName";
        private Property referenceProperty;

        protected override void Arrange()
        {
            base.Arrange();

            var loggingViewModel = SectionViewModel.CreateSection(Container, LoggingSettings.SectionName, LoggingSection);
            var traceSourceData = loggingViewModel.GetDescendentsOfType<TraceSourceData>()
                    .Where(
                        e => e.Properties.Any(
                            p => p.PropertyName.StartsWith("SendTo"))
                            ).Last();
            referenceProperty = traceSourceData.Properties.Where(p => p.PropertyName.StartsWith("SendTo")).Last();
            traceListener = loggingViewModel.GetDescendentsOfType<TraceListenerData>().Where(e => e.Name == (string)referenceProperty.Value).Single();
        }

        protected override void Act()
        {
            traceListener.Property("Name").Value = NewNameValue;
        }

        [TestMethod]
        public void then_reference_property_value_is_updated()
        {
            Assert.AreEqual(NewNameValue, referenceProperty.Value);
        }
    }

    [TestClass]
    public class when_referenced_trace_listener_cant_find_tracelistener : given_logging_configuration
    {
        SectionViewModel loggingViewModel;

        protected override void Arrange()
        {
            base.Arrange();

            var traceListenerRef = base.LoggingSection.TraceSources.Get("General").TraceListeners.First();
            traceListenerRef.Name = "Broken reference";

            loggingViewModel = SectionViewModel.CreateSection(Container, LoggingSettings.SectionName, base.LoggingSection);
        }


        [TestMethod]
        public void then_value_is_name_of_element_that_couldnt_be_found()
        {
            var loggingCategory = loggingViewModel.GetDescendentsOfType<TraceSourceData>().Where(x => x.Name == "General").First();
            Assert.IsTrue(loggingCategory.Properties.Where(x=> x.DisplayName.StartsWith("Send To") &&  String.Equals(x.Value as string, "Broken reference")).Any());
        }
    }

    [TestClass]
    public class when_trace_listener_reference_is_updated_to_random_Value : given_logging_configuration
    {
        SectionViewModel loggingViewModel;
        ElementViewModel traceSourceModel;

        protected override void Arrange()
        {
            base.Arrange();

            loggingViewModel = SectionViewModel.CreateSection(Container, LoggingSettings.SectionName, base.LoggingSection);
            traceSourceModel = loggingViewModel.GetDescendentsOfType<TraceSourceData>().Where(x=>x.Name == "General").First();
        }

        protected override void  Act()
        {
            var prop = traceSourceModel.Properties.Where(x=>x.DisplayName.StartsWith("Send To")).First();
            prop.Value = "break this reference";
        }

        [TestMethod]
        public void then_trace_listener_reference_is_updated()
        {
            Assert.IsTrue( traceSourceModel.DescendentElements(x=>x.Name == "break this reference").Any());
        }

    }

    [TestClass]
    public class when_new_listener_is_selected_reference_is_added : given_logging_configuration
    {
        SectionViewModel loggingViewModel;
        private ElementViewModel traceSource;
        private Property newTraceListenerProperty;
        private object originalPropertyValue;

        protected override void Arrange()
        {
            base.Arrange();

            loggingViewModel = SectionViewModel.CreateSection(Container, LoggingSettings.SectionName, base.LoggingSection);
            traceSource = loggingViewModel.GetDescendentsOfType<TraceSourceData>().Where(e => e.Name == "General").First();
            newTraceListenerProperty = traceSource.Properties.Where(p => p.PropertyName == "NewTraceListener").First();
            originalPropertyValue = newTraceListenerProperty.Value;
        }

        protected override void Act()
        {
            newTraceListenerProperty.Value = "Email Listener";
        }

        [TestMethod]
        public void then_reference_data_is_added_for_the_value()
        {
            var referenceData = traceSource.GetDescendentsOfType<TraceListenerReferenceData>();
            Assert.IsTrue(referenceData.Any(r => r.Name == "Email Listener"));
        }

        [TestMethod]
        public void then_property_value_does_not_change()
        {
            Assert.AreEqual(originalPropertyValue, newTraceListenerProperty.Value);
        }
    }   
   
}
