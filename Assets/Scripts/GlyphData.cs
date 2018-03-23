using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace HoloGlyph{
    // Describes a keyword data
    public class GlyphData
    {   
        public string device_id { get; set; }
        public string sensor_id { get; set; }
        public string mark = HoloGlyph.VizGlyph.UNDEFINED;
        public string sensor_name = HoloGlyph.VizGlyph.UNDEFINED;
        public string marker = HoloGlyph.VizGlyph.UNDEFINED;
        public string one_glyph = HoloGlyph.VizGlyph.UNDEFINED; //it's mean there is only one glyph for multiple sensor
        public string channel = HoloGlyph.VizGlyph.UNDEFINED;
        public string value = HoloGlyph.VizGlyph.UNDEFINED;
        public string normal_value = HoloGlyph.VizGlyph.UNDEFINED;
        //public string range_color = HoloGlyph.VizGlyph.UNDEFINED;//concatenated color

        List<GlyphData> _fields;

        public GlyphData(string _device_id, string _sensor_id)
        {
            _fields = new List<GlyphData>();
            device_id = _device_id;
            sensor_id = _sensor_id;
        }

        public List<GlyphData> Fields{
            get{
                return _fields;
            }
        }

    } 
}