﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls.WpfPropertyGrid.Attributes;
using Hawk.Core.Connectors;
using Hawk.Core.Utils;
using Hawk.Core.Utils.MVVM;
using Hawk.Core.Utils.Plugins;
using Hawk.ETL.Interfaces;

namespace Hawk.ETL.Plugins.Filters
{
    [XFrmWork("空对象过滤器","检查文本是否为空白符或null")]
    public class NullFT : PropertyChangeNotifier, IColumnDataFilter
    {
        #region Constructors and Destructors

        public NullFT()
        {
            this.Enabled = true;
            this.Column = "";

        }

        #endregion

        #region Properties

        [Category("1.基本选项")]
        [PropertyOrder(6)]
        [DisplayName("求反操作")]
        public bool Revert { get; set; }

        [Category("1.基本选项")]
        [PropertyOrder(6)]
        [DisplayName("作用列名")]
        public string Column { get; set; }


        [Browsable(false)]
        public TableInfo TableInfo { get; set; }

        [Category("1.基本选项")]
        [PropertyOrder(5)]
        [DisplayName("模块名")]
        public string Name { get; set; }


        [Category("1.基本选项")]
        [PropertyOrder(6)]
        [DisplayName("优先级")]
        public double Priority { get; set; }



        private bool _enabled;

        [Category("1.基本选项")]
        [PropertyOrder(1)]
        [DisplayName("启用")]
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged("Enabled");
                }


            }
        }


        [Browsable(false)]
        public string TypeName
        {
            get
            {
                XFrmWorkAttribute item = AttributeHelper.GetCustomAttribute(this.GetType());
                if (item == null)
                {
                    return this.GetType().ToString();
                }
                return item.Name;
            }
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return this.TypeName + " " + this.Column;
        }

        #endregion

        #region Implemented Interfaces

        #region IColumnDataFilter
        public   bool FilteData(IFreeDocument data)
        {
            bool r = true;
            r = data != null && FilteDataBase(data);
       
            return Revert ? !r : r;
        }
        public virtual bool FilteDataBase(IFreeDocument data)
        {
            object item = data[this.Column];
            if (item == null)
            {
                return false;
            }
            if (item is string)
            {
                var s = (string)item;
                if (string.IsNullOrWhiteSpace(s))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region IColumnProcess

        public virtual void Finish()
        {
        }

        public virtual bool Init(IEnumerable<IFreeDocument> datas)
        {
            return false;
        }

        #endregion

        #region IDictionarySerializable

        public void DictDeserialize(IDictionary<string, object> docu, Scenario scenario = Scenario.Database)
        {
            this.UnsafeDictDeserialize(docu);
        }

        public FreeDocument DictSerialize(Scenario scenario = Scenario.Database)
        {
            var dict = this.UnsafeDictSerialize();
            dict.Add("Type", this.GetType().Name);

            dict.Add("Group", "Filter");

            return dict;
        }

        #endregion

        #endregion
    }
}