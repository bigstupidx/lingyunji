using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using XTools;

namespace PackTool
{
    public class ASyncOperation
    {
        public ASyncOperation()
        {
            error = null;
            isDone = false;
            progress = 0f;

            timecheck = new TimeCheck();
            timecheck.begin();
        }

        protected TimeCheck timecheck;

        public bool isDone { get; protected set; }

        public float totaltime { get { return timecheck.delay; } }

        public string error { get; protected set; }

        public virtual float progress { get; protected set; }

        protected virtual void error_message(string text)
        {
            error = text;
            isDone = true;
        }
    }
}