﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Console.Wpf.ViewModel;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Console.Wpf.ComponentModel.Editors
{
    public class MultilineTextEditor : TextBox
    {
        public MultilineTextEditor()
        {
            this.AcceptsReturn = true;
            this.AcceptsTab = true;
            this.MinLines = 5;


            this.DataContextChanged += new DependencyPropertyChangedEventHandler(MultilineTextEditor_DataContextChanged);
        }

        Property property;
        Binding propertyBinding;
        Binding readonlyBinding;

        void MultilineTextEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            property = e.NewValue as Property;
            if (property != null)
            {
                propertyBinding = new Binding("Value");
                propertyBinding.Source = property;
                this.SetBinding(TextBox.TextProperty, propertyBinding);

                readonlyBinding = new Binding("ReadOnly");
                readonlyBinding.Source = property;
                this.SetBinding(TextBox.IsReadOnlyProperty, readonlyBinding);
            }
        }


        
    }
}