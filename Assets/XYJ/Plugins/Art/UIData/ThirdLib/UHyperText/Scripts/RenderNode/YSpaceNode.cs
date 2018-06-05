﻿﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WXB
{
    class YSpaceNode : NodeBase
	{
        public float d_offset;

        public override float getHeight() { return d_offset; }

        public override float getWidth() { return 0.001f; }

        public override void render(float maxWidth, RenderCache cache, ref float x, ref uint yline, List<Line> lines, float offsetX, float offsetY)
		{
            if (d_bNewLine == true)
            {
                yline++;
                x = offsetX;
            }
            else
            {

            }
        }

        public override void Release()
        {
            base.Release();

            d_offset = 0f;
        }
	};
}
