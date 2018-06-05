#if !USE_HOT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace xys.hot.UI
{
    class InfoBase
    {
        protected Text name;
        protected Text value;
        public double val
        {
            get;
            private set;
        }
        public int propertyID {
            get;
            private set;
        }
        public Color defaultColor
        {
            get;
            private set;
        }

        public InfoBase() { }

        public InfoBase(Text name, Text value)
        {
            this.name = name;
            this.value = value;
            defaultColor = name.color;
        }
        public void SetValue(double value, Color color)
        {
            this.val = value;
            this.value.text = value.ToString();
            this.value.color = color;
        }
        public void SetName(int id,string name, Color color)
        {
            this.propertyID = id;
            this.name.text = name;
            this.name.color = color;
        }
        public void Show()
        {
            name.gameObject.SetActive(true);
            value.gameObject.SetActive(true);
        }
        public void Hide()
        {
            name.gameObject.SetActive(false);
            value.gameObject.SetActive(false);
        }
        public  string GetName()
        {
            return name.text;
        }
    }
}
#endif